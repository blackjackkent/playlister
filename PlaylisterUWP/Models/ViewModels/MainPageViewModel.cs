namespace PlaylisterUWP.Models.ViewModels
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	public class MainPageViewModel : INotifyPropertyChanged
    {
	    // These fields hold the values for the public properties.
	    private bool _isLoggedIn;
	    private ObservableCollection<YouTubeUploadViewModel> _recentUploads;

		public event PropertyChangedEventHandler PropertyChanged;

	    // This method is called by the Set accessor of each property.
	    // The CallerMemberName attribute that is applied to the optional propertyName
	    // parameter causes the property name of the caller to be substituted as an argument.
	    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
	    {
		    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	    }

	    public MainPageViewModel()
	    {
		    _isLoggedIn = false;
			_recentUploads = new ObservableCollection<YouTubeUploadViewModel>(new List<YouTubeUploadViewModel>());
	    }

	    public bool IsLoggedIn
	    {
		    get => _isLoggedIn;

		    set
		    {
			    if (value == _isLoggedIn) return;
			    _isLoggedIn = value;
			    NotifyPropertyChanged();
		    }
	    }

	    public ObservableCollection<YouTubeUploadViewModel> RecentUploads
	    {
		    get => _recentUploads;

		    set
		    {
			    if (value == _recentUploads) return;
			    _recentUploads = value;
			    NotifyPropertyChanged();
		    }
	    }
	}
}
