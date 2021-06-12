using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.mirle.ibg3k0.stc.Common
{
    public class PropertyChangedEventArgs : EventArgs
    {
        private String propertyName;
        public String PropertyName
        {
            get { return propertyName; }
        }
        private Object propertyValue;
        public Object PropertyValue
        {
            get { return propertyValue; }
        }
        public PropertyChangedEventArgs(String propertyName)
            : this(propertyName, null)
        {
            //this.propertyName = propertyName;
        }
        public PropertyChangedEventArgs(String propertyName, Object propertyValue)
        {
            this.propertyName = propertyName;
            this.propertyValue = propertyValue;
        }
    }

    public class EventDictionary<T> : Dictionary<string, EventHandler<T>>
    {
        public virtual void Add(string propertyName, EventHandler<T> e)
        {
            if (e == null)
                throw new ArgumentException("Event Handler cannot be null.",
                    "e");
            base.Add(propertyName, e);
        }
    }

    public class PropertyChangedVO
    {
        protected Dictionary<string, ArrayList> delegatesDic = new Dictionary<string, ArrayList>();

        private Dictionary<string, EventHandler<PropertyChangedEventArgs>> RealPropertyChangedDic =
            new Dictionary<string, EventHandler<PropertyChangedEventArgs>>();
        public delegate void ChangedEventHandler<T>(object sender, T e);


        public virtual void addEventHandler(string handlerID, string propertyName, EventHandler<PropertyChangedEventArgs> handler)
        {
            if (!delegatesDic.ContainsKey(handlerID))
            {
                delegatesDic.Add(handlerID, new ArrayList());
            }
            delegatesDic[handlerID].Add(handler);
            //   RealPropertyChanged += handler;
            if (RealPropertyChangedDic.ContainsKey(propertyName))
            {
                RealPropertyChangedDic[propertyName] += handler;
                //RealPropertyChangedDic[propertyName] = RealPropertyChangedDic[propertyName] + handler;
            }
            else
            {
                RealPropertyChangedDic.Add(propertyName, handler);
            }
        }

        public virtual void removeEventHandler(string handlerID, string propertyName, EventHandler<PropertyChangedEventArgs> handler)
        {
            if (!delegatesDic.ContainsKey(handlerID))
            {
                return;
            }
            delegatesDic[handlerID].Remove(handler);
            //RealPropertyChanged -= handler;
            if (RealPropertyChangedDic.ContainsKey(propertyName))
            {
                RealPropertyChangedDic[propertyName] -= handler;
            }
        }

        public virtual void removeEventHandler(string handlerID)
        {
            if (!delegatesDic.ContainsKey(handlerID))
            {
                return;
            }

            ArrayList list = delegatesDic[handlerID];
            EventHandler<PropertyChangedEventArgs> handler;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                handler = (EventHandler<PropertyChangedEventArgs>)list[i];
                list.RemoveAt(i);
                //RealPropertyChanged -= handler;
                List<String> keys = RealPropertyChangedDic.Keys.ToList();
                foreach (string propertyName in keys)
                {
                    RealPropertyChangedDic[propertyName] -= handler;
                }
            }
        }

        public virtual void RemoveAllEvents()
        {
            foreach (string handlerID in delegatesDic.Keys)
            {
                removeEventHandler(handlerID);
            }
        }

        protected virtual void OnPropertyChanged(String propertyName)
        {
            //EventHandler<PropertyChangedEventArgs> tmpEventHandler = RealPropertyChanged;
            EventHandler<PropertyChangedEventArgs> tmpEventHandler = null;
            if (RealPropertyChangedDic.ContainsKey(propertyName))
            {
                tmpEventHandler = RealPropertyChangedDic[propertyName];
            }
            if (tmpEventHandler != null)
            {
                tmpEventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected virtual void OnPropertyChanged(String propertyName, Object propertyValue)
        {
            //EventHandler<PropertyChangedEventArgs> tmpEventHandler = RealPropertyChanged;
            EventHandler<PropertyChangedEventArgs> tmpEventHandler = null;
            if (RealPropertyChangedDic.ContainsKey(propertyName))
            {
                tmpEventHandler = RealPropertyChangedDic[propertyName];
            }
            if (tmpEventHandler != null)
            {
                tmpEventHandler(this, new PropertyChangedEventArgs(propertyName, propertyValue));
            }
        }

    }
}
