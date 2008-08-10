using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Matveev.Common
{
    public static class InstanceCollector<T>
    {
        public static Dictionary<string, T> Instances
        {
            get
            {
                Dictionary<string, T> instances = new Dictionary<string, T>();

                foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        FindInstances(instances, type);
                    }
                }

                return instances;
            }
        }

        private static void FindInstances(Dictionary<string, T> instances, Type type)
        {
            foreach (Type nestedType in type.GetNestedTypes())
            {
                FindInstances(instances, nestedType);
            }

            foreach (PropertyInfo property in type.GetProperties())
            {
                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod == null)
                    continue;
                if (getMethod.IsStatic == false)
                    continue;

                // TODO: Это плохо
                Type t = typeof(T);
                if (t.IsInterface && property.PropertyType.GetInterface(t.Name) != null)
                {
                    instances.Add(type.Name, (T)getMethod.Invoke(null, null));
                }
                else
                {
                    if (property.PropertyType.IsSubclassOf(typeof(T)))
                        instances.Add(type.Name, (T)property.GetValue(null, null));
                }
            }
        }
    }
}
