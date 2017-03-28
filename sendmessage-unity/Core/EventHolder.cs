using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using MessageTrans.Interal;

namespace MessageTrans
{
    public static class EventHolder
    {
        public static System.Action<string> MessageNotHandled;

        public static Dictionary<string, Delegate> m_needHandle = new Dictionary<string, Delegate>();
        public static void NoMessageHandle(string rMessage)
        {
            if (MessageNotHandled == null)
            {
                Debug.LogWarning("MessageDispatcher: Unhandled Message of type " + rMessage);
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
        public static void RegisterEvent(string key, UnityAction<string> action){
            AddDelegate(key, action);
        }
        public static void RemoveEvent(string key, UnityAction<string> action)
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
        public static void NotifyObserver(string rMessage)
        {
            bool lReportMissingRecipient = true;
            JSONClass data = JSONNode.Parse(rMessage).AsObject;
            Debug.Log("rMessage" + rMessage);
            if (m_needHandle.ContainsKey(data["Key"]))
            {
                var body = data["Body"];
                Debug.Log(body);
                if (body != null)
                {
                    if (data != null)
                    {
                        m_needHandle[data["Key"]].DynamicInvoke(body.ToString());
                    }
                    else
                    {
                        m_needHandle[data["Key"]].DynamicInvoke();
                    }
                }
                else
                {
                    m_needHandle[data["Key"]].DynamicInvoke();
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