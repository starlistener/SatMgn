using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;

using WHC.Framework.Commons;
using WHC.Framework.ControlUtil;
using WHC.Framework.BaseUI;

using WHC.Dictionary;
using WHC.Dictionary.UI;

namespace WHC.WareHouseMis.UI
{
    public partial class MainForm : RibbonForm
    {
        #region 属性变量
        private AppConfig config = new AppConfig();
        //月结线程
        private BackgroundWorker worker;
        private BackgroundWorker annualWorker;
        //全局热键
        private RegisterHotKeyHelper hotKey2 = new RegisterHotKeyHelper();

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
        /// <summary>
        /// Sets command status
        /// </summary>
        public string CommandStatus
        {
            get { return lblCommandStatus.Caption; }
            set { lblCommandStatus.Caption = value; }
        }
        /// <summary>
        /// Sets user status
        /// </summary>
        public string UserStatus
        {
            get { return lblCurrentUser.Caption; }
            set { lblCurrentUser.Caption = value; }
        } 
        #endregion 

        public MainForm()
        {
            InitializeComponent();
            
            SplashScreen.Splasher.Status = "正在展示相关的内容...";
            System.Threading.Thread.Sleep(100);

            InitUserRelated();

            InitSkinGallery();
            UserLookAndFeel.Default.SetSkinStyle("Office 2010 Blue");

            SplashScreen.Splasher.Status = "初始化完毕...";
            System.Threading.Thread.Sleep(50);

            SplashScreen.Splasher.Close();
            SetHotKey();
        }
                 
        /// <summary>
        /// 初始化用户相关的系统信息
        /// </summary>
        private void InitUserRelated()
        {            
            #region 根据权限显示对象的初始化窗体
            //if (Portal.gc.HasFunction("ItemDetail"))
            //{
                ChildWinManagement.LoadMdiForm(this, typeof(FrmItemDetail));
            //}
            #endregion

            #region 初始化系统名称
            try
            {
                string Manufacturer = config.AppConfigGet("Manufacturer");
                string ApplicationName = config.AppConfigGet("ApplicationName");
                string AppWholeName = string.Format("{0}-{1}", Manufacturer, ApplicationName);
                Portal.gc.gAppUnit = Manufacturer;
                Portal.gc.gAppMsgboxTitle = AppWholeName;
                Portal.gc.gAppWholeName = AppWholeName;

                this.Text = AppWholeName;
                this.notifyIcon1.BalloonTipText = AppWholeName;
                this.notifyIcon1.BalloonTipTitle = AppWholeName;
                this.notifyIcon1.Text = AppWholeName;

                UserStatus = string.Format("当前用户：{0}({1})", Portal.gc.LoginInfo.FullName, Portal.gc.LoginInfo.Name);
                CommandStatus = string.Format("欢迎使用 {0}", Portal.gc.gAppWholeName);
            }
            catch { }

            #endregion

            InitAuthorizedUI();//根据权限屏蔽
            if (this.ribbonControl.Pages.Count > 0)
            {
                ribbonControl.SelectedPage = ribbonControl.Pages[0];
            }
        }

        /// <summary>
        /// 设置Alt+S的显示/隐藏窗体全局热键
        /// </summary>
        private void SetHotKey()
        {
            try
            {
                hotKey2.Keys = Keys.S;
                hotKey2.ModKey = RegisterHotKeyHelper.MODKEY.MOD_ALT;
                hotKey2.WindowHandle = this.Handle;
                hotKey2.WParam = 10003;
                hotKey2.HotKey += new RegisterHotKeyHelper.HotKeyPass(hotKey2_HotKey);
                hotKey2.StarHotKey();
            }
            catch (Exception ex)
            {
                MessageDxUtil.ShowError(ex.Message);
                LogTextHelper.WriteLine(ex.ToString());
            }
        }

        void hotKey2_HotKey()
        {
            notifyMenu_Show_Click(null, null);
        }

        void InitSkinGallery()
        {
            DevExpress.XtraBars.Helpers.SkinHelper.InitSkinGallery(rgbiSkins, true);
            this.ribbonControl.Toolbar.ItemLinks.Add(rgbiSkins);
        }

        public void CloseAllDocuments()
        {
        }

        #region 托盘菜单操作

        private void notifyMenu_About_Click(object sender, EventArgs e)
        {
            Portal.gc.About();
        }

