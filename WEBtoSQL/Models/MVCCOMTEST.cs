using System.Data;
using System.Data.SqlClient;

namespace WEBtoSQL.Models
{
    internal class MVCCOMTEST
    {
        public string Type { get; set; }
        public string STATUS { get; set; }

        public string LoadData(string type)
        {
            try
            {
                // 定義連接字串
                string connectionString = @"Data Source=localhost;Initial Catalog=DB_ADAPTER;Integrated Security=True";

                // 檢索資料的 SQL 查詢
                string SQL = "SELECT STATUS FROM TB_MVCCOMTEST WHERE Type = @type";

                // 建立連接和資料適配器
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // 打開連接
                    con.Open();

                    // 建立帶有 SQL 查詢和連接的資料適配器
                    using (SqlDataAdapter da = new SqlDataAdapter(SQL, con))
                    {
                        // 將參數添加到 SQL 命令中
                        da.SelectCommand.Parameters.AddWithValue("@type", type);

                        // 使用查詢結果填充資料集
                        DataSet ds = new DataSet();
                        da.Fill(ds);

                        // 檢查是否返回了任何行
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            // 返回第一行中的值
                            return ds.Tables[0].Rows[0]["STATUS"].ToString();
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

        public void Update(string type, string status)
        {
            try
            {
                // 定義連接字符串
                string connectionString = @"Data Source=localhost;Initial Catalog=DB_ADAPTER;Integrated Security=True";

                // 更新數據的 SQL 查詢
                string SQL = "UPDATE TB_MVCCOMTEST SET STATUS=@status WHERE type = @type";

                // 建立連接和命令
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(SQL, con))
                    {
                        // 將參數添加到命令中
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@status", status);

                        // 打開連接
                        con.Open();

                        // 執行命令
                        cmd.ExecuteNonQuery();

                        // 關閉連接
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
