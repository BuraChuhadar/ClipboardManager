using ClipboardManager.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace ClipboardManager.Views
{
    /// <summary>
    /// Interaction logic for Clipboard.xaml
    /// </summary>
    public partial class ClipboardView : Window
    {
        
        ClipboardController clipboardController;

        public ClipboardView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var clipboardIcon = new System.Windows.Forms.NotifyIcon()
            {
                Icon = new System.Drawing.Icon((Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), @"Resources\ApplicationIcon.ico"))),
                ContextMenu = new ContextMenuController().ContextMenu,
                Visible = true
            };
            HotkeyController.Hotkey = new HotkeyController.HotKey(Key.V, ModifierKeys.Shift | ModifierKeys.Control, OnHotKeyPressed);
            clipboardController = new ClipboardController(this);
            clipboardController.ClipboardChanged += ClipboardController_ClipboardChanged;
            this.Deactivated += ClipboardView_Deactivated;
        }

        private void ClipboardView_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        #region ClipboardEvents
        private void ClipboardController_ClipboardChanged(object sender, EventArgs e)
        {
            if(System.Windows.Forms.Clipboard.ContainsText())
            {
                var clipboardValue = System.Windows.Forms.Clipboard.GetText();
                if(!string.IsNullOrEmpty(clipboardValue.Trim()))
                {
                    var clipboardItemsGrid = new Grid();
                    clipboardItemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.9, GridUnitType.Star) });
                    clipboardItemsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.1, GridUnitType.Star) });
                    var clipboardTextblock = new TextBlock();
                    clipboardTextblock.ToolTip = new ToolTip().Content = clipboardValue.Length > 100 ? $@"{clipboardValue.Substring(0, 100)} ..." : clipboardValue;
                    clipboardTextblock.TextTrimming = TextTrimming.CharacterEllipsis;
                    clipboardTextblock.TextWrapping = TextWrapping.NoWrap;
                    clipboardTextblock.Height = 20;
                    clipboardTextblock.Text = clipboardValue;
                    Grid.SetColumn(clipboardTextblock, 0);
                    clipboardTextblock.MouseUp += ClipboardTextBlock_MouseUp;
                    clipboardTextblock.MouseEnter += ClipboardTextBlock_MouseEnter;
                    clipboardTextblock.MouseLeave += ClipboardTextBlock_MouseLeave;

                    var clipboardCheckbox = new CheckBox()
                    {
                        Width = this.Width * 0.10
                    };
                    clipboardCheckbox.Checked += ClipboardCheckbox_Checked;
                    clipboardCheckbox.Unchecked += ClipboardCheckbox_UnChecked;
                    clipboardItemsGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                    Grid.SetColumn(clipboardCheckbox, 1);

                    DockPanel.SetDock(clipboardTextblock, Dock.Left);
                    DockPanel.SetDock(clipboardCheckbox, Dock.Right);

                    clipboardItemsGrid.Children.Add(clipboardTextblock);
                    clipboardItemsGrid.Children.Add(clipboardCheckbox);
                    this.ClipboardPanelUnPinned.Children.Add(clipboardItemsGrid);
                    this.ScrollViewerUnPinned.ScrollToEnd();
                }   
            }
        }

        private void ClipboardCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            ClipboardPanel.RowDefinitions[0].Height = new GridLength(0.2, GridUnitType.Star);
            ClipboardPanel.RowDefinitions[1].Height = new GridLength(0.8, GridUnitType.Star);
            var currentCheckbox = (CheckBox)(sender);
            var currentClipboardItem = (Grid)(currentCheckbox).Parent;
            MoveClipboardItem(currentClipboardItem,ClipboardPanelUnPinned,ClipboardPanelPinned);
        }

        private void MoveClipboardItem(Grid currentClipboardItem,StackPanel fromPanel, StackPanel toPanel)
        {
            fromPanel.Children.Remove(currentClipboardItem);
            toPanel.Children.Insert(toPanel.Children.Count, currentClipboardItem);
        }

        private void ClipboardCheckbox_UnChecked(object sender, RoutedEventArgs e)
        {
            var currentCheckbox = (CheckBox)(sender);
            var currentClipboardItem = (Grid)(currentCheckbox).Parent;
            MoveClipboardItem(currentClipboardItem, ClipboardPanelPinned, ClipboardPanelUnPinned);
            if(ClipboardPanelPinned.Children.Count == 0)
            {
                ClipboardPanel.RowDefinitions[0].Height = GridLength.Auto;
                ClipboardPanel.RowDefinitions[1].Height = new GridLength(1, GridUnitType.Star);
            }
        }
        #endregion

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
            clipboardController?.UnRegisterListener(); //Stop listening because we are injecting our clipboard message to the clipboard list.
            System.Windows.Forms.Clipboard.SetText(ClipboardTextBlock.Text.ToString());
            clipboardController?.RegisterListenter(); //Re-register the listener
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
