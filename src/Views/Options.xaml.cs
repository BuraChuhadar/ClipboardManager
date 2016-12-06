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
using Microsoft.Win32;

namespace Clipboard.Views
{
    /// <summary>
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
        }

        private void SetStartup(bool setStartUp)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (setStartUp)
            {
                rk.SetValue(this.Name, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                rk.DeleteValue(this.Name, false);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LoadOnStartup = chckLoadStartup.IsChecked.Value;
            Properties.Settings.Default.Save();
            SetStartup(Properties.Settings.Default.LoadOnStartup);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chckLoadStartup.IsChecked = Properties.Settings.Default.LoadOnStartup;
        }
    }
}
