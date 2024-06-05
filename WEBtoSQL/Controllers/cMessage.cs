using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;

namespace Global
{
    /// <summary>
    /// 訊息記錄器
    /// </summary>
    public class MessageLogger
    {
        /// <summary>
        /// 預設訊息的目錄名稱
        /// </summary>
        private const string DirectoryName_Default = "Operation_Log";

        /// <summary>
        /// 操作訊息的記錄器名稱
        /// </summary>
        private const string LoggerName_Oper = "Operation";

        //********************************  事件定義  ********************************

        /// <summary>
        /// 另存操作訊息
        /// </summary>
        public EventHandler<OperationMessageSaveAsEventArgs> OperationMessageSaveAs;

        /// <summary>
        /// 另存例外錯誤訊息
        /// <para>僅計對內部所產生的例外錯誤, 不包含由外部傳入的例外錯誤資料</para>
        /// </summary>
        public EventHandler<ExceptionMessageSaveAsEventArgs> ExceptionMessageSaveAs;

        //********************************  變數成員  ********************************

        /// <summary>
        /// 記錄器設定元件
        /// </summary>
        private LoggingConfiguration _config = new LoggingConfiguration();

        /// <summary>
        /// 刪除逾時記錄的最後確認日期
        /// </summary>
        private DateTime LogCheckDate = new DateTime();

        //********************************  外部存取  ********************************

        //********************************  內部存取  ********************************

        /// <summary>
        /// 類別名稱
        /// </summary>
        private string ClassName
        {
            get { return System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name; }
        }

        /// <summary>
        /// 操作訊息的記錄檔條件
        /// </summary>
        private BufferingTargetWrapper FileTarget_Operation
        {
            get
            {
                return new BufferingTargetWrapper
                {
                    Name = LoggerName_Oper,
                    BufferSize = 100,
                    FlushTimeout = 500,
                    WrappedTarget = new FileTarget()
                    {
                        FileName = "C:\\Program Files (x86)\\Sunsda\\IPC Protocol Service\\" + DirectoryName_Default + "/${date:format=yyyyMM}/${date:format=yyyyMMdd}.txt",
                        Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff} ${message}"
                    }
                };
            }
        }

        /// <summary>
        /// 例外訊息的記錄檔條件
        /// </summary>
        private FileTarget FileTarget_Exception
        {
            get
            {
                return new FileTarget
                {
                    Name = "Exception",
                    FileName = "C:\\Program Files (x86)\\Sunsda\\IPC Protocol Service\\" + DirectoryName_Default + "/${date:format=yyyyMM}/${date:format=yyyyMMdd}_Ex.txt",
                    Layout = "${date:format=yyyy-MM-dd HH\\:mm\\:ss.fff} ${message} " +
                             "${onexception:inner=${newline}-------- Exception Trace --------${newline}${exception:format=ToString}${newline}.}"
                };
            }
        }

        //******************************  外部Function  ******************************

        /// <summary>
        /// 建立新的訊息記錄器模組
        /// </summary>
        public MessageLogger()
        {
            _config = new LoggingConfiguration();

            _config.AddRule(NLog.LogLevel.Trace, NLog.LogLevel.Error, FileTarget_Operation, LoggerName_Oper);


            _config.AddRuleForOneLevel(NLog.LogLevel.Fatal, FileTarget_Exception);

            LogManager.Configuration = _config;
        }

