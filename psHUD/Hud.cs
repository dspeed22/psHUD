using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.PowerShell;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;
using System.Collections;

namespace psHUD
{
    class Hud
    {
        bool isHidden = false;
        bool isRunning = true;
        bool hotKeyPressed = false;
        int keyId;
        AutoResetEvent hotKeyEvent;
        #region PInvoke

        private IntPtr windowHandle;
        private const int HWND_TOPMost = -1;
        private const int SWP_NOSIZE = 0x0001;
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYERED = 0x80000;
        private const int LWA_ALPHA = 0x2;
        private int tildeKey = 0xC0;

        void MakeTransparent(byte pct)
        {

            int newDwLong = ((int)User32.GetWindowLong(windowHandle, GWL_EXSTYLE)) ^ WS_EX_LAYERED;
            User32.SetWindowLong(windowHandle, GWL_EXSTYLE, newDwLong);
            User32.SetLayeredWindowAttributes(windowHandle, 0, pct, LWA_ALPHA);
        }
        #endregion


        public void Init(string[] args)
        {
            hotKeyEvent = new AutoResetEvent(false);
            windowHandle = Process.GetCurrentProcess().MainWindowHandle;
            SetupHotkey();
            SetupConsole();

            Thread p = new Thread(new ParameterizedThreadStart(InitPowershell));
            p.IsBackground = true;
            p.Start(args);

            while (isRunning)
            {
                if (hotKeyPressed)
                    ToggleWindow();
                hotKeyEvent.WaitOne();
            }

            Cleanup();
        }

        private void Cleanup()
        {
            HotKeyManager.UnregisterHotKey(keyId);
        }

        private void SetupHotkey()
        {

            keyId = HotKeyManager.RegisterHotKey((Keys)tildeKey, KeyModifiers.Control);
            HotKeyManager.HotKeyPressed += new EventHandler<HotKeyEventArgs>(HotKeyManager_HotKeyPressed);
        }


        void HotKeyManager_HotKeyPressed(object sender, HotKeyEventArgs e)
        {
            hotKeyPressed = true;
            hotKeyEvent.Set();
        }

        private void ToggleWindow()
        {

            //doing the 2 step process to give the illusion of animating.
            //also make the window grab/release focus
            if (isHidden)
            {
                User32.ShowWindow(windowHandle, 5);//show
                User32.ShowWindow(windowHandle, 9);//restore
            }
            else
            {
                User32.ShowWindow(windowHandle, 6);//minimize
                User32.ShowWindow(windowHandle, 0);//hide
            }
            isHidden = !isHidden;
            hotKeyPressed = false;
        }

        private void InitPowershell(object args)
        {
            var config = RunspaceConfiguration.Create();
            string[] arg = (string[])args;
            ConsoleShell.Start(
                config,
                "psHUD: a Powershell heads up display (v0.1)",
                "",
                arg
            );

            isRunning = false;
            hotKeyEvent.Set();

        }

        private void SetupConsole()
        {
            Console.Title = "psHUD";
            Console.SetWindowSize(Console.LargestWindowWidth - 5, 25);
            MakeTransparent(210);  //0 to 255
            Setposition(1, 0);

        }

        private void Setposition(int x, int y)
        {

            User32.SetWindowPos(windowHandle,
            new IntPtr(HWND_TOPMost),
            x, y, 0, 0,
            SWP_NOSIZE);
        }
    }
}
