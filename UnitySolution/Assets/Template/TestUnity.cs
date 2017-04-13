using System;
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;


public class TestUnity : MonoBehaviour
{
    #region
    public IntPtr m_hWnd;
    /// <summary>
    /// 发送windows消息方便user32.dll中的SendMessage函数使用
    /// </summary>
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
    //user32.dll中的SendMessage
    [DllImport("user32.dll")]
    public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, ref COPYDATASTRUCT lParam);
    //user32.dll中的获得窗体句柄
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string strClassName, string strWindowName);
    //宏定义 
    private const ushort IPC_VER = 1;
    private const int IDT_ASYNCHRONISM = 0x0201;
    private const uint WM_COPYDATA = 0x004A;
    private const ushort IPC_CMD_GF_SOCKET = 1;
    private const ushort IPC_SUB_GF_SOCKET_SEND = 1;
    private const int IPC_SUB_GF_CLIENT_READY = 1;
    private const int IPC_CMD_GF_CONTROL = 2;
    private const int IPC_BUFFER = 10240;//最大缓冲长度
    //查找的窗体
    private IntPtr hWndPalaz;
    //数据包头配合使用
    //定义结构和发送的结构对应
    public unsafe struct IPC_Head
    {
        public int wVersion;
        public int wPacketSize;
        public int wMainCmdID;
        public int wSubCmdID;
    }
    public unsafe struct IPC_Buffer
    {
        public IPC_Head Head;
        public fixed byte cbBuffer[IPC_BUFFER];  //json数据存的地方
    }
    #endregion
    /// <summary>
    /// 发送把json转换为指针传到SendData()方法
    /// </summary>
    private void sendJson()
    {
        IntPtr hWndPalaz = FindWindow(null, "window");//就是窗体的的标题

        if (hWndPalaz != null)
        {
            //获得游戏本身句柄 
            m_hWnd = FindWindow("UnityWndClass", null);
            Debug.Log(m_hWnd +"...." + hWndPalaz);

            //发送用户准备好消息（这个是个json插件我就不提供了你们自己搞自己的json new一个实例这里不改会报错）
            JSONObject jsStart = new JSONObject();
            jsStart.AddField("六六", "是我");
            jsStart.AddField("sya", "学习游戏为了装逼小组");
            jsStart.AddField("doing", "this is your time");


            string uRstr = jsStart.ToString();
            byte[] bytes = Encoding.UTF8.GetBytes(uRstr);
            IntPtr pData = Marshal.AllocHGlobal(2 * bytes.Length);
            Marshal.Copy(bytes, 0, pData, bytes.Length);
            SendData(m_hWnd, IPC_CMD_GF_SOCKET, IPC_SUB_GF_SOCKET_SEND, pData, (ushort)bytes.Length);
        }

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
    public unsafe bool SendData(IntPtr hWndServer, ushort wMainCmdID, ushort wSubCmdID, IntPtr pData, ushort wDataSize)
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
        SendMessage(hWndServer, 0x004A, (int)m_hWnd, ref CopyDataStruct);


        return true;
    }
    void Update()
    {
        sendJson();//一直发送方便测试
    }
}