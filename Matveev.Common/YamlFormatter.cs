using System.Runtime.Serialization;
using System;
using System.IO;

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
                Type type = graph.GetType();
                if (type.IsArray)
                {
                    Array array = (Array)graph;
                    for (int i = 0; i < array.Length; i++)
                    {
                        writer.Write("- ");
                        writer.WriteLine(array.GetValue(i));
                    }
                    return;
                }
                foreach (var field in type.GetFields())
                {
                    writer.WriteLine("{0}: {1}", field.Name, field.GetValue(graph));
                }
                foreach (var property in type.GetProperties())
                {
                    writer.WriteLine("{0}: {1}", property.Name, property.GetValue(graph, null));
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
