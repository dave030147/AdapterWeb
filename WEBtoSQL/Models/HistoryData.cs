using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WEBtoSQL.Models
{
    internal class HistoryData
    {
        public string? Timestamp { get; set; }
        public string? Facing { get; set; }
        public string? Type { get; set; }
        public string? BoardID { get; set; }
        public string? CreatedBy { get; set; }
        public string? Failed { get; set; }
        public string? ProductTypeID { get; set; }
        public string? Flipped { get; set; }
        public string? TopBarcode { get; set; }
        public string? BottomBarcode { get; set; }
        public string? Length { get; set; }
        public string? Width { get; set; }
        public string? Thickness { get; set; }
        public string? ConveyorSpeed { get; set; }
        public string? TopClearanceHeight { get; set; }
        public string? BottomClearanceHeight { get; set; }
        public string? Weight { get; set; }
        public string? WorkOrderID { get; set; }
        public string? BatchID { get; set; }
        public string? Route { get; set; }
        public string? Action { get; set; }
        public string? SUB { get; set; }

        public List<HistoryData> BoardArrivedHistory { get; internal set; }
        public List<HistoryData> BoardDepartedHistory { get; internal set; }

        string[] messageTypes = { "SendBoardInfo", "BoardAvailable", "SendWorkOrderInfo" };

        string fileContent;

        static DateTime Date = DateTime.Now;

        string filePath = "C:\\Program Files (x86)\\Sunsda\\IPC Protocol Service\\Log_Trans\\" + Date.ToString("yyyyMM") + "\\" + Date.ToString("yyyyMMdd") + "_HermesMessage.txt"; // Replace with your CSV file path

        public string OutputFilePath { get; set; } // CSV 文件輸出路徑

        public string datetime { get; set; }

        public List<HistoryData> ConvertXmlToList(string type , DateTime time)
        {
            bool readSuccess = false;
            List<HistoryData> HistoryList = new List<HistoryData>();

            while (!readSuccess)
            {
                try
                {
                    filePath = "C:\\Program Files (x86)\\Sunsda\\IPC Protocol Service\\Log_Trans\\" + time.ToString("yyyyMM") + "\\" + time.ToString("yyyyMMdd") + "_HermesMessage.txt"; // Replace with your CSV file path

                    if (File.Exists(filePath))
                    {
                        fileContent = File.ReadAllText(filePath);
                    }
                    else
                    {
                        fileContent = "";
                        Console.WriteLine($"File not found: {filePath}");
                    }
                    readSuccess = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading XML file: {ex.Message}");
                }
            }
            if(fileContent != null)
            {
                // 使用 ".END" 作為切割模式
                string[] messages = fileContent.Split(new[] { ".END" }, StringSplitOptions.RemoveEmptyEntries);
                datetime = Date.ToString();

                // 印出每個訊息
                foreach (string message in messages)
                {
                    // 使用正則表達式來匹配包含完整信文的文本片段
                    foreach (string messageType in messageTypes)
                    {
                        if (message.Contains(messageType) && !message.Contains("ServiceDescription"))
                        {
                            //Console.WriteLine(message);

                            // 創建新的 HistoryData 物件
                            HistoryData data = new HistoryData();

                            // 使用正則表達式匹配時間戳
                            string timestampPattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}\.\d{3})";
                            Match timestampMatch = Regex.Match(message, timestampPattern);
                            string timestamp = timestampMatch.Success ? timestampMatch.Groups[1].Value : "";

                            // 使用正則表達式匹配面向
                            string facingPattern = @"Facing:\s*([^\|]+)";
                            Match facingMatch = Regex.Match(message, facingPattern);
                            string facing = facingMatch.Success ? facingMatch.Groups[1].Value.Trim() : "";

                            // 創建 XML 字串並解析為 XmlDocument
                            string xmlString = "<root>" + message + "</root>";
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.LoadXml(xmlString);

                            if (message.Contains("BoardAvailable") && ((type == "Arrived" && facing == "Upstream") || (type == "Departed" && facing == "Downstream")))
                            {
                                // 提取 XML 中的屬性值並設定到 HistoryData 物件中
                                XmlNode Node = xmlDoc.SelectSingleNode("//BoardAvailable");
                                if (Node != null)
                                {
                                    data.Type = "Main";
                                    data.Timestamp = timestamp;
                                    data.Facing = facing;
                                    data.BoardID = Node.Attributes["BoardId"]?.Value;
                                    data.CreatedBy = Node.Attributes["BoardIdCreatedBy"]?.Value;
                                    data.Failed = Node.Attributes["FailedBoard"]?.Value;
                                    data.ProductTypeID = Node.Attributes["ProductTypeId"]?.Value;
                                    data.Flipped = Node.Attributes["FlippedBoard"]?.Value;
                                    data.TopBarcode = Node.Attributes["TopBarcode"]?.Value;
                                    data.BottomBarcode = Node.Attributes["BottomBarcode"]?.Value;
                                    data.Length = Node.Attributes["Length"]?.Value;
                                    data.Width = Node.Attributes["Width"]?.Value;
                                    data.Thickness = Node.Attributes["Thickness"]?.Value;
                                    data.ConveyorSpeed = Node.Attributes["ConveyorSpeed"]?.Value;
                                    data.TopClearanceHeight = Node.Attributes["TopClearanceHeight"]?.Value;
                                    data.BottomClearanceHeight = Node.Attributes["BottomClearanceHeight"]?.Value;
                                    data.Weight = Node.Attributes["Weight"]?.Value;
                                    data.WorkOrderID = Node.Attributes["WorkOrderId"]?.Value;
                                    data.BatchID = Node.Attributes["BatchId"]?.Value;
                                    data.Route = Node.Attributes["Route"]?.Value;
                                    data.Action = Node.Attributes["Action"]?.Value;

                                    XmlNodeList subBoardNodes = Node.SelectNodes("SubBoards/SB");
                                    if (subBoardNodes != null && subBoardNodes.Count > 0)
                                    {
                                        foreach (XmlNode subBoardNode in subBoardNodes)
                                        {
                                            data.Type = "Sub";
                                            data.Timestamp = timestamp;
                                            string pos = subBoardNode.Attributes["Pos"]?.Value;
                                            string bc = subBoardNode.Attributes["Bc"]?.Value;
                                            string st = subBoardNode.Attributes["St"]?.Value;

                                            data.SUB += $"Pos: {pos}, Bc: {bc}, St: {st}; ";
                                        }
                                    }

                                    // 將 HistoryData 物件添加到列表中
                                    HistoryList.Add(data);
                                }
                            }
                            else if (message.Contains("SendWorkOrderInfo") && type == "Arrived")
                            {
                                // 提取 XML 中的屬性值並設定到 HistoryData 物件中
                                XmlNode Node = xmlDoc.SelectSingleNode("//SendWorkOrderInfo");
                                if (Node != null)
                                {
                                    data.Timestamp = timestamp;
                                    data.Facing = facing;
                                    data.BoardID = Node.Attributes["BoardId"]?.Value;
                                    data.CreatedBy = Node.Attributes["BoardIdCreatedBy"]?.Value;
                                    data.Failed = Node.Attributes["FailedBoard"]?.Value;
                                    data.ProductTypeID = Node.Attributes["ProductTypeId"]?.Value;
                                    data.Flipped = Node.Attributes["FlippedBoard"]?.Value;
                                    data.TopBarcode = Node.Attributes["TopBarcode"]?.Value;
                                    data.BottomBarcode = Node.Attributes["BottomBarcode"]?.Value;
                                    data.Length = Node.Attributes["Length"]?.Value;
                                    data.Width = Node.Attributes["Width"]?.Value;
                                    data.Thickness = Node.Attributes["Thickness"]?.Value;
                                    data.ConveyorSpeed = Node.Attributes["ConveyorSpeed"]?.Value;
                                    data.TopClearanceHeight = Node.Attributes["TopClearanceHeight"]?.Value;
                                    data.BottomClearanceHeight = Node.Attributes["BottomClearanceHeight"]?.Value;
                                    data.Weight = Node.Attributes["Weight"]?.Value;
                                    data.WorkOrderID = Node.Attributes["WorkOrderId"]?.Value;
                                    data.BatchID = Node.Attributes["BatchId"]?.Value;
                                    data.Route = Node.Attributes["Route"]?.Value;
                                    data.Action = Node.Attributes["Action"]?.Value;

                                    // 將 HistoryData 物件添加到列表中
                                    HistoryList.Add(data);
                                }
                            }
                            else if (message.Contains("SendBoardInfo") && type == "Arrived")
                            {
                                // 提取 XML 中的屬性值並設定到 HistoryData 物件中
                                XmlNode Node = xmlDoc.SelectSingleNode("//SendBoardInfo");
                                if (Node != null)
                                {
                                    data.Timestamp = timestamp;
                                    data.Facing = facing;
                                    data.BoardID = Node.Attributes["BoardId"]?.Value;
                                    data.CreatedBy = Node.Attributes["BoardIdCreatedBy"]?.Value;
                                    data.Failed = Node.Attributes["FailedBoard"]?.Value;
                                    data.ProductTypeID = Node.Attributes["ProductTypeId"]?.Value;
                                    data.Flipped = Node.Attributes["FlippedBoard"]?.Value;
                                    data.TopBarcode = Node.Attributes["TopBarcode"]?.Value;
                                    data.BottomBarcode = Node.Attributes["BottomBarcode"]?.Value;
                                    data.Length = Node.Attributes["Length"]?.Value;
                                    data.Width = Node.Attributes["Width"]?.Value;
                                    data.Thickness = Node.Attributes["Thickness"]?.Value;
                                    data.ConveyorSpeed = Node.Attributes["ConveyorSpeed"]?.Value;
                                    data.TopClearanceHeight = Node.Attributes["TopClearanceHeight"]?.Value;
                                    data.BottomClearanceHeight = Node.Attributes["BottomClearanceHeight"]?.Value;
                                    data.Weight = Node.Attributes["Weight"]?.Value;
                                    data.WorkOrderID = Node.Attributes["WorkOrderId"]?.Value;
                                    data.BatchID = Node.Attributes["BatchId"]?.Value;
                                    data.Route = Node.Attributes["Route"]?.Value;
                                    data.Action = Node.Attributes["Action"]?.Value;

                                    // 將 HistoryData 物件添加到列表中
                                    HistoryList.Add(data);
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(HistoryList);
            return HistoryList;
        }
    }
}
