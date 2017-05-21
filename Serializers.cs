using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace DropBoxLoadBalancer.Infrastructure.Infrastructure
{
    public static class Serializers
    {
        public static byte[] SerializeObject<T>(T item)
        {
            try
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    StringBuilder sb = new StringBuilder();
                    XmlWriter xmlWriter = XmlWriter.Create(sb, new XmlWriterSettings() { Encoding = UTF8Encoding.ASCII });
                    xs.Serialize(xmlWriter, item);
                    return ASCIIEncoding.UTF8.GetBytes(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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

        public static void SerializeObjectToFile<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                FileStream fileStream = new FileStream(fileName, FileMode.Create);
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileStream);
                    fileStream.Dispose();
                    stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }

        public static T DeSerializeObjectToFile<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlReader reader2 = XmlReader.Create(fileName);
                xmlDocument.Load(reader2);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = XmlReader.Create(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Dispose();
                    }

                    reader2.Dispose();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }

    }
}
