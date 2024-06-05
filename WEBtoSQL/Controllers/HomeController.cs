using Kanban.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.ServiceProcess;
using Web.Hubs;
using WEBtoSQL.Models;
using System.IO.Ports;
using FiftyOne.Foundation.Mobile.Detection.Entities;
using Microsoft.AspNetCore.Diagnostics;
using Global;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Globalization;
using System.Collections;

namespace WEBtoSQL.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<SingleRHub> _hubContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<HomeController> _logger;
        private List<MVCSETTING> MVCsetting;
        private List<MVCDEFAILT_SUB> MVCdefailt_sub;
        private List<MVCDEFAILT_SUB_New> MVCdefailt_sub_new;
        private List<MVCKEYIN_SUB> MVCkeyin_sub;

        private string[] myPorts = new string[2];
        private static string User_Name = "", User_PW = "", User_Level = "", Error_File = "";
        cLanguage cLanguage = new cLanguage();
        cSetting cSetting = new cSetting();
        private static string Language_now = "";
        private int Language = 0;
        private Timer timer;
        private static bool hasread = false;

        //宣告資料庫
        static MVCALWAYSREAD MVCAlwaysread = new MVCALWAYSREAD();
        static MVCINQUIRYBARCODE MVCInquirybarcode = new MVCINQUIRYBARCODE();
        static MVCSETTING MVCSetting = new MVCSETTING();
        static MVCCLOUMNRUNNING MVCCloumnrunning = new MVCCLOUMNRUNNING();
        static MVCDEFAILT_SUB MVCDefailt_sub = new MVCDEFAILT_SUB();
        static MVCDEFAILT_SUB_New MVCDefailt_sub_new = new MVCDEFAILT_SUB_New();
        static MVCKEYIN_SUB MVCKeyin_sub = new MVCKEYIN_SUB();
        static XMLtoList_Single XMLtoList_Single = new XMLtoList_Single();
        static XMLtoList_FIFO XMLtoList_FIFO = new XMLtoList_FIFO();
        static SendXML SendXML = new SendXML();
        static ReceviceXML ReceviceXML = new ReceviceXML();
		static SYSTEMXML SystemXML = new SYSTEMXML();
		static MVCCOMTEST MVCComtest = new MVCCOMTEST();
        static HistoryData HistoryData = new HistoryData();

        // 取得程式啟動的時間
        static DateTime starttime = DateTime.Now;

        /*****  SinglR函數   *****/
        private static string Upstream_Color = "", Downstream_Color = "", Supervisory_Color = "", KeyinIsAsked = "", TopBarCode = "", BottomBarCode = "",
            OperatingStatus_Color = "", OperatingStatus_disconnected = "", OperatingStatus_unknown = "", OperatingStatus_standby = "", OperatingStatus_manual = "",
            OperatingStatus_auto = "", OperatingStatus_warning = "", OperatingStatus_alarm = "", LinkMode_online = "", LinkMode_offline = "", LinkMode_Color = "",
            AlarmMessaage = "",  Upstream_Status_Color = "", Downstream_Status_Color = "", Supervisory_Status_Color = "", AlarmState_Status_Color = "", 
            AlarmState_Status_havealarm = "", AlarmState_Status_normal = "", SendMessageFrom = "", SendMessaage = "", ReceiveMessaageFrom = "", ReceiveMessaage = "",
            SystemMessaage = "", AlarmState = "", License_Hermes = "", QBIisAsked = "", Lockinput = "", BoardMode = "", isSingle = "";

        //0.5觸發一次更新資料到網頁上
        public void StartTimer()
        {
            // 創建 Timer，第一個參數是回調函數，第二個參數是狀態對象，第三個參數是延遲開始的時間，第四個參數是計時器間隔
            timer = new Timer(TimerCallback!, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(500));
        }

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment, IHubContext<SingleRHub> hubContext, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _hostingEnvironment = hostingEnvironment;
            _hubContext = hubContext;

            // 指定要檢查的服務名稱
            string serviceName = "SUNSDAMVC";

            // 使用 ServiceController 類來檢查服務狀態
            ServiceController sc = new ServiceController(serviceName);

            try
            {
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    StartTimer();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("發生錯誤: " + ex.Message);
            }

            // 加載 CSV 檔案
            MVCsetting = MVCSetting.LoadALLData();
            MVCdefailt_sub = MVCDefailt_sub.LoadCsv();
            MVCdefailt_sub_new = MVCDefailt_sub_new.LoadCsv();
            MVCkeyin_sub = MVCKeyin_sub.LoadCsv();
        }

        //等待畫面，備用重啟伺服器，重設讀取
        public IActionResult Waiting()
        {
            // 指定要檢查的服務名稱
            string serviceName = "SUNSDAMVC";

            // 使用 ServiceController 類來檢查服務狀態
            ServiceController sc = new ServiceController(serviceName);

            Language_now = cSetting.Language;
            ViewBag.Language_now = Language_now;
            Language = cLanguage.Language(Language_now);

            try
            {
                ViewBag.ServiceStatus = cLanguage.Waiting_title_start[Language];
                ViewBag.WaitMessage = cLanguage.Waiting_Message_start[Language];
                if (sc.Status == ServiceControllerStatus.Running)
                {
                    SendXML.ReLoad();
                    ReceviceXML.ReLoad();
                    return RedirectToAction("index", "Home");
                }
                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    ViewBag.ServiceStatus = cLanguage.Waiting_title_stop[Language];
                    ViewBag.WaitMessage = cLanguage.Waiting_Message_stop[Language];
                    sc.Start();
                }
            }
            catch (Exception ex)
            {
                ViewBag.ServiceStatus = cLanguage.Waiting_title_erroe[Language];
                ViewBag.WaitMessage = cLanguage.Waiting_Message_error[Language];
                Console.WriteLine("Error occurred: " + ex.Message);
            }

            return View();
        }

        [HttpPost]
        //更新當前語系
        public ActionResult Index_UpdateLanguage(string Language)
        {
            cSetting.Language = Language;
            cSetting.Save();
            return Content("OK");
        }
        
        //首頁
        public IActionResult Index()
        {
            // 指定要檢查的服務名稱
            string serviceName = "SUNSDAMVC";

            // 使用 ServiceController 類來檢查服務狀態
            ServiceController sc = new ServiceController(serviceName);

            try
            {
                if (sc.Status != ServiceControllerStatus.Running)
                {
                    return RedirectToAction("Waiting", "Home");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred: " + ex.Message);
                return RedirectToAction("Waiting", "Home");
            }

            MessageLogger m = new MessageLogger();

            #region 多國語系
            /*****   多國語系的ViewBag   *****/
            Language_now = cSetting.Language;
            ViewBag.Language_now = Language_now;
            Language = cLanguage.Language(Language_now);
            /***  Header ***/
            ViewBag.Header_Home = cLanguage.Header_Home[Language];
            ViewBag.Header_accountmanage = cLanguage.Header_accountmanage[Language];
            ViewBag.Header_setting = cLanguage.Header_setting[Language];
            ViewBag.Header_comtest = cLanguage.Header_comtest[Language];
            ViewBag.Header_resetserver = cLanguage.Header_resetserver[Language];
            ViewBag.Header_resetalarm = cLanguage.Header_resetalarm[Language];
            ViewBag.Header_language = cLanguage.Header_language[Language];
            ViewBag.Header_login = cLanguage.Header_login[Language];
            ViewBag.Header_logout = cLanguage.Header_logout[Language];
            ViewBag.Header_history = cLanguage.Herder_history[Language];
            /***  訊息視窗 ***/
            ViewBag.btn_msgbox_ok = cLanguage.btn_msgbox_ok[Language];
            ViewBag.btn_msgbox_cancel = cLanguage.btn_msgbox_cancel[Language];
            /***  登入視窗  ***/
            ViewBag.modal_account = cLanguage.modal_account[Language];
            ViewBag.modal_userid = cLanguage.modal_userid[Language];
            ViewBag.modal_userpassword = cLanguage.modal_userpassword[Language];
            ViewBag.modal_login = cLanguage.modal_login[Language];
            /***  測試操作視窗  ***/
            ViewBag.modal_equipmentname = cLanguage.modal_equipmentname[Language];
            ViewBag.lab_RN = cLanguage.lab_RN[Language];
            /* Message */
            ViewBag.modal_msg_loginsuccess = cLanguage.modal_msg_loginsuccess[Language];
            ViewBag.modal_msg_loginfailed_password = cLanguage.modal_msg_loginfailed_password[Language];
            ViewBag.modal_msg_loginfailed_id = cLanguage.modal_msg_loginfailed_id[Language];
            ViewBag.msg_isvaluebiggerthan0 = cLanguage.msg_isvaluebiggerthan0[Language];
            ViewBag.msg_noMRnoTest = cLanguage.msg_noMRnoTest[Language];
            ViewBag.msg_subsuccess = cLanguage.msg_subsuccess[Language];
            /***  帳號管理  ***/
            ViewBag.modal_accountmanagement = cLanguage.modal_accountmanagement[Language];
            ViewBag.modal_addaccount = cLanguage.modal_addaccount[Language];
            ViewBag.modal_userlv = cLanguage.modal_userlv[Language];
            ViewBag.modal_username = cLanguage.modal_username[Language];
            ViewBag.modal_operate = cLanguage.modal_operate[Language];
            ViewBag.modal_btn_edit = cLanguage.modal_btn_edit[Language];
            ViewBag.modal_btn_delete = cLanguage.modal_btn_delete[Language];
            ViewBag.modal_newid = cLanguage.modal_newid[Language];
            ViewBag.modal_newpassword = cLanguage.modal_newpassword[Language];
            ViewBag.modal_checkpassword = cLanguage.modal_checkpassword[Language];
            ViewBag.modal_btn_submit = cLanguage.modal_btn_submit[Language];
            ViewBag.modal_btn_addition = cLanguage.modal_btn_addition[Language];
            /* Message */
            ViewBag.modal_msg_nopermissions = cLanguage.modal_msg_nopermissions[Language];
            ViewBag.modal_msg_lowlv = cLanguage.modal_msg_lowlv[Language];
            ViewBag.modal_msg_login_empty = cLanguage.modal_msg_login_empty[Language];
            ViewBag.modal_msg_passwordnosame = cLanguage.modal_msg_passwordnosame[Language];
            ViewBag.modal_msg_creatsuccess = cLanguage.modal_msg_creatsuccess[Language];
            ViewBag.modal_msg_msg_creatfailed = cLanguage.modal_msg_msg_creatfailed[Language];
            ViewBag.modal_msg_accountisusing = cLanguage.modal_msg_accountisusing[Language];
            ViewBag.modal_msg_isremove1 = cLanguage.modal_msg_isremove1[Language];
            ViewBag.modal_msg_isremove2 = cLanguage.modal_msg_isremove2[Language];
            ViewBag.modal_msg_removesuccess = cLanguage.modal_msg_removesuccess[Language];
            ViewBag.modal_msg_removefailed = cLanguage.modal_msg_removefailed[Language];
            ViewBag.msg_restartservice = cLanguage.msg_restartservice[Language];
            ViewBag.msg_restartalarm = cLanguage.msg_restartalarm[Language];
            ViewBag.msg_noID = cLanguage.msg_noID[Language];
            ViewBag.msg_noPW = cLanguage.msg_noPW[Language];
            ViewBag.msg_dele_storage = cLanguage.msg_dele_storage[Language];
            ViewBag.msg_nobarcode = cLanguage.msg_nobarcode[Language];
            /***  歷史資料  ***/
            ViewBag.btn_before = cLanguage.btn_before[Language];
            ViewBag.btn_date = cLanguage.btn_date[Language];
            ViewBag.btn_after = cLanguage.btn_after[Language];
            ViewBag.btn_today = cLanguage.btn_today[Language];
            ViewBag.btn_prepage = cLanguage.btn_prepage[Language];
            ViewBag.btn_nextpage = cLanguage.btn_nextpage[Language];
            ViewBag.btn_Productiondate = cLanguage.btn_Productiondate[Language];
            /***  帳戶修改  ***/
            ViewBag.modal_editaccount = cLanguage.modal_editaccount[Language];
            ViewBag.modal_btn_editsubmit = cLanguage.modal_btn_editsubmit[Language];
            /* Message */
            ViewBag.modal_msg_modifysuccess = cLanguage.modal_msg_modifysuccess[Language];
            ViewBag.modal_msg_modifyfailed = cLanguage.modal_msg_modifyfailed[Language];
            ViewBag.msg_format_wrong = cLanguage.msg_format_wrong[Language];
            ViewBag.msg_format_wrong_empty = cLanguage.msg_format_wrong_empty[Language];
            ViewBag.msg_readernor = cLanguage.msg_readernor[Language];
            ViewBag.msg_readererr = cLanguage.msg_readererr[Language];
            ViewBag.msg_isreadererror = cLanguage.msg_isreadererror[Language];
            /***  Body ***/
            ViewBag.status_upstream = cLanguage.status_upstream[Language];
            ViewBag.status_downstream = cLanguage.status_downstream[Language];
            ViewBag.status_supervisory = cLanguage.status_supervisory[Language];
            ViewBag.lab_status = cLanguage.lab_status[Language];
            ViewBag.lab_messagefrom = cLanguage.lab_messagefrom[Language];
            ViewBag.titl_supervisory = cLanguage.titl_supervisory[Language];
            ViewBag.titl_upstream = cLanguage.titl_upstream[Language];
            ViewBag.titl_downstream = cLanguage.titl_downstream[Language];
            ViewBag.titl_receivemessage = cLanguage.titl_receivemessage[Language];
            ViewBag.titl_sendmessage = cLanguage.titl_sendmessage[Language];
            ViewBag.titl_systemmessage = cLanguage.titl_systemmessage[Language];
            /***  是否按鈕  ***/
            ViewBag.btn_true = cLanguage.btn_true[Language];
            ViewBag.btn_false = cLanguage.btn_false[Language];
            /*** Hermes workorder ***/
            ViewBag.btn_ManualKeyin = cLanguage.btn_ManualKeyin[Language];
            ViewBag.btn_Supervisory = cLanguage.btn_Supervisory[Language];
            ViewBag.btn_Stay = cLanguage.btn_Stay[Language];
            ViewBag.btn_CreateData = cLanguage.btn_CreateData[Language];
            ViewBag.WorkOrderData = cLanguage.WorkOrderData[Language];
            ViewBag.NoDataAct = cLanguage.NoDataAct[Language];
            ViewBag.warning_WorkOrderData = cLanguage.warning_WorkOrderData[Language];
            ViewBag.warning_NoDataAct = cLanguage.warning_NoDataAct[Language];
            ViewBag.table_workorder = cLanguage.table_workorder[Language];
            ViewBag.table_WorkOrderData = cLanguage.table_WorkOrderData[Language];
            ViewBag.table_FailedBoard = cLanguage.table_FailedBoard[Language];
            ViewBag.table_ProductTypeID = cLanguage.table_ProductTypeID[Language];
            ViewBag.table_FlippedBoard = cLanguage.table_FlippedBoard[Language];
            ViewBag.table_Length = cLanguage.table_Length[Language];
            ViewBag.table_Width = cLanguage.table_Width[Language];
            ViewBag.table_Thickness = cLanguage.table_Thickness[Language];
            ViewBag.table_ConveyorSpeed = cLanguage.table_ConveyorSpeed[Language];
            ViewBag.table_TopClearanceHeight = cLanguage.table_TopClearanceHeight[Language];
            ViewBag.table_BottomClearanceHeight = cLanguage.table_BottomClearanceHeight[Language];
            ViewBag.table_Weight = cLanguage.table_Weight[Language];
            ViewBag.table_WorkOrderID = cLanguage.table_WorkOrderID[Language];
            ViewBag.table_BatchID = cLanguage.table_BatchID[Language];
            ViewBag.table_Route = cLanguage.table_Route[Language];
            ViewBag.table_Action = cLanguage.table_Action[Language];
            ViewBag.btn_FailedBoard_good = cLanguage.btn_FailedBoard_good[Language];
            ViewBag.btn_FailedBoard_failed = cLanguage.btn_FailedBoard_failed[Language];
            ViewBag.btn_FailedBoard_unknown = cLanguage.btn_FailedBoard_unknown[Language];
            ViewBag.btn_FlippedBoard_top = cLanguage.btn_FlippedBoard_top[Language];
            ViewBag.btn_FlippedBoard_bottom = cLanguage.btn_FlippedBoard_bottom[Language];
            ViewBag.btn_FlippedBoard_unknown = cLanguage.btn_FlippedBoard_unknown[Language];
            ViewBag.msg_isvaluebiggerequa0 = cLanguage.msg_isvaluebiggerequal0[Language];
            ViewBag.table_machine = cLanguage.table_machine[Language];
            ViewBag.table_SB_Pos = cLanguage.table_SB_Pos[Language];
            ViewBag.table_SB_Bc = cLanguage.table_SB_Bc[Language];
            ViewBag.table_SB_St = cLanguage.table_SB_St[Language];
            ViewBag.btn_SB_Add = cLanguage.btn_SB_Add[Language];
            ViewBag.btn_SB_Delete = cLanguage.btn_SB_Delete[Language];
            ViewBag.btn_SB_AllDele = cLanguage.btn_SB_AllDele[Language];
            ViewBag.msg_issubdele1 = cLanguage.msg_issubdele1[Language];
            ViewBag.msg_issubdele2 = cLanguage.msg_issubdele2[Language];
            ViewBag.msg_sub_alldelete = cLanguage.msg_sub_alldelete[Language];
            #endregion

            //機內
            BoardMode = MVCSetting.LoadData("MACHINE", "MACHINE", "BoardMode");

            if (BoardMode == "1" || BoardMode == "2")
            {
                isSingle = "0";
            }
            else
            {
                isSingle = "1";
            }

            ViewBag.isSingle = isSingle;

            /*****   SQL資料   *****/
            //載入CSV資料
            ViewBag.Setting = MVCsetting;
            ViewBag.Workorder_sub = MVCdefailt_sub;
            ViewBag.Workorder_sub_new = MVCdefailt_sub_new;
            //是否有LicenseKey
            ViewBag.License_Hermes = MVCAlwaysread.LoadData("License");
            //Reader1是否開啟
            ViewBag.Reader1Enable = MVCSetting.LoadData("OutPort", "Reader1", "Enable");
            //Reader2是否開啟
            ViewBag.Reader2Enable = MVCSetting.LoadData("OutPort", "Reader2", "Enable");
            //Machine是否開啟
            ViewBag.MachineEnable = MVCSetting.LoadData("OutPort", "Reader3", "Enable");
            if (Convert.ToBoolean(MVCInquirybarcode.LoadData("isServiceReStar")))
            {
                ViewBag.RecipeType = "newRecipe";
                hasread = true;
            }
            else
            {
                ViewBag.RecipeType = "Recipe";
                hasread = true;
            }

            ViewBag.readeriserror = MVCCloumnrunning.LoadData("Reader");
            ViewBag.machineiserror = MVCCloumnrunning.LoadData("MACHINE");

            SystemMessaage = SystemXML.GetMessage();
            // 將字串以換行符 ("\r\n") 為基準拆分為行
            string[] lines = SystemMessaage.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            // 反轉行的順序
            Array.Reverse(lines);

            // 將反轉後的行重新連接起來
            ViewBag.SystemMessage = string.Join("\r\n", lines);



            return View();
        }

        //SingleR刷新畫面資料
        private void TimerCallback(object state)
        {
            /*****          SQL          *****/
            //載入CSV資料:Hermes狀態
            List<MVCCLOUMNRUNNING> MVCcloumnrunning = MVCCloumnrunning.LoadALLData();
            foreach (var running in MVCcloumnrunning)
            {
                // Check if the record meets all the criteria
                if (running.ColumnName == "Upstream")
                {
                    if(running.IfRunning != null)
                    {
                        Upstream_Color = running.IfRunning;
                    }
                }
                if (running.ColumnName == "Downstream")
                {
                    if (running.IfRunning != null)
                    {
                        Downstream_Color = running.IfRunning;
                    }
                    
                }
                if (running.ColumnName == "Supervisory")
                {
                    if (running.IfRunning != null)
                    {
                        Supervisory_Color = running.IfRunning;
                    }
                    
                }
                if (running.ColumnName == "Upstream_Status")
                {
                    if (running.IfRunning != null)
                    {
                        Upstream_Status_Color = running.IfRunning;
                    }
                    
                }
                if (running.ColumnName == "Downstream_Status")
                {
                    if (running.IfRunning != null)
                    {
                        Downstream_Status_Color = running.IfRunning;
                    }
                    
                }
                if (running.ColumnName == "Supervisory_Status")
                {
                    if (running.IfRunning != null)
                    {
                        Supervisory_Status_Color = running.IfRunning;
                    }
                }
                if (running.ColumnName == "Lockinput")
                {
                    if (running.IfRunning != null)
                    {
                        Lockinput = running.IfRunning;
                    }
                    
                }
            }
            try
            {
                KeyinIsAsked = MVCInquirybarcode.LoadData("KeyinIsAsked");
                QBIisAsked = MVCInquirybarcode.LoadData("QBIisAsked");
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
            }


            //載入CSV資料:發送、接收、系統訊息
            SendMessaage = SendXML.GetMessage();
            SendMessageFrom = SendXML.messagefrom;
            ReceiveMessaage = ReceviceXML.GetMessage();
            ReceiveMessaageFrom = ReceviceXML.messagefrom;

            //License驗證
            License_Hermes = MVCAlwaysread.LoadData("License");

            List<MVCSTORAGE> MVCStorage = new List<MVCSTORAGE>();

            if (BoardMode == "1" || BoardMode == "2")
            {
                MVCStorage = XMLtoList_FIFO.ConvertXmlToCsv();
            }
            else
            {
                MVCStorage = XMLtoList_Single.ConvertXmlToCsv();
            }

            _hubContext.Clients.All.SendAsync("ReceiveMessage1", KeyinIsAsked, QBIisAsked, MVCStorage, Lockinput);
            _hubContext.Clients.All.SendAsync("ReceiveMessage2", Upstream_Color, Downstream_Color, Supervisory_Color, Supervisory_Status_Color, Upstream_Status_Color,
                Downstream_Status_Color);
            _hubContext.Clients.All.SendAsync("ReceiveMessage3", SendMessageFrom, SendMessaage, ReceiveMessaageFrom, ReceiveMessaage,
                License_Hermes);
        }

        //設定頁
        public IActionResult Privacy()
        {
            /*****        COM讀取        *****/
            ViewBag.myPorts = SerialPort.GetPortNames(); //取得所有port的名字的方法
                                                         //string[] myPorts = { "COM1", "COM2" };
                                                         //ViewBag.myPorts = myPorts;
            #region 多國語系
            /*****   多國語系的ViewBag   *****/
            Language_now = cSetting.Language;
            ViewBag.Language_now = Language_now;
            Language = cLanguage.Language(Language_now);
            /***  Header ***/
            ViewBag.Header_Home = cLanguage.Header_Home[Language];
            ViewBag.Header_accountmanage = cLanguage.Header_accountmanage[Language];
            ViewBag.Header_setting = cLanguage.Header_setting[Language];
            ViewBag.Header_comtest = cLanguage.Header_comtest[Language];
            ViewBag.Header_resetserver = cLanguage.Header_resetserver[Language];
            ViewBag.Header_resetalarm = cLanguage.Header_resetalarm[Language];
            ViewBag.Header_language = cLanguage.Header_language[Language];
            ViewBag.Header_login = cLanguage.Header_login[Language];
            ViewBag.Header_logout = cLanguage.Header_logout[Language];
            /***  訊息視窗 ***/
            ViewBag.btn_msgbox_ok = cLanguage.btn_msgbox_ok[Language];
            ViewBag.btn_msgbox_cancel = cLanguage.btn_msgbox_cancel[Language];
            /***  登入視窗  ***/
            ViewBag.modal_account = cLanguage.modal_account[Language];
            ViewBag.modal_userid = cLanguage.modal_userid[Language];
            ViewBag.modal_userpassword = cLanguage.modal_userpassword[Language];
            ViewBag.modal_login = cLanguage.modal_login[Language];
            /***  測試操作視窗  ***/
            ViewBag.modal_equipmentname = cLanguage.modal_equipmentname[Language];
            ViewBag.lab_RN = cLanguage.lab_RN[Language];
            /* Message */
            ViewBag.modal_msg_loginsuccess = cLanguage.modal_msg_loginsuccess[Language];
            ViewBag.modal_msg_loginfailed_password = cLanguage.modal_msg_loginfailed_password[Language];
            ViewBag.modal_msg_loginfailed_id = cLanguage.modal_msg_loginfailed_id[Language];
            ViewBag.msg_isreadererror = cLanguage.msg_isreadererror[Language];
            /***  帳號管理  ***/
            ViewBag.modal_accountmanagement = cLanguage.modal_accountmanagement[Language];
            ViewBag.modal_addaccount = cLanguage.modal_addaccount[Language];
            ViewBag.modal_userlv = cLanguage.modal_userlv[Language];
            ViewBag.modal_username = cLanguage.modal_username[Language];
            ViewBag.modal_operate = cLanguage.modal_operate[Language];
            ViewBag.modal_btn_edit = cLanguage.modal_btn_edit[Language];
            ViewBag.modal_btn_delete = cLanguage.modal_btn_delete[Language];
            ViewBag.modal_newid = cLanguage.modal_newid[Language];
            ViewBag.modal_newpassword = cLanguage.modal_newpassword[Language];
            ViewBag.modal_checkpassword = cLanguage.modal_checkpassword[Language];
            ViewBag.modal_btn_submit = cLanguage.modal_btn_submit[Language];
            ViewBag.modal_btn_addition = cLanguage.modal_btn_addition[Language];
            /* Message */
            ViewBag.modal_msg_nopermissions = cLanguage.modal_msg_nopermissions[Language];
            ViewBag.modal_msg_lowlv = cLanguage.modal_msg_lowlv[Language];
            ViewBag.modal_msg_login_empty = cLanguage.modal_msg_login_empty[Language];
            ViewBag.modal_msg_passwordnosame = cLanguage.modal_msg_passwordnosame[Language];
            ViewBag.modal_msg_creatsuccess = cLanguage.modal_msg_creatsuccess[Language];
            ViewBag.modal_msg_msg_creatfailed = cLanguage.modal_msg_msg_creatfailed[Language];
            ViewBag.modal_msg_accountisusing = cLanguage.modal_msg_accountisusing[Language];
            ViewBag.modal_msg_isremove1 = cLanguage.modal_msg_isremove1[Language];
            ViewBag.modal_msg_isremove2 = cLanguage.modal_msg_isremove2[Language];
            ViewBag.modal_msg_removesuccess = cLanguage.modal_msg_removesuccess[Language];
            ViewBag.modal_msg_removefailed = cLanguage.modal_msg_removefailed[Language];
            ViewBag.msg_restartservice = cLanguage.msg_restartservice[Language];
            ViewBag.msg_restartalarm = cLanguage.msg_restartalarm[Language];
            ViewBag.msg_noID = cLanguage.msg_noID[Language];
            ViewBag.msg_noPW = cLanguage.msg_noPW[Language];
            ViewBag.msg_readernor = cLanguage.msg_readernor[Language];
            ViewBag.msg_readererr = cLanguage.msg_readererr[Language];
            ViewBag.msg_noMRnoTest = cLanguage.msg_noMRnoTest[Language];
            ViewBag.msg_subsuccess = cLanguage.msg_subsuccess[Language];
            /***  帳戶修改  ***/
            ViewBag.modal_editaccount = cLanguage.modal_editaccount[Language];
            ViewBag.modal_btn_editsubmit = cLanguage.modal_btn_editsubmit[Language];
            /* Message */
            ViewBag.modal_msg_modifysuccess = cLanguage.modal_msg_modifysuccess[Language];
            ViewBag.modal_msg_modifyfailed = cLanguage.modal_msg_modifyfailed[Language];
            /***  操作按鈕 ***/
            ViewBag.btn_edit = cLanguage.btn_edit[Language];
            ViewBag.btn_save = cLanguage.btn_save[Language];
            ViewBag.btn_cancel = cLanguage.btn_cancel[Language];
            ViewBag.btn_true = cLanguage.btn_true[Language];
            ViewBag.btn_false = cLanguage.btn_false[Language];
            ViewBag.btn_remit = cLanguage.btn_remit[Language];
            ViewBag.btn_import = cLanguage.btn_import[Language];
            ViewBag.btn_clear = cLanguage.btn_clear[Language];
            ViewBag.btn_edit_station = cLanguage.btn_edit_station[Language];
            /***  Body ***/
            ViewBag.titl_type = cLanguage.titl_type[Language];
            ViewBag.titl_name = cLanguage.titl_name[Language];
            ViewBag.titl_value = cLanguage.titl_value[Language];
            ViewBag.titl_operate = cLanguage.titl_operate[Language];
            /* Common */
            ViewBag.table_information = cLanguage.table_information[Language];
            ViewBag.table_modelname = cLanguage.table_modelname[Language];
            ViewBag.table_machineid = cLanguage.table_machineid[Language];
            ViewBag.table_displayname = cLanguage.table_displayname[Language];
            ViewBag.table_version = cLanguage.table_version[Language];
            ViewBag.table_BoardMode = cLanguage.table_BoardMode[Language];
            ViewBag.btn_single = cLanguage.btn_single[Language];
            ViewBag.btn_fifo = cLanguage.btn_fifo[Language];
            ViewBag.btn_match = cLanguage.btn_match[Language];
            ViewBag.warning_BoardMode = cLanguage.warning_BoardMode[Language];
            ViewBag.table_fifoRange = cLanguage.table_fifoRange[Language];
            ViewBag.table_tcpip = cLanguage.table_tcpip[Language];
            ViewBag.table_tcpport = cLanguage.table_tcpport[Language];
            ViewBag.msg_P_Int = cLanguage.msg_P_Int[Language];
            /* Hermes */
            ViewBag.table_upstream = cLanguage.table_upstream[Language];
            ViewBag.table_downstream = cLanguage.table_downstream[Language];
            ViewBag.table_supervisory = cLanguage.table_supervisory[Language];
            ViewBag.table_enable = cLanguage.table_enable[Language];
            ViewBag.table_checkalive1 = cLanguage.table_checkalive1[Language];
            ViewBag.table_checkalive2 = cLanguage.table_checkalive2[Language];
            ViewBag.table_queryresponse1 = cLanguage.table_queryresponse1[Language];
            ViewBag.table_queryresponse2 = cLanguage.table_queryresponse2[Language];
            ViewBag.table_order = cLanguage.table_order[Language];
            ViewBag.table_RWI = cLanguage.table_RWI[Language];
            ViewBag.table_Command = cLanguage.table_Command[Language];
            ViewBag.table_Reserved_quantity = cLanguage.table_Reserved_quantity[Language];
            ViewBag.table_Give_Command_Order = cLanguage.table_Give_Command_Order[Language];
            ViewBag.table_Command_effect = cLanguage.table_Command_effect[Language];
            ViewBag.table_Command_Path = cLanguage.table_Command_Path[Language];
            ViewBag.select_ordre_standard = cLanguage.select_ordre_standard[Language];
            ViewBag.select_ordre_BAMR = cLanguage.select_ordre_BAMR[Language];
            ViewBag.select_ordre_MRBA = cLanguage.select_ordre_MRBA[Language];
            ViewBag.table_hostadress = cLanguage.table_hostadress[Language];
            ViewBag.table_clientadress = cLanguage.table_clientadress[Language];
            ViewBag.table_strattransportdelaytime1 = cLanguage.table_strattransportdelaytime1[Language];
            ViewBag.table_strattransportdelaytime2 = cLanguage.table_strattransportdelaytime2[Language];
            /* 服務宣告 */
            ViewBag.table_checkaliveresponse = cLanguage.table_checkaliveresponse[Language];
            ViewBag.table_queryworkorderinfo = cLanguage.table_queryworkorderinfo[Language];
            ViewBag.table_sendworkorderinfo = cLanguage.table_sendworkorderinfo[Language];
            ViewBag.table_configuration = cLanguage.table_configuration[Language];
            ViewBag.table_boardtracking = cLanguage.table_boardtracking[Language];
            ViewBag.table_boardforecast = cLanguage.table_boardforecast[Language];
            ViewBag.table_queryboardinfo = cLanguage.table_queryboardinfo[Language];
            ViewBag.table_sendboardinfo = cLanguage.table_sendboardinfo[Language];
            /*** Message ***/
            ViewBag.msg_isvaluebiggerequa0 = cLanguage.msg_isvaluebiggerequal0[Language];
            ViewBag.msg_isvaluebiggerthan0 = cLanguage.msg_isvaluebiggerthan0[Language];
            ViewBag.msg_isvaluefalse1 = cLanguage.msg_isvaluefalse1[Language];
            ViewBag.msg_isvaluefalse2 = cLanguage.msg_isvaluefalse2[Language];
            ViewBag.msg_isvaluefalse3 = cLanguage.msg_isvaluefalse3[Language];
            ViewBag.msg_savesuccess = cLanguage.msg_savesuccess[Language];
            ViewBag.msg_savefailed = cLanguage.msg_savefailed[Language];
            ViewBag.msg_issave = cLanguage.msg_issave[Language];
            ViewBag.msg_saving = cLanguage.msg_saving[Language];
            ViewBag.msg_isleave = cLanguage.msg_isleave[Language];
            ViewBag.msg_validateIP = cLanguage.msg_validateIP[Language];
            ViewBag.msg_portrange = cLanguage.msg_portrange[Language];
            ViewBag.msg_comparison = cLanguage.msg_comparison[Language];
            ViewBag.msg_beforeremit = cLanguage.msg_beforeremit[Language];
            ViewBag.msg_importsuccess = cLanguage.msg_importsuccess[Language];
            ViewBag.msg_importfault = cLanguage.msg_importfault[Language];
            ViewBag.msg_format_wrong = cLanguage.msg_format_wrong[Language];
            ViewBag.msg_format_wrong_empty = cLanguage.msg_format_wrong_empty[Language];
            ViewBag.msg_ASCIIwrong = cLanguage.msg_ASCIIwrong[Language];
            ViewBag.msg_rangewrong = cLanguage.msg_rangewrong[Language];
            ViewBag.msg_issubdele1 = cLanguage.msg_issubdele1[Language];
            ViewBag.msg_issubdele2 = cLanguage.msg_issubdele2[Language];
            ViewBag.msg_sub_alldelete = cLanguage.msg_sub_alldelete[Language];
            ViewBag.msg_open_reader = cLanguage.msg_open_reader[Language];
            /*** Hermes workorder ***/
            ViewBag.btn_ManualKeyin = cLanguage.btn_ManualKeyin[Language];
            ViewBag.btn_Supervisory = cLanguage.btn_Supervisory[Language];
            ViewBag.btn_Stay = cLanguage.btn_Stay[Language];
            ViewBag.btn_CreateData = cLanguage.btn_CreateData[Language];
            ViewBag.WorkOrderData = cLanguage.WorkOrderData[Language];
            ViewBag.NoDataAct = cLanguage.NoDataAct[Language];
            ViewBag.warning_WorkOrderData = cLanguage.warning_WorkOrderData[Language];
            ViewBag.warning_NoDataAct = cLanguage.warning_NoDataAct[Language];
            ViewBag.table_workorder = cLanguage.table_workorder[Language];
            ViewBag.table_WorkOrderData = cLanguage.table_WorkOrderData[Language];
            ViewBag.table_FailedBoard = cLanguage.table_FailedBoard[Language];
            ViewBag.table_ProductTypeID = cLanguage.table_ProductTypeID[Language];
            ViewBag.table_FlippedBoard = cLanguage.table_FlippedBoard[Language];
            ViewBag.table_Length = cLanguage.table_Length[Language];
            ViewBag.table_Width = cLanguage.table_Width[Language];
            ViewBag.table_Thickness = cLanguage.table_Thickness[Language];
            ViewBag.table_ConveyorSpeed = cLanguage.table_ConveyorSpeed[Language];
            ViewBag.table_TopClearanceHeight = cLanguage.table_TopClearanceHeight[Language];
            ViewBag.table_BottomClearanceHeight = cLanguage.table_BottomClearanceHeight[Language];
            ViewBag.table_Weight = cLanguage.table_Weight[Language];
            ViewBag.table_Route = cLanguage.table_Route[Language];
            ViewBag.table_Action = cLanguage.table_Action[Language];
            ViewBag.table_WorkOrderID = cLanguage.table_WorkOrderID[Language];
            ViewBag.table_BatchID = cLanguage.table_BatchID[Language];
            ViewBag.table_SubBoards = cLanguage.table_SubBoards[Language];
            ViewBag.table_SB_Pos = cLanguage.table_SB_Pos[Language];
            ViewBag.table_SB_Bc = cLanguage.table_SB_Bc[Language];
            ViewBag.table_SB_St = cLanguage.table_SB_St[Language];
            ViewBag.btn_FailedBoard_good = cLanguage.btn_FailedBoard_good[Language];
            ViewBag.btn_FailedBoard_failed = cLanguage.btn_FailedBoard_failed[Language];
            ViewBag.btn_FailedBoard_unknown = cLanguage.btn_FailedBoard_unknown[Language];
            ViewBag.btn_FlippedBoard_top = cLanguage.btn_FlippedBoard_top[Language];
            ViewBag.btn_FlippedBoard_bottom = cLanguage.btn_FlippedBoard_bottom[Language];
            ViewBag.btn_FlippedBoard_unknown = cLanguage.btn_FlippedBoard_unknown[Language];
            ViewBag.btn_barcode_number = cLanguage.btn_barcode_number[Language];
            ViewBag.btn_barcode_date = cLanguage.btn_barcode_date[Language];
            ViewBag.btn_barcode_none = cLanguage.btn_barcode_none[Language];
            ViewBag.btn_SB_Add = cLanguage.btn_SB_Add[Language];
            ViewBag.btn_SB_Delete = cLanguage.btn_SB_Delete[Language];
            ViewBag.btn_SB_AllDele = cLanguage.btn_SB_AllDele[Language];
            /*** Digital IO ***/
            ViewBag.table_input = cLanguage.table_input[Language];
            ViewBag.table_output = cLanguage.table_output[Language];
            ViewBag.selsect_disable = cLanguage.selsect_disable[Language];
            ViewBag.selsect_Up_BA = cLanguage.selsect_Up_BA[Language];
            ViewBag.selsect_Down_MR = cLanguage.selsect_Down_MR[Language];
            ViewBag.selsect_isfliped = cLanguage.selsect_isfliped[Language];
            ViewBag.selsect_Up_FBA = cLanguage.selsect_Up_FBA[Language];
            ViewBag.selsect_Up_MR = cLanguage.selsect_Up_MR[Language];
            ViewBag.selsect_Down_BA = cLanguage.selsect_Down_BA[Language];
            ViewBag.selsect_isflip = cLanguage.selsect_isflip[Language];
            ViewBag.selsect_Down_FBA = cLanguage.selsect_Down_FBA[Language];
            ViewBag.select_high_potential = cLanguage.select_high_potential[Language];
            ViewBag.select_low_potential = cLanguage.select_low_potential[Language];
            ViewBag.select_alarm = cLanguage.select_alarm[Language];
            /*** OutPorts ***/
            ViewBag.table_Linkby = cLanguage.table_Linkby[Language];
            ViewBag.table_port = cLanguage.table_port[Language];
            ViewBag.table_STmessage = cLanguage.table_STmessage[Language];
            ViewBag.table_ERRmessage = cLanguage.table_ERRmessage[Language];
            ViewBag.table_format = cLanguage.table_format[Language];
            ViewBag.table_baudrate = cLanguage.table_baudrate[Language];
            ViewBag.table_parity = cLanguage.table_parity[Language];
            ViewBag.table_databits = cLanguage.table_databits[Language];
            ViewBag.table_stopbit = cLanguage.table_stopbit[Language];
            ViewBag.table_STaddress = cLanguage.table_STaddress[Language];
            ViewBag.table_writeprefix = cLanguage.table_writeprefix[Language];
            ViewBag.table_writeseparator = cLanguage.table_writeseparator[Language];
            ViewBag.table_writesuffix = cLanguage.table_writesuffix[Language];
            ViewBag.table_writetimeout = cLanguage.table_writetimeout[Language];
            ViewBag.table_machine = cLanguage.table_machine[Language];
            ViewBag.table_decimalplaces = cLanguage.table_decimalplaces[Language];
            ViewBag.msg_usesameselect = cLanguage.msg_usesameselect[Language];
            ViewBag.msg_usesameip = cLanguage.msg_usesameip[Language];
            ViewBag.msg_pleaseenter = cLanguage.msg_pleaseenter[Language];
            ViewBag.table_D_RN = cLanguage.table_D_RN[Language];
            ViewBag.table_D_wid = cLanguage.table_D_wid[Language];
            ViewBag.table_RNAddrType = cLanguage.table_RNAddrType[Language];
            ViewBag.table_WidAddrType = cLanguage.table_WidAddrType[Language];
            ViewBag.table_datatype = cLanguage.table_datatype[Language];
            ViewBag.sele_LowAddressFirst = cLanguage.sele_LowAddressFirst[Language];
            ViewBag.sele_HighAddressFirst = cLanguage.sele_HighAddressFirst[Language];
            ViewBag.sele_LowAddressHighByteFirst = cLanguage.sele_LowAddressHighByteFirst[Language];
            ViewBag.sele_LowAddressLowByteFirst = cLanguage.sele_LowAddressLowByteFirst[Language];
            ViewBag.sele_HighAddressHighByteFirst = cLanguage.sele_HighAddressHighByteFirst[Language];
            ViewBag.sele_HighAddressLowByteFirst = cLanguage.sele_HighAddressLowByteFirst[Language];

            #endregion

            //保留記錄檔天數
            ViewBag.lbl_days = cLanguage.lbl_days[Language];

            /*****   識別啟用Work Order視窗、權限及刪除LOG天數的ViewBag   *****/
            string serviceName = "SUNSDAMVC"; // 替换为您的服务名称

            
            bool servicerunning = false;
            
            while (!servicerunning)
            {
                try
                {
                    ServiceController service = new ServiceController(serviceName);
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        servicerunning = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            
            ViewBag.Setting = MVCsetting;
            ViewBag.Workorder_sub = MVCdefailt_sub_new;
            //是否有LicenseKey
            ViewBag.License_Hermes = Convert.ToBoolean(MVCAlwaysread.LoadData("License"));
            //是否啟用
            ViewBag.WODS = MVCSetting.LoadData("Hermes", "newRecipe", "WorkOrderData");
            //何種資料帶入方式
            ViewBag.NO_WODS = MVCSetting.LoadData("Hermes", "newRecipe", "QWINoData");
            //LOG刪除天數
            ViewBag.days = MVCSetting.LoadData("AutoDelete", "Log", "Day");
            //TopBarCode生成方式
            ViewBag.TopEnterType = MVCSetting.LoadData("Hermes", "newRecipe", "Topbarcodetype");
            //BottomBarCode生成方式
            ViewBag.BottomEnterType = MVCSetting.LoadData("Hermes", "newRecipe", "Bottombarcodetype");
            //Reader1是否開啟
            ViewBag.Reader1Enable = MVCSetting.LoadData("OutPort", "Reader1", "Enable");
            //Reader2是否開啟
            ViewBag.Reader2Enable = MVCSetting.LoadData("OutPort", "Reader2", "Enable");
            //Machine是否開啟
            ViewBag.MachineEnable = MVCSetting.LoadData("OutPort", "Machine", "Enable");
            ViewBag.readeriserror = MVCCloumnrunning.LoadData("Reader");
            ViewBag.machineiserror = MVCCloumnrunning.LoadData("MACHINE");
            return View();
        }

        public class SystemData
        {
            public string Group { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
            public string? Value { get; set; }
        }

        public class SubData
        {
            public string SB_Pos { get; set; }
            public string SB_Bc { get; set; }
            public string SB_St { get; set; }
        }

        [HttpPost]
        //儲存系統參數資料
        public IActionResult Edit(string List,string UserID,string SUBList)
        {
            
            List<SystemData> systemDataList = JsonConvert.DeserializeObject<List<SystemData>>(List);
            List<SubData>? SubDataList = JsonConvert.DeserializeObject<List<SubData>>(SUBList);
            string Type = "";
            string Name = "";

            // 将 JSON 字符串解析为对象列表
            List<dynamic> objects = JsonConvert.DeserializeObject<List<dynamic>>(List);

            // 遍历对象列表，找到指定关键字的值
            foreach (var obj in objects)
            {
                string name = obj.Name;
                if (name == "QBIisAsked" || name == "QBIisSend")
                {
                    Name = "AskBarCode";
                    break;
                }
                else
                {
                    Name = "Setting";
                }
            }

            //儲存設定
            foreach (var SystemData in systemDataList)
            {
                MVCSetting.Update(SystemData.Group, SystemData.Type, SystemData.Name, SystemData.Value);
                Type = SystemData.Type;
            }

            //儲存子版資料
            if(SubDataList.Count != 0)
            {
                MVCDefailt_sub_new.DeleteAll(MVCdefailt_sub_new);
                foreach (var SubData in SubDataList)
                {
                    MVCDefailt_sub_new.AddNew(MVCdefailt_sub_new, SubData.SB_Pos, SubData.SB_Bc, SubData.SB_St);
                }
            }

            //告知變更設定檔
            MVCAlwaysread.Update("ResetSetting", "Y");

            MessageLogger m = new MessageLogger();
            m.SaveOperationMessage(Name + List + SUBList, UserID);

            return RedirectToAction("Privacy");
        }

        //QWI、QBI問不到資料時使用彈出式視窗
        public IActionResult Inquirybarcode(string List, string UserID, string SUBList)
        {

            List<SystemData> systemDataList = JsonConvert.DeserializeObject<List<SystemData>>(List);
            List<SubData>? SubDataList = JsonConvert.DeserializeObject<List<SubData>>(SUBList);
            string Name = "";

            // 将 JSON 字符串解析为对象列表
            List<dynamic> objects = JsonConvert.DeserializeObject<List<dynamic>>(List);

            // 遍历对象列表，找到指定关键字的值
            foreach (var obj in objects)
            {
                string name = obj.Name;
                if (name == "QBIisAsked" || name == "QBIisSend")
                {
                    Name = "AskBarCode";
                    break;
                }
                else
                {
                    Name = "Setting";
                }
            }

            foreach (var SystemData in systemDataList)
            {
                MVCInquirybarcode.Update(SystemData.Name, SystemData.Value);
            }
            if (SubDataList.Count != 0)
            {
                MVCKeyin_sub.DeleteAll(MVCkeyin_sub);
                foreach (var SubData in SubDataList)
                {
                    MVCKeyin_sub.AddNew(MVCkeyin_sub, SubData.SB_Pos, SubData.SB_Bc, SubData.SB_St);
                }
            }
            MessageLogger m = new MessageLogger();
            m.SaveOperationMessage(Name + List + SUBList, UserID);

            return RedirectToAction("Privacy");
        }

        [HttpPost]
        //儲存系統參數資料
        public IActionResult ComTest(string List)
        {
            string ISERROR = "",Name = "";
            bool isMR = false;

            List<SystemData> systemDataList = JsonConvert.DeserializeObject<List<SystemData>>(List);

            // 将 JSON 字符串解析为对象列表
            List<dynamic> objects = JsonConvert.DeserializeObject<List<dynamic>>(List);

            foreach (var SystemData in systemDataList)
            {
                if (SystemData.Type == "NAME")
                    Name = SystemData.Value;
                if (Name == "M1")
                {
                    if (MVCCloumnrunning.LoadData("Upstream_Status") == "MachineReady")
                    {
                        isMR = true;
                    }
                    else
                    {
                        isMR = false;
                    }

                    if (isMR == false)
                    {
                        ISERROR = "noMR";
                        break;
                    }
                }
                MVCComtest.Update(SystemData.Type, SystemData.Value);
            }

            while(Name != "M1" &&(ISERROR == "" || ISERROR == null))
            {
                ISERROR = MVCComtest.LoadData("ISERROR");
            }

            var result = new { IsError = ISERROR };

            MVCComtest.Update("ISERROR", "");

            // 返回 JSON 格式的結果
            return Json(result);
        }

        //刪除機內資料
        public ActionResult dele_storage(string boardID, string UserID)
        {
            if (BoardMode == "1" || BoardMode == "2")
            {
                XMLtoList_FIFO.delete(boardID);
            }
            else
            {
                XMLtoList_Single.delete(boardID);
            }

            /*****       Log       ******/
            MessageLogger m = new MessageLogger();
            if(UserID == "undefined")
            {
                UserID = "Guest";
            }
            m.SaveOperationMessage("Delete Strange[{\"BoardID\":\"" + boardID + "\"}]", UserID);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //重新啟動服務
        public ActionResult ResetService()
        {
            string serviceName = "SUNSDAMVC"; // 替换为您的服务名称

            ServiceController service = new ServiceController(serviceName);

            try
            {
                if (service.Status == ServiceControllerStatus.Running)
                {
                    if (timer != null)
                    {
                        timer.Dispose(); // 停止計時器
                    }
                    SendXML.CancelLoad();
                    ReceviceXML.CancelLoad();
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                }
                service.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // 返回到之前的页面
            return RedirectToAction("Waiting", "Home");
        }

        [HttpPost]
        //關閉警示燈
        public IActionResult ResetAlarm()
        {
            MVCAlwaysread.Update("ResetAlarm", "Y");
            return View();
        }

        /***********************      帳號驗證     ***********************/
        AuthorityInfo AuthorityInfo = new AuthorityInfo();
        [HttpPost]
        public IActionResult GetMaterial(string Account, string Name, string PW, string Level, bool ischeck, bool isModify, bool isDelete, string UserID)
        {
            bool Beused = false;
            bool Deleted = false;
            bool Modify_Success = false;
            bool isPWtrue = false;
            bool noAccount = false;
            MessageLogger m = new MessageLogger();

            if (isDelete)
            {
                if (AuthorityInfo.DeleUserMaterial(Account))
                {
                    Deleted = true;
                    m.SaveOperationMessage(Account + " is deleted.", UserID);
                }
            }
            else
            {
                if (ischeck)
                {
                    User_Name = AuthorityInfo.GetUserMaterial(Account).Item1;
                    User_PW = AuthorityInfo.GetUserMaterial(Account).Item2;
                    User_Level = AuthorityInfo.GetUserMaterial(Account).Item3;

                    if (User_PW == PW) 
                    { 
                        isPWtrue = true;
                        m.SaveOperationMessage("Login sucess.", Account);
                    }
                    else if (User_PW == "無此帳號")
                    { 
                        noAccount = true;
                        m.SaveOperationMessage("No account.", Account);
                    }
                    else
                    {
                        m.SaveOperationMessage("Login fault.", Account);
                    }
                }
                else
                {
                    if (AuthorityInfo.UpdateUserMaterial(Account, Name, PW, Level, isModify))
                    {
                        Modify_Success = true;
                        if(isModify)
                        {
                            m.SaveOperationMessage(Account + " is modified.", UserID);
                        }
                        else
                        {
                            m.SaveOperationMessage(Account + " is created.", UserID);
                        }
                    }
                    else
                    {
                        Beused = true;
                        Modify_Success = false;
                    }
                }
            }


            // 假設 AuthorityInfo.GetUserMaterial 方法返回一個結果，你可以使用這個結果構建回應
            var result = new { name = User_Name, pw = isPWtrue, level = User_Level, used = Beused, deleted = Deleted, modifysuccess = Modify_Success, IDtrue = noAccount };

            // 返回 JSON 格式的結果
            return Json(result);
        }

        [HttpPost]
        public IActionResult LOGOUT(string UserID)
        {
            MessageLogger m = new MessageLogger();
            m.SaveOperationMessage("Logout.", UserID);
            return View();
        }

        public IActionResult ASkHistoryData(int Page, string Datetime)
        {
            DateTime dateTime = DateTime.Parse(Datetime.Replace("\"", ""));
            List<HistoryData> ArrivedHistory = HistoryData.ConvertXmlToList("Arrived", dateTime);
            List<HistoryData> DepartedHistory = HistoryData.ConvertXmlToList("Departed", dateTime);

            int maxPage = 0;

            if(ArrivedHistory.Count > DepartedHistory.Count)
                maxPage = ArrivedHistory.Count / 50;
            else
                maxPage = DepartedHistory.Count / 50;

            if (ArrivedHistory.Count > 50)
            {
                ArrivedHistory = ArrivedHistory.Skip(50* Page).Take(50).ToList();
            }
            if(DepartedHistory.Count > 50)
            {
                DepartedHistory = DepartedHistory.Skip(50* Page).Take(50).ToList();
            }

            var result = new { Arrived = ArrivedHistory, Departed = DepartedHistory, MaxPage = maxPage};

            // 返回 JSON 格式的結果
            return Json(result);
        }
    }
}