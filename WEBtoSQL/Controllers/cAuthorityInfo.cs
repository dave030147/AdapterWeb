using System.Xml;

namespace Kanban.Models
{
    public class AuthorityInfo
    {
        public const string XmlHeader = "AuthorityInfo";
        private readonly string _filePathDefault = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "AuthorityInfo.cfg";

        //********************************  工程帳號  ********************************

        /// <summary>
        /// 工程帳號的使用者名稱
        /// </summary>
        private readonly string Service_UserName = "Sunsda";

        /// <summary>
        /// 工程帳號的登入帳號
        /// </summary>
        private readonly string Service_Account = "sunsda";

        /// <summary>
        /// 工程帳號的登入密碼
        /// </summary>
        private string Service_Password
        {
            get
            {
                return DateTime.Now.ToString("MMdd") + "34637199" + DateTime.Now.ToString("HHmm");
            }
        }

        private static Dictionary<string, Tuple<string, string, string>> UserMaterial = new Dictionary<string, Tuple<string, string ,string>>();
        //public string UserAccount { get; set; } = string.Empty;
        //public string UserName { get; set; } = string.Empty;
        //public string PassWord { get; set; } = string.Empty;
        //public string UserLevel { get; set; } = string.Empty;
        public List<Tuple<string, string, string, string>> GetUserMaterials()
        {
            return UserMaterial.Select(kvp => new Tuple<string, string, string, string>(kvp.Key, kvp.Value.Item1, kvp.Value.Item2, kvp.Value.Item3)).ToList();
        }

        public AuthorityInfo()
        {
            LoadFile(_filePathDefault);
        }

        private AuthorityInfo(bool IsLoadData)
        {
            if (IsLoadData) { Load(); }
        }

        public bool Load()
        {
            return LoadFile(_filePathDefault);
        }

        public bool Save()
        {
            return SaveFile(_filePathDefault);
        }
        
        // 更新使用者帳號密碼名稱及權限
        public bool UpdateUserMaterial(string Account, string Name, string PW, string Level ,bool isModify)
        {
            if (UserMaterial.ContainsKey(Account))
            {
                if (isModify)
                {
                    UserMaterial[Account] = new Tuple<string, string, string>(Name, PW, Level);
                    Save();
                    return true;
                }
                else
                {
                    return false;
                }
                
            }
            else
            {
                UserMaterial.Add(Account, new Tuple<string, string, string>(Name, PW, Level));
                Save();
                return true;
            }
        }

        // 取得使用者帳號密碼名稱及權限
        public Tuple<string, string, string> GetUserMaterial(string Account)
        {
            if (Account == Service_Account)
            {
                return new Tuple<string, string, string>(Service_UserName, Service_Password, "Administrator");
            }
            else
            {
                if (UserMaterial.TryGetValue(Account, out var Name_PW_Level))
                {
                    return Name_PW_Level;
                }
                else
                {
                    return new Tuple<string, string, string>("無此帳號", "無此帳號", "無此帳號");
                }
            }
        }

        // 刪除使用者帳號密碼名稱及權限
        public bool DeleUserMaterial(string Account)
        {
            if (UserMaterial.ContainsKey(Account))
            {
                UserMaterial.Remove(Account);
                Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        //// 重置使用者帳號密碼名稱及權限
        //public void ResetUserMaterial()
        //{
        //    UserMaterial.Clear();
        //}

        private bool LoadFile(string File_Path)
        {
            try
            {
                if (File.Exists(File_Path))
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    string strData = string.Empty;

                    // 使用 using 敘述確保 StreamReader 資源被正確釋放
                    using (StreamReader sr = new StreamReader(File_Path))
                    {
                        strData = sr.ReadToEnd();
                    }

                    xmlDoc.LoadXml(strData);

                    if (xmlDoc.DocumentElement?.Name == "AuthorityInfo")
                    {
                        XmlNodeList userMaterialNodes = xmlDoc.SelectNodes("/AuthorityInfo/UserMaterial")!;

                        if (userMaterialNodes != null)
                        {
                            foreach (XmlNode userMaterialNode in userMaterialNodes)
                            {
                                string account = userMaterialNode.Attributes?["Account"]?.Value!;
                                string name = userMaterialNode.Attributes?["Name"]?.Value!;
                                string pw = userMaterialNode.Attributes?["PW"]?.Value!;
                                string level = userMaterialNode.Attributes?["Level"]?.Value!;

                                if (!string.IsNullOrEmpty(account))
                                {
                                    UserMaterial[account] = new Tuple<string, string, string>(name, pw, level);
                                }
                            }

                            return true;
                        }
                    }

                    // 若 XML 格式不正確，或無法找到相應元素，可能需要進行錯誤處理
                    return false;
                }
                else
                {
                    // 若檔案不存在，可能需要進行錯誤處理
                    return false;
                }
            }
            catch (Exception)
            {
                // 處理例外狀況，例如記錄錯誤訊息
                return false;
            }
        }


        private bool SaveFile(string File_Path)
        {
            try
            {
                FileInfo fi = new FileInfo(File_Path);

                if (!fi.Directory!.Exists)
                {
                    fi.Directory.Create();
                }

                XmlDocument xmlDoc = new XmlDocument();

                XmlElement xmlRoot = xmlDoc.CreateElement(XmlHeader);

                foreach (var kvp in UserMaterial)
                {
                    XmlElement UserMaterialEle = xmlDoc.CreateElement("UserMaterial");
                    UserMaterialEle.SetAttribute("Account", kvp.Key);
                    UserMaterialEle.SetAttribute("Name", kvp.Value.Item1.ToString());
                    UserMaterialEle.SetAttribute("PW", kvp.Value.Item2.ToString());
                    UserMaterialEle.SetAttribute("Level", kvp.Value.Item3.ToString());

                    xmlRoot.AppendChild(UserMaterialEle);
                }

                xmlDoc.AppendChild(xmlRoot);

                // 使用 using 敘述確保 StreamWriter 資源被正確釋放
                using (StreamWriter sw = new StreamWriter(File_Path, false))
                {
                    sw.Write(xmlDoc.OuterXml);
                }

                return true;
            }
            catch (Exception)
            {
                // 處理例外狀況
                return false;
            }
        }


        private void SetDefault()
        {
            UserMaterial["admin"] = new Tuple<string, string, string>("Administrator", "admin", "Administrator");
        }
    }
}
