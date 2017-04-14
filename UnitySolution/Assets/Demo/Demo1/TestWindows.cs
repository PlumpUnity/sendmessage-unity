using UnityEngine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;
public class TestWindows : MonoBehaviour
{
    //钩子接收消息的结构
    public struct CWPSTRUCT
    {
        public int lparam;
        public int wparam;
        public uint message;
        public IntPtr hwnd;
    }
    //建立钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, uint dwThreadId);


    //移除钩子
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern bool UnhookWindowsHookEx(int idHook);


    //把信息传递到下一个监听
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    private static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);
    //回调委托
    private delegate int HookProc(int nCode, int wParam, int lParam);
    
    //钩子
    int idHook = 0;
    //是否安装了钩子
    bool isHook = false;
    private const int WH_CALLWNDPROC = 4;  //钩子类型 全局钩子

    //定义结构和发送的结构对应
    public unsafe struct IPC_Head
    {
        public int wVersion;
        public int wPacketSize;
        public int wMainCmdID;
        public int wSubCmdID;
    }
    private const int IPC_BUFFER = 10240;//最大缓冲长度
    public unsafe struct IPC_Buffer
    {
        public IPC_Head Head;
        public fixed byte cbBuffer[IPC_BUFFER];  //json数据存的地方
    }
    public struct COPYDATASTRUCT
    {
        public int dwData;
        public int cbData;
        public IntPtr lpData;
    }
    void Start()
    {
        //安装钩子
        HookLoad();
    }


    void OnDestroy()
    {
        //关闭钩子
        HookClosing();
    }
    private void HookLoad()
    {
        Debug.Log("开始运行");
        //安装钩子
        {
            //钩子委托
            HookProc lpfn = new HookProc(Hook);
            //关联进程的主模块
            IntPtr hInstance = IntPtr.Zero;// GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);
            idHook = SetWindowsHookEx(WH_CALLWNDPROC, lpfn, hInstance, (uint)AppDomain.GetCurrentThreadId());
            if (idHook > 0)
            {
                Debug.Log("钩子[" + idHook + "]安装成功");
                isHook = true;
                //保持活动 避免 回调过程 被垃圾回收
                 GCHandle.Alloc(lpfn);
            }
            else
            {
                Debug.Log("钩子安装失败");
                isHook = false;
                UnhookWindowsHookEx(idHook);
            }
        }
    }


    //卸载钩子
    private void HookClosing()
    {
        if (isHook)
        {
            UnhookWindowsHookEx(idHook);
        }
    }


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
                SetUI(str);
                Debug.Log("json数据：" + str);
            }
            if (CallNextProc)
            {
                return CallNextHookEx(idHook, nCode, wParam, lParam);
            }
            else
            {
                //return 1;
                return CallNextHookEx(idHook, nCode, wParam, lParam);
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            return 0;
        }

    }
    public void SetUI(string value)
    {
        FindObjectOfType<UnityEngine.UI.Text>().text = value;
    }
}
