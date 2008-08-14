using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UI
{
    public static class InstanceCollector<T>
    {
        private static Dictionary<string, T> _instances;

        public static Dictionary<string, T> Instances
        {
            get
            {
                if (InstanceCollector<T>._instances == null)
                {
                    Dictionary<string, T> instances = new Dictionary<string, T>();

                    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            FindInstances(instances, type);
                        }
                    }
                    InstanceCollector<T>._instances = instances;
                }

                return InstanceCollector<T>._instances;
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
                {
                    continue;
                }
                if (getMethod.IsStatic == false)
                {
                    continue;
                }

                // Это плохо
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
