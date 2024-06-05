using System.Data;
using System.Data.SqlClient;

namespace WEBtoSQL.Models
{
    internal class MVCCLOUMNRUNNING
    {
        public string ColumnName { get; set; }
        public string IfRunning { get; set; }

        public string LoadData(string columnname)
        {
            try
            {
                // 定義連接字串
                string connectionString = @"Data Source=localhost;Initial Catalog=DB_ADAPTER;Integrated Security=True";

                // 檢索資料的 SQL 查詢
                string SQL = "SELECT IfRunning FROM TB_MVCCLOUMNRUNNING WHERE ColumnName = @columnname";

                // 建立連接和資料適配器
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 打開連接
                    con.Open();

                    // 建立帶有 SQL 查詢和連接的資料適配器
                    using (SqlDataAdapter da = new SqlDataAdapter(SQL, con))
                    {
                        // 將參數添加到 SQL 命令中
                        da.SelectCommand.Parameters.AddWithValue("@columnname", columnname);

                        // 使用查詢結果填充資料集
                        DataSet ds = new DataSet();
                        da.Fill(ds);

                        // 檢查是否返回了任何行
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            // 返回第一行中的值
                            return ds.Tables[0].Rows[0]["IfRunning"].ToString();
                        }
                    }
                }

                // 如果沒有返回任何行或出現錯誤，則返回 null
                return "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                // 如果沒有返回任何行或出現錯誤，則返回 null
                return "";
            }
        }

        public List<MVCCLOUMNRUNNING> LoadALLData()
        {
            try
            {
                // 定義連接字串
                string connectionString = @"Data Source=localhost;Initial Catalog=DB_ADAPTER;Integrated Security=True";

                // 檢索所有資料的 SQL 查詢
                string SQL = "SELECT * FROM TB_MVCCLOUMNRUNNING";

                // 建立連接和資料適配器
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 打開連接
                    con.Open();

                    // 建立帶有 SQL 查詢和連接的資料適配器
                    using (SqlDataAdapter da = new SqlDataAdapter(SQL, con))
                    {
                        // 使用查詢結果填充資料集
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // 定義儲存 MVCSETTING 物件的列表
                        List<MVCCLOUMNRUNNING> runnings = new List<MVCCLOUMNRUNNING>();

                        // 將每一行資料轉換為 MVCSETTING 物件並添加到列表中
                        foreach (DataRow row in dt.Rows)
                        {
                            MVCCLOUMNRUNNING running = new MVCCLOUMNRUNNING
                            {
                                ColumnName = row["ColumnName"].ToString(),
                                IfRunning = row["IfRunning"].ToString()
                            };
                            runnings.Add(running);
                        }

                        // 返回列表
                        return runnings;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                return null;
            }
        }
    }
}
