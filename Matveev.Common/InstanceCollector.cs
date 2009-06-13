using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Matveev.Common
{
    public static class InstanceCollector<T> where T : class
    {
        private const BindingFlags _REQUESTED_FLAGS = BindingFlags.Static | BindingFlags.Public;

        public static Dictionary<string, T> Instances
        {
            get
            {
                Dictionary<string, T> instances = new Dictionary<string, T>();

                foreach(Type type in Assembly.Load("Matveev.Mtk.Library").GetTypes())
                {
                    FindInstances(instances, type);
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

            foreach (FieldInfo field in type.GetFields(_REQUESTED_FLAGS))
            {
                if (IsInstanceOf(typeof(T), field.FieldType))
                {
                    instances.Add(type.Name + "." + field.Name, (T)field.GetValue(null));
                }
            }

            foreach (PropertyInfo property in type.GetProperties(_REQUESTED_FLAGS))
            {
                MethodInfo getMethod = property.GetGetMethod();
                if (getMethod == null)
                    continue;
                if (getMethod.IsStatic == false)
                    continue;

                if (IsInstanceOf(typeof(T), property.PropertyType))
                {
                    instances.Add(type.Name + "." + property.Name, (T)property.GetValue(null, null));
                }
            }
        }

        private static bool IsInstanceOf(Type super, Type derived)
        {
            if (super.IsInterface)
            {
                return derived.GetInterface(super.Name) != null;
            }
            return derived.IsSubclassOf(super);
        }
    }
}
