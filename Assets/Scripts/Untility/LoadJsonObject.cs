using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Untility
{
    public class LoadJsonObject
    {
        /// <summary>
        /// 加载游戏外的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        static public T CreateObjectFromStreamingAssets<T>(string path)
        {
            try
            {
                FileStream f = new FileStream(path, FileMode.Open);
                StreamReader reader = new StreamReader(f);

                //反序列化后的对象
                object deserializedObject = null;
                //使用Json.Net进行反序列化
                deserializedObject = JsonConvert.DeserializeObject(reader.ReadToEnd(), typeof(T));
                reader.Close();
                f.Close();
                return (T)deserializedObject;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError(e.Message + "\n" + e.StackTrace);
                return default(T);
            }
        }

        /// <summary>
        /// 加载游戏内的资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        static public T CreateObjectFromResource<T>(string path)
        {
            TextAsset obj = Resources.Load<TextAsset>(path);
            if (obj == null)
            {
                Debug.LogError("加载json文件：" + path + "失败");
                return default(T);
            }
            else
            {
                Stream s = new MemoryStream(obj.bytes);
                StreamReader sr = new StreamReader(s, Encoding.UTF8);

                try
                {
                    //反序列化后的对象
                    object deserializedObject = null;
                    //使用Json.Net进行反序列化
                    deserializedObject = JsonConvert.DeserializeObject(sr.ReadToEnd(), typeof(T));
                    sr.Close();
                    s.Close();
                    return (T)deserializedObject;
                }
                catch (Exception e)
                {
                    Debug.LogError("加载json文件：" + path + "失败\n" + e.Message);
                    return default(T);
                }
            }
        }

        static public void WriteJsonFromObject(object obj, string path)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            //setting.DefaultValueHandling = DefaultValueHandling.
            string str = JsonConvert.SerializeObject(obj, Formatting.Indented, setting);
            StreamWriter s = new StreamWriter(path);
            s.Write(str);
            s.Flush();
            s.Close();
        }
    }
}
