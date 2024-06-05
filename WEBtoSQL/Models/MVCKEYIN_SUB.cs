using Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEBtoSQL.Models
{
    internal class MVCKEYIN_SUB
    {
        public string SB_Pos { get; set; }
        public string SB_Bc { get; set; }
        public string SB_St { get; set; }
        public List<MVCKEYIN_SUB> MVCkeyin_sub { get; internal set; }

        public string filePath = "C:\\inetpub\\wwwroot\\Data\\TB_MVCKEYIN_SUB.csv"; // Replace with your CSV file path

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

        public List<MVCKEYIN_SUB> LoadCsv()
        {
            var MVCkeyin_sub = new List<MVCKEYIN_SUB>();
            bool LoadSuccess = false;

            while (!LoadSuccess && !isCancelled)
            {
                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        // Skip the header
                        reader.ReadLine();

                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var values = line.Split(',');
                            MVCkeyin_sub.Add(new MVCKEYIN_SUB
                            {
                                SB_Pos = values[0],
                                SB_Bc = values[1],
                                SB_St = values[2],
                            });
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
                        m.SaveOperationMessage(ex.Message, "TB_MVCKEYIN_SUB");
                        LoadError = true;
                    }
                }
            }
                
            return MVCkeyin_sub;
        }

        public void AddNew(List<MVCKEYIN_SUB> MVCkeyin_sub, string sb_pos, string sb_bc, string sb_st)
        {
            // Insert a new record
            MVCkeyin_sub.Add(new MVCKEYIN_SUB { SB_Pos = sb_pos, SB_Bc = sb_bc, SB_St = sb_st });
            WriteCsv(filePath, MVCkeyin_sub); // Update CSV file
        }

        public void Update(List<MVCKEYIN_SUB> MVCkeyin_sub, string sb_pos, string sb_bc, string sb_st)
        {
            // Update a single record based on multiple criteria
            bool isRecordUpdated = false;

            foreach (var setting in MVCkeyin_sub)
            {
                // Check if the record meets all the criteria
                if (setting.SB_Pos == sb_pos)
                {
                    setting.SB_Bc = sb_bc;
                    setting.SB_St = sb_st;
                    isRecordUpdated = true;
                    break; // Exit the loop after updating the first matching record
                }
            }

            // Write the updated records to the CSV file, if any record was updated
            if (isRecordUpdated)
            {
                WriteCsv(filePath, MVCkeyin_sub);
                Console.WriteLine("CSV file updated.");
            }
            else
            {
                Console.WriteLine("No matching record found to update.");
            }
        }
        //List<MVCKEYIN_SUB> MVCkeyin_sub = MVCKeyin_sub.LoadCsv();
        //MVCKeyin_sub.DeleteAll(MVCkeyin_sub);
        public void DeleteAll(List<MVCKEYIN_SUB> MVCkeyin_sub)
        {
            MVCkeyin_sub.Clear();
            WriteCsv(filePath, MVCkeyin_sub);
        }
        public void Delete(List<MVCKEYIN_SUB> MVCkeyin_sub, string sb_pos)
        {
            var recordToRemove = MVCkeyin_sub.FirstOrDefault(s => s.SB_Pos == sb_pos);
            if (recordToRemove != null)
            {
                // Remove the found record from the list
                MVCkeyin_sub.Remove(recordToRemove);
                WriteCsv(filePath, MVCkeyin_sub);
                Console.WriteLine("Record removed.");
            }
            else
            {
                Console.WriteLine("No matching record found.");
            }
        }

        public void WriteCsv(string filePath, List<MVCKEYIN_SUB> MVCkeyin_sub)
        {
            bool writeSuccess = false;

            while (!writeSuccess)
            {
                try
                {
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("SB_Pos,SB_Bc,SB_St"); // Write the header

                        foreach (var setting in MVCkeyin_sub)
                        {
                            writer.WriteLine($"{setting.SB_Pos},{setting.SB_Bc},{setting.SB_St}");
                        }
                    }
                    writeSuccess = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Details: {ex.Message}");

                }
            }
        }
    }
}
