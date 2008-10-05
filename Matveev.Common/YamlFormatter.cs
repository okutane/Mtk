using System.Runtime.Serialization;
using System;
using System.IO;
using System.Collections;
using System.Globalization;

namespace Matveev.Common
{
    public class YamlFormatter : IFormatter
    {

        #region IFormatter Members

        public SerializationBinder Binder
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public StreamingContext Context
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public object Deserialize(Stream serializationStream)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Stream serializationStream, object graph)
        {
            using (TextWriter writer = new StreamWriter(serializationStream))
            {
                writer.WriteLine("---");
                Serialize(writer, graph, 0);
            }
        }

        private static void Serialize(TextWriter writer, object graph, int indentSize)
        {
            Type type = graph.GetType();
            IEnumerable enumerable = graph as IEnumerable;
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    writer.Write("- ");
                    writer.WriteLine(item);
                }
            }
            else
            {
                foreach (var field in type.GetFields())
                {
                    writer.WriteLine("{0}: {1}", field.Name, field.GetValue(graph));
                }
                foreach (var property in type.GetProperties())
                {
                    object value = property.GetValue(graph, null);
                    if (value is IEnumerable && !(value is string))
                    {
                        writer.WriteLine("{0} :", property.Name);
                        Serialize(writer, value, indentSize + 2);
                        continue;
                    }
                    string formattedValue = string.Format(NumberFormatInfo.InvariantInfo, "{0}", value);
                    writer.WriteLine("{0}: {1}", property.Name, formattedValue);
                }
            }
        }

        public ISurrogateSelector SurrogateSelector
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