        /// <summary>
        /// 查詢系統訊息
        /// </summary>
        /// <param name="Start_Time">開始時間</param>
        /// <param name="End_Time">結束時間</param>
        /// <returns></returns>
        public List<string> QuerySystemMessage(DateTime Start_Time, DateTime End_Time)
        {
            try
            {
                List<string> retList = new List<string>();

                for (DateTime dtDate = Start_Time.Date; dtDate < End_Time; dtDate = dtDate.AddDays(1))
                {
                    string strQueryPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + DirectoryName_Default + "\\" + dtDate.ToString("yyyyMM") + "\\";
                    string strFileName = dtDate.ToString("yyyyMMdd") + "_Sys.txt";

                    retList.AddRange(LoadMessage(strQueryPath + strFileName, Start_Time, End_Time));
                }

                return retList;
            }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// 查詢操作履歷
        /// </summary>
        /// <param name="Start_Time">開始時間</param>
        /// <param name="End_Time">結束時間</param>
        /// <returns></returns>
        public List<string> QueryOperationMessage(DateTime Start_Time, DateTime End_Time)
        {
            try
            {
                List<string> retList = new List<string>();

                for (DateTime dtDate = Start_Time.Date; dtDate < End_Time; dtDate = dtDate.AddDays(1))
                {
                    string strQueryPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + DirectoryName_Default + "\\" + dtDate.ToString("yyyyMM") + "\\";
                    string strFileName = dtDate.ToString("yyyyMMdd") + ".txt";

                    retList.AddRange(LoadMessage(strQueryPath + strFileName, Start_Time, End_Time));
                }

                return retList;
            }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// 查詢設備錯誤訊息履歷
        /// </summary>
        /// <param name="Start_Time">開始時間</param>
        /// <param name="End_Time">結束時間</param>
        /// <returns></returns>
        public List<string> QueryAlarmMessage(DateTime Start_Time, DateTime End_Time)
        {
            try
            {
                List<string> retList = new List<string>();

                for (DateTime dtDate = Start_Time.Date; dtDate < End_Time; dtDate = dtDate.AddDays(1))
                {
                    string strFilePath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + DirectoryName_Default + "\\" + dtDate.ToString("yyyyMM") + "\\" + dtDate.ToString("yyyyMMdd") + "_DeviceAlarm.txt";

                    retList.AddRange(LoadMessage(strFilePath, Start_Time, End_Time));
                }

                return retList;
            }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// 將操作履歷寫入至檔案
        /// <para>[ 無當日檔案時自動新增，將訊息附加至檔案的最末端 ]</para>
        /// </summary>
        /// <param name="Message_Content">訊息內容</param>
        /// <param name="Operator_Name">操作者名稱</param>
        public void SaveOperationMessage(string Message_Content, string Operator_Name)
        {
            if (Message_Content is null || Message_Content == string.Empty) { return; }

            Logger logger_oper = LogManager.GetLogger(LoggerName_Oper);

            try
            {
                logger_oper.Info((Operator_Name != string.Empty ? "[" + Operator_Name + "] " : string.Empty) + Message_Content + "\r\n");
                OperationMessageSaveAs?.BeginInvoke(this, new OperationMessageSaveAsEventArgs(DateTime.Now, Message_Content, Operator_Name), null, null);
            }
            catch (Exception ex2)
            {
                string strMessage = "{" + ClassName + "} - " + "Save Operation Log Error!\r\n";

                logger_oper.Fatal(ex2, strMessage);
                ExceptionMessageSaveAs?.BeginInvoke(this, new ExceptionMessageSaveAsEventArgs(DateTime.Now, strMessage, ex2), null, null);
            }
        }

        /// <summary>
        /// 刪除過期記錄檔 (當日已進行過則不執行)
        /// </summary>
        /// <param name="KeepDays">記錄檔保留天數</param>
        public void DeleteOverdueFiles(int KeepDays)
        {
            if (DateTime.Now.Date.Equals(LogCheckDate.Date) == true) { return; }

            LogCheckDate = DateTime.Now.Date;

            //****************  Default Path  ****************
            DirectoryInfo DirInfo_Default = new DirectoryInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\" + DirectoryName_Default + "\\");

            if (DirInfo_Default.Exists == true)
            {
                foreach (FileInfo fi in DirInfo_Default.GetFiles("*.txt", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (DateTime.Now.Date.Subtract(fi.CreationTime.Date).Days > KeepDays || DateTime.Now.Date.Subtract(fi.LastWriteTime.Date).Days > KeepDays) { fi.Delete(); }
                    }
                    catch { }
                }

                foreach (DirectoryInfo di in DirInfo_Default.GetDirectories("*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (di.Exists == true && di.GetFiles("*", SearchOption.AllDirectories).Length == 0) { di.Delete(); }
                    }
                    catch { }
                }
            }
        }

        //******************************  內部Function  ******************************

        /// <summary>
        /// 將訊息從檔案中讀出，並判別記錄時間是否在指定範圍中
        /// </summary>
        /// <param name="File_Path">檔案路徑</param>
        /// <param name="Start_Time">開始時間</param>
        /// <param name="End_Time">結束時間</param>
        /// <returns></returns>
        private List<string> LoadMessage(string File_Path, DateTime Start_Time, DateTime End_Time)
        {
            try
            {
                List<string> retList = new List<string>();

                FileInfo fi = new FileInfo(File_Path);
                if (fi.Directory.Exists == false || fi.Exists == false) { return new List<string>(); }

                using (StreamReader sr = new StreamReader(File_Path))
                {
                    DateTime dtLogTime = DateTime.MinValue;

                    while (sr.Peek() >= 0)
                    {
                        string strMSG = sr.ReadLine();

                        if (strMSG.Length >= 19 && DateTime.TryParse(strMSG.Substring(0, 19), out DateTime dtTempTime) == true)
                        {
                            dtLogTime = dtTempTime;
                        }

                        if (dtLogTime >= Start_Time && dtLogTime < End_Time) { retList.Add(strMSG); }
                    }
                }

                return retList;
            }
            catch { return new List<string>(); }
        }

        //******************************  事件資料類別  ******************************

        /// <summary>
        /// 訊息顯示事件資料類別
        /// </summary>
        public class DisplayMessageEventArgs : EventArgs
        {
            /// <summary>
            /// 記錄時間
            /// </summary>
            public DateTime RecordTime { get; private set; }

            /// <summary>
            /// 訊息內容清單
            /// </summary>
            public List<string> MessageList;

            /// <summary>
            /// 建立新的Hermes相關基本事件資料類別
            /// </summary>
            /// <param name="Record_Time">記錄時間</param>
            /// <param name="Interface_Info">訊息內容清單</param>
            public DisplayMessageEventArgs(DateTime Record_Time, List<string> Message_List)
            {
                RecordTime = Record_Time;
                MessageList = Message_List;
            }
        }

        /// <summary>
        /// 另存訊息事件資料基本類別
        /// </summary>
        public class SaveAsBaseEventArgs : EventArgs
        {
            /// <summary>
            /// 記錄時間
            /// </summary>
            public DateTime RecordTime { get; private set; }

            /// <summary>
            /// 訊息內容
            /// </summary>
            public string MessageContent { get; private set; }

            /// <summary>
            /// 建立新的另存訊息事件資料基本類別
            /// </summary>
            /// <param name="Record_Time">記錄時間</param>
            /// <param name="Message_Content">訊息內容</param>
            public SaveAsBaseEventArgs(DateTime Record_Time, string Message_Content)
            {
                RecordTime = Record_Time;
                MessageContent = Message_Content;
            }
        }

        /// <summary>
        /// 另存操作訊息事件資料類別
        /// </summary>
        public class OperationMessageSaveAsEventArgs : SaveAsBaseEventArgs
        {
            /// <summary>
            /// 操作者名稱
            /// </summary>
            public string OperatorName { get; private set; }

            /// <summary>
            /// 建立新的另存操作訊息事件資料類別
            /// </summary>
            /// <param name="Record_Time">記錄時間</param>
            /// <param name="Message_Content">訊息內容</param>
            /// <param name="Operator_Name">操作者名稱</param>
            public OperationMessageSaveAsEventArgs(DateTime Record_Time, string Message_Content, string Operator_Name) : base (Record_Time, Message_Content)
            {
                OperatorName = Operator_Name;
            }
        }

        /// <summary>
        /// 另存例外錯誤訊息事件資料類別
        /// </summary>
        public class ExceptionMessageSaveAsEventArgs : SaveAsBaseEventArgs
        {
            /// <summary>
            /// 例外資訊
            /// </summary>
            public Exception ex { get; private set; }

            /// <summary>
            /// 建立新的另存例外錯誤訊息事件資料類別
            /// </summary>
            /// <param name="Record_Time">記錄時間</param>
            /// <param name="Message_Content">訊息內容</param>
            /// <param name="Exception_Info">例外資訊</param>
            public ExceptionMessageSaveAsEventArgs(DateTime Record_Time, string Message_Content, Exception Exception_Info) : base(Record_Time, Message_Content)
            {
                ex = Exception_Info;
            }
        }

        //**********************************  列舉  **********************************

        /// <summary>
        /// 訊息等級
        /// </summary>
        public enum MessageLevel : int
        {
            /// <summary>
            /// 資料追蹤
            /// </summary>
            Trace = 0,

            /// <summary>
            /// 系統除錯
            /// </summary>
            Debug = 1,

            /// <summary>
            /// 訊息通知
            /// </summary>
            Information = 2,

            /// <summary>
            /// 警告
            /// </summary>
            Warning = 3,

            /// <summary>
            /// 錯誤
            /// </summary>
            Error = 4
        }
    }
}
