using UnityEngine;
using System.Collections;
namespace MessageTrans
{
    public interface IMessage {
        string Key { get; set; }
    }
    public interface IMessage<T>:IMessage{
        T Body { get; set; }
    }
}
