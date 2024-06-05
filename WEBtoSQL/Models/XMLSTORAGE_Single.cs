using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace WEBtoSQL.Models
{
    internal class XMLtoList_Single
    {
        string filePath = "C:\\inetpub\\wwwroot\\Data\\TemporarilyBaInfo_Down.prm"; // Replace with your CSV file path

        public string OutputFilePath { get; set; } // CSV 文件輸出路徑

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

        public List<MVCSTORAGE> ConvertXmlToCsv()
        {
            bool readSuccess = false;
            string xmlString = "";

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
                    Console.WriteLine($"Error reading XML file: {ex.Message}");
                }
            }
            
            xmlString = "<root>" + xmlString + "</root>";

            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine("Type,BoardID,CreatedBy,Failed,ProductTypeID,Flipped,TopBarcode,BottomBarcode,Length,Width,Thickness,ConveyorSpeed,TopClearanceHeight,BottomClearanceHeight,Weight,WorkOrderID,BatchID,Route,Action,SB_Pos,SB_Bc,SB_St");

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            List<MVCSTORAGE> mvcStorageList = new List<MVCSTORAGE>();

            XmlNodeList boardAvailableNodes = xmlDoc.SelectNodes("//BoardAvailable");

            foreach (XmlNode node in boardAvailableNodes)
            {
                MVCSTORAGE storage = new MVCSTORAGE();
                storage.Type = "Main";
                storage.BoardID = GetValueOrDefault(node.Attributes["BoardId"]);
                storage.CreatedBy = GetValueOrDefault(node.Attributes["BoardIdCreatedBy"]);
                storage.Failed = GetValueOrDefault(node.Attributes["FailedBoard"]);
                storage.ProductTypeID = GetValueOrDefault(node.Attributes["ProductTypeId"]);
                storage.Flipped = GetValueOrDefault(node.Attributes["FlippedBoard"]);
                storage.TopBarcode = GetValueOrDefault(node.Attributes["TopBarcode"]);
                storage.BottomBarcode = GetValueOrDefault(node.Attributes["BottomBarcode"]);
                storage.Length = GetValueOrDefault(node.Attributes["Length"]);
                storage.Width = GetValueOrDefault(node.Attributes["Width"]);
                storage.Thickness = GetValueOrDefault(node.Attributes["Thickness"]);
                storage.ConveyorSpeed = GetValueOrDefault(node.Attributes["ConveyorSpeed"]);
                storage.TopClearanceHeight = GetValueOrDefault(node.Attributes["TopClearanceHeight"]);
                storage.BottomClearanceHeight = GetValueOrDefault(node.Attributes["BottomClearanceHeight"]);
                storage.Weight = GetValueOrDefault(node.Attributes["Weight"]);
                storage.WorkOrderID = GetValueOrDefault(node.Attributes["WorkOrderId"]);
                storage.BatchID = GetValueOrDefault(node.Attributes["BatchId"]);
                storage.Route = GetValueOrDefault(node.Attributes["Route"]);
                storage.Action = GetValueOrDefault(node.Attributes["Action"]);
                mvcStorageList.Add(storage);

                XmlNodeList subBoards = node.SelectNodes("SubBoards/SB");
                foreach (XmlNode subBoard in subBoards)
                {
                    MVCSTORAGE subStorage = new MVCSTORAGE();
                    subStorage.Type = "Sub";
                    subStorage.BoardID = storage.BoardID;
                    subStorage.SB_Pos = GetValueOrDefault(subBoard.Attributes["Pos"]);
                    subStorage.SB_Bc = GetValueOrDefault(subBoard.Attributes["Bc"]);
                    subStorage.SB_St = GetValueOrDefault(subBoard.Attributes["St"]);
                    mvcStorageList.Add(subStorage);
                }
            }
            return mvcStorageList;
        }

        XmlDocument xmlDoc = new XmlDocument();
        public void delete(string BoardID)
        {
            bool readSuccess = false;
            string xmlString = "";

            while (!readSuccess)
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
                    Console.WriteLine($"Error reading XML file: {ex.Message}");

                }
            }

            xmlString = "<root>" + xmlString + "</root>";

            xmlDoc.LoadXml(xmlString);

            // 选择所有的 BoardAvailable 节点
            XmlNodeList boardAvailableNodes = xmlDoc.SelectNodes("//BoardAvailable");

            // 使用 LINQ 查询选择具有指定 BoardId 属性的节点
            var query = from XmlNode node in boardAvailableNodes
                        where node.Attributes["BoardId"]?.Value == BoardID
                        select node;

            // 删除匹配的节点
            foreach (XmlNode node in query.ToList())
            {
                node.ParentNode.RemoveChild(node);
            }

            // 删除指定的 Hermes 元素，如果其下没有 BoardAvailable 子元素
            XmlNodeList hermesNodes = xmlDoc.SelectNodes("//Hermes");
            foreach (XmlNode hermesNode in hermesNodes)
            {
                if (hermesNode.SelectSingleNode("BoardAvailable") == null)
                {
                    hermesNode.ParentNode.RemoveChild(hermesNode);
                }
            }

            // 将修改后的 XML 内容写回文件，去除 <Root> 及 </Root>
            File.WriteAllText(filePath, xmlDoc.InnerXml.Replace("<root>", "").Replace("</root>", "").Replace("><", ">\r\n<").TrimStart('\r', '\n'));
        }

        private string GetValueOrDefault(XmlAttribute attribute)
        {
            return attribute != null ? attribute.Value : "";
        }
    }
}
