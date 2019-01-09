using System.Windows.Controls;

namespace MiniInterpreterUI
{
	public class Switcher
    {
		public static MainWindow pageSwitcher;

		public static void Switch(UserControl newPage) => pageSwitcher.Navigate(newPage);
	}
}
