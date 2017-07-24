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
using Winforms = System.Windows.Forms;

namespace ClipboardManager.Views
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
            using (var rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                try
                {
                    if (setStartUp)
                    {
                        rk.SetValue(Winforms.Application.ProductName, $@"""{Winforms.Application.ExecutablePath}""");

                    }
                    else
                    {
                        rk.DeleteValue(Winforms.Application.ProductName, false);
                    }
                }
                finally
                {
                    Properties.Settings.Default.LoadOnStartup = chckLoadStartup.IsChecked.Value;
                    Properties.Settings.Default.Save();
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
           
            SetStartup(Properties.Settings.Default.LoadOnStartup);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chckLoadStartup.IsChecked = Properties.Settings.Default.LoadOnStartup;
        }
    }
}
