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
using ClipboardManager.Models;
using System.Collections.ObjectModel;
using ClipboardManager.Utils;

namespace ClipboardManager.Views
{
    /// <summary>
    /// Interaction logic for Clipboard.xaml
    /// </summary>
    public partial class ClipboardView : Window
    {

        ClipboardController clipboardController;
        public ObservableCollection<ClipboardData> clipboardDataList = new ObservableCollection<ClipboardData>();


        public ICommand CmdItemPinnedDelete { get; set; }
        public ICommand CmdItemPin { get; set; }

        public ICommand CmdItemUnPin { get; set; }
        public ICommand CmdItemUnPinnedDelete { get; set; }

        private bool _canBeDeActivated = true;


        public ClipboardView()
        {
            InitializeComponent();
            ClipboardPanelCurrentItems.DataContext = clipboardDataList;
            ClipboardPanelPinnedItems.DataContext = clipboardDataList;

            this.CmdItemPinnedDelete = new RelayCommand<object>(new Action<object>(this.CmdItemPinnedDelete_Click));
            this.CmdItemPin = new RelayCommand<object>(new Action<object>(this.CmdItemPin_Click));

            this.CmdItemUnPinnedDelete = new RelayCommand<object>(new Action<object>(this.CmdItemUnPinnedDelete_Click));
            this.CmdItemUnPin = new RelayCommand<object>(new Action<object>(this.CmdItemUnPin_Click));
        }

        protected async void CmdItemPinnedDelete_Click(object obj)
        {
            if(Guid.TryParse(obj?.ToString(), out Guid itemGuid))
            {
               
                var itemToDelete = ClipboardController.ClipboardRecentData.Where(c => c.Id == itemGuid).FirstOrDefault();
                if (itemToDelete != null)
                {
                    ClipboardController.ClipboardRecentData.Remove(itemToDelete);
                    await RenderClipboardItems();
                }
            }
        }


        protected async void CmdItemUnPinnedDelete_Click(object obj)
        {
            if (Guid.TryParse(obj?.ToString(), out Guid itemGuid))
            {

                var itemToDelete = ClipboardController.ClipboardPinnedData.Where(c => c.Id == itemGuid).FirstOrDefault();
                if (itemToDelete != null)
                {
                    ClipboardController.ClipboardPinnedData.Remove(itemToDelete);
                    await RenderClipboardItems();
                }
            }
        }


        protected async void CmdItemPin_Click(object obj)
        {
            if (Guid.TryParse(obj?.ToString(), out Guid itemGuid))
            {

                var itemToPin = ClipboardController.ClipboardRecentData.Where(c => c.Id == itemGuid).FirstOrDefault();
                if (itemToPin != null)
                {
                    ClipboardController.ClipboardRecentData.Remove(itemToPin);
                    ClipboardController.ClipboardPinnedData.Add(itemToPin);
                    await RenderClipboardItems();
                }
            }
        }

        protected async void CmdItemUnPin_Click(object obj)
        {
            if (Guid.TryParse(obj?.ToString(), out Guid itemGuid))
            {

                var itemToUnPin = ClipboardController.ClipboardPinnedData.Where(c => c.Id == itemGuid).FirstOrDefault();
                if (itemToUnPin != null)
                {
                    ClipboardController.ClipboardPinnedData.Remove(itemToUnPin);
                    ClipboardController.ClipboardRecentData.Add(itemToUnPin);
                    await RenderClipboardItems();
                }
            }
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
            btnCurrentItems.Tag = true;
            clipboardController.ClipboardChanged += ClipboardController_ClipboardChanged;
            this.Deactivated += ClipboardView_Deactivated;
        }

        private void ClipboardView_Deactivated(object sender, EventArgs e)
        {
            if(_canBeDeActivated)
            {
                this.Hide();
            }
        }

        #region ClipboardEvents
        private async void ClipboardController_ClipboardChanged(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Clipboard.ContainsText())
            {
                var clipboardValue = System.Windows.Forms.Clipboard.GetText();
                if (!string.IsNullOrEmpty(clipboardValue.Trim()))
                {
                    ClipboardController.ClipboardRecentData.Add(new ClipboardText(clipboardValue));
                    await RenderClipboardItems();
                }
            }
        }

