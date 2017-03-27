using UnityEngine;
using System.Collections;
using System;
namespace MessageTrans
{
    public class Message : IMessage
    {
        public string Key { get; set; }
        public new string ToString
        {
            get
            {
                string msg = "";
                msg += "\nObserverName:" + Key.ToString();
                return msg;
            }
        }
        public bool IsUsing { get; set; }

        public bool Destroy { get; set; }
        public virtual void Clear()
        {
            Key = null;
            IsUsing = false;
        }

        // ******************************** OBJECT POOL ********************************

        /// <summary>
        /// Allows us to reuse objects without having to reallocate them over and over
        /// </summary>
        private static ObjectPool<Message> sPool = new ObjectPool<Message>(1, 1);

        //public static int Length { get { return sPool.Length; } }
        /// <summary>
        /// Pulls an object from the pool.
        /// </summary>
        /// <returns></returns>
        public static Message Allocate(string key)
        {
            Message lInstance = sPool.Allocate();
            if (lInstance == null) { lInstance = new Message(); }

            lInstance.Key = key;

            lInstance.IsUsing = true;
            return lInstance;
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public void Release()
        {
            if (this == null) { return; }

            Clear();

            sPool.Release((Message)this);
        }
    }
    public class Message<T> : IMessage<T>
    {
        public T Body { get; set; }
        public string Key { get; set; }
        public new string ToString
        {
            get
            {
                string msg = "";
                msg += "\nObserverName:" + Key.ToString();
                msg += "\nBody:" + ((Body == null) ? "null" : Body.ToString());
                return msg;
            }
        }
        public bool IsUsing { get; set; }
        public virtual void Clear()
        {
            Body = default(T);
            Key = null;
            IsUsing = false;
        }

        // ******************************** OBJECT POOL ********************************

        /// <summary>
        /// Allows us to reuse objects without having to reallocate them over and over
        /// </summary>
        private static ObjectPool<Message<T>> sPool = new ObjectPool<Message<T>>(1, 1);

        //public static int Length { get { return sPool.Length; } }
        /// <summary>
        /// Pulls an object from the pool.
        /// </summary>
        /// <returns></returns>
        public static Message<T> Allocate(string key, T body)
        {
            Message<T> lInstance = sPool.Allocate();
            if (lInstance == null) { lInstance = new Message<T>(); }

            lInstance.Key = key;
            lInstance.Body = body;

            lInstance.IsUsing = true;
            return lInstance;
        }

        /// <summary>
        /// Returns an element back to the pool.
        /// </summary>
        /// <param name="rEdge"></param>
        public void Release()
        {
            if (this == null) { return; }

            Clear();

            // Make it available to others.
            if (this is Message<T>)
            {
                sPool.Release((Message<T>)this);
            }
        }
    }

}
