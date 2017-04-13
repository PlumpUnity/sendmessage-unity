using System;
using MessageTrans.Interal;

namespace MessageTrans
{
    public class DataSender : IDataSender
    {
        private IntPtr handle;
        public void RegistHandle(IntPtr handle)
        {
            this.handle = handle;
        }

        public bool SendMessage(string key)
        {
            if (handle == IntPtr.Zero)
                return false;
            else
            {
                JSONClass node = new JSONClass();
                node["Key"] = key;
                return DataUtility.SendString(handle, node.ToString());
            }
        }
        public bool SendMessage(string key, string body)
        {
            if (handle == IntPtr.Zero)
                return false;
            else
            {
                JSONClass node = new JSONClass();
                node["Key"] = key;
                node["Body"] = body;
                return DataUtility.SendString(handle, node.ToString());
            }
        }
    }

}
