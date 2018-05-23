using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Data;
using System.Globalization;
using Microsoft.Win32;

using WHC.Security.Entity;
using WHC.Framework.Commons;
using WHC.Framework.ControlUtil;
using WHC.Framework.BaseUI;

namespace WHC.WareHouseMis.UI
{
    public class GlobalControl
    {
        #region 系统全局变量
        public MainForm MainDialog;
        public List<CListItem> ManagedWareHouse = new List<CListItem>();

        public string Login_Name_Key = "WareHouseMis_LoginName";
        public string gAppMsgboxTitle = string.Empty;   //程序的对话框标题
        public string gAppUnit = string.Empty; //单位名称
        public string gAppWholeName = "";

        public UserInfo LoginInfo = null;//登录用户信息
        //登录用户具有的功能字典集合
        public Dictionary<string, FunctionInfo> FunctionDict = new Dictionary<string, FunctionInfo>();

        #endregion

        #region 基本操作函数

        /// <summary>
        /// 看用户是否具有某个功能
        /// </summary>
        /// <param name="controlID"></param>
        /// <returns></returns>
        public bool HasFunction(string controlID)
        {
            bool result = false;
            if (FunctionDict.ContainsKey(controlID))
            {
                result = true;
            }

            return result;
        }
        
        /// <summary>
        /// 退出系统
        /// </summary>
        public void Quit()
        {
            if (Portal.gc.MainDialog != null)
            {
                Portal.gc.MainDialog.Hide();
                Portal.gc.MainDialog.CloseAllDocuments();
            }

            Application.Exit();
        }

        /// <summary>
        /// 打开帮助文档
        /// </summary>
        public void Help()
        {
            try
            {
                const string helpfile = "Help.chm";
                Process.Start(helpfile);
            }
            catch (Exception)
            {
                MessageDxUtil.ShowWarning("文件打开失败");
            }
        }

        /// <summary>
        /// 关于对话框信息
        /// </summary>
        public void About()
        {
            AboutBox dlg = new AboutBox();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.ShowDialog();
        }
                 
        /// <summary>
        /// 弹出提示消息窗口
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="content"></param>
        public void Notify(string caption, string content)
        {
            Notify(caption, content, 400, 200, 5000);
        }

        /// <summary>
        /// 弹出提示消息窗口
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="content"></param>
        public void Notify(string caption, string content, int width, int height, int waitTime)
        {
            NotifyWindow notifyWindow = new NotifyWindow(caption, content);
            notifyWindow.TitleClicked += new System.EventHandler(notifyWindowClick);
            notifyWindow.TextClicked += new EventHandler(notifyWindowClick);
            notifyWindow.SetDimensions(width, height);
            notifyWindow.WaitTime = waitTime;
            notifyWindow.Notify();

            //保存到系统消息表
            //SystemMessageInfo messageInfo = new SystemMessageInfo();
            //messageInfo.ID = Guid.NewGuid().ToString();
            //messageInfo.Title = caption;
            //messageInfo.Content = content;
            //try
            //{
            //    BLLFactory<SystemMessage>.Instance.Insert(messageInfo);
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.Error(ex);
            //    MessageDxUtil.ShowError(ex.Message);
            //}
        }

        private void notifyWindowClick(object sender, EventArgs e)
        {
            //SystemMessageInfo info = BLLFactory<SystemMessage>.Instance.FindLast();
            //if (info != null)
            //{
            //    //FrmEditMessage dlg = new FrmEditMessage();
            //    //dlg.ID = info.ID;
            //    //dlg.ShowDialog();
            //}
        }

        #endregion

    }
}
