using Client;
using GameOfNougtsAndCrossesLib;
using MyLibraryNP;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace NoughtsAndCrosses
{
    /// <summary>
    /// Логика взаимодействия для Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        PlayerViewModel viewModel = new PlayerViewModel(new Player("", PlayerMode.CrossMode, new SocketInfo(), false));
        public Config()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Button_Accept_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow((PlayerViewModel)DataContext);
            Visibility = Visibility.Hidden;
            mainWindow.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (localHostCheckBox.IsChecked == true)
                UseLocalHost();
            else
                CancelUseLocalHost();
        }

        private void UseLocalHost()
        {
            viewModel.IpAddress = LocalHost.IpAddress;
            ipAdd.IsReadOnly = true;
            viewModel.Port = LocalHost.Port;
            port.IsReadOnly = true;
        }

        private void CancelUseLocalHost()
        {
            ipAdd.Text = "";
            ipAdd.IsReadOnly = false;
            port.Text = "";
            port.IsReadOnly = false;
        }
    }
}
