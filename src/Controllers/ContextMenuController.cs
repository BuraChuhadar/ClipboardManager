using ClipboardManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ClipboardManager.Controllers
{
    public class ContextMenuController
    {
        public ContextMenu ContextMenu { get; set; } = new ContextMenu();
        
        public ContextMenuController()
        {
            BuildContextMenu();
        }

        public void BuildContextMenu()
        {
            ContextMenu.MenuItems.Add("&Options");
            ContextMenu.MenuItems.Add("&Exit");
            ContextMenu.MenuItems[0].Click += ContextMenuController_Options_Click;
            ContextMenu.MenuItems[1].Click += ContextMenuController_Exit_Click;
        }

        private static void OpenMenu<T>() where T : Window, new()
        {
            if (!App.Current.Windows.OfType<T>().Any())
            {
                var about = new T();
                about.Show();
            }
        }

        private void ContextMenuController_Exit_Click(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ContextMenuController_Options_Click(object sender, EventArgs e)
        {
            OpenMenu<Options>();
        }
    }
}
