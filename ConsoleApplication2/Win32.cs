using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace psHUD
{
    /// <summary>
    /// Summary description for Win32.
    /// </summary>
    public class User32
    {
        [DllImport("user32", SetLastError = true)]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32", SetLastError = true)]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey,
           byte bAlpha, uint dwFlags);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern System.UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
    }

    public class Dwanoi
    {
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

    }

   

    [StructLayout(LayoutKind.Sequential)]
    public struct DWM_BLURBEHIND
    {
        public DwmBlurBehindDwFlags dwFlags;
        public bool fEnable;
        public IntPtr hRgnBlur;
        public bool fTransitionOnMaximized;
    }
    [Flags()]
    public enum DwmBlurBehindDwFlags : uint
    {
        DWM_BB_ENABLE = 0x1,
        DWM_BB_BLURREGION = 0x2,
        DWM_BB_TRANSITIONONMAXIMIZED = 0x4
    }

    /// <summary>
    /// Defines a delegate for Message handling
    /// </summary>
    public delegate void MessageEventHandler(object Sender, ref Message msg, ref bool Handled);

    /// <summary>
    /// Inherits from System.Windows.Form.NativeWindow. Provides an Event for Message handling
    /// </summary>
    public class NativeWindowWithEvent : System.Windows.Forms.NativeWindow
    {
        public event MessageEventHandler ProcessMessage;
        protected override void WndProc(ref Message m)
        {
            if (ProcessMessage != null)
            {
                bool Handled = false;
                ProcessMessage(this, ref m, ref Handled);
                if (!Handled) base.WndProc(ref m);
            }
            else base.WndProc(ref m);
        }
    }

    /// <summary>
    /// Inherits from NativeWindowWithEvent and automatic creates/destroys of a dummy window
    /// </summary>
    public class DummyWindowWithEvent : NativeWindowWithEvent, IDisposable
    {
        public DummyWindowWithEvent()
        {
            CreateParams parms = new CreateParams();
            // this.CreateHandle(parms);
        }
        public void Dispose()
        {
            if (this.Handle != (IntPtr)0)
            {
                this.DestroyHandle();
            }
        }
    }
}
