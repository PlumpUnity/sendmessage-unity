using System;
namespace MessageTrans
{
    public interface IDataReceiver
    {
        void RegistHook();
        void RemoveHook();
        void RegisterEvent(string key, Action action);
        void RemoveEvent(string key, Action action);
        void RegisterEvent(string key, Action<string> action);
        void RemoveEvent(string key, Action<string> action);
    }
}