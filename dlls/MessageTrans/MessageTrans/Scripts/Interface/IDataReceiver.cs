using System;
namespace MessageTrans
{
    public interface IDataReceiver
    {
        Action<string> MessageNotHandled { get; set; }
        event Action<string> OnError;
        bool RegistHook();
        bool RemoveHook();
        void RegisterEvent(string key, Action action);
        void RemoveEvent(string key, Action action);
        void RegisterEvent(string key, Action<string> action);
        void RemoveEvent(string key, Action<string> action);
    }
}