
namespace MessageTrans.Interal
{

public unsafe struct IPC_Head
{
    public int wVersion;
    public int wPacketSize;
    public int wMainCmdID;
    public int wSubCmdID;
}
}