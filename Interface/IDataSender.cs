using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MessageTrans
{
    public interface IDataSender
    {
        void RegistHandle(IntPtr handle);
        bool SendData(string data);
        bool SendData<T>(T data);
    }

}
