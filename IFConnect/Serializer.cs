using Fds.IFAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace IFConnect
{
    /// <summary>
    /// JSON SerDes helper methods.
    /// </summary>
    /// <remarks>
    /// Credit to Matt Laban @ FDS
    /// </remarks>
    public class Serializer
    {
        #region JSON

        public static string SerializeJson<T>(T data)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            var json = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, data);
                json = Encoding.Default.GetString(memoryStream.ToArray());
            }

            return json;
        }

        public static T DeserializeJson<T>(string jsonData)
        {
            if (string.IsNullOrEmpty(jsonData))
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var data = Encoding.ASCII.GetBytes(jsonData);
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(memoryStream);
            }
        }

        #endregion

        #region XML

        public static string SerializeXML<T>(T data)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var settings = new XmlWriterSettings { Indent = true };
            var xml = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memoryStream, settings))
                {
                    serializer.WriteObject(writer, data);
                }
                xml = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return xml;
        }

        public static T DeserializeXML<T>(string xmlData)
        {
            if (string.IsNullOrEmpty(xmlData))
                return default(T);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(xmlData);
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                DataContractSerializer serializer = new DataContractSerializer(typeof(T), new List<Type> { typeof(APIAircraftState) });
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max);
                return (T)serializer.ReadObject(reader, false);
            }
        }

        internal static object DeserializeXML(string xmlData, Type type)
        {
            if (string.IsNullOrEmpty(xmlData))
                return null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                var data = Encoding.UTF8.GetBytes(xmlData);
                memoryStream.Write(data, 0, data.Length);
                memoryStream.Seek(0, SeekOrigin.Begin);
                DataContractSerializer serializer = new DataContractSerializer(type);
                XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(memoryStream, XmlDictionaryReaderQuotas.Max);
                return serializer.ReadObject(reader, false);
            }
        }

        #endregion
    }
}
