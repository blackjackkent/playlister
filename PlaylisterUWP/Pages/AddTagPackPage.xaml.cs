namespace PlaylisterUWP
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using Windows.Storage;
	using Windows.UI.Core;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;
	using Windows.UI.Xaml.Media.Animation;
	using Windows.UI.Xaml.Navigation;
	using Infrastructure;
	using Infrastructure.Exceptions;
	using MetroLog;
	using Models.ViewModels;
	using Newtonsoft.Json;

	public sealed partial class AddTagPackPage : Page
	{
		private readonly ILogger _log = LogManagerFactory.DefaultLogManager.GetLogger<MainPage>();
		public AddTagPackPageViewModel ViewModel { get; set; }
		public MainPage MainPage { get; set; }

		public AddTagPackPage()
		{
			InitializeComponent();
			ViewModel = new AddTagPackPageViewModel();
			DataContext = ViewModel;
		}

		/// <summary>
		/// Processes the OAuth 2.0 Authorization Response
		/// </summary>
		/// <param name="e"></param>
		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			var mainPage = e.Parameter as MainPage;
			if (mainPage != null)
			{
				MainPage = mainPage;
			}
			else
			{
				Debug.WriteLine(e.Parameter);
			}

			Frame rootFrame = Window.Current.Content as Frame;

			if (rootFrame != null && rootFrame.CanGoBack)
			{
				// If we have pages in our in-app backstack and have opted in to showing back, do so
				SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
			}
		}

		private async void AddTagPackSubmitButton_OnClick(object sender, RoutedEventArgs e)
		{
			try
			{
				SaveTagPack();
				var frame = Window.Current.Content as Frame;
				if (frame?.CanGoBack ?? false)
				{
					frame.GoBack();
				}
				else
				{
					Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo());
				}
			}
			catch (InvalidViewModelException ex)
			{
				await DisplayViewModelErrorPopup();
			}
			catch (Exception ex)
			{
				await DisplayGenericErrorPopup(ex);
			}
		}

		private async Task DisplayGenericErrorPopup(Exception ex)
		{
			var errorDialog = new ContentDialog
			{
				Title = "Tag Pack Creation Error",
				Content = $"Error creating tag pack: {ex.Message}",
				CloseButtonText = "OK"
			};
			await errorDialog.ShowAsync();
		}

		private async Task DisplayViewModelErrorPopup()
		{
			var errorDialog = new ContentDialog
			{
				Title = "Tag Pack Creation Error",
				Content = "Your tag pack must include a title and list of tags",
				CloseButtonText = "OK"
			};
			await errorDialog.ShowAsync();
		}

		private void SaveTagPack()
		{
			if (string.IsNullOrEmpty(ViewModel.TagPackTitle) || string.IsNullOrEmpty(ViewModel.TagPackTags))
			{
				throw new InvalidViewModelException();
			}
			var packViewModel = new TagPackViewModel
			{
				Id = Guid.NewGuid(),
				Tags = ViewModel.TagPackTags,
				Title = ViewModel.TagPackTitle
			};
			var localSettings = ApplicationData.Current.LocalSettings;
			var allPacksSerialized = (string) localSettings.Values["tagPacks"];
			if (string.IsNullOrEmpty(allPacksSerialized))
			{
				allPacksSerialized = "[]";
			}
			var allPacks = JsonConvert.DeserializeObject<List<TagPackViewModel>>(allPacksSerialized);
			allPacks.Add(packViewModel);
			allPacksSerialized = JsonConvert.SerializeObject(allPacks);
			localSettings.Values["tagPacks"] = allPacksSerialized;
		}
	}
}
