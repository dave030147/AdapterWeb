using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace WEBtoSQL.Controllers
{
	public class cSetting
	{
		public const string strXmlHeader = "Setting";
		private readonly string FilePath_Default = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "Setting.cfg";

		private string SettingData
		{
			get { return System.Reflection.MethodBase.GetCurrentMethod()!.DeclaringType!.Name; }
		}

		public string Language { get; set; } = string.Empty;


        public cSetting()
		{
			LoadFile(FilePath_Default);
		}

        private void SaveTimerCallback(object state)
        {
            using (StreamReader sr = new StreamReader(FilePath_Default))
            {
                Save();
            }
        }


        private cSetting(bool IsLoadData)
		{
			if (IsLoadData) { Load(); }
		}

		public bool Load()
		{
			return LoadFile(FilePath_Default);
		}

		public bool Save()
		{
			return SaveFile(FilePath_Default);
		}

		private bool LoadFile(string File_Path)
		{
			try
			{
				if (File.Exists(File_Path))
				{
					XmlDocument xmlDoc = new XmlDocument();
					string strData = string.Empty;

					using (StreamReader sr = new StreamReader(File_Path))
					{
						strData = sr.ReadToEnd();
					}
					xmlDoc.LoadXml(strData);

					if (xmlDoc.ChildNodes.Count > 0)
					{
						XmlElement xmlRoot = (XmlElement)xmlDoc.FirstChild!;

						foreach (XmlElement xmlEle in xmlRoot!.ChildNodes)
						{
							switch (xmlEle.Name)
							{
								case "Language":
									Language = xmlEle.InnerText;
									break;
                            }
						}

						return true;
					}
					else
					{
						if (File_Path == FilePath_Default)
						{
							SetDefault();
							Save();
						}
						return false;
					}
				}
				else
				{
					if (File_Path == FilePath_Default)
					{
						SetDefault();
						Save();
					}
					return false;
				}
			}
			catch (Exception)
			{
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

				XmlElement xmlRoot = xmlDoc.CreateElement(strXmlHeader);

				XmlElement xmlLanguage = xmlDoc.CreateElement("Language");
				xmlLanguage.InnerText = Language;
				xmlRoot.AppendChild(xmlLanguage);
                xmlDoc.AppendChild(xmlRoot);

				using (StreamWriter sw = new StreamWriter(File_Path, false))
				{
					sw.Write(xmlDoc.OuterXml);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private void SetDefault()
		{
			Language = "en-US";
        }
    }
}
