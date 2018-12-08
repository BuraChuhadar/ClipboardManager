using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardManager.Models
{
    public abstract class ClipboardData
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}
