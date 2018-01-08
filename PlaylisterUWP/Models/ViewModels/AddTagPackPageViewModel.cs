namespace PlaylisterUWP.Models.ViewModels
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	public class AddTagPackPageViewModel : INotifyPropertyChanged
	{
		// These fields hold the values for the public properties.
		private string _tagPackTitle;

		private string _tagPackTags;

		public event PropertyChangedEventHandler PropertyChanged;

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public string TagPackTitle
		{
			get => _tagPackTitle;

			set
			{
				if (value == _tagPackTitle) return;
				_tagPackTitle = value;
				NotifyPropertyChanged();
			}
		}

		public string TagPackTags
		{
			get => _tagPackTags;

			set
			{
				if (value == _tagPackTags) return;
				_tagPackTags = value;
				NotifyPropertyChanged();
			}
		}
	}
}
