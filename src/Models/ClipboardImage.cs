using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardManager.Models
{
    class ClipboardImage : ClipboardData, IClipboardData<Image>
    {
        public Image Data { get; }

        public ClipboardImage(Image value)
        {
            Data = value;
        }
    }
}
