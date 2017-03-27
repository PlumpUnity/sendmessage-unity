using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
namespace MessageTrans
{
    public static class EventHolder
    {
        public static System.Action<IMessage> MessageNotHandled;

        public static Dictionary<string, Delegate> m_needHandle = new Dictionary<string, Delegate>();
        public static void NoMessageHandle(IMessage rMessage)
        {
            if (MessageNotHandled == null)
            {
                Debug.LogWarning("MessageDispatcher: Unhandled Message of type " + rMessage.Key);
            }
            else
            {
                MessageNotHandled(rMessage);
            }
        }

        #region 注册注销事件
        public static void RegisterEvent(string key,UnityAction action){
            AddDelegate(key, action);
        }
        public static void RemoveEvent(string key, UnityAction action)
        {
            RemoveDelegate(key, action);
        }
        public static void RegisterEvent<T>(string key, UnityAction<T> action){
            AddDelegate(key, action);
        }
        public static void RemoveEvent<T>(string key, UnityAction<T> action)
        {
            RemoveDelegate(key, action);
        }
        static void AddDelegate(string key, Delegate handle)
        {
            // First check if we know about the message type
            if (!m_needHandle.ContainsKey(key))
            {
                m_needHandle.Add(key, handle);
            }
            else
            {
                m_needHandle[key] = Delegate.Combine(m_needHandle[key], handle);
            }
        }
        static bool RemoveDelegate(string key, Delegate handle)
        {
            if (m_needHandle.ContainsKey(key))
            {
                m_needHandle[key] = Delegate.Remove(m_needHandle[key], handle);
                if (m_needHandle[key] == null)
                {
                    m_needHandle.Remove(key);
                    return false;
                }
            }
            return true;
        }
        public static void RemoveEvents(string key)
        {
            if (m_needHandle.ContainsKey(key))
            {
                m_needHandle.Remove(key);
            }
        }
        public static bool HaveEvent(string key)
        {
            return m_needHandle.ContainsKey(key);
        }
        #endregion

        #region 触发事件
        public static void NotifyObserver(IMessage rMessage)
        {
            bool lReportMissingRecipient = true;

            if (m_needHandle.ContainsKey(rMessage.Key))
            {
                var body = rMessage.GetType().GetProperty("Body");

                if (body != null)
                {
                    var data = body.GetValue(rMessage, null);

                    if (data != null)
                    {
                        m_needHandle[rMessage.Key].DynamicInvoke(data);
                    }
                    else
                    {
                        m_needHandle[rMessage.Key].DynamicInvoke();
                    }
                }
                else
                {
                    m_needHandle[rMessage.Key].DynamicInvoke();
                }

                lReportMissingRecipient = false;
            }

            // If we were unable to send the message, we may need to report it
            if (lReportMissingRecipient)
            {
                NoMessageHandle(rMessage);
            }
        }
        #endregion
    }

}