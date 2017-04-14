using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using ControlProgram.Plugins;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Threading;
using MessageTrans;
namespace ControlProgram
{
    class UnityLoder
    {
        string UnityexeName{
            get{
                return ControlProgram.Fixed.AppPath.ThreeDExePath;
            }
        }
        private Process app;
        private Panel ScalePanel;
        private Window window;
        private Grid leftUp;
        private IntPtr unityHWND;
        private IntPtr windowHandle;
        private DataSender sender;

        public UnityLoder(MainWindow window,Grid leftUp, StackPanel ScalePanel)
        {
            this.window = window;
            this.ScalePanel = ScalePanel;
            this.leftUp = leftUp;

            window.SizeChanged += AdaptWindow;
            window.Closed +=(x,y)=> {
                app.Kill();
                app.Close();
                app.Dispose();
            };
            sender = new DataSender();
        }
        public void Init3dScene(string path)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = path;
            info.CreateNoWindow = true;
            info.UseShellExecute = true;
            IntPtr mainHandle = new WindowInteropHelper(window).Handle;
            HwndSource source = PresentationSource.FromDependencyObject(ScalePanel) as HwndSource;
            windowHandle = source.Handle;
            Console.WriteLine(windowHandle.ToString());
            Console.WriteLine(mainHandle.ToString());
            info.Arguments = "-parentHWND " + windowHandle.ToInt32();
            app = Process.Start(info);
            app.WaitForInputIdle();
            Win32API.EnumChildWindows(windowHandle, WindowEnum, IntPtr.Zero);
            AdaptWindow(null, null);
        }

        public void CloseWindow()
        {
            if (app != null)
            {
                app.Kill();
                app.Dispose();
                app.Close();
            }
           
        }
        public void SendInfo(string str)
        {
            sender.RegistHandle(unityHWND);
            sender.SendMessage("simpleText", str);
        }
        private void AdaptWindow(object sender, EventArgs e)
        {
            ActivateUnityWindow();
            Win32API.MoveWindow(unityHWND, (int)leftUp.ActualWidth,(int)leftUp.ActualHeight, (int)ScalePanel.ActualWidth, (int)ScalePanel.ActualHeight, true);
        }
        internal void ActivateUnityWindow()
        {
            Win32API.SendMessage(unityHWND, Win32API.WM_ACTIVATE, Win32API.WA_ACTIVE, IntPtr.Zero);
        }

        internal void DeactivateUnityWindow()
        {
            Win32API.SendMessage(unityHWND, Win32API.WM_ACTIVATE, Win32API.WA_INACTIVE, IntPtr.Zero);
        }

        private int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            unityHWND = hwnd;
            ActivateUnityWindow();
            return 0;
        }

    }
}



//while(true)
//{
//    app.Refresh();
//    app.WaitForInputIdle();
//    if (app.MainWindowHandle != null)
//    {
//        break;
//    }
//}
//UnityLoaded();