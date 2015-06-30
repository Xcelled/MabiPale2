using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MabiPale2.Plugins.EntityLogger
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public CollectionView Entities { get; private set; }

		private readonly ObservableCollection<Entities.Entity> _source;

		public MainWindow(ObservableCollection<Entities.Entity> ents)
		{
			_source = ents;

			Entities = CollectionViewSource.GetDefaultView(_source) as CollectionView;

			DataContext = this;

			InitializeComponent();
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			this.Hide();
		}

		private void Clear_Click(object sender, RoutedEventArgs e)
		{
			_source.Clear();
		}

		private void Info_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Entity Logger reads all logged packets and displays information about the creatures and props found.", Title, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}
	}
}
