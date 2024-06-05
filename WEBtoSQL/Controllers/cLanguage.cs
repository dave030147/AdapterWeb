using System.Diagnostics;
using System.Reflection;

namespace WEBtoSQL.Controllers
{
    public class cLanguage
    {
        public int Language(string choose)
        {
            if (choose == "zh-TW")
            {
                return 1;
            }
            else if (choose == "zh-CN")
            {
                return 2;
            }
            //沒有選擇就是選擇英語
            return 0;
        }

        /****************      Header     ****************/
        public string[] Header_Home = { "Home", "首頁", "首頁" };
        public string[] Header_accountmanage = { "Account Manage", "帳號管理", "帐号管理" };
        public string[] Header_setting = { "Settings", "設定", "设置" };
        public string[] Header_comtest = { "Com Port Test", "Com Port測試", "Com Port测试" };
        public string[] Header_resetserver = { "Restart the server", "重新開啟伺服器", "重新开启伺服器" };
        public string[] Header_resetalarm = { "Error Remove", "確認異常排除", "确认异常排除" };
        public string[] Header_language = { "English", "中文(繁)", "中文(简)" };
        public string[] Header_login = { "Log in", "登入", "登录" };
        public string[] Header_logout = { "Log out", "登出", "登出" };
        public string[] Herder_history = { "History Data", "歷史資料", "历史资料" };
        /****************      訊息視窗     ****************/
        public string[] btn_msgbox_ok = { "OK", "確定", "确定" };
        public string[] btn_msgbox_cancel = { "Cancel", "取消", "取消" };
        /****************      登入畫面     ****************/
        public string[] modal_account = { "Account", "帳戶", "帐户" };
        public string[] modal_userid = { "User ID", "使用者帳號", "使用者帐号" };
        public string[] modal_userpassword = { "User Password", "使用者密碼", "使用者密码" };
        public string[] modal_login = { "Login", "登入", "登录" };
        /***************     測試操作畫面    ***************/
        public string[] modal_equipmentname = { "Account", "設備名稱", "帐户" };
        public string[] lab_RN = { "Recipe Name", "配方名稱", "配方名称" };
        /********  Message  ********/
        public string[] modal_msg_loginsuccess = { "Sign in suceesfully.", "登入成功", "登入成功" };
        public string[] modal_msg_loginfailed_password = { "Login failed, wrong password.", "登入失敗，密碼錯誤", "登入失败，密码错误" };
        public string[] modal_msg_loginfailed_id = { "Login failed, there is no such account.", "登入失敗，無此帳號", "登入失败，无此帐号" };
        public string[] msg_restartservice = { "Do you want to restart service?", "確定要重啟伺服器嗎?", "确定要重启伺服器吗?" };
        public string[] msg_restartalarm = { "Do you want to remove Error display?", "確定要異常排除嗎?", "确定要异常排除吗?" };
        public string[] msg_noID = { "please enter your account.", "請輸入帳號", "请输入帐号" };
        public string[] msg_noPW = { "Please enter password.", "請輸入密碼", "请输入密码" };
        public string[] msg_nobarcode = { "Topbarcode and Bottombarcode must have at least one filled-in information.", "Topbarcode、Bottombarcode至少得有一個填入資料", "Topbarcode、Bottombarcode至少得有一个填入资料" };
        /***************     歷史資料畫面    ***************/
        public string[] btn_before = { "Before", "前一日", "前一日" };
        public string[] btn_date = { "Select date", "選擇日期", "选择日期" };
        public string[] btn_after = { "After", "後一日", "后一日" };
        public string[] btn_today = { "Today", "當日", "当日" };
        public string[] btn_prepage = { "Previous", "上一頁", "上一页" };
        public string[] btn_nextpage = { "Next", "下一頁", "下一页" };
        public string[] btn_Productiondate = { "Production", "生產日期", "生产日期" };
        /****************      帳號管理     ****************/
        public string[] modal_accountmanagement = { "Account management", "帳號管理", "帐号管理" };
        public string[] modal_addaccount = { "Add new account", "新增帳號", "新增帐户" };
        public string[] modal_userlv = { "User Level", "使用者層級", "使用者层级" };
        public string[] modal_username = { "User Name", "使用者名稱", "用户名" };
        public string[] modal_operate = { "Operate", "操作", "操作" };
        public string[] modal_btn_edit = { "Edit", "修改", "修改" };
        public string[] modal_btn_delete = { "Delete", "刪除", "删除" };
        public string[] modal_newid = { "New ID", "新帳號", "新帐号" };
        public string[] modal_newpassword = { "New Password", "新密碼", "新密码" };
        public string[] modal_checkpassword = { "Check Password", "確認密碼", "确认密码" };
        //已建立public string[] modal_userlv = { "User Level", "使用者層級", "使用者层级" };
        public string[] modal_btn_submit = { "Submit", "提交", "提交" };
        public string[] modal_btn_addition = { "Addition", "新增", "新增" };
        /********  Message  ********/
        public string[] modal_msg_nopermissions = { "No permission, please log in first.", "沒有權限，請先登入", "没有权限，请先登入" };
        public string[] modal_msg_lowlv = { "No permission, please use higher level permissions.", "沒有權限，請使用更高階的權限", "没有权限，请使用更高阶的权限" };
        public string[] modal_msg_login_empty = { "Please fill in all fields and select account level.", "請填寫所有欄位並選擇帳號等級", "请填写所有栏位并选择帐号等级" };
        public string[] modal_msg_passwordnosame = { "The password is inconsistent, please re-enter it.", "密碼不一致請重新輸入", "密码不一致请重新输入" };
        public string[] modal_msg_creatsuccess = { "added successfully.", "新增成功", "新增成功" };
        public string[] modal_msg_msg_creatfailed = { "Failed to add, this account is already in use.", "新增失敗，此帳號已被使用", "新增失败，此帐号已被使用" };
        public string[] modal_msg_accountisusing = { "Unable to delete, this account is in use.", "無法刪除，此帳號正在使用中", "无法删除，此帐号正在使用中" };
        public string[] modal_msg_isremove1 = { "Are you sure you want to delete the user", "確定要刪除使用者", "确定要删除使用者" };
        public string[] modal_msg_isremove2 = { "?", "嗎？", "吗？" };
        public string[] modal_msg_removesuccess = { "successfully deleted.", "刪除成功", "删除成功" };
        public string[] modal_msg_removefailed = { "failed to delete.", "刪除失敗", "删除失败" };
        public string[] modal_msg_modifysuccess = { "successfully modified.", "修改成功", "修改成功" };
        public string[] modal_msg_modifyfailed = { "fail to edit.", "修改失敗", "修改失败" };
        /****************      帳戶資訊修改     ****************/
        public string[] modal_editaccount = { "Edit account information", "修改帳戶資訊", "修改帐户资讯" };
        //已建立public string[] modal_userid = { "User ID", "使用者帳號", "使用者帐号" };
        //已建立public string[] modal_username = { "User Name", "使用者名稱", "用户名" };
        //已建立public string[] modal_userpassword = { "User Password", "使用者密碼", "使用者密码" };
        //已建立public string[] modal_checkpassword = { "Check Password", "確認密碼", "确认密码" };
        //已建立public string[] modal_userlv = { "User Level", "使用者層級", "使用者层级" };
        public string[] modal_btn_editsubmit = { "Submit Edit", "提交修改", "提交修改" };    
        /********  Message  ********/
        /****************      首頁     ****************/
        public string[] status_upstream = { "Upstream", "Hermes 上游", "Hermes 上游" };
        public string[] status_downstream = { "Downstream", "Hermes 下游", "Hermes 下游" };
        public string[] status_supervisory = { "Supervisory", "Hermes 上位", "Hermes 上位" };
        public string[] lab_status = { "Status", "通訊狀態", "通讯状态" };
        public string[] lab_messagefrom = { "Message From:", "訊息來源:", "讯息来源:" };
        public string[] titl_supervisory = { "Supervisory", "Hermes 上位", "Hermes 上位" };
        public string[] titl_upstream = { "Upstream", "上游", "上游" };
        public string[] titl_downstream = { "Downstream", "下游", "下游" };
        public string[] titl_receivemessage = { "RecevieMessage", "接收到的訊息", "接收到的讯息" };
        public string[] titl_sendmessage = { "SendMessage", "已傳送的訊息", "已传送的讯息" };
        public string[] titl_systemmessage = { "System Message", "系統訊息", "系统讯息" };
        public string[] titl_enterbarcode = { "Please enter TopBarCodec or BottomBarCode.", "請輸入TopBarCodec或BottomBarCode", "请输入TopBarCodec或BottomBarCode" };
        /********  Message  ********/
        public string[] msg_dele_storage = { "Whether to delete the board with this ID (all BoardIDs with the same BoardID will be deleted):", "是否刪除此ID的板子(會將同BoardID都刪除):", "是否删除此ID的板子(会将同BoardID都删除):" };
        /****************      設定     ****************/
        /********  Table Title  ********/
        public string[] titl_type = { "Type", "類型", "类型" };
        public string[] titl_name = { "Name", "名稱", "名称" };
        public string[] titl_value = { "Value", "值", "值" };
        public string[] titl_operate = { "Operate", "操作", "操作" };
        /********  操作按鈕  ********/
        public string[] btn_edit = { "Edit", "編輯", "编辑" };
        public string[] btn_save = { "Save", "儲存", "储存" };
        public string[] btn_cancel = { "Cancel", "取消", "取消" };
        public string[] btn_true = { "True", "是", "是" };
        public string[] btn_false = { "False", "否", "否" };
        public string[] btn_remit = { "Export", "匯出", "汇出" };
        public string[] btn_import = { "Import", "匯入", "汇入" };
        public string[] btn_clear = { "Clear", "清除", "清除" };
        public string[] btn_edit_station = { "Add, modify, delete templates", "新增修改刪除樣板", "新增修改删除样板" };
        /********  Common  ********/
        public string[] table_information = { "Information", "系統資訊", "系统资讯" };
        public string[] table_modelname = { "Model Name", "設備型號", "设备型号" };
        public string[] table_machineid = { "Machine ID", "設備編號", "设备编号" };
        public string[] table_displayname = { "Display Name", "設備顯示名稱", "设备显示名称" };
        public string[] table_version = { "Version", "版本", "版本" };
        public string[] table_BoardMode = { "Board data processing mode", "板資料處理模式", "板资料处理模式" };
        public string[] btn_single = { "Single", "單片", "单片" };
        public string[] btn_fifo = { "FIFO", "先進先出", "先进先出" };
        public string[] btn_match = { "Match", "匹配", "匹配" };
        public string[] warning_BoardMode = { "Match requires a Reader to be used", "匹配需要有掃碼機才能使用", "匹配需要有扫码机才能使用" };
        public string[] table_fifoRange = { "On-board temporary storage", "機內暫存", "机内暂存" };
        public string[] table_tcpip = { "TCP IP", "TCP IP位置", "TCP IP位置" };
        public string[] table_tcpport = { "TCP Port", "TCP 埠號", "TCP 埠号" };
        public string[] msg_P_Int = { "positive integer.", "正整數", "正整数" };
        /********  Hermes  ********/
        public string[] table_upstream = { "Upstream", "Hermes 上游", "上游" };
        public string[] table_downstream = { "Downstream", "Hermes 下游", "下游" };
        public string[] table_supervisory = { "Supervisory", "Hermes 上位", "Hermes 上位" };
        public string[] table_enable = { "Enable", "是否啟用", "是否启用" };
        public string[] table_checkalive1 = { "Check Alive", "存活確認間隔秒數", "存活确认间隔秒数" };
        public string[] table_checkalive2 = { "Interval(s)", "秒", "秒" };
        public string[] table_queryresponse1 = { "Query Response (0.01~n)", "查詢回復逾時秒數 (0.01~n)", "查询回复逾时秒数 (0.01~n)" };
        public string[] table_queryresponse2 = { "Timeout(s)", "秒", "秒" };
        public string[] table_strattransportdelaytime1 = { "Strattransport Delay Time (0.01~n)", "延遲啟動秒數 (0.01~n)", "延迟启动秒数 (0.01~n)" };
        public string[] table_strattransportdelaytime2 = { "(s)", "秒", "秒" };
        public string[] table_order = { "Send BA and MR in order", "發送BA及MR先後順序", "发送BA及MR先后顺序" };
        public string[] table_RWI = { "Reply Work Order Info", "回覆工單訊息", "回复工单信息" };
        public string[] table_Command = { "Command", "命令", "命令" };
        public string[] table_Reserved_quantity = { "Command reserved quantity", "命令預留數量", "命令预留数量" };
        public string[] table_Command_effect = { "Command effect", "命令是否作用", "命令是否作用" };
        public string[] table_Give_Command_Order = { "Give Command Order", "發送命令", "发送命令" };
        public string[] table_Command_Path = { "Command effect path", "命令作用方向", "命令方向作用" };
        public string[] select_ordre_standard = { "Standard", "標準", "标准" };
        public string[] select_ordre_BAMR = { "BA first and MR later", "先BA後MR", "先BA后MR" };
        public string[] select_ordre_MRBA = { "MR first and BA later", "先MR後BA", "先MR后BA" };
        //已存在  public string[] table_tcpport = { "TCP Port", "TCP 埠號", "TCP 埠号" };
        public string[] table_hostadress = { "Host Adress", "連線IP位置", "连线IP位置" };
        public string[] table_clientadress = { "Client Adress", "允許連線IP位置", "允许连线IP位置" };
        /***  服務宣告  ***/
        public string[] table_checkaliveresponse = { "Check Alive Response", "回應存活確認", "回应存活确认" };
        public string[] table_queryworkorderinfo = { "Query Work Order Info", "查詢工單資訊", "查询工单资讯" };
        public string[] table_sendworkorderinfo = { "Send Work Order Info", "發送工單資訊", "发送工单资讯" };
        public string[] table_configuration = { "Configuration", "組態設定相關", "组态设定相关" };
        public string[] table_boardtracking = { "Board Tracking", "板況追蹤相關", "板况追踪相关" };
        //public string[] table_replyworkorderinfo = { "Reply Work Order Info", "", "" };
        public string[] table_boardforecast = { "Board Forecast", "板預告", "板预告" };
        public string[] table_queryboardinfo = { "Query Board Info", "查詢板資訊", "查询板资讯" };
        //public string[] table_command = { "Command", "", "" };
        public string[] table_sendboardinfo = { "Send Board Info", "發送板資訊(Send Board Info)", "发送板资讯(Send Board Info)" };
        /********    CFX    ********/
        public string[] table_local = { "Local", "本機端", "本机端" };
        public string[] table_broker = { "Broker", "訊息派發伺服器", "讯息派发伺服器" };
        public string[] table_target = { "Target", "請求目標站別", "请求目标站别" };
        //已存在public string[] table_enable = { "Enable", "是否啟用", "是否启用" };
        public string[] table_handleid = { "Handle ID", "控制代碼", "控制代码" };
        public string[] table_requesturi = { "Request Uri", "接收請求的Uri路徑", "接收请求的Uri路径" };
        public string[] table_uri = { "Uri", "伺服器Uri路徑", "伺服器Uri路径" };
        public string[] table_publishpath = { "Publish Path", "訊息發布路徑", "讯息发布路径" };
        public string[] table_subscriptionpath = { "Subscription Path", "訂閱訊息路徑", "订阅讯息路径" };
        public string[] table_tequesttimeout = { "Tequest Timeout", "請求逾時", "请求逾时" };
        /********  Message  ********/
        public string[] msg_modifying = { "Please save the data being modified before editing other items.", "請先將正在修改的資料儲存，再編輯其他項", "请先将正在修改的资料储存，再编辑其他项" };
        public string[] msg_isvaluefalse1 = { "Please enter a value ranges from ", "請輸入", "请输入" };
        public string[] msg_isvaluefalse2 = { "&nbsp; to ", "到", "到" };
        public string[] msg_isvaluefalse3 = { ".(Supports up to the second decimal place.)", "有效值(支援到小數點第二位)", "有效值(支援到小数点第二位)" };
        public string[] msg_isvaluebiggerequal0 = { "Please enter 0 or a valid positive integer.", "請輸入0或有效正整數", "请输入0或有效正整数" };
        public string[] msg_isvaluebiggerthan0 = { "Please enter a valid positive integer.", "請輸入有效正整數", "请输入有效正整数" };
        public string[] msg_savesuccess = { "Saved successfully. After all settings are completed, please restart the server to complete the changes.", "儲存成功，全部設定完成後請重啟伺服器完成變更", "储存成功，全部设定完成后请重启伺服器完成变更" };
        public string[] msg_savefailed = { "Save failed.", "儲存失敗:", "储存失败:" };
        public string[] msg_readernor = { "Reader test successful.", "Reader測試成功", "Reader测试成功" };
        public string[] msg_readererr = { "Reader test failed.", "Reader測試失敗", "Reader测试失败" };
        public string[] msg_subsuccess = { "Submit successfully.", "提交成功", "提交成功" };
        public string[] msg_subfailed = { "Submit failed.", "提交失敗:", "提交失败:" };
        public string[] msg_issave = { "Do you want to save changes?", "是否要儲存變更?", "是否要储存变更?" };
        public string[] msg_saving = { "Storing.", "儲存中", "储存中" };
        public string[] msg_isleave = { "There are unsaved changes. Do you want to leave?", "有未儲存的變更，是否離開?", "有未储存的变更，是否离开?" };
        public string[] msg_validateIP = { "IP address format is incorrect.", "地址格式不正確", "地址格式不正确" };
        public string[] msg_portrange = { "Please enter the port number from 0 to 65536.", "請輸入0~65536的埠號", "请输入0~65536的埠号" };
        public string[] msg_comparison = { "The maximum value cannot be less than the minimum value.", "最大值不可小於最小值", "最大值不可小于最小值" };
        public string[] msg_beforeremit = { "The data has changed, please save it first and then export it.", "資料變更，請先存檔再匯出", "资料变更，请先存档再汇出" };
        public string[] msg_importsuccess = { "The import was successful, please confirm that the data is correct and save the changes.", "匯入成功，請確認資料無誤後，儲存變更", "汇入成功，请确认资料无误后，储存变更" };
        public string[] msg_importfault = { "Import failed,", "匯入失敗，", "汇入失败，" };
        public string[] msg_isreadererror = { "Reader connection error, will not be able to select.", "Reader連線錯誤，將無法選擇", "Reader连线错误，将无法选择" };
        public string[] msg_noMRnoTest = { "Current status cannot be tested.", "目前狀態無法測試", "目前状态无法测试" };
        //public string[] msg_CSVwrong = { "CSV format error.", "CSV格式錯誤", "CSV格式错误" };
        public string[] msg_ASCIIwrong = { "format error.", "格式錯誤", "格式错误" };
        public string[] msg_rangewrong = { "Out of range", "超出範圍", "超出范围" };
        public string[] msg_format_wrong = { "Please use numbers and English, do not use words outside the format.", "請使用數字及英文，勿使用格式外的文字", "请使用数字及英文，勿使用格式外的文字" };
        public string[] msg_format_wrong_empty = { "Please use numbers and English, do not use words outside the format, and cannot be empty.", "請使用數字及英文，勿使用格式外的文字，且不得為空", "请使用数字及英文，勿使用格式外的文字，且不得为空" };
        public string[] msg_issubdele1 = { "Whether to delete the subboard at position ", "是否刪除位置:", "是否删除位置:" };
        public string[] msg_issubdele2 = { "?", "的子板?", "的子板?" };
        public string[] msg_sub_alldelete = { "Out of range", "是否所有刪除子板資料?", "是否所有删除子板资料?" };
        public string[] msg_open_reader = { "At least one Reader must be enabled.", "Reader至少要啟用一隻", "Reader至少要启用一只" };
        //public string[] msg_ = { "", "", "" };
        /********  workorder  ********/
        public string[] btn_ManualKeyin = { "ManualKeyin", "手動輸入", "手动输入" };
        public string[] btn_Supervisory = { "Inquire", "查詢", "查询" };
        public string[] WorkOrderData = { "Work Order Data:", "工單資訊來源:", "工单资讯来源:" };
        public string[] btn_Stay = { "Stop and wait", "停下等待", "停下等待" };
        public string[] btn_CreateData = { "Create new profile", "創建新資料", "创建新资料" };
        public string[] NoDataAct = { "No data action:", "無資料動作:", "无资料动作:" };
        public string[] warning_WorkOrderData = { "To enable the inquire function, you need to enable the Supervisory, QBI and QWI", "啟用查詢功能需開啟上位、QBI及QWI", "启用查询功能需开启上位、QBI及QWI" };
        public string[] warning_NoDataAct = { "What to do when QBI or QWI cannot ask for information", "QBI或QWI詢問不到資料時處理方式", "QBI或QWI询问不到资料时处理方式" };
        public string[] table_workorder = { "Work Order", "工單資訊", "工单资讯" };
        public string[] table_WorkOrderData = { "Work Order", "工單資訊來源", "工单资讯来源" };
        public string[] table_FailedBoard = { "FailedBoard", "板子判定狀況", "板子判定状况" };
        public string[] table_ProductTypeID = { "ProductTypeID", "產品類型", "产品类型" };
        public string[] table_FlippedBoard = { "FlippedBoard", "板子翻轉狀況", "板子翻转状况" };
        public string[] table_Length = { "Length", "板長", "板长" };
        public string[] table_Width = { "Width", "板寬", "板宽" };
        public string[] table_Thickness = { "Thickness", "板厚", "板厚" };
        public string[] table_ConveyorSpeed = { "ConveyorSpeed", "輸送帶速度", "输送带速度" };
        public string[] table_TopClearanceHeight = { "TopClearanceHeight", "頂部間隙高度", "顶部间隙高度" };
        public string[] table_BottomClearanceHeight = { "BottomClearanceHeight", "底部間隙高度", "底部间隙高度" };
        public string[] table_Weight = { "Weight", "板重", "板重" };
        public string[] table_Route = { "Route", "路線", "路线" };
        public string[] table_Action = { "Action", "動作", "动作" };
        public string[] table_WorkOrderID = { "WorkOrderID", "工單號碼", "工单号码" };
        public string[] table_BatchID = { "BatchID", "批號", "批号" };
        public string[] table_SubBoards = { "SubBoards", "子板", "子板" };
        public string[] table_SB_Pos = { "Position", "位置", "位置" };
        public string[] table_SB_Bc = { "Barcode", "條碼", "条码" };
        public string[] table_SB_St = { "State", "狀態", "状态" };
        public string[] btn_FailedBoard_good = { "Good", "正常板", "正常板" };
        public string[] btn_FailedBoard_failed = { "Failed", "故障板", "故障板" };
        public string[] btn_FailedBoard_unknown = { "Unknown", "未知", "未知" };
        public string[] btn_FlippedBoard_top = { "Top", "正面", "正面" };
        public string[] btn_FlippedBoard_bottom = { "Bottom", "背面", "背面" };
        public string[] btn_FlippedBoard_unknown = { "Unknown", "未知", "未知" };
        public string[] btn_barcode_number = { "Serial No.", "流水號", "流水号" };
        public string[] btn_barcode_date = { "Date", "日期", "日期" };
        public string[] btn_barcode_none = { "None", "無", "无" };
        public string[] btn_SB_Add = { "Add New SubBoard", "新增子版", "新增子版" };
        public string[] btn_SB_Delete = { "Delete A SubBoard", "刪除子版", "删除子版" };
        public string[] btn_SB_AllDele = { "Delete All SubBoards", "刪除全部", "删除全部" };
        /********  Digital  ********/
        public string[] table_input = { "Input", "輸入", "输入" };
        public string[] table_output = { "Output", "輸出", "输出" };
        public string[] selsect_disable = { "Disable", "不啟用", "不启用" };
        public string[] selsect_Up_BA = { "UpStream Board Available(Good)", "上游BA(好板)", "上游BA(好板)" };
        public string[] selsect_Down_MR = { "DownStream Machine Ready", "下游MR", "下游MR" };
        public string[] selsect_isfliped = { "Whether the device has been flipped", "設備是否已翻轉", "设备是否已翻转" };
        public string[] selsect_Up_FBA = { "UpStream Board Available(Fail)", "上游BA(壞板)", "上游BA(坏板)" };
        public string[] selsect_Up_MR = { "UpStream Machine Ready", "上游MR", "上游MR" };
        public string[] selsect_Down_BA = { "DownStream Board Available(Good)", "下游BA(好板)", "下游BA(好板)" };
        public string[] selsect_isflip = { "Whether the device is flipped", "設備是否翻轉", "设备是否翻转" };
        public string[] selsect_Down_FBA = { "DownStream Board Available(Fail)", "下游BA(壞板)", "下游BA(坏板)" };
        public string[] select_high_potential = { "High Potential", "高電位", "高电位" };
        public string[] select_low_potential = { "Low Potential", "低電位", "低电位" };
        public string[] select_alarm = { "Warning buzzer", "警告蜂鳴器", "警告蜂鸣器" };
        /********  OutPorts  ********/
        public string[] table_Linkby = { "Link By", "關聯於", "关联于" };
        //public string[] table_enable = { "Enable", "是否啟用", "是否启用" };
        public string[] table_port = { "Port", "通訊埠", "通讯端口" };
        public string[] table_STmessage = { "Start Message", "啟動信文", "启动信文" };
        public string[] table_ERRmessage = { "Read Failure Message", "讀取失敗信文", "读取失败信文" };
        public string[] table_format = { "Format", "格式", "格式" };
        public string[] table_baudrate = { "BaudRate", "鮑率", "鲍率" };
        public string[] table_parity = { "Parity", "奇偶性", "奇偶性" };
        public string[] table_databits = { "DataBits", "資料位元", "数据位元" };
        public string[] table_stopbit = { "StopBit", "停止位元", "停止位元" };
        public string[] table_STaddress = { "ModBus starting address", "ModBus初始地址", "ModBus起始位址" };
        public string[] table_writeprefix = { "WritePrefix", "寫入前綴", "写入前缀" };
        public string[] table_writeseparator = { "WriteSeparator", "寫入分隔符", "写入分隔符" };
        public string[] table_writesuffix = { "WriteSuffix", "寫入後綴", "写入后缀" };
        public string[] table_writetimeout = { "WriteTimeout", "寫入超時時長", "写入超时时长" };
        public string[] table_machine = { "Machime", "設備", "设备" };
        public string[] table_decimalplaces = { "Supports decimal places", "支援小數點位數", "支援小数点位数" };
        public string[] msg_usesameselect = { "Used the same options.", "使用了相同的選項", "使用了相同的选项" };
        public string[] msg_usesameip = { "Used the same IP.", "使用了相同的IP位置", "使用了相同的IP位置" };
        public string[] msg_pleaseenter = { "Please Enter ", "請輸入", "请输入" };
        public string[] table_D_RN = { "Recipe Name ModBus starting address", "配方名稱ModBus初始地址", "配方名称ModBus起始位址" };
        public string[] table_D_wid = { "Width ModBus starting address", "寬度ModBus初始地址", "宽度ModBus起始位址" };
        public string[] table_RNAddrType = { "Recipe Name Address Type", "配方名稱型態", "配方名称型态" };
        public string[] table_WidAddrType = { "Width Address Type", "寬度型態", "宽度型态" };
        public string[] table_datatype = { "Data type", "資料類型", "资料类型" };
        public string[] sele_LowAddressFirst = { "LowAddressFirst", "低位優先", "低位优先" };
        public string[] sele_HighAddressFirst = { "HighAddressFirst", "高位優先", "高位优先" };
        public string[] sele_LowAddressHighByteFirst = { "LowAddressHighByteFirst", "低位置高位元優先", "低位置高位元优先" };
        public string[] sele_LowAddressLowByteFirst = { "LowAddressLowByteFirst", "低位置低位元優先", "低位置低位元优先" };
        public string[] sele_HighAddressHighByteFirst = { "HighAddressHighByteFirst", "高位置高位元優先", "高位置高位元优先" };
        public string[] sele_HighAddressLowByteFirst = { "HighAddressLowByteFirst", "高位置低位元優先", "高位置低位元优先" };

        //Waiting網頁
        public string[] Waiting_title_start = { "Start UP...", "啟動中...", "启动中..." };
        public string[] Waiting_title_stop = { "Stop", "停止中", "停止中" };
        public string[] Waiting_title_erroe = { "Error", "錯誤", "错误" };
        public string[] Waiting_Message_start = { "Wait for server to start.", "等待伺服器啟動", "等待服务器启动" };
        public string[] Waiting_Message_stop = { "Please start the server.", "請啟動伺服器", "请启动伺服器" };
        public string[] Waiting_Message_error = { "Server not found.", "找不到伺服器", "找不到伺服器" };

        //記錄檔保留天數
        public string[] lbl_days = { "Number of days to retain log files:", "記錄檔保留天數:", "记录档保留天数:" };
        
    }
}
