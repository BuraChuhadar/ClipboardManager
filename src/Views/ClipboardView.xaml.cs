using Clipboard.Controllers;
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

namespace Clipboard.Views
{
    /// <summary>
    /// Interaction logic for Clipboard.xaml
    /// </summary>
    public partial class ClipboardView : Window
    {
        System.Windows.Forms.NotifyIcon clipboardIcon = new System.Windows.Forms.NotifyIcon();
        public ClipboardView()
        {
            clipboardIcon.Visible = true;
            clipboardIcon.Icon = new System.Drawing.Icon(@"Resources\Icon1.ico");
            clipboardIcon.ShowBalloonTip(5000, "Title", "Text", System.Windows.Forms.ToolTipIcon.Info);
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            var hotkeyController = new HotkeyController.HotKey(Key.V, HotkeyController.KeyModifier.Shift | HotkeyController.KeyModifier.Ctrl, OnHotKeyHandler);
        }

        private void OnHotKeyHandler(HotkeyController.HotKey obj)
        {
            this.Show();
        }
    }
}
