using System;
using Newtonsoft.Json;

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
                var data = Message.Allocate(key);
                return DataUtility.SendString(handle, JsonConvert.SerializeObject(data));
            }
        }

        public bool SendMessage<T>(string key,T body)
        {
            if (handle == IntPtr.Zero)
                return false;
            else
            {
                var data = Message<T>.Allocate(key,body);
                return DataUtility.SendString(handle, JsonConvert.SerializeObject(data));
            }
        }
    }

}
