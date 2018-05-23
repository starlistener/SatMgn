using System.Collections;
using System.Data;
using System.Xml;

namespace InstallerAction
{
	/// <summary>
	/// 用于获取或设置Web.config/*.exe.config中节点数据的辅助类
	/// </summary>
	public sealed class AppConfig
	{
        private AppConfig() { }

        /// <summary>
        /// 设置程序的config文件
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <param name="keyValue">键值</param>
        public static void AppConfigSet(string filePath, string keyName, string keyValue)
        {
            //由于存在多个Add键值，使得访问appSetting的操作不成功，故注释下面语句，改用新的方式
            /* 
            string xpath = "//add[@key='" + keyName + "']";
			XmlDocument document = new XmlDocument();
			document.Load(filePath);

			XmlNode node = document.SelectSingleNode(xpath);
			node.Attributes["value"].Value = keyValue;
			document.Save(filePath); 
             */

            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            XmlNodeList nodes = document.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的key属性
                XmlAttribute att = nodes[i].Attributes["key"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                if (att != null && (att.Value == keyName))
                {
                    att = nodes[i].Attributes["value"];
                    //对目标元素中的第二个属性赋值
                    if (att != null)
                    {
                        att.Value = keyValue;
                        break;
                    }
                }
            }
            document.Save(filePath);
        }

        /// <summary>
        /// 读取程序的config文件
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <returns></returns>
        public static string AppConfigGet(string filePath, string keyName)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(filePath);

                XmlNodeList nodes = document.GetElementsByTagName("add");
                for (int i = 0; i < nodes.Count; i++)
                {
                    //获得将当前元素的key属性
                    XmlAttribute att = nodes[i].Attributes["key"];
                    //根据元素的第一个属性来判断当前的元素是不是目标元素
                    if (att != null && (att.Value == keyName))
                    {
                        att = nodes[i].Attributes["value"];
                        if (att != null)
                        {
                            strReturn = att.Value;
                            break;
                        }
                    }
                }
            }
            catch
            { ; }

            return strReturn;
        }

        /// <summary>
        /// 设置程序的config文件的Enterprise Library的数据库链接地址
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <param name="keyValue">键值</param>
        public static void SetConnectionString(string filePath, string keyName, string keyValue)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            XmlNodeList nodes = document.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的name属性
                XmlAttribute att = nodes[i].Attributes["name"];
                //根据元素的第一个属性来判断当前的元素是不是目标元素
                if (att != null && (att.Value == keyName))
                {
                    att = nodes[i].Attributes["connectionString"];
                    if (att != null)
                    {
                        att.Value = keyValue;
                        break;
                    }
                }
            }
            document.Save(filePath);
        }

        /// <summary>
        /// 读取程序的config文件Enterprise Library的数据库链接地址
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <returns></returns>
        public static string GetConnectionString(string filePath, string keyName)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(filePath);

                XmlNodeList nodes = document.GetElementsByTagName("add");
                for (int i = 0; i < nodes.Count; i++)
                {
                    //获得将当前元素的key属性
                    XmlAttribute att = nodes[i].Attributes["name"];
                    //根据元素的第一个属性来判断当前的元素是不是目标元素
                    if (att != null && (att.Value == keyName))
                    {
                        att = nodes[i].Attributes["connectionString"];
                        if (att != null)
                        {
                            strReturn = att.Value;
                            break;
                        }
                    }
                }
            }
            catch
            { ; }

            return strReturn;
        }

        /// <summary>
        /// 设置程序的config文件的Elmah的数据库链接地址
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <param name="keyValue">键值</param>
        public static void SetElmahConnectionString(string filePath, string keyValue)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            XmlNodeList nodes = document.GetElementsByTagName("errorLog");
            for (int i = 0; i < nodes.Count; i++)
            {
                //获得将当前元素的name属性
                XmlAttribute att = nodes[i].Attributes["connectionString"];
                if (att != null)
                {
                    att.Value = keyValue;
                    break;
                }
            }
            document.Save(filePath);
        }

        /// <summary>
        /// 读取程序的config文件Elmah的数据库链接地址
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <returns></returns>
        public static string GetElmahConnectionString(string filePath)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument document = new XmlDocument();
                document.Load(filePath);

                XmlNodeList nodes = document.GetElementsByTagName("errorLog");
                for (int i = 0; i < nodes.Count; i++)
                {
                    //获得将当前元素的connectionString属性
                    XmlAttribute att = nodes[i].Attributes["connectionString"];
                    if (att != null)
                    {
                        strReturn = att.Value;
                        break;
                    }
                }
            }
            catch
            { ; }

            return strReturn;
        }

        /// <summary>
        /// 获取指定键名中的子项的值
        /// </summary>
        /// <param name="filePath">Web.config或者*.exe.config文件的绝对路径</param>
        /// <param name="keyName">键名</param>
        /// <param name="subKeyName">以分号(;)为分隔符的子项名称</param>
        /// <returns>对应子项名称的值（即是=号后面的值）</returns>
        public static string GetSubValue(string filePath, string keyName, string subKeyName)
        {
            string connectionString = AppConfigGet(filePath, keyName).ToLower();
            string[] item = connectionString.Split(new char[] { ';' });

            for (int i = 0; i < item.Length; i++)
            {
                string itemValue = item[i].ToLower();
                if (itemValue.IndexOf(subKeyName.ToLower()) >= 0) //如果含有指定的关键字
                {
                    int startIndex = item[i].IndexOf("="); //等号开始的位置
                    return item[i].Substring(startIndex + 1); //获取等号后面的值即为Value
                }
            }
            return string.Empty;
        }
	}
}