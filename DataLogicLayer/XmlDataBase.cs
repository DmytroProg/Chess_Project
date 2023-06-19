using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DataLogicLayer
{
    public class XmlDataBase
    {
        private static XmlSerializer serializer = null;

        public static void Write<T>(string path, T value)
        {
            serializer = new XmlSerializer(typeof(T));
            using (Stream stream = File.Create(path))
            {
                serializer.Serialize(stream, value);
            }
        }

        public static T Read<T>(string path)
        {
            serializer = new XmlSerializer(typeof(T));
            using (Stream stream = File.OpenRead(path))
            {
                return (T)serializer.Deserialize(stream);
            }
        }
    }
}
