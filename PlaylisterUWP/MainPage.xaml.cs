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
	using System.Threading.Tasks;
	using Windows.Foundation;
	using Windows.UI.ViewManagement;
	using Infrastructure;
	using MetroLog;
	using Models.ViewModels;
	using Pages;

	public sealed partial class MainPage : Page
	{
		private const string youtubeChannelEndpoint = "https://www.googleapis.com/youtube/v3/channels?part=snippet&mine=true";
		private readonly ILogger _log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

		public MainPage()
		{
			this.InitializeComponent();
			this.ViewModel = new MainPageViewModel();
			this.DataContext = ViewModel;
			ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
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

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			_log.Debug("Initializing Auth URI");
			AuthService.LaunchGoogleAuthUri();
		}

		private async void Logout_Button_Click(object sender, RoutedEventArgs e)
		{
			var localSettings = ApplicationData.Current.LocalSettings;
			localSettings.Values["authToken"] = null;
			ViewModel.IsLoggedIn = false;
		}

		private async Task DisplayLoginError()
		{
			var loginErrorDialog = new ContentDialog
			{
				Title = "Login Error",
				Content = "There was an error connecting to your YouTube account. Please try again later.",
				CloseButtonText = "Ok"
			};
			var result = await loginErrorDialog.ShowAsync();
		}
	}
}
