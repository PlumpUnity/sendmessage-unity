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
        bool Start;

        public UnityLoder(MainWindow window,Grid leftUp, StackPanel ScalePanel)
        {
            this.window = window;
            this.ScalePanel = ScalePanel;
            this.leftUp = leftUp;

            window.SizeChanged += AdaptWindow;
            window.Closed +=(x,y)=> { app.Kill(); };
        }
        public void Init3dScene()
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = UnityexeName;
            info.WindowStyle = ProcessWindowStyle.Minimized;
            app = Process.Start(info);

            while (app.MainWindowHandle == IntPtr.Zero)
            {
                app.WaitForInputIdle();
                app.Refresh();
            }
            //没代码提示
           window.Dispatcher.BeginInvoke(
           System.Windows.Threading.DispatcherPriority.Loaded, new Action<object,EventArgs>(LoadThreed), this, null);
        }
        /// <summary>
        /// 加载三维场景 
        /// </summary>
        private void LoadThreed(object ob, EventArgs es) { 
            WindowInteropHelper helper = new WindowInteropHelper(window);
            IntPtr ptr = helper.Handle;
            Win32API.SetParent(app.MainWindowHandle, ptr);
            Win32API.SetWindowLong(new HandleRef(this, app.MainWindowHandle), Win32API.GWL_STYLE, Win32API.WS_VISIBLE);
            Start = true;
            AdaptWindow(null,null);
        }
        private void AdaptWindow(object sender, EventArgs e)
        {
            if (!Start){
                return;
            }

            Win32API.MoveWindow(app.MainWindowHandle, (int)leftUp.ActualWidth,(int)leftUp.ActualHeight, (int)ScalePanel.ActualWidth, (int)ScalePanel.ActualHeight, true);
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