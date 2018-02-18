using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClipboardManager.Controllers
{
    class Utiliies
    {
        public static IEnumerable<Key> ParseKeys(IEnumerable<string> stringKeys)
        {
            foreach (var stringKey in stringKeys)
            {
                if (Enum.TryParse(stringKey, out Key key))
                {
                    yield return key;
                }
            }
        }

        public static ModifierKeys ConvertToModifierKey(Key keyValue)
        {
            switch (keyValue)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    return ModifierKeys.Control;
                case Key.LeftAlt:
                case Key.RightAlt:
                    return ModifierKeys.Alt;
                case Key.LeftShift:
                case Key.RightShift:
                    return ModifierKeys.Shift;
                case Key.LWin:
                case Key.RWin:
                    return ModifierKeys.Windows;
                default:
                    return ModifierKeys.None;
            }
        }
    }
}
