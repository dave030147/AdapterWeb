using Global;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Global.MessageLogger;

namespace WEBtoSQL.Models
{
    internal class MVCDEFAILT_SUB
    {
        public string SB_Pos { get; set; }
        public string SB_Bc { get; set; }
        public string SB_St { get; set; }
        public List<MVCDEFAILT_SUB> MVCdefailt_sub { get; internal set; }

        public string filePath = "C:\\inetpub\\wwwroot\\Data\\TB_MVCDEFAILT_SUB.csv"; // Replace with your CSV file path

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

        public List<MVCDEFAILT_SUB> LoadCsv()
        {
            var MVCdefailt_sub = new List<MVCDEFAILT_SUB>();
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
                            MVCdefailt_sub.Add(new MVCDEFAILT_SUB
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
                        m.SaveOperationMessage(ex.Message, "TB_MVCDEFAILT_SUB");
                        LoadError = true;
                    }
                }
            }
            return MVCdefailt_sub;
        }

        public void AddNew(List<MVCDEFAILT_SUB> MVCdefailt_sub, string sb_pos, string sb_bc, string sb_st)
        {
            // Insert a new record
            MVCdefailt_sub.Add(new MVCDEFAILT_SUB { SB_Pos = sb_pos, SB_Bc = sb_bc, SB_St = sb_st });
            WriteCsv(filePath, MVCdefailt_sub); // Update CSV file
        }

        public void Update(List<MVCDEFAILT_SUB> MVCdefailt_sub, string sb_pos, string sb_bc, string sb_st)
        {
            // Update a single record based on multiple criteria
            bool isRecordUpdated = false;

            foreach (var setting in MVCdefailt_sub)
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
                WriteCsv(filePath, MVCdefailt_sub);
                Console.WriteLine("CSV file updated.");
            }
            else
            {
                Console.WriteLine("No matching record found to update.");
            }
        }
        //List<MVCDEFAILT_SUB> MVCdefailt_sub = MVCDefailt_sub.LoadCsv();
        //MVCDefailt_sub.DeleteAll(MVCdefailt_sub);
        public void DeleteAll(List<MVCDEFAILT_SUB> MVCdefailt_sub)
        {
            MVCdefailt_sub.Clear();
            WriteCsv(filePath, MVCdefailt_sub);
        }

        public void Delete(List<MVCDEFAILT_SUB> MVCdefailt_sub, string sb_pos)
        {
            var recordToRemove = MVCdefailt_sub.FirstOrDefault(s => s.SB_Pos == sb_pos);
            if (recordToRemove != null)
            {
                // Remove the found record from the list
                MVCdefailt_sub.Remove(recordToRemove);
                WriteCsv(filePath, MVCdefailt_sub);
                Console.WriteLine("Record removed.");
            }
            else
            {
                Console.WriteLine("No matching record found.");
            }
        }

        public void WriteCsv(string filePath, List<MVCDEFAILT_SUB> MVCdefailt_sub)
        {
            bool writeSuccess = false;

            while (!writeSuccess)
            {
                try
                {
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("SB_Pos,SB_Bc,SB_St"); // Write the header

                        foreach (var setting in MVCdefailt_sub)
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
