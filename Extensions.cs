using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace DropBoxLoadBalancer.Infrastructure.Infrastructure
{
    public static class Extensions
    {
        public static byte[] SerializeObject<T>(T item)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                   // XmlReader xmlReader = XmlReader.Create()
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    xs.Serialize(stream, item);
                    stream.ToArray();
                }
            }
            catch (Exception)
            {
                return new byte[0];
            }
            return new byte[0];
        }

        public static T DeserializeObject<T>(byte[] data) where T: class
        {
            T result = null;
            try
            {
                
                using (MemoryStream stream = new MemoryStream(data, 0, data.Length))
                {
                   
                    XmlSerializer serializer = new XmlSerializer(typeof(T));
                    XmlReader xmlReader = XmlReader.Create(stream);
                    StringReader reader = new StringReader(UTF8Encoding.UTF8.GetString(data));
                    result = (T)serializer.Deserialize(reader);
                    return result;
                }
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            return result;
        }

    }
}
