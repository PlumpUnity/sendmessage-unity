using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MessageTrans
{
    public interface IDataSender
    {
        void RegistHandle(IntPtr handle);
        bool SendMessage(string key);
        bool SendMessage(string key,string body);
    }

}
