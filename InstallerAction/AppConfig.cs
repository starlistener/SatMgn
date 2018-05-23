using System.Collections;
using System.Data;
using System.Xml;

namespace InstallerAction
{
	/// <summary>
	/// ���ڻ�ȡ������Web.config/*.exe.config�нڵ����ݵĸ�����
	/// </summary>
	public sealed class AppConfig
	{
        private AppConfig() { }

        /// <summary>
        /// ���ó����config�ļ�
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
        /// <param name="keyValue">��ֵ</param>
        public static void AppConfigSet(string filePath, string keyName, string keyValue)
        {
            //���ڴ��ڶ��Add��ֵ��ʹ�÷���appSetting�Ĳ������ɹ�����ע��������䣬�����µķ�ʽ
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
                //��ý���ǰԪ�ص�key����
                XmlAttribute att = nodes[i].Attributes["key"];
                //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
                if (att != null && (att.Value == keyName))
                {
                    att = nodes[i].Attributes["value"];
                    //��Ŀ��Ԫ���еĵڶ������Ը�ֵ
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
        /// ��ȡ�����config�ļ�
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
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
                    //��ý���ǰԪ�ص�key����
                    XmlAttribute att = nodes[i].Attributes["key"];
                    //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
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
        /// ���ó����config�ļ���Enterprise Library�����ݿ����ӵ�ַ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
        /// <param name="keyValue">��ֵ</param>
        public static void SetConnectionString(string filePath, string keyName, string keyValue)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            XmlNodeList nodes = document.GetElementsByTagName("add");
            for (int i = 0; i < nodes.Count; i++)
            {
                //��ý���ǰԪ�ص�name����
                XmlAttribute att = nodes[i].Attributes["name"];
                //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
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
        /// ��ȡ�����config�ļ�Enterprise Library�����ݿ����ӵ�ַ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
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
                    //��ý���ǰԪ�ص�key����
                    XmlAttribute att = nodes[i].Attributes["name"];
                    //����Ԫ�صĵ�һ���������жϵ�ǰ��Ԫ���ǲ���Ŀ��Ԫ��
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
        /// ���ó����config�ļ���Elmah�����ݿ����ӵ�ַ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
        /// <param name="keyValue">��ֵ</param>
        public static void SetElmahConnectionString(string filePath, string keyValue)
        {
            XmlDocument document = new XmlDocument();
            document.Load(filePath);

            XmlNodeList nodes = document.GetElementsByTagName("errorLog");
            for (int i = 0; i < nodes.Count; i++)
            {
                //��ý���ǰԪ�ص�name����
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
        /// ��ȡ�����config�ļ�Elmah�����ݿ����ӵ�ַ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
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
                    //��ý���ǰԪ�ص�connectionString����
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
        /// ��ȡָ�������е������ֵ
        /// </summary>
        /// <param name="filePath">Web.config����*.exe.config�ļ��ľ���·��</param>
        /// <param name="keyName">����</param>
        /// <param name="subKeyName">�Էֺ�(;)Ϊ�ָ�������������</param>
        /// <returns>��Ӧ�������Ƶ�ֵ������=�ź����ֵ��</returns>
        public static string GetSubValue(string filePath, string keyName, string subKeyName)
        {
            string connectionString = AppConfigGet(filePath, keyName).ToLower();
            string[] item = connectionString.Split(new char[] { ';' });

            for (int i = 0; i < item.Length; i++)
            {
                string itemValue = item[i].ToLower();
                if (itemValue.IndexOf(subKeyName.ToLower()) >= 0) //�������ָ���Ĺؼ���
                {
                    int startIndex = item[i].IndexOf("="); //�Ⱥſ�ʼ��λ��
                    return item[i].Substring(startIndex + 1); //��ȡ�Ⱥź����ֵ��ΪValue
                }
            }
            return string.Empty;
        }
	}
}