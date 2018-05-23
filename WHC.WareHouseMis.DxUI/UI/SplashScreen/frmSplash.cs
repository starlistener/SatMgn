using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WHC.WareHouseMis.UI.SplashScreen
{
    public partial class frmSplash : Form,ISplashForm
    {
        public frmSplash()
        {
            InitializeComponent();
        }

        #region ISplashForm

        void ISplashForm.SetStatusInfo(string NewStatusInfo)
        {
            lbStatusInfo.Text = NewStatusInfo;
        }

        #endregion

        private void lbStatusInfo_Click(object sender, EventArgs e)
        {

        }
    }
}