using Clipboard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Clipboard.Controllers
{
    public class ClipboardController: IDisposable
    {

        private bool _disposed = false;
        Window _windowSource = null;

        internal static class NativeMethods
        {
            // See http://msdn.microsoft.com/en-us/library/ms649021%28v=vs.85%29.aspx
            public const int WM_CLIPBOARDUPDATE = 0x031D;
            public static IntPtr HWND_MESSAGE = new IntPtr(-3);

            // See http://msdn.microsoft.com/en-us/library/ms632599%28VS.85%29.aspx#message_only
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool AddClipboardFormatListener(IntPtr hwnd);

            /// <summary>
            /// Removes the given window from the system-maintained clipboard format listener list.
            /// </summary>
            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

            [DllImport("User32.dll")]
            public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool GetGUIThreadInfo(uint hTreadID, ref GUITHREADINFO lpgui);

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [StructLayout(LayoutKind.Sequential)]
            public struct GUITHREADINFO
            {
                public int cbSize;
                public int flags;
                public IntPtr hwndActive;
                public IntPtr hwndFocus;
                public IntPtr hwndCapture;
                public IntPtr hwndMenuOwner;
                public IntPtr hwndMoveSize;
                public IntPtr hwndCaret;
                public RECT rectCaret;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int iLeft;
                public int iTop;
                public int iRight;
                public int iBottom;
            }

            public const uint WM_PASTE = 0x0302;


        }

        public ClipboardController(Window windowSource)
        {
            HwndSource source = PresentationSource.FromVisual(windowSource) as HwndSource;
            _windowSource = windowSource;
            if (source == null)
            {
                throw new ArgumentException(
                    "Window source MUST be initialized first, such as in the Window's OnSourceInitialized handler."
                    , nameof(_windowSource));
            }

            source.AddHook(WndProc);

            // get window handle for interop
            IntPtr windowHandle = new WindowInteropHelper(_windowSource).Handle;

            // register for clipboard events
            NativeMethods.AddClipboardFormatListener(windowHandle);
        }

        public event EventHandler ClipboardChanged;

        IntPtr GetFocusedHandle()
        {
            var info = new NativeMethods.GUITHREADINFO();
            info.cbSize = Marshal.SizeOf(info);
            if (!NativeMethods.GetGUIThreadInfo(0, ref info))
                throw new Win32Exception();
            
            return info.hwndFocus;
        }

        public void Paste()
        {
            IntPtr hWnd = GetFocusedHandle();
            
            NativeMethods.PostMessage(hWnd, NativeMethods.WM_PASTE, IntPtr.Zero, IntPtr.Zero);
            //NativeMethods.SendMessage(hWnd, 0x000C, 0, "Test");

        }

        private void UnRegisterListener()
        {
            if (_windowSource != null)
            {
                // get window handle for interop
                IntPtr windowHandle = new WindowInteropHelper(_windowSource).Handle;
                NativeMethods.RemoveClipboardFormatListener(windowHandle);     // Remove our window from the clipboard's format listener list.
            }
            
        }

        private void OnClipboardChanged()
        {
            ClipboardChanged?.Invoke(this, EventArgs.Empty);
        }

        private static readonly IntPtr WndProcSuccess = IntPtr.Zero;

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_CLIPBOARDUPDATE)
            {
                OnClipboardChanged();
                handled = true;
            }

            return WndProcSuccess;
        }

        // ******************************************************************
        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            System.Diagnostics.Debug.WriteLine("Disposing");
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // ******************************************************************
        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be _disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be _disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this._disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    UnRegisterListener();
                }

                // Note disposing has been done.
                _disposed = true;
            }
        }
    }
}
