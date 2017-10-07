using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaylisterUWP.Models.ViewModels
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;

	public class UploadVideoViewModel : INotifyPropertyChanged
	{
		// These fields hold the values for the public properties.
		private bool _isLoggedIn;

		public event PropertyChangedEventHandler PropertyChanged;

		// This method is called by the Set accessor of each property.
		// The CallerMemberName attribute that is applied to the optional propertyName
		// parameter causes the property name of the caller to be substituted as an argument.
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
		
		public UploadVideoViewModel()
		{
			_isLoggedIn = false;
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
	}
}
