using Clipboard.Controllers;
using Clipboard.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        ClipboardController clipboardController;

        public ClipboardView()
        {
            clipboardIcon.Visible = true;
            clipboardIcon.Icon = new System.Drawing.Icon(@"Resources\Icon1.ico");
            InitializeComponent();
           

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new HotkeyController.HotKey(Key.V, HotkeyController.KeyModifier.Shift | HotkeyController.KeyModifier.Ctrl, OnHotKeyPaste);
            clipboardController = new ClipboardController(this);
            clipboardController.ClipboardChanged += ClipboardController_ClipboardChanged;
        }

        private void ClipboardController_ClipboardChanged(object sender, EventArgs e)
        {
            if(System.Windows.Forms.Clipboard.ContainsText())
            {

            }
            Debug.WriteLine(System.Windows.Forms.Clipboard.GetText());
        }

        private void OnHotKeyPaste(HotkeyController.HotKey obj)
        {
            //clipboardController.ClipboardChanged -= ClipboardController_ClipboardChanged;
            System.Windows.Forms.Clipboard.SetText("Test");

            clipboardController?.Paste();
            //clipboardController.ClipboardChanged += ClipboardController_ClipboardChanged;
            // this.Show();
        }

    }
}
