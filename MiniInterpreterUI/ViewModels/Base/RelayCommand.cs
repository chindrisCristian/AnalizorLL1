using System;
using System.Windows.Input;

namespace MiniInterpreterUI
{
	public class RelayCommand : ICommand
	{
		#region Private members
		private Action mAction;
		#endregion

		#region Constructor
		public RelayCommand(Action action) => mAction = action;
		#endregion

		#region Public events
		/// <summary>
		/// The event that is fired when the <see cref="CanExecute(object)"/> value has changed
		/// </summary>
		public event EventHandler CanExecuteChanged = (sender, e) => { };
		#endregion

		#region Command methods
		/// <summary>
		/// A RelayCommand can always execute
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public bool CanExecute(object parameter) => true;

		/// <summary>
		/// Execute the commands Action
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			mAction();
		}
		#endregion
	}
}
