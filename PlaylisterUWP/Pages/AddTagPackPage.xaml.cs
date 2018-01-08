namespace PlaylisterUWP
{
	using System;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using Windows.Storage;
	using Windows.UI.Core;
	using Windows.UI.Xaml;
	using Windows.UI.Xaml.Controls;
	using Windows.UI.Xaml.Media.Animation;
	using Windows.UI.Xaml.Navigation;
	using Infrastructure;
	using MetroLog;
	using Models.ViewModels;

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
	}
}
