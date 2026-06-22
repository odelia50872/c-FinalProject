using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GatherUp.Infrastructure.XML
{

    public static class XMLSerializer
    {
        public static void SaveToXML<T>(string filePath, T data) where T : new()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, data);
            }
        }

        public static T LoadFromXML<T>(string filePath) where T : new()
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("XML file not found", filePath);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            using (StreamReader reader = new StreamReader(filePath))
            {
                return (T)(serializer.Deserialize(reader) ?? new T());
            }
        }
    }
}

