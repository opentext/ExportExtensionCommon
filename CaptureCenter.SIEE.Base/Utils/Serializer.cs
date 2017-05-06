// Objekte nach XML serialisieren und von XML deserialisieren

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ExportExtensionCommon
{
    public class Serializer
    {
        public static void SerializeToBinFile(object obj, string filename, Encoding encoding)
        {
            Stream stream = new FileStream(filename, System.IO.FileMode.Create);
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, obj);
            stream.Close();
        }

        public static object DeserializeFromBinFile(string filename, Type objectType, Encoding encoding)
        {
            Stream stream = new FileStream(filename, System.IO.FileMode.Open);
            IFormatter formatter = new BinaryFormatter();
            object res = formatter.Deserialize(stream);
            stream.Close();
            return res;
        }

        /// <summary>
        /// Serialisiert ein Objekt in eine XML-Datei 
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt</param>
        /// <param name="filename">Der Dateiname der zu erzeugenden XML-Datei</param>
        /// <param name="encoding">Die Codierung der zu erzeugenden XML-Datei</param>
        public static void SerializeToXmlFile(object obj,
           string filename, Encoding encoding)
        {
            // XmlSerializer f�r den Typ des Objekts erzeugen
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            // Objekt �ber ein StreamWriter-Objekt serialisieren 
            using (StreamWriter streamWriter = new StreamWriter(filename, false, encoding))
            {
                serializer.Serialize(streamWriter, obj);
            }
        }

        /// <summary>
        /// Serialisiert ein Objekt in eine XML-Datei 
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt</param>
        /// <param name="filename">Der Dateiname der zu erzeugenden XML-Datei</param>
        /// <param name="encoding">Die Codierung der zu erzeugenden XML-Datei</param>
        /// <param name="extraTypes"></param>
        public static void SerializeToXmlFile(object obj,
           string filename, Encoding encoding, Type[] extraTypes)
        {
            // XmlSerializer f�r den Typ des Objekts erzeugen
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), extraTypes);
            // Objekt �ber ein StreamWriter-Objekt serialisieren 
            using (StreamWriter streamWriter = new StreamWriter(filename, false, encoding))
            {
                serializer.Serialize(streamWriter, obj);
            }
        }

        /// <summary>
        /// Serialisiert ein Objekt in einen XML-String
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt</param>
        /// <param name="encoding">Die Codierung der zu erzeugenden XML-Datei</param>
        /// <returns>Gibt einen String zur�ck, der die Daten des Objekts in XML-Form enth�lt</returns>
        public static string SerializeToXmlString(object obj, Encoding encoding)
        {
            MemoryStream memoryStream = null;
            StreamWriter streamWriter = null;
            try
            {
                // XmlSerializer f�r den Typ des Objekts erzeugen
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                byte[] buffer;

                // Objekt �ber ein MemoryStream-Objekt serialisieren
                memoryStream = new MemoryStream();
                streamWriter = new StreamWriter(memoryStream, encoding);
                serializer.Serialize(streamWriter, obj);
                // MemoryStream in einen String umwandeln und diesen zur�ckgeben
                buffer = memoryStream.ToArray();
                return encoding.GetString(buffer, 0, buffer.Length);
            }
            finally
            {
                if (streamWriter == null)
                    memoryStream.Close();
                else
                    streamWriter.Close();
            }
        }

        /// <summary>
        /// Serialisiert ein Objekt in einen XML-String
        /// </summary>
        /// <param name="obj">Das zu serialisierende Objekt</param>
        /// <param name="encoding">Die Codierung der zu erzeugenden XML-Datei</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>
        /// Gibt einen String zur�ck, der die Daten des Objekts in XML-Form enth�lt
        /// </returns>
        public static string SerializeToXmlString(object obj, Encoding encoding, Type[] extraTypes)
        {
            MemoryStream memoryStream = null;
            // XmlSerializer f�r den Typ des Objekts erzeugen
            XmlSerializer serializer = new XmlSerializer(obj.GetType(), extraTypes);

            // Objekt �ber ein MemoryStream-Objekt serialisieren
            try
            {
                memoryStream = new MemoryStream();
                {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream, encoding))
                    {
                        memoryStream = null;
                        serializer.Serialize(streamWriter, obj);
                    }
                    // MemoryStream in einen String umwandeln und diesen zur�ckgeben
                    byte[] buffer = memoryStream.ToArray();
                    return encoding.GetString(buffer, 0, buffer.Length);
                }
            }
            finally
            {
                if (memoryStream != null) memoryStream.Close();
            }
        }

        /// <summary>
        /// Deserialisiert ein Objekt aus einer XML-Datei
        /// </summary>
        /// <param name="filename">Dateiname der XML-Datei</param>
        /// <param name="objectType">Der Typ des zu deserialisierenden Objekts</param>
        /// <param name="encoding">Die Codierung der XML-Datei</param>
        /// <returns>Gibt das deserialisierte Objekt zur�ck</returns>
        public static object DeserializeFromXmlFile(string filename,
           Type objectType, Encoding encoding)
        {
            StreamReader streamReader = null;
            // XmlSerializer f�r den Typ des Objekts erzeugen
            XmlSerializer serializer = new XmlSerializer(objectType);
            object o = null;
            // Objekt �ber ein StreamReader-Objekt serialisieren
            using (streamReader = new StreamReader(filename, encoding))
            {
                o = serializer.Deserialize(streamReader);
            }
            return o;
          }

        /// <summary>
        /// Deserialisiert ein Objekt aus einer XML-Datei
        /// </summary>
        /// <param name="filename">Dateiname der XML-Datei</param>
        /// <param name="objectType">Der Typ des zu deserialisierenden Objekts</param>
        /// <param name="encoding">Die Codierung der XML-Datei</param>
        /// <param name="extraTypes"></param>
        /// <returns>Gibt das deserialisierte Objekt zur�ck</returns>
        public static object DeserializeFromXmlFile(string filename,
           Type objectType, Encoding encoding, Type[] extraTypes)
        {
            // XmlSerializer f�r den Typ des Objekts erzeugen
            XmlSerializer serializer = new XmlSerializer(objectType, extraTypes);
            object o = null;
            // Objekt �ber ein StreamReader-Objekt serialisieren
            using (StreamReader streamReader = new StreamReader(filename, encoding))
            {
                o = serializer.Deserialize(streamReader);
            }
            return o;
        }

        /// <summary>
        /// Deserialisiert ein Objekt aus einem XML-String
        /// </summary>
        /// <param name="xmlString">Der String mit den XML-Daten</param>
        /// <param name="objectType">Der Typ des zu deserialisierenden Objekts</param>
        /// <param name="encoding">Die Codierung der XML-Daten</param>
        /// <returns>Gibt das deserialisierte Objekt zur�ck</returns>
        public static object DeserializeFromXmlString(string xmlString, Type objectType, Encoding encoding)
        {
            MemoryStream memoryStream = null;
            try
            {
                // XmlSerializer f�r den Typ des Objekts erzeugen
                XmlSerializer serializer = new XmlSerializer(objectType);

                // Objekt �ber ein MemoryStream-Objekt deserialisieren
                memoryStream = new MemoryStream(encoding.GetBytes(xmlString));
                return serializer.Deserialize(memoryStream);
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Close();
                }
            }
        }

        /// <summary>
        /// Aus Stream deserialisieren
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="objectType"></param>
        /// <param name="encoding"></param>
        /// <param name="extraTypes"></param>
        /// <returns></returns>
        public static object DeserializeFromStream(Stream stream, Type objectType, Encoding encoding, Type[] extraTypes)
        {
            // XmlSerializer f�r den Typ des Objekts erzeugen
            XmlSerializer serializer = new XmlSerializer(objectType, extraTypes);
            // JIRA OCC-6381: Preserve Whitespaces. For replacement characters within snap match settings, they will be translated to \b, \n, ... later on
            XmlReaderSettings readerSettings = new XmlReaderSettings();
            readerSettings.IgnoreWhitespace = false;
            XmlReader reader = XmlReader.Create(stream, readerSettings);
            return serializer.Deserialize(reader);
        }

        /// <summary>
        /// Deserialisiert ein Objekt aus einem XML-String
        /// </summary>
        /// <param name="xmlString">Der String mit den XML-Daten</param>
        /// <param name="objectType">Der Typ des zu deserialisierenden Objekts</param>
        /// <param name="encoding">Die Codierung der XML-Daten</param>
        /// <param name="extraTypes">The extra types.</param>
        /// <returns>Gibt das deserialisierte Objekt zur�ck</returns>
        public static object DeserializeFromXmlString(string xmlString, Type objectType, Encoding encoding, Type[] extraTypes)
        {
            MemoryStream memoryStream = null;
            try
            {
                // XmlSerializer f�r den Typ des Objekts erzeugen
                XmlSerializer serializer = new XmlSerializer(objectType, extraTypes);

                // Objekt �ber ein MemoryStream-Objekt deserialisieren
                memoryStream = new MemoryStream(encoding.GetBytes(xmlString));
                return serializer.Deserialize(memoryStream);
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Close();
                }
            }
        }
    }
}
