using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BeatmapTool
{
    public class TObject<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }
        public TObject(T1 Item1, T2 Item2)
        {
            this.Item1 = Item1;
            this.Item2 = Item2;
        }
        public TObject()
        {
        }
    }
    public class DynamicObject : System.Dynamic.DynamicObject
    {
        private Dictionary<string, object> storage = new Dictionary<string, object>();
        public override bool TryGetMember(System.Dynamic.GetMemberBinder binder, out object result)
        {
            if (storage.ContainsKey(binder.Name))
            {
                result = storage[binder.Name];
                return true;
            }
            result = null;
            return true;
        }

        public override bool TrySetMember(System.Dynamic.SetMemberBinder binder, object value)
        {
            string key = binder.Name;
            if (storage.ContainsKey(key))
                storage[key] = value;
            else
                storage.Add(key, value);
            return true;
        }

        public override string ToString()
        {
            StringBuilder message = new StringBuilder();
            foreach (var item in storage)
                message.AppendFormat("{0} : {1}\n", item.Key, item.Value);
            return message.ToString();
        }
    }


    public class Object<T>
    {
        public static explicit operator T(Object<T> value)
        {
            return value.Value;
        }
        public static implicit operator Object<T>(T value)
        {
            return new Object<T>(value);
        }
        public static bool operator ==(T o, Object<T> o2)
        {
            return (o == null && o2 == null) || o != null && o.Equals(o2.Value);
        }
        public static bool operator ==(Object<T> o2, T o)
        {
            return (o == null && o2 == null) || o != null && o.Equals(o2.Value);
        }
        public static bool operator !=(T o, Object<T> o2)
        {
            return !((o == null && o2 == null) || o != null && o.Equals(o2.Value));
        }
        public static bool operator !=(Object<T> o2, T o)
        {
            return !((o == null && o2 == null) || o != null && o.Equals(o2.Value));
        }
        public Object() { }
        public Object(T value)
        {
            this.Value = value;
        }
        public T Value { get; set; }
        public override string ToString()
        {
            return Value.ToString();
        }
        public override bool Equals(object obj)
        {
            if (obj is Object<T>)
                obj = ((Object<T>)obj).Value;
            return !(Value != null && obj != null) || Value.Equals(obj) || base.Equals(obj);
        }
        public override int GetHashCode()
        {
            if (Value == null)
                return base.GetHashCode();
            return Value.GetHashCode();
            //return base.GetHashCode();
        }
    }
}
