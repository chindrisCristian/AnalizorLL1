using System.ComponentModel;

using PropertyChanged;

namespace MiniInterpreterUI
{
	/// <summary>
	/// A base view model that fires property changed events as needed
	/// </summary>
	[AddINotifyPropertyChangedInterface]
	public class BaseViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// The event that is fired when any child property changes its value
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

		/// <summary>
		/// Call this to fire a<see cref="PropertyChanged"/> event
		/// </summary>
		/// <param name="name"></param>
		public void OnPropertyChanged(string name)
		{
			PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

	}
}
