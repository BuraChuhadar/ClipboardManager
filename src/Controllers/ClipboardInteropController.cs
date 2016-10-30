using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Clipboard.Controllers
{
    class ClipboardInteropController
    {
        [DllImport("user32.dll")]
        internal static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("user32.dll")]
        internal static extern bool CloseClipboard();

        [DllImport("user32.dll")]
        internal static extern bool SetClipboardData(uint uFormat, IntPtr data);
        
    }
}
