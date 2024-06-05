using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WEBtoSQL.Models
{
    internal class MVCSETTING_Example
    {
        public string Group { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }

        public string filePath = "C:\\inetpub\\wwwroot\\TB_MVCSETTING.csv"; // Replace with your CSV file path

        private static volatile bool isCancelled = false;

        public static void CancelLoad()
        {
            // 設置中斷標誌為 true
            isCancelled = true;
        }

        public List<MVCSETTING> LoadCsv()
        {
            var MVCsetting = new List<MVCSETTING>();
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
                            MVCsetting.Add(new MVCSETTING
                            {
                                Group = values[1],
                                Type = values[2],
                                Name = values[3],
                                Value = values[4],
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Details: {ex.Message}");
                }
            }
            return MVCsetting;
        }

        public void AddNew(List<MVCSETTING> MVCsetting, int id, string group, string type, string name, string value)
        {
            // Insert a new record
            MVCsetting.Add(new MVCSETTING { Group = group, Type = type, Name = name, Value = value });
            WriteCsv(filePath, MVCsetting); // Update CSV file
        }

        public void Update(List<MVCSETTING> MVCsetting, string group, string type, string name, string value)
        {
            // Update a single record based on multiple criteria
            bool isRecordUpdated = false;

            foreach (var setting in MVCsetting)
            {
                // Check if the record meets all the criteria
                if (setting.Group == group && setting.Type == type && setting.Name == name)
                {
                    setting.Value = value;
                    isRecordUpdated = true;
                    break; // Exit the loop after updating the first matching record
                }
            }

            // Write the updated records to the CSV file, if any record was updated
            if (isRecordUpdated)
            {
                WriteCsv(filePath, MVCsetting);
                Console.WriteLine("CSV file updated.");
            }
            else
            {
                Console.WriteLine("No matching record found to update.");
            }
        }

        public void Delete(List<MVCSETTING> MVCsetting, string group, string type)
        {
            var recordToRemove = MVCsetting.FirstOrDefault(s => s.Group == group && s.Type == type);
            if (recordToRemove != null)
            {
                // Remove the found record from the list
                MVCsetting.Remove(recordToRemove);
                WriteCsv(filePath, MVCsetting);
                Console.WriteLine("Record removed.");
            }
            else
            {
                Console.WriteLine("No matching record found.");
            }
        }

        public void WriteCsv(string filePath, List<MVCSETTING> MVCsetting)
        {



            {
                try
                {
                    using (var writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine("Group,Type,Name,Value"); // Write the header

                        foreach (var setting in MVCsetting)
                        {
                            writer.WriteLine($"{setting.Group},{setting.Type},{setting.Name},{setting.Value}");
                        }
                    }

    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Details: {ex.Message}");
                }
            }
        }
    }
}
