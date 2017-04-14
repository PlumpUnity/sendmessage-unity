using System;
using System.Collections;
using System.Collections.Generic;
using MessageTrans.Interal;

namespace MessageTrans.Interal
{
    public class EventHolder
    {
        public System.Action<string> MessageNotHandled;

        public Dictionary<string, Delegate> m_needHandle = new Dictionary<string, Delegate>();
        public void NoMessageHandle(string rMessage)
        {
            if (MessageNotHandled == null)
            {
                //Debug.LogWarning("MessageDispatcher: Unhandled Message of type " + rMessage);
            }
            else
            {
                MessageNotHandled(rMessage);
            }
        }

        #region 注册注销事件
      
        public void AddDelegate(string key, Delegate handle)
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
        public bool RemoveDelegate(string key, Delegate handle)
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
        public void RemoveEvents(string key)
        {
            if (m_needHandle.ContainsKey(key))
            {
                m_needHandle.Remove(key);
            }
        }
        public bool HaveEvent(string key)
        {
            return m_needHandle.ContainsKey(key);
        }
        #endregion

        #region 触发事件
        public void NotifyObserver(string rMessage)
        {
            bool lReportMissingRecipient = true;
            JSONClass data = JSONNode.Parse(rMessage).AsObject;
            string key = data["Key"].Value;
            if (m_needHandle.ContainsKey(key))
            {
                var body = data["Body"];
                if (body != null)
                {
                    if (body.Value != null)
                    {
                        m_needHandle[key].DynamicInvoke(body.Value);
                    }
                    else
                    {
                        m_needHandle[key].DynamicInvoke();
                    }
                }
                else
                {
                    m_needHandle[key].DynamicInvoke();
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