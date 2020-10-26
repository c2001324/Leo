using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;


namespace Untility
{
    /// <summary>
    /// 读取ini文件
    /// #为注释符号
    /// </summary>
    public class IniReader
    {
        class Session
        {
            public string name;
            public Dictionary<string, List<string>> m_Data;
            public Session(string strName)
            {
                name = strName;
                m_Data = new Dictionary<string, List<string>>();
            }

            public void AddValue(string key, string value)
            {
                if (key != "")
                {
                    key = key.ToLower();
                    if (key[0] == '+')
                    {
                        key = key.Substring(1);
                        if (!m_Data.ContainsKey(key))
                        {
                            m_Data.Add(key, new List<string>());
                        }

                        m_Data[key].Add(value);
                    }
                    else
                    {
                        if (m_Data.ContainsKey(key))
                        {
                            m_Data[key].Clear();
                        }
                        else
                        {
                            m_Data.Add(key, new List<string>());
                        }
                        m_Data[key].Add(value);
                    }
                }
            }

            public void RemoveValue(string key)
            {
                if (m_Data.ContainsKey(key))
                {
                    m_Data.Remove(key);
                }
            }

            private string _getValue(string key, string defValue = "")
            {
                key = key.ToLower();
                if (m_Data.ContainsKey(key) && m_Data[key].Count > 0)
                {
                    return m_Data[key][0];
                }
                else
                {
                    return defValue;
                }
            }

            private string[] _getValues(string key, string defValue = "")
            {
                key = key.ToLower();
                if (m_Data.ContainsKey(key))
                {
                    return m_Data[key].ToArray();
                }
                else
                {
                    return new string[] { defValue };
                }
            }

            public string GetValueString(string key, string defValue = "")
            {
                return _getValue(key, defValue);
            }

            public string[] GetValuesString(string key, string defValue = "")
            {
                return _getValues(key, defValue);
            }

            public int GetValueInt(string key, int defValue = 0)
            {
                string v = _getValue(key);
                try
                {
                    return int.Parse(v);
                }
                catch
                {
                    return defValue;
                }
            }

            public int[] GetValuesInt(string key, int defValue = 0)
            {
                List<int> list = new List<int>();
                int value = 0;
                foreach (string str in _getValues(key))
                {
                    if (int.TryParse(str, out value))
                    {
                        list.Add(value);
                    }
                    else
                    {
                        list.Add(defValue);
                    }
                }
                return list.ToArray();
            }

            public float GetValueFloat(string key, float defValue = 0f)
            {
                string v = _getValue(key);
                try
                {
                    return float.Parse(v);
                }
                catch
                {
                    return defValue;
                }
            }

            public float[] GetValuesFloat(string key, float defValue = 0f)
            {
                List<float> list = new List<float>();
                float value = 0;
                foreach (string str in _getValues(key))
                {
                    if (float.TryParse(str, out value))
                    {
                        list.Add(value);
                    }
                    else
                    {
                        list.Add(defValue);
                    }
                }
                return list.ToArray();
            }


        }

        private string m_defSession = "default";
        private StreamReader m_StreamReader;
        private Dictionary<string, Session> m_Data;

        public static IniReader Create(string strPath)
        {
            try
            {
                return new IniReader(strPath);                
            }
            catch (Exception e)
            {
                Debug.LogError("读取文件" + strPath + "失败。\n" + e.Message);
                return null;
            }
        }

        IniReader(string strPath)
        {
            try
            {
                strPath = strPath.Replace(".ini", "");
                TextAsset obj = Resources.Load<TextAsset>(strPath);
                Stream s = new MemoryStream(obj.bytes);
                m_StreamReader = new StreamReader(s, Encoding.UTF8);
                _initialize();
            }
            catch (Exception e)
            {
                m_StreamReader = null;
                throw (e);
            }
        }

        public bool CanRead()
        {
            if (m_StreamReader == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">健</param>
        /// <param name="def">值</param>
        /// <param name="session">Session</param>
        /// <returns>返回值</returns>
        public string GetValueString(string session, string key, string def = "")
        {
            if (m_StreamReader == null)
            {
                return def;
            }
            else
            {
                if (session == "")
                {
                    session = m_defSession;
                }
                Session se = _getSession(session);
                if (se == null)
                {
                    return def;
                }
                else
                {
                    return se.GetValueString(key, def);
                }
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">健</param>
        /// <param name="def">值</param>
        /// <param name="session">Session</param>
        /// <returns>返回值</returns>
        public string[] GetValuesString(string session, string key, string def = "")
        {
            if (m_StreamReader == null)
            {
                return new string[] { def };
            }
            else
            {
                if (session == "")
                {
                    session = m_defSession;
                }
                Session se = _getSession(session);
                if (se == null)
                {
                    return new string[] { def };
                }
                else
                {
                    return se.GetValuesString(key, def);
                }
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">健</param>
        /// <param name="def">值</param>
        /// <param name="session">Session</param>
        /// <returns>返回值</returns>
        public int GetValueInt(string session, string key, int def = 0)
        {
            if (m_StreamReader == null)
            {
                return def;
            }
            else
            {
                if (session == "")
                {
                    session = m_defSession;
                }
                Session se = _getSession(session);
                if (se == null)
                {
                    return def;
                }
                else
                {
                    return se.GetValueInt(key, def);
                }
            }
        }


        public int[] GetValuesInt(string session, string key, int def = 0)
        {
            if (m_StreamReader == null)
            {
                return new int[] { def };
            }
            else
            {
                if (session == "")
                {
                    session = m_defSession;
                }
                Session se = _getSession(session);
                if (se == null)
                {
                    return new int[] { def };
                }
                else
                {
                    return se.GetValuesInt(key, def);
                }
            }
        }

        /// <summary>
        /// 获取配置值
        /// </summary>
        /// <param name="key">健</param>
        /// <param name="def">值</param>
        /// <param name="session">Session</param>
        /// <returns>返回值</returns>
        public float GetValueFloat(string session, string key, float def = 0f)
        {
            if (m_StreamReader == null)
            {
                return def;
            }
            else
            {
                if (session == "")
                {
                    session = m_defSession;
                }
                Session se = _getSession(session);
                if (se == null)
                {
                    return def;
                }
                else
                {
                    return se.GetValueFloat(key, def);
                }
            }
        }


        public float[] GetValuesFloat(string session, string key, float def = 0f)
        {
            if (m_StreamReader == null)
            {
                return new float[] { def };
            }
            else
            {
                if (session == "")
                {
                    session = m_defSession;
                }
                Session se = _getSession(session);
                if (se == null)
                {
                    return new float[] { def };
                }
                else
                {
                    return se.GetValuesFloat(key, def);
                }
            }
        }

        private void _initialize()
        {
            m_Data = new Dictionary<string, Session>();
            //解析文件
            Session curSession = new Session(m_defSession);//当前Session
            Session defSession = curSession;//默认Session
            m_Data.Add(m_defSession, defSession);

            int nLine = 0;
            while (!m_StreamReader.EndOfStream)
            {
                string strLine = m_StreamReader.ReadLine();
                nLine++;
                strLine = strLine.Replace(" ", "");
                if (!_isCanRead(strLine))
                {
                    continue;
                }

                if (_isSession(strLine))
                {
                    curSession = _getSessionOrCreate(_getSessionName(strLine));
                }
                else
                {
                    string[] data = _getKeyAndValue(strLine);
                    if (data == null)
                    {
                        throw (new Exception("配置文件出错：第" + nLine + "行"));
                    }
                    else
                    {
                        curSession.AddValue(data[0], data[1]);
                    }
                }
            }
        }

        private bool _isCanRead(string str)
        {
            if (str == null || str == "" || str[0] == '#' || str.IndexOf("//") == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private Session _getSession(string name)
        {
            if (m_Data.ContainsKey(name))
            {
                return m_Data[name];
            }
            else
            {
                return null;
            }
        }

        private Session _getSessionOrCreate(string name)
        {
            if (m_Data.ContainsKey(name))
            {
                return m_Data[name];
            }
            else
            {
                Session newSession = new Session(name);
                m_Data.Add(name, newSession);
                return newSession;
            }
        }

        private string _getSessionName(string str)
        {
            return str.Substring(1, str.Length - 2);
        }

        private bool _isSession(string str)
        {
            int begin = str.IndexOf('[');
            int end = str.IndexOf(']');
            if (begin == 0 && end == str.Length - 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string[] _getKeyAndValue(string str)
        {
            //处理小括号
            int index = str.IndexOf("=(");
            if (index >= 0)
            {
                List<string> data = new List<string>();
                data.Add(str.Substring(0, index));
                index += 2;
                data.Add(str.Substring(index, str.Length - index - 1));
                return data.ToArray();
            }
            else
            {
                string[] data = str.Split(new char[] { '=' });
                if (data.Length != 2)
                {
                    return null;
                }
                else
                {
                    return data;
                }
            }
        }

        /// <summary>
        /// 提取两个字符间的字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="beginMark"></param>
        /// <param name="endMark"></param>
        /// <returns></returns>
        public static string ParseParamString(string str, char beginMark = '(', char endMark = ')')
        {
            string newStr = str;
            if (GetCharCount(str, beginMark) == GetCharCount(str, endMark))
            {
                if (str[0] == beginMark && str[str.Length - 1] == endMark)
                {
                    newStr = str.Substring(1);
                    newStr = newStr.Substring(0, newStr.Length - 1);
                }
            }

            return newStr;
        }

        /// <summary>
        /// 从字符串解析为Key，Value
        /// 例如：monster=1000会解析为Key(monster)，Value(1000)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static KeyValuePair<string, string> SplitParamStringToKeyAndValue(string param, char split = '=')
        {
            int index = param.IndexOf(split);
            if (index > 0)
            {
                return new KeyValuePair<string, string>(param.Substring(0, index), param.Substring(index + 1));
            }
            else
            {
                return new KeyValuePair<string, string>();
            }
        }

        /// <summary>
        /// 获取字符串中某个字符是数量
        /// </summary>
        /// <param name="str"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static int GetCharCount(string str, char c)
        {
            return str.Split(c).Length - 1;
        }

        /// <summary>
        /// 分割字符串，Id=1000, Atk=(A=10, B=20), Def=(A=10, B=20)
        /// </summary>
        /// <param name="param"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        static public List<string> SplitParamString(string param, char split = ',')
        {
            //删除空格
            param = param.Replace(" ", "");
            //删除两边的小括号
            param = ParseParamString(param);
            List<string> list = new List<string>();

            //解析内容
            int index = 0;
            int startIndex = 0;
            do
            {
                index = param.IndexOf(split, startIndex);
                if (index >= 0)
                {
                    string begin = param.Substring(0, index);
                    string end = param.Substring(index + 1);
                    int count1 = GetCharCount(begin, '(');
                    int count2 = GetCharCount(begin, ')');
                    if (count1 == count2)
                    {
                        list.Add(begin);
                        param = param.Substring(index + 1);
                        startIndex = 0;
                    }
                    else
                    {
                        startIndex = index + 1;
                    }
                }
                else
                {
                    list.Add(param);
                }
            }
            while (index >= 0);

            return list;
        }
    }
}
