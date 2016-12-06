using Clipboard.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clipboard.Controllers
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
            ContextMenu.MenuItems[0].Click += ContextMenuController_Click;
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
