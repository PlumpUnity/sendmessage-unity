using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;
using System.Text;

namespace MessageTrans
{
    public static class DataUtility
    {
        //宏定义 
        public const ushort IPC_VER = 1;
        public const int IDT_ASYNCHRONISM = 0x0201;
        public const uint WM_COPYDATA = 0x004A;
        public const ushort IPC_CMD_GF_SOCKET = 1;
        public const ushort IPC_SUB_GF_SOCKET_SEND = 1;
        public const int IPC_SUB_GF_CLIENT_READY = 1;
        public const int IPC_CMD_GF_CONTROL = 2;
        public const int IPC_BUFFER = 10240;//最大缓冲长度
        public const int WH_CALLWNDPROC = 4;  //钩子类型 全局钩子

        //user32.dll中的SendMessage
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, ref COPYDATASTRUCT lParam);
        //user32.dll中的获得窗体句柄
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);
        //建立钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint dwThreadId);
        //移除钩子
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        //把信息传递到下一个监听
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);
        //回调委托
        public delegate int HookProc(int nCode, int wParam, int lParam);
        /// <summary>
        /// 向指定进程发送字符串
        /// </summary>
        /// <param name="m_hWnd"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SendString(IntPtr m_hWnd, string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            IntPtr pData = Marshal.AllocHGlobal(2 * bytes.Length);
            Marshal.Copy(bytes, 0, pData, bytes.Length);
            return SendData(m_hWnd, IPC_CMD_GF_SOCKET, IPC_SUB_GF_SOCKET_SEND, pData, (ushort)bytes.Length);
        }
        /// <summary>
        /// SendMessage发送
        /// </summary>
        /// <param name="hWndServer">指针</param>
        /// <param name="wMainCmdID">主命令</param>
        /// <param name="wSubCmdID">次命令</param>
        /// <param name="pData">json转换的指针</param>
        /// <param name="wDataSize">数据大小</param>
        /// <returns></returns>
        static unsafe bool SendData(IntPtr m_hWnd, ushort wMainCmdID, ushort wSubCmdID, IntPtr pData, ushort wDataSize)
        {
            //给IPCBuffer结构赋值
            IPC_Buffer IPCBuffer;
            IPCBuffer.Head.wVersion = IPC_VER;
            IPCBuffer.Head.wSubCmdID = wSubCmdID;
            IPCBuffer.Head.wMainCmdID = wMainCmdID;
            IPCBuffer.Head.wPacketSize = (ushort)Marshal.SizeOf(typeof(IPC_Head));

            //内存操作
            if (pData != null)
            {
                //效验长度
                if (wDataSize > 1024) return false;
                //拷贝数据
                IPCBuffer.Head.wPacketSize += wDataSize;


                byte[] bytes = new byte[IPC_BUFFER];
                Marshal.Copy(pData, bytes, 0, wDataSize);


                for (int i = 0; i < IPC_BUFFER; i++)
                {
                    IPCBuffer.cbBuffer[i] = bytes[i];
                }
            }


            //发送数据
            COPYDATASTRUCT CopyDataStruct;
            IPC_Buffer* pPCBuffer = &IPCBuffer;
            CopyDataStruct.lpData = (IntPtr)pPCBuffer;
            CopyDataStruct.dwData = (IntPtr)IDT_ASYNCHRONISM;
            CopyDataStruct.cbData = IPCBuffer.Head.wPacketSize;
            SendMessage(m_hWnd, 0x004A, (int)m_hWnd, ref CopyDataStruct);


            return true;
        }

        /// <summary>
        /// 注册来钩子
        /// </summary>
        /// <param name="onReceive"></param>
        /// <param name="idHook"></param>
        /// <param name="isHook"></param>
        /// <param name="gc"></param>
        /// <returns></returns>
        public static bool HookLoad(HookProc Hook, out int idHook, out bool isHook, ref GCHandle gc)
        {
            //安装钩子
            //钩子委托
            HookProc lpfn = new HookProc(Hook);
            //关联进程的主模块
            IntPtr hInstance = IntPtr.Zero;// GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            idHook = SetWindowsHookEx(WH_CALLWNDPROC, lpfn, hInstance, (uint)AppDomain.GetCurrentThreadId());
            if (idHook > 0)
            {
                //Debug.Log("钩子[" + idHook + "]安装成功");
                isHook = true;
                //保持活动 避免 回调过程 被垃圾回收
                gc = GCHandle.Alloc(lpfn);
                return true;
            }
            else
            {
                //Debug.Log("钩子安装失败");
                isHook = false;
                UnhookWindowsHookEx(idHook);
                return false;
            }
        }
    }

}
