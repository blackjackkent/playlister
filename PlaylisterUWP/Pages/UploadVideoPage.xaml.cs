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
	using Windows.Storage;
	using Models.ViewModels;

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class UploadVideoPage : Page
	{
		public UploadVideoViewModel ViewModel { get; set; }
		public UploadVideoPage()
		{
			this.InitializeComponent();
			this.ViewModel = new UploadVideoViewModel();
			this.DataContext = ViewModel;
			var localSettings = ApplicationData.Current.LocalSettings;
			var authToken = (string)localSettings.Values["authToken"];
			if (string.IsNullOrEmpty(authToken))
			{
				
			ViewModel.IsLoggedIn = true;
			}
			else
			{
				ViewModel.IsLoggedIn = false;
			}
		}
	}
}
