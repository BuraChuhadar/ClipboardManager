using ClipboardManager.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            ContextMenu.MenuItems.Add("&Close");
            ContextMenu.MenuItems[0].Click += ContextMenuController_Click;
            ContextMenu.MenuItems[1].Click += ContextMenuController_Click1;
        }

        private void ContextMenuController_Click1(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        private void ContextMenuController_Click(object sender, EventArgs e)
        {            
            if (!App.Current.Windows.OfType<Options>().Any())
            {
                var options = new Options();
                options.Show();
            }
        }
    }
}
