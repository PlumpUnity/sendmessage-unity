using System;
namespace MessageTrans
{
    public struct CWPSTRUCT
    {
        public int lparam;
        public int wparam;
        public uint message;
        public IntPtr hwnd;
    }
}
