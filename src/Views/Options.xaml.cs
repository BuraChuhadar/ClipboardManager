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
using ClipboardManager.Controllers;

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
                    Properties.Settings.Default.LoadOnStartup = setStartUp;
                    Properties.Settings.Default.Save();
                }
            }
        }

        
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            //Save settings
            Properties.Settings.Default.LoadOnStartup = chckLoadStartup.IsChecked.Value;
            Properties.Settings.Default.ShortcutKeys = txtShortcutKeys.Text;
            Properties.Settings.Default.Save();
            SetStartup(Properties.Settings.Default.LoadOnStartup);

            //Set New Modifiers
            var stringKeys = Properties.Settings.Default.ShortcutKeys.Split('+').Select(c=> c.Trim());
            var keys = Utiliies.ParseKeys(stringKeys).ToList();
            var keyModifierList = keys.Where(c=> Utiliies.ConvertToModifierKey(c) != ModifierKeys.None);
            var key = keys.Except(keyModifierList).FirstOrDefault();

            var keyModifiers = ModifierKeys.None;
            foreach (var keyModifier in keyModifierList)
            {
                keyModifiers |= Utiliies.ConvertToModifierKey(keyModifier);
            }

            HotkeyController.Hotkey.SetModifierKeys(key, keyModifiers);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            chckLoadStartup.IsChecked = Properties.Settings.Default.LoadOnStartup;
            txtShortcutKeys.Text = Properties.Settings.Default.ShortcutKeys;
        }

        private void TxtShortcutKeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (!txtShortcutKeys.Text.Contains(e.Key.ToString()))
            {
                if(txtShortcutKeys.Text == txtShortcutkeyPlaceholder)
                {
                    txtShortcutKeys.Text = "";
                }
                

                if (string.IsNullOrEmpty(txtShortcutKeys.Text))
                {
                    txtShortcutKeys.Text = e.Key.ToString();
                }
                else
                {
                    txtShortcutKeys.Text += $" + {e.Key.ToString()}";
                }

                if (!string.IsNullOrEmpty(txtShortcutKeys.Text))
                {
                    var stringKeys = txtShortcutKeys.Text.Split('+').Select(c => c.Trim());
                    var keys = Utiliies.ParseKeys(stringKeys).ToList();
                    var keyModifierList = keys.Where(c => Utiliies.ConvertToModifierKey(c) != ModifierKeys.None);
                    var key = keys.Except(keyModifierList).LastOrDefault();
                    var currentKeySet = "";
                    foreach (var modifierKey in keyModifierList)
                    {
                        currentKeySet += modifierKey + " + ";
                    }
                    currentKeySet += $" {key}";
                    txtShortcutKeys.Text = currentKeySet;
                }
            }
        }

        string txtShortcutkeyPlaceholder = "Press your key combination";
        private void TxtShortcutKeys_GotFocus(object sender, RoutedEventArgs e)
        {
            txtShortcutKeys.Text = txtShortcutkeyPlaceholder;
        }

        private void TxtShortcutKeys_LostFocus(object sender, RoutedEventArgs e)
        {
            if(txtShortcutKeys.Text == "")
            {
                txtShortcutKeys.Text = Properties.Settings.Default.ShortcutKeys;
            }
        }
        

        private void TxtShortcutKeys_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
