using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PlaylisterUWP.Pages
{
	using System.Diagnostics;
	using Windows.Storage;
	using Infrastructure;
	using MetroLog;
	using Models.ViewModels;

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class UploadVideoPage : Page
	{
		private const string youtubeChannelEndpoint = "https://www.googleapis.com/youtube/v3/channels?part=snippet&mine=true";
		private readonly ILogger _log = LogManagerFactory.DefaultLogManager.GetLogger<AuthPage>();

		public UploadVideoViewModel ViewModel { get; set; }
		public UploadVideoPage()
		{
			this.InitializeComponent();
			this.ViewModel = new UploadVideoViewModel();
			this.DataContext = ViewModel;
			var localSettings = ApplicationData.Current.LocalSettings;
			var authToken = (string)localSettings.Values["authToken"];
			ViewModel.IsLoggedIn = !string.IsNullOrEmpty(authToken);
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
