using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace WEBtoSQL.Models
{
    internal class SendXML
    {
        string filePath = "C:\\inetpub\\wwwroot\\Data\\TB_MVCMESSAGE_SEND.prm"; // Replace with your CSV file path

        public string OutputFilePath { get; set; } // CSV 文件輸出路徑

        public string xmlString = "", messagefrom = "";

        private static volatile bool isCancelled = false;

        public static void CancelLoad()
        {
            // 設置中斷標誌為 true
            isCancelled = true;
        }

        public static void ReLoad()
        {
            // 設置中斷標誌為 false
            isCancelled = false;
        }

        public string GetMessage()
        {
            bool readSuccess = false;

            while (!readSuccess && !isCancelled)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        xmlString = reader.ReadToEnd(); // 讀取整個文件內容
                    }
                    readSuccess = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"讀取XML文件時出錯: {ex.Message}");
                }
            }

            // 提取Downstream
            int startIndex = xmlString.IndexOf("<Face>") + "<Face>".Length;
            int endIndex = xmlString.IndexOf("</Face>");
            //if(startIndex)
            if(endIndex > startIndex && endIndex - startIndex > 0)
            {
                try
                {
                    messagefrom = xmlString.Substring(startIndex, endIndex - startIndex);
                }catch (Exception ex) { }
            }

            // 刪除<Face>Downstream</Face>
            xmlString = xmlString.Replace($"<Face>{messagefrom}</Face>", "");

            return xmlString;
        }
    }
}
