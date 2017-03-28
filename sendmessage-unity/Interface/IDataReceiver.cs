using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace MessageTrans
{
    public interface IDataReceiver
    {
        void RegistHook();
        void RemoveHook();
    }
}