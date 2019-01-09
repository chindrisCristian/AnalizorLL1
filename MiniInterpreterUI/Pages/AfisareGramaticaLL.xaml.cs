using System.Windows.Controls;

namespace MiniInterpreterUI
{
	/// <summary>
	/// Interaction logic for AfisareGramaticaLL.xaml
	/// </summary>
	public partial class AfisareGramaticaLL : UserControl
	{
		public AfisareGramaticaLL(BaseViewModel viewModel)
		{
			InitializeComponent();
			DataContext = viewModel as AfisareGramaticaLLVM;
		}

		private void TasDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
		{
			e.Column.Header = (DataContext as AfisareGramaticaLLVM).TableResults.Columns[e.PropertyName].Caption;
		}
	}
}
