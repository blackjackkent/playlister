// Copyright 2016 Google Inc.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Diagnostics;

namespace PlaylisterUWP
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Threading.Tasks;
	using Windows.Foundation;
	using Windows.UI.ViewManagement;
	using Google.Apis.Services;
	using Google.Apis.YouTube.v3;
	using Infrastructure;
	using MetroLog;
	using Models.ViewModels;

	public sealed partial class MainPage : Page
	{
		private readonly ILogger _log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

		public MainPage()
		{
			InitializeComponent();
			ViewModel = new MainPageViewModel();
			DataContext = ViewModel;
			ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;

			var localSettings = ApplicationData.Current.LocalSettings;
			if (localSettings.Values["authToken"] != null)
			{
				ViewModel.IsLoggedIn = true;
				FetchYouTubeInfo();
			}
		}

		private async void FetchYouTubeInfo()
		{
			var service = new YouTubeService(new BaseClientService.Initializer
			{
				HttpClientInitializer = AuthService.GetUserCredential(),
				ApplicationName = "PlaylisterUWP"
			});

			var channelsListRequest = service.Channels.List("contentDetails");
			channelsListRequest.Mine = true;
			var channelsListResponse = await channelsListRequest.ExecuteAsync();
			var channel = channelsListResponse.Items.FirstOrDefault();
			if (channel == null)
			{
				ViewModel.RecentUploads = new ObservableCollection<YouTubeUploadViewModel>(new List<YouTubeUploadViewModel>());
				return;
			}
			var uploadListId = channel.ContentDetails.RelatedPlaylists.Uploads;
			var playlistItemsListRequest = service.PlaylistItems.List("snippet");
			playlistItemsListRequest.PlaylistId = uploadListId;
			playlistItemsListRequest.MaxResults = 10;
			playlistItemsListRequest.PageToken = "";
			var playlistItemsListResponse = await playlistItemsListRequest.ExecuteAsync();
			var videoItems = playlistItemsListResponse.Items;
			var viewModels = videoItems.Select(v => new YouTubeUploadViewModel(v)).ToList();
			ViewModel.RecentUploads = new ObservableCollection<YouTubeUploadViewModel>(viewModels);
		}

		public MainPageViewModel ViewModel { get; set; }

		/// <summary>
		/// Processes the OAuth 2.0 Authorization Response
		/// </summary>
		/// <param name="e"></param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			var uri = e.Parameter as Uri;
			if (uri != null)
			{
				await RunAuthenticationFlow(uri);
			}
			else
			{
				Debug.WriteLine(e.Parameter);
			}
		}

		private async Task RunAuthenticationFlow(Uri uri)
		{
			var authResponse = AuthService.ParseAuthorizationResponse(uri, _log);
			var localSettings = ApplicationData.Current.LocalSettings;
			var expectedState = (string)localSettings.Values["state"];
			if (authResponse.State != expectedState)
			{
				_log.Error($"Received request with invalid state ({authResponse.State})");
				return;
			}
			localSettings.Values["state"] = null;
			_log.Debug(Environment.NewLine + "Authorization code: " + authResponse.Code);
			var codeVerifier = (String)localSettings.Values["codeVerifier"];
			var success = await AuthService.PerformCodeExchangeAsync(authResponse.Code, codeVerifier);
			if (success)
			{
				ViewModel.IsLoggedIn = true;
			}
			else
			{
				ViewModel.IsLoggedIn = false;
				await DisplayLoginError();
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_log.Debug("Initializing Auth URI");
			AuthService.LaunchGoogleAuthUri();
		}

		private void Logout_Button_Click(object sender, RoutedEventArgs e)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values["authToken"] = null;
			localSettings.Values["refreshToken"] = null;
			ViewModel.IsLoggedIn = false;
		}

		private static async Task DisplayLoginError()
		{
			var loginErrorDialog = new ContentDialog
			{
				Title = "Login Error",
				Content = "There was an error connecting to your YouTube account. Please try again later.",
				CloseButtonText = "Ok"
			};
			await loginErrorDialog.ShowAsync();
		}

		private void UploadedVideosList_OnItemClick(object sender, ItemClickEventArgs e)
		{
			var url = ((YouTubeUploadViewModel) e.ClickedItem).Url;
			var uri = new Uri(url);
			Windows.System.Launcher.LaunchUriAsync(uri);
		}
	}
}
