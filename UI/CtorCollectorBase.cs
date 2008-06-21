using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace UI
{
    public class CtorCollectorBase<T>
    {
        public Dictionary<string, T> Objects;

        public CtorCollectorBase()
        {
            this.Objects = new Dictionary<string, T>();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(T)))
                    {
                        ConstructorInfo ctor = type.GetConstructor(new Type[0]);
                        object o = ctor.Invoke(null);
                        this.Objects.Add(type.Name, (T)o);
                    }
                }
            }
        }
    }
}