        private void notifyMenu_Show_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Maximized;
                this.Show();
                this.BringToFront();
                this.Activate();
                this.Focus();
            }
            else
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
        }

        private void notifyMenu_Exit_Click(object sender, EventArgs e)
        {
            try
            {
                this.ShowInTaskbar = false;
                Portal.gc.Quit();
            }
            catch
            {
                // Nothing to do.
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyMenu_Show_Click(sender, e);
        }

        private void MainForm_MaximizedBoundsChanged(object sender, EventArgs e)
        {
            this.Hide();
        }

        /// <summary>
        /// 缩小到托盘中，不退出
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //如果我们操作【×】按钮，那么不关闭程序而是缩小化到托盘，并提示用户.
            if (this.WindowState != FormWindowState.Minimized)
            {
                e.Cancel = true;//不关闭程序

                //最小化到托盘的时候显示图标提示信息，提示用户并未关闭程序
                this.WindowState = FormWindowState.Minimized;
                notifyIcon1.ShowBalloonTip(3000, "程序最小化提示",
                     "图标已经缩小到托盘，打开窗口请双击图标即可。也可以使用Alt+S键来显示/隐藏窗体。",
                     ToolTipIcon.Info);
            }
        }

        private void MainForm_Move(object sender, EventArgs e)
        {
            if (this == null)
            {
                return;
            }

            //最小化到托盘的时候显示图标提示信息
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Hide();
                notifyIcon1.ShowBalloonTip(3000, "程序最小化提示",
                    "图标已经缩小到托盘，打开窗口请双击图标即可。也可以使用Alt+S键来显示/隐藏窗体。",
                    ToolTipIcon.Info);
            }
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Portal.gc.Notify("系统信息提示", "图标已经缩小到托盘，打开窗口请双击图标即可。\r\n软件来自爱启迪技术有限公司！\r\n软件支持网站：Http://www.iqidi.com");

        }

        #endregion

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void Exit(object sender, EventArgs args)
        {
            DialogResult dr = MessageDxUtil.ShowYesNoAndTips("点击“Yes”退出系统，点击“No”返回");

            if (dr == DialogResult.Yes)
            {
                notifyIcon1.Visible = false;
                Application.ExitThread();
            }
            else if (dr == DialogResult.No)
            {
                return;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Init();
        }

        /// <summary>
        /// 根据权限屏蔽功能
        /// </summary>
        private void InitAuthorizedUI()
        {
            this.tool_Report.Enabled = Portal.gc.HasFunction("Report");
            this.tool_Dict.Enabled = Portal.gc.HasFunction("Dictionary");
            this.tool_ItemDetail.Enabled = Portal.gc.HasFunction("ItemDetail");
            this.tool_Purchase.Enabled = Portal.gc.HasFunction("Purchase");
            this.tool_StockSearch.Enabled = Portal.gc.HasFunction("StockSearch");
            this.tool_TakeOut.Enabled = Portal.gc.HasFunction("TakeOut");
            this.tool_WareHouse.Enabled = Portal.gc.HasFunction("WareHouse");
            //this.menu_run_systemLog.Enabled = Portal.gc.HasFunction("LoginLog");
            this.tool_Settings.Enabled = Portal.gc.HasFunction("Parameters");
            this.tool_MonthlyStatistic.Enabled = Portal.gc.HasFunction("MonthlyStatistic");
            this.tool_AnnualStatistic.Enabled = Portal.gc.HasFunction("AnnualStatistic");
            this.tool_ClearAll.Enabled = Portal.gc.HasFunction("ClearAllData");
            this.tool_ImportItemDetail.Enabled = Portal.gc.HasFunction("ImportItemDetail");
        }

        private void Init()
        {
            CCalendar cal = new CCalendar();
            this.lblCalendar.Caption = cal.GetDateInfo(System.DateTime.Now).Fullinfo;
        }


        #region 工具条操作
        private void barItemExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Exit(null, null);
        }

        private void btnHelp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Portal.gc.Help();
        }

        private void btnAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Portal.gc.About();
        }

        private void btnRegister_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //Portal.gc.ShowRegDlg();
        }

        private void btnBug_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
        }

        private void btnMyWeb_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Process.Start("http://www.iqidi.com");
        }

        private void menuLogo_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                Process.Start("http://www.iqidi.com");
            }
            catch (Exception)
            {
                MessageDxUtil.ShowError("打开浏览器失败");
            }
        }

        private void tool_Dict_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmDictionary dlg = new FrmDictionary();
            dlg.ShowDialog();
        }

        private void tool_Security_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            WHC.Security.UI.Portal.StartLogin();
        }

        private void tool_ModifyPass_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FrmModifyPassword dlg = new FrmModifyPassword();
            dlg.ShowDialog();
        }

        private void tool_loginLog_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            //ChildWinManagement.LoadMdiForm(this, typeof(FrmLogHistroy));
        }

        #endregion

        private void btnRelogin_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (MessageDxUtil.ShowYesNoAndWarning("您确定需要重新登录吗？") != DialogResult.Yes)
                return;


            Portal.gc.MainDialog.Hide();

            Logon dlg = new Logon();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            if (DialogResult.OK == dlg.ShowDialog())
            {
                if (dlg.bLogin)
                {
                    CloseAllDocuments();
                    InitUserRelated();
                }

            }
            dlg.Dispose();
            Portal.gc.MainDialog.Show();
        }

        private void tool_ItemDetail_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            ChildWinManagement.LoadMdiForm(this, typeof(FrmItemDetail));
        }

    }
}
