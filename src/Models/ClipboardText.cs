using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardManager.Models
{
    class ClipboardText : ClipboardData, IClipboardData<string>
    {
        public string Data { get; }
        public string Preview { get;  }

        public ClipboardText(string value)
        {
            Data = value;
            if(value?.Length >= 100)
            {
                Preview = value?.TrimStart()?.Substring(0, 100) + "...";
            }
            else
            {
                Preview = value?.TrimStart();
            }
        }
    }
}
