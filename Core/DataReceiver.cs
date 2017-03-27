using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.Text;

namespace MessageTrans
{
    public class DataReceiver : IDataReceiver
    {
        Action<string> onDataReceive;
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
                    if(onDataReceive != null) onDataReceive(str);
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
            onDataReceive = null;
            if (isHook)
            {
                DataUtility.UnhookWindowsHookEx(idHook);
            }
        }

        public void RegistReceiveEvent(Action<string> onReceive)
        {
            onDataReceive = onReceive;
        }

        public void RegistReceiveEvent<T>(Action<T> onReceive)
        {
            throw new NotImplementedException();
        }
    }
}

