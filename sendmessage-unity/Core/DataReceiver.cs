using Newtonsoft.Json;
using UnityEngine;
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
        private bool _bCallNext;
        public bool CallNextProc
        {
            get { return _bCallNext; }
            set { _bCallNext = value; }
        }
        private JsonSerializerSettings setting;

        public DataReceiver()
        {
            setting = new JsonSerializerSettings();
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
                if (CallNextProc)
                {
                    return DataUtility.CallNextHookEx(idHook, nCode, wParam, lParam);
                }
                else
                {
                    //return 1;
                    return DataUtility.CallNextHookEx(idHook, nCode, wParam, lParam);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return 0;
            }
        }

        public void RegistHook()
        {
            DataUtility.HookLoad(Hook, out idHook, out isHook, ref gc);
        }

        public void RemoveHook()
        {
            if (isHook)
            {
                DataUtility.UnhookWindowsHookEx(idHook);
            }
        }

        public void OnReceived(string data)
        {
            EventHolder.NotifyObserver(data);
        }

    }
}

