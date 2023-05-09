using System;
using Cognex.DataMan.SDK;
using System.Xml;
using NLog;
using ILogger = NLog.ILogger;

namespace SerialIDTracker.Utils
{
    internal class XmlFiles
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        public static string GetString(string resultXml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();

                doc.LoadXml(resultXml);

                XmlNode full_string_node = doc.SelectSingleNode("result/general/full_string");

                if (full_string_node != null && Singleton.Instance.System != null && Singleton.Instance.System.State == ConnectionState.Connected)
                {
                    XmlAttribute encoding = full_string_node.Attributes["encoding"];
                    if (encoding != null && encoding.InnerText == "base64")
                    {
                        if (!string.IsNullOrEmpty(full_string_node.InnerText))
                        {
                            byte[] code = Convert.FromBase64String(full_string_node.InnerText);
                            return Singleton.Instance.System.Encoding.GetString(code, 0, code.Length);
                        }
                        else
                        {
                            return "";
                        }
                    }

                    return full_string_node.InnerText;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

            return "";
        }
    }
}
