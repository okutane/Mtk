using System.Runtime.Serialization;
using System;
using System.IO;
using System.Collections;

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
            if (graph is IEnumerable)
            {
                foreach (var item in (IEnumerable)graph)
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
                    if (value is IEnumerable)
                    {
                        writer.WriteLine("{0} :", property.Name);
                        Serialize(writer, value, indentSize + 2);
                        continue;
                    }
                    writer.WriteLine("{0}: {1}", property.Name, value);
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
