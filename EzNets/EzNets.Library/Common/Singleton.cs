using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace EzNets.Library.Common
{
    public class Singleton<T> where T : class
    {
        static object locker = new object();
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        if (instance == null)
                        {
                            ConstructorInfo oConInfo = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                            instance = (T)oConInfo.Invoke(null);
                        }
                    }
                }
                return instance;
            }
        }
    }
}
