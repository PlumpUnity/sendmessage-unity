using System.Runtime.InteropServices;
using System;
namespace MessageTrans
{
    public unsafe struct IPC_Buffer
    {
        public IPC_Head Head;
        public fixed byte cbBuffer[DataUtility.IPC_BUFFER];  //json数据存的地方
    }
}
