using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITEMS_SignPlacement.Templates
{
    internal class Singleton<T> where T : class, new()
    {
        protected Singleton() { }

        class SingletonCreator
        {
            static SingletonCreator() { }
            // Private object instantiated with private constructor
            internal static readonly T instance = new T();
        }

        public static T Instance
        {
            get { return SingletonCreator.instance; }
        }
    }
}
}
