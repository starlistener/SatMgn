using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DevExpress.XtraEditors;
using WHC.Dictionary.BLL;
using WHC.Framework.Commons;
using WHC.Framework.ControlUtil;

namespace WHC.WareHouseMis.UI
{
    /// <summary>
    /// 扩展函数封装
    /// </summary>
    public static class ExtensionMethod
    {        
        #region ComboBoxEdit控件

        /// <summary>
        /// 绑定下拉列表控件为指定的数据字典列表
        /// </summary>
        /// <param name="combo">下拉列表控件</param>
        /// <param name="dictTypeName">数据字典类型名称</param>
        public static void BindDictItems(this ComboBoxEdit combo, string dictTypeName)
        {
            combo.Properties.Items.Clear();
            Dictionary<string, string> dict = BLLFactory<DictData>.Instance.GetDictByDictType(dictTypeName);
            foreach (string key in dict.Keys)
            {
                combo.Properties.Items.Add(new CListItem(key, dict[key]));
            }
        } 

        #endregion

    }
}
