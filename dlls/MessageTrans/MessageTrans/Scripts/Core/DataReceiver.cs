using System.Runtime.InteropServices;
using System;
using MessageTrans.Interal;

namespace MessageTrans
{

    public class DataReceiver : IDataReceiver
    {
        //钩子
        private int idHook = 0;
        //是否安装了钩子
        private bool isHook = false;
        private GCHandle gc;
        private EventHolder eholder;
        public Action<string> MessageNotHandled { get { return eholder.MessageNotHandled; } set { eholder.MessageNotHandled = value; } }
        public event Action<string> OnError;

        public DataReceiver()
        {
            eholder = new Interal.EventHolder();
        }
      
        //钩子回调
        private unsafe int Hook(int nCode, int wParam, int lParam)
        {
            try
            {
                IntPtr p = new IntPtr(lParam);
                CWPSTRUCT m = (CWPSTRUCT)Marshal.PtrToStructure(p, typeof(CWPSTRUCT));

                if (m.message == 74)
                {
                    COPYDATASTRUCT entries = (COPYDATASTRUCT)Marshal.PtrToStructure((IntPtr)m.lparam, typeof(COPYDATASTRUCT));
                    IPC_Buffer entries1 = (IPC_Buffer)Marshal.PtrToStructure((IntPtr)entries.lpData, typeof(IPC_Buffer));
                    IntPtr intp = new IntPtr(entries1.cbBuffer);
                    string str = new string((sbyte*)intp);
                    OnReceived(str);
                }
                return DataUtility.CallNextHookEx(idHook, nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                if (OnError != null) OnError(ex.Message);
                return 0;
            }
        }

        public bool RegistHook()
        {
            return DataUtility.HookLoad(Hook, out idHook, out isHook, ref gc);
        }

        public bool RemoveHook()
        {
            if (isHook)
            {
               return DataUtility.UnhookWindowsHookEx(idHook);
            }
            return true;
        }

        public void RegisterEvent(string key, Action action)
        {
            eholder.AddDelegate(key, action);
        }
        public void RemoveEvent(string key, Action action)
        {
            eholder.RemoveDelegate(key, action);
        }
        public void RegisterEvent(string key, Action<string> action)
        {
            eholder.AddDelegate(key, action);
        }
        public void RemoveEvent(string key, Action<string> action)
        {
            eholder.RemoveDelegate(key, action);
        }
        private void OnReceived(string data)
        {
            eholder.NotifyObserver(data);
        }

    }
}

