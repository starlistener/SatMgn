using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WHC.Pager.Entity;
using WHC.Framework.Commons;
using WHC.Framework.ControlUtil;
using WHC.Framework.BaseUI;
using WHC.Dictionary;

namespace WHC.WareHouseMis.UI
{
    public partial class FrmItemDetail : BaseDock
    {
        public FrmItemDetail()
        {
            InitializeComponent();

            InitDictItem();

            this.winGridViewPager1.OnPageChanged += new EventHandler(winGridViewPager1_OnPageChanged);
            this.winGridViewPager1.OnStartExport += new EventHandler(winGridViewPager1_OnStartExport);
            this.winGridViewPager1.OnEditSelected += new EventHandler(winGridViewPager1_OnEditSelected);

            if (Portal.gc.HasFunction("ItemDetail/Add"))
            {
                this.winGridViewPager1.OnAddNew += new EventHandler(winGridViewPager1_OnAddNew);
            }
            if (Portal.gc.HasFunction("ItemDetail/Delete"))
            {
                this.winGridViewPager1.OnDeleteSelected += new EventHandler(winGridViewPager1_OnDeleteSelected);
            }

            this.winGridViewPager1.OnRefresh += new EventHandler(winGridViewPager1_OnRefresh);
            this.winGridViewPager1.AppendedMenu = this.contextMenuStrip1;
            this.winGridViewPager1.ShowLineNumber = true;
            this.winGridViewPager1.gridView1.DataSourceChanged += new EventHandler(gridView1_DataSourceChanged);
            this.winGridViewPager1.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(gridView1_CustomColumnDisplayText);

            this.btnAddNew.Enabled = Portal.gc.HasFunction("ItemDetail/Add");

            //关联回车键进行查询
            foreach (Control control in this.layoutControl1.Controls)
            {
                control.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SearchControl_KeyUp);
            }
        }