        private Task<bool> RenderClipboardItems()
        {
            clipboardDataList.Clear();
            IEnumerable<ClipboardText> currentItems = null;
            if (ScrollViewerCurrentItems.Visibility == Visibility.Visible)
            {
                currentItems = ClipboardController.ClipboardRecentData.Where(c => c is ClipboardText).Cast<ClipboardText>();
            }
            else if (ScrollViewerPinned.Visibility == Visibility.Visible)
            {
                currentItems = ClipboardController.ClipboardPinnedData.Where(c => c is ClipboardText).Cast<ClipboardText>();
            }

            foreach (var clipboardData in currentItems)
            {
                clipboardDataList.Add(clipboardData);
            }
            DoScrollViewAction(ScrollViewerCurrentItems.ScrollToEnd, ScrollViewerPinned.ScrollToEnd);

            return Task.FromResult(true);

            void DoScrollViewAction(Action ScrollViewerCurrentItemsTask, Action ScrollViewerPinnedTask)
            {
                if (ScrollViewerCurrentItems.Visibility == Visibility.Visible)
                {
                    ScrollViewerCurrentItemsTask();
                }
                else if (ScrollViewerPinned.Visibility == Visibility.Visible)
                {
                    ScrollViewerPinnedTask();
                }
            }
        }

        private async void ClipboardCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            var currentCheckbox = (CheckBox)(sender);
            var currentClipboardItem = (Grid)(currentCheckbox).Parent;
            if (ScrollViewerCurrentItems.Visibility == Visibility.Visible)
            {
                await MoveClipboardItem(Operation.Add, (Guid)currentCheckbox.Tag);
            }
            else if (ScrollViewerPinned.Visibility == Visibility.Visible)
            {
                await MoveClipboardItem(Operation.Remove, (Guid)currentCheckbox.Tag);
            }
        }

        enum Operation
        {
            Add,
            Remove
        }

        private async Task<bool> MoveClipboardItem(Operation operation, Guid clipboardId)
        {
            ClipboardData clipboardItem = null;
            if (ScrollViewerCurrentItems.Visibility == Visibility.Visible)
            {
                clipboardItem = ClipboardController.ClipboardRecentData.Where(c => c.Id == clipboardId).FirstOrDefault();
            }
            else if (ScrollViewerPinned.Visibility == Visibility.Visible)
            {
                clipboardItem = ClipboardController.ClipboardPinnedData.Where(c => c.Id == clipboardId).FirstOrDefault();
            }

            if (clipboardItem != null)
            {
                switch (operation)
                {
                    case Operation.Add:
                        ClipboardController.ClipboardPinnedData.Add(clipboardItem);
                        ClipboardController.ClipboardRecentData.Remove(clipboardItem);
                        break;
                    case Operation.Remove:
                        ClipboardController.ClipboardRecentData.Add(clipboardItem);
                        ClipboardController.ClipboardPinnedData.Remove(clipboardItem);
                        break;
                }
            }
            return await RenderClipboardItems();
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
            if (mouse.Y >= resolution.Height / 2)
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

        private void BtnCurrentItems_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerPinned.Visibility = Visibility.Collapsed;
            ClipboardPanelPinnedItems.Visibility = Visibility.Collapsed;

            ScrollViewerCurrentItems.Visibility = Visibility.Visible;
            ClipboardPanelCurrentItems.Visibility = Visibility.Visible;


            ToggleButton(sender);
        }

        private void ToggleButton(object sender)
        {
            foreach (var menuComponent in Menu.Children)
            {
                if (menuComponent is Button)
                {
                    ((Button)menuComponent).Tag = false;
                }
            }
            ((Button)(sender)).Tag = true;
            RenderClipboardItems();
        }

        private void BtnPinnedItems_Click(object sender, RoutedEventArgs e)
        {
            ScrollViewerPinned.Visibility = Visibility.Visible;
            ClipboardPanelPinnedItems.Visibility = Visibility.Visible;

            ScrollViewerCurrentItems.Visibility = Visibility.Collapsed;
            ClipboardPanelCurrentItems.Visibility = Visibility.Collapsed;

            ToggleButton(sender);
        }


        private void BtnSettings_Click(object sender, RoutedEventArgs e)
        {
            this._canBeDeActivated = false;
            var options = new Options();
            if(options.ShowDialog() != null)
            {
                this._canBeDeActivated = true;
            }
        }
        
        private void ClipboardPanelCurrentItems_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ItemsControl.ContainerFromElement(ClipboardPanelCurrentItems, e.OriginalSource as DependencyObject) is ListBoxItem item)
            {
                if (Guid.TryParse(((ClipboardManager.Models.ClipboardData)item?.DataContext)?.Id.ToString(), out Guid itemGuid))
                {
                    var itemToPaste = (ClipboardText)ClipboardController.ClipboardRecentData.Where(c => c.Id == itemGuid).FirstOrDefault();
                    if (itemToPaste != null)
                    {
                        clipboardController?.UnRegisterListener(); //Stop listening because we are injecting our clipboard message to the clipboard list.
                        System.Windows.Forms.Clipboard.SetText(itemToPaste.Data);
                        clipboardController?.RegisterListenter(); //Re-register the listener
                        this.Hide();
                        clipboardController?.Paste();
                    }
                }
            }
        }
    }
}
