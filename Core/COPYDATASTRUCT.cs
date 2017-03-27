using System.Runtime.InteropServices;
using System.Text;
using System;

namespace MessageTrans {
    /// <summary>
    /// 发送windows消息方便user32.dll中的SendMessage函数使用
    /// </summary>
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData;
        public int cbData;
        public IntPtr lpData;
    }
}