        /// <summary>
        /// 对显示的字段内容进行转义
        /// </summary>
        void gridView1_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.ColumnType == typeof(DateTime))
            {
                string columnName = e.Column.FieldName;
                if (e.Value != null && Convert.ToDateTime(e.Value) <= Convert.ToDateTime("1900-1-1"))
                {
                    e.DisplayText = "";
                }
            }
        }

        private void gridView1_DataSourceChanged(object sender, EventArgs e)
        {
            if (this.winGridViewPager1.gridView1.Columns.Count > 0 && this.winGridViewPager1.gridView1.RowCount > 0)
            {
                //统一设置100宽度
                foreach (DevExpress.XtraGrid.Columns.GridColumn column in this.winGridViewPager1.gridView1.Columns)
                {
                    column.Width = 100;
                }

                //可特殊设置特别的宽度
                this.winGridViewPager1.gridView1.Columns["Note"].Width = 200;
                this.winGridViewPager1.gridView1.Columns["ItemNo"].Width = 140;
                this.winGridViewPager1.gridView1.Columns["ItemBigType"].Width = 140;
                this.winGridViewPager1.gridView1.Columns["WareHouse"].Width = 140;
            }
        }

        /// <summary>
        /// 编写初始化窗体的实现，可以用于刷新
        /// </summary>
        public override void  FormOnLoad()
        {   
            BindData();
        }

        private void InitDictItem()
        {
            this.txtManufacture.Properties.Items.Clear();
            this.txtManufacture.Properties.Items.AddRange(DictItemUtil.GetDictByDictType("供货商"));

            this.txtBigType.Properties.Items.Clear();
            this.txtBigType.Properties.Items.AddRange(DictItemUtil.GetDictByDictType("备件属类"));

            this.txtItemType.Properties.Items.Clear();
            this.txtItemType.Properties.Items.AddRange(DictItemUtil.GetDictByDictType("备件类别"));

            this.txtSource.Properties.Items.Clear();
            this.txtSource.Properties.Items.AddRange(DictItemUtil.GetDictByDictType("来源"));

            //this.txtWareHouse.Properties.Items.Clear();
            //this.txtWareHouse.Properties.Items.AddRange(BLLFactory<WareHouse>.Instance.GetAllWareHouse().ToArray());

            this.txtDept.Properties.Items.Clear();
            this.txtDept.Properties.Items.AddRange(DictItemUtil.GetDictByDictType("部门"));
        }

        private void winGridViewPager1_OnRefresh(object sender, EventArgs e)
        {
            BindData();
        }

        private void winGridViewPager1_OnDeleteSelected(object sender, EventArgs e)
        {
            if (MessageDxUtil.ShowYesNoAndTips("您确定删除选定的记录么？") == DialogResult.No)
            {
                return;
            }

            int[] rowSelected = this.winGridViewPager1.GridView1.GetSelectedRows();
            foreach (int iRow in rowSelected)
            {
                //string ID = this.winGridViewPager1.GridView1.GetRowCellDisplayText(iRow, "ID");
                //BLLFactory<ItemDetail>.Instance.Delete(ID);
            }
            BindData();
        }

        private void winGridViewPager1_OnEditSelected(object sender, EventArgs e)
        {
            string ID = this.winGridViewPager1.gridView1.GetFocusedRowCellDisplayText("ID");
            List<string> IDList = new List<string>();
            for (int i = 0; i < this.winGridViewPager1.gridView1.RowCount; i++)
            {
                string strTemp = this.winGridViewPager1.GridView1.GetRowCellDisplayText(i, "ID");
                IDList.Add(strTemp);
            }

            if (!string.IsNullOrEmpty(ID))
            {
                FrmEditItemDetail dlg = new FrmEditItemDetail();
                dlg.ID = ID;
                dlg.IDList = IDList;
                if (DialogResult.OK == dlg.ShowDialog())
                {
                    BindData();
                }
            }
        }
        
        private void winGridViewPager1_OnAddNew(object sender, EventArgs e)
        {
            btnAddNew_Click(null, null);
        }

        private void winGridViewPager1_OnStartExport(object sender, EventArgs e)
        {
            string where = GetConditionSql();
            //this.winGridViewPager1.AllToExport = BLLFactory<ItemDetail>.Instance.FindToDataTable(where);
        }

        private void winGridViewPager1_OnPageChanged(object sender, EventArgs e)
        {
            BindData();
        }
                        
        /// <summary>
        /// 高级查询条件语句对象
        /// </summary>
        private SearchCondition advanceCondition;

        /// <summary>
        /// 根据查询条件构造查询语句
        /// </summary> 
        private string GetConditionSql()
        {
            //如果存在高级查询对象信息，则使用高级查询条件，否则使用主表条件查询
            SearchCondition condition = advanceCondition;
            if (condition == null)
            {
                condition = new SearchCondition();
                condition.AddCondition("ItemName", this.txtName.Text, SqlOperator.Like)
                    .AddCondition("ItemBigType", this.txtBigType.Text, SqlOperator.Like)
                    .AddCondition("ItemType", this.txtItemType.Text, SqlOperator.Like)
                    .AddCondition("Specification", this.cmbSpecNumber.Text, SqlOperator.Like)
                    .AddCondition("MapNo", this.txtMapNo.Text, SqlOperator.Like)
                    .AddCondition("Material", this.txtMaterial.Text, SqlOperator.Like)
                    .AddCondition("Source", this.txtSource.Text, SqlOperator.Like)
                    .AddCondition("Note", this.txtNote.Text, SqlOperator.Like)
                    .AddCondition("Manufacture", this.txtManufacture.Text, SqlOperator.Like)
                    .AddCondition("ItemNo", this.txtItemNo.Text, SqlOperator.LikeStartAt)
                    .AddCondition("WareHouse", this.txtWareHouse.Text, SqlOperator.Like)
                    .AddCondition("Dept", this.txtDept.Text, SqlOperator.Like)
                    .AddCondition("UsagePos", this.txtUsagePos.Text, SqlOperator.Like)
                    .AddCondition("StoragePos", this.txtStoragePos.Text, SqlOperator.Like);
            }
            string where = condition.BuildConditionSql().Replace("Where", "");
            return where;
        }

        /// <summary>
        /// 绑定数据到界面上的分页控件
        /// </summary>
        private void BindData()
        {
            this.winGridViewPager1.DisplayColumns = "ID,ItemNo,ItemName,Manufacture,MapNo,Specification,Material,ItemBigType,ItemType,Unit,Price,Source,StoragePos,UsagePos,StockQuantity,AlarmQuantity,Note,Dept,WareHouse";
            //this.winGridViewPager1.ColumnNameAlias = BLLFactory<ItemDetail>.Instance.GetColumnNameAlias();
            #region 添加别名解析

            this.winGridViewPager1.AddColumnAlias("ID", "编号");
            this.winGridViewPager1.AddColumnAlias("ItemNo", "项目编号");
            this.winGridViewPager1.AddColumnAlias("ItemName", "项目名称");
            this.winGridViewPager1.AddColumnAlias("Manufacture", "供货商");
            this.winGridViewPager1.AddColumnAlias("MapNo", "图号");
            this.winGridViewPager1.AddColumnAlias("Specification", "规格型号");
            this.winGridViewPager1.AddColumnAlias("Material", "材质");
            this.winGridViewPager1.AddColumnAlias("ItemBigType", "备件属类");
            this.winGridViewPager1.AddColumnAlias("ItemType", "备件类别");
            this.winGridViewPager1.AddColumnAlias("Unit", "单位");
            this.winGridViewPager1.AddColumnAlias("Price", "单价");
            this.winGridViewPager1.AddColumnAlias("Source", "来源");
            this.winGridViewPager1.AddColumnAlias("StoragePos", "库位");
            this.winGridViewPager1.AddColumnAlias("UsagePos", "使用位置");
            this.winGridViewPager1.AddColumnAlias("StockQuantity", "当前库存");
            this.winGridViewPager1.AddColumnAlias("WareHouse", "所属库房");
            this.winGridViewPager1.AddColumnAlias("Dept", "所属部门");
            this.winGridViewPager1.AddColumnAlias("Note", "备注");

            #endregion

            //string where = GetConditionSql();
            //List<ItemDetailInfo> list = BLLFactory<ItemDetail>.Instance.FindWithPager(where, this.winGridViewPager1.PagerInfo);
            string tableColumns = "ID|int,ItemNo,ItemName,StockQuantity|int,Manufacture,MapNo,Specification,Material,ItemBigType,ItemType,Unit,Price|decimal,Source,StoragePos,UsagePos,Note,WareHouse,Dept";
            DataTable dt = DataTableHelper.CreateTable(tableColumns);
            DataRow dr = null;
            //foreach (ItemDetailInfo info in list)
            //{
            //    dr = dt.NewRow();
            //    dr["ID"] = info.ID;
            //    dr["ItemBigType"] = info.ItemBigType;
            //    dr["ItemName"] = info.ItemName;
            //    dr["ItemNo"] = info.ItemNo;
            //    dr["ItemType"] = info.ItemType;
            //    dr["Manufacture"] = info.Manufacture;
            //    dr["MapNo"] = info.MapNo;
            //    dr["Material"] = info.Material;
            //    dr["Note"] = info.Note;
            //    dr["Price"] = info.Price;
            //    dr["Source"] = info.Source;
            //    dr["Specification"] = info.Specification;
            //    dr["StoragePos"] = info.StoragePos;
            //    dr["Unit"] = info.Unit;
            //    dr["UsagePos"] = info.UsagePos;
            //    dr["WareHouse"] = info.WareHouse;
            //    dr["Dept"] = info.Dept;

            //    StockInfo stockInfo = BLLFactory<Stock>.Instance.FindByItemNo(info.ItemNo);
            //    int quantity = 0;
            //    if (stockInfo != null)
            //    {
            //        quantity = stockInfo.StockQuantity;
            //    }
            //    dr["StockQuantity"] = quantity;
            //    dt.Rows.Add(dr);
            //}

            this.winGridViewPager1.DataSource = dt.DefaultView;//new WHC.Pager.WinControl.SortableBindingList<ItemDetailInfo>(list);
            this.winGridViewPager1.PrintTitle = Portal.gc.gAppUnit + " -- " + "备件信息报表";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            advanceCondition = null;//必须重置查询条件，否则可能会使用高级查询条件了
            BindData();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            FrmEditItemDetail dlg = new FrmEditItemDetail();
            if (DialogResult.OK == dlg.ShowDialog())
            {
                BindData();
            }
        }

        private void SearchControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSearch_Click(null, null);
            }
        }

        //private FrmAdvanceSearch dlg;
        private void btnAdvanceSearch_Click(object sender, EventArgs e)
        {
        //    if (dlg == null)
        //    {
        //        dlg = new FrmAdvanceSearch();
        //        dlg.FieldTypeTable = BLLFactory<ItemDetail>.Instance.GetFieldTypeList();
        //        dlg.DisplayColumns = "ID,ItemNo,ItemName,Manufacture,MapNo,Specification,Material,ItemBigType,ItemType,Unit,Price,Source,StoragePos,UsagePos,StockQuantity,AlarmQuantity,Note,Dept,WareHouse";
        //        dlg.ColumnNameAlias = BLLFactory<ItemDetail>.Instance.GetColumnNameAlias();//字段列显示名称转义

        //        #region 下拉列表数据

        //        dlg.AddColumnListItem("Manufacture", Portal.gc.GetDictData("供货商"));
        //        dlg.AddColumnListItem("ItemBigType", Portal.gc.GetDictData("备件属类"));
        //        dlg.AddColumnListItem("ItemType", Portal.gc.GetDictData("备件类别"));
        //        dlg.AddColumnListItem("Source", Portal.gc.GetDictData("来源"));
        //        dlg.AddColumnListItem("Dept", Portal.gc.GetDictData("部门"));

        //        dlg.AddColumnListItem("Material", BLLFactory<ItemDetail>.Instance.GetFieldList("Material"));
        //        dlg.AddColumnListItem("Specification", BLLFactory<ItemDetail>.Instance.GetFieldList("Specification"));
        //        dlg.AddColumnListItem("Unit", BLLFactory<ItemDetail>.Instance.GetFieldList("Unit"));
        //        dlg.AddColumnListItem("WareHouse", BLLFactory<WareHouse>.Instance.GetAllWareHouse());

        //        //dlg.AddColumnListItem("Sex", "男,女");

        //        #endregion

        //        dlg.ConditionChanged += new FrmAdvanceSearch.ConditionChangedEventHandler(dlg_ConditionChanged);
        //    }
        //    dlg.ShowDialog();
        }

        void dlg_ConditionChanged(SearchCondition condition)
        {
            advanceCondition = condition;
            BindData();
        }
    }
}
