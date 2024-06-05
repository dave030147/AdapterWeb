using Global;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEBtoSQL.Models
{
    internal class MVCMESSAGE
    {
        public string MessageClassName { get; set; }
        public string Direction { get; set; }
        public string Message { get; set; }
        public string update_time { get; set; }

        public string filePath = "C:\\inetpub\\wwwroot\\Data\\TB_MVCMESSAGE.csv"; // Replace with your CSV file path

        private static volatile bool isCancelled = false;

        public bool LoadError = false;
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

        public List<MVCMESSAGE> LoadCsv()
        {
            var MVCmessage = new List<MVCMESSAGE>();
            bool LoadSuccess = false;

            while (!LoadSuccess && !isCancelled)
            {
                try
                {
                    using (var parser = new TextFieldParser(filePath))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");

                        // Skip the header
                        parser.ReadLine();

                        while (!parser.EndOfData)
                        {
                            string[] fields = parser.ReadFields();

                            // Check if the line has enough fields
                            if (fields.Length >= 4)
                            {
                                MVCmessage.Add(new MVCMESSAGE
                                {
                                    MessageClassName = fields[0],
                                    Direction = fields[1],
                                    Message = fields[2],
                                    update_time = fields[3],
                                });
                            }
                        }
                    }
                    LoadSuccess = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Details: {ex.Message}");

                    if (!ex.Message.Contains("because it is being used by another process."))
                    {
                        MessageLogger m = new MessageLogger();
                        m.SaveOperationMessage(ex.Message, "TB_MVCMESSAGE");
                        LoadError = true;
                    }
                }
            }
            return MVCmessage;
        }
    }
}
