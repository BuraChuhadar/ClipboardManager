using ClipboardManager.Controllers;
using ClipboardManager.Models;
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

namespace ClipboardManager.Views
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
            clipboardIcon.ContextMenu = new ContextMenuController().ContextMenu;
            InitializeComponent();
           

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Hide();
            new HotkeyController.HotKey(Key.V, HotkeyController.KeyModifier.Shift | HotkeyController.KeyModifier.Ctrl, OnHotKeyPressed);
            clipboardController = new ClipboardController(this);
            clipboardController.ClipboardChanged += ClipboardController_ClipboardChanged;
            this.Deactivated += ClipboardView_Deactivated;
        }

        private void ClipboardView_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void ClipboardController_ClipboardChanged(object sender, EventArgs e)
        {
            if(System.Windows.Forms.Clipboard.ContainsText())
            {
                var clipboardValue = System.Windows.Forms.Clipboard.GetText();
                if(!string.IsNullOrEmpty(clipboardValue.Trim()))
                {
                    var clipboardTextblock = new TextBlock();
                    clipboardTextblock.ToolTip = new ToolTip().Content = clipboardValue.Length > 100 ? $@"{clipboardValue.Substring(0, 100)} ..." : clipboardValue;
                    clipboardTextblock.TextTrimming = TextTrimming.CharacterEllipsis;
                    clipboardTextblock.TextWrapping = TextWrapping.NoWrap;
                    clipboardTextblock.Height = 20;
                    clipboardTextblock.Text = clipboardValue;
                    clipboardTextblock.MouseUp += ClipboardTextBlock_MouseUp;
                    clipboardTextblock.MouseEnter += ClipboardTextBlock_MouseEnter;
                    clipboardTextblock.MouseLeave += ClipboardTextBlock_MouseLeave;
                    this.ClipboardPanel.Children.Add(clipboardTextblock);
                    this.ScrollViewer.ScrollToEnd();
                }   
            }
        }

        #region LabelActions
        private void ClipboardTextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            var ClipboardTextBlock = (TextBlock)sender;
            ClipboardTextBlock.Background = Brushes.Transparent;
            ClipboardTextBlock.Foreground = Brushes.Black;
        }

        private void ClipboardTextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            var ClipboardTextBlock = (TextBlock)sender;
            ClipboardTextBlock.Background = Brushes.Blue;
            ClipboardTextBlock.Foreground = Brushes.White;
        }

        private void ClipboardTextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
          
            var ClipboardTextBlock = (TextBlock)sender;
            clipboardController.UnRegisterListener(); //Stop listening because we are injecting our clipboard message to the clipboard list.
            System.Windows.Forms.Clipboard.SetText(ClipboardTextBlock.Text.ToString());
            clipboardController.RegisterListenter(); //Re-register the listener
            this.Hide();
            clipboardController?.Paste();
        }
        #endregion

        private void OnHotKeyPressed(HotkeyController.HotKey obj)
        {
            MoveToMousePosition();
            this.Show();
            this.Activate();
        }


        private void MoveToMousePosition()
        {
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var mouse = transform.Transform(GetMousePosition());
            var resolution = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            if (mouse.Y >= resolution.Height/2)
            {
                Top = mouse.Y - this.Height + 1;
            }
            else
            {
                Top = mouse.Y + 1;
            }
            Left = mouse.X + 1;
        }

        public System.Windows.Point GetMousePosition()
        {
            System.Drawing.Point point = System.Windows.Forms.Control.MousePosition;
            return new System.Windows.Point(point.X, point.Y);
        }
    }
}
