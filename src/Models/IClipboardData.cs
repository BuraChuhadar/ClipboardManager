using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardManager.Models
{
    public interface IClipboardData<T> where T : class
    { 
        T Data { get; }
    }
}
