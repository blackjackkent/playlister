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
	using Windows.Foundation;
	using Windows.UI.ViewManagement;
	using Infrastructure;
	using MetroLog;
	using Pages;

	public sealed partial class MainPage : Page
	{
		private const string youtubeChannelEndpoint = "https://www.googleapis.com/youtube/v3/channels?part=snippet&mine=true";
		private readonly ILogger _log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();

		public MainPage()
		{
			this.InitializeComponent();
			MyTagPacksPage.Navigate(typeof(MyTagPacksPage));
			AddTagPackPage.Navigate(typeof(AddTagPackPage));
			UploadVideoPage.Navigate(typeof(UploadVideoPage));
			ApplicationView.PreferredLaunchViewSize = new Size(800, 600);
			ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
		}
		
		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			_log.Debug("Initializing Auth URI");
			AuthService.LaunchGoogleAuthUri();
		}

		/// <summary>
		/// Processes the OAuth 2.0 Authorization Response
		/// </summary>
		/// <param name="e"></param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			var uri = e.Parameter as Uri;
			if (uri != null)
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
			}
			else
			{
				Debug.WriteLine(e.Parameter);
			}
		}
	}
}
