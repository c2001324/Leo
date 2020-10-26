using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Untility
{
    public class Tool
    {
        #region 随机
        /// <summary>
        /// 获取随机的结果
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static bool GetRating(float rate, System.Random random = null)
        {
            if (rate >= 1)
            {
                return true;
            }
            else
            {
                float seek = 0f;
                if (random != null)
                {
                    seek = random.Range(0f, -1f);
                }
                else
                {
                    seek = UnityEngine.Random.Range(0f, 1f);
                }
                rate = rate > 0 ? rate : -rate;
                return rate >= seek;
            }
        }

        /// <summary>
        /// 获取随机的结果
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public static bool GetRating(int rate, int maxRate = 100)
        {
            return rate > UnityEngine.Random.Range(0, maxRate);
        }

        /// <summary>
        /// 获取随机数，不包括max最大值
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandom(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float GetRandom(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 获取随机数，不包括max最大值
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandom(int min, int max, System.Random random)
        {
            return random.Range(min, max -1);
        }

        public static float GetRandom(float min, float max, System.Random random)
        {
            return random.Range(min, max);
        }


        /// <summary>
        /// 随机排列列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void RandSortList<T>(ref List<T> list)
        {
            if (list != null && list.Count > 1)
            {
                RandSortList(ref list, new System.Random());
            }
        }

        /// <summary>
        /// 随机排列列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void RandSortList<T>(ref List<T> list, System.Random random)
        {
            if (list != null && list.Count > 1)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    int index = random.Next(0, list.Count - 1);
                    if (i != index)
                    {
                        T temp = list[i];
                        list[i] = list[index];
                        list[index] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// 在数组中随机一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomInList<T>(List<T> list)
        {
            if (list != null && list.Count > 0)
            {
                if (list.Count == 1)
                {
                    return list[0];
                }
                else
                {
                    return list[GetRandom(0, list.Count)];
                }
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 在数组中随机一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomInList<T>(List<T> list, System.Random random)
        {
            if (list != null && list.Count > 0)
            {
                if (list.Count == 1)
                {
                    return list[0];
                }
                else
                {
                    return list[random.Range(0, list.Count)];
                }
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 在数组中随机一个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T GetRandomInArray<T>(T[] list)
        {
            if (list != null && list.Length > 0)
            {
                if (list.Length == 1)
                {
                    return list[0];
                }
                else
                {
                    return list[GetRandom(0, list.Length)];
                }
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 生成随机排列的一个整数数组
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static List<int> CreateRandomList(int begin, int end)
        {
            int count = end - begin + 1;
            List<int> list = new List<int>(count);
            for(int i = 0; i < count; i++)
            {
                list.Add(begin + i);
            }
            RandSortList<int>(ref list);
            return list;
        }

        public static T RandomValueByWeights<T>(List<JWeight<T>> datas, System.Random random)
        {
            return RandomValueByWeights<T>(datas.ToArray(), random);
        }

        public static T RandomValueByWeights<T>(JWeight<T>[] datas, System.Random random)
        {
            int[] weights = new int[datas.Length];
            for (int i =0; i < datas.Length; i++)
            {
                weights[i] = datas[i].weight;
            }
            int index = RandomIndexByWeight(weights, random);
            if (index >= 0)
            {
                return datas[index].value;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// 根据权重列表随机一个对应的项
        /// </summary>
        /// <param name="weightList"></param>
        /// <returns></returns>
        public static int RandomIndexByWeight(List<int> weightList, System.Random random)
        {
            return RandomIndexByWeight(weightList.ToArray(), random);
        }

        /// <summary>
        /// 根据权重列表随机一个对应的项
        /// </summary>
        /// <param name="weightList"></param>
        /// <returns></returns>
        public static int RandomIndexByWeight(int[] weightList, System.Random random)
        {
            if (weightList.Length == 1)
            {
                return 0;
            }

            int totalWeight = 0;
            for (int i = 0; i < weightList.Length; i++)
            {
                if (weightList[i] < 0)
                {
                    return i;
                }
                else
                {
                    totalWeight += weightList[i];
                }
            }

            int seek = random.Range(1, totalWeight);
            int head = 0;
            for (int i = 0; i < weightList.Length; i++)
            {
                if (seek > head && seek <= head + weightList[i])
                {
                    if (weightList[i] == 0)
                    {
                        if (i == 0)
                        {
                            continue;
                        }
                        else
                        {
                            return i - 1;
                        }
                    }
                    else
                    {
                        return i;
                    }
                }
                else
                {
                    head += weightList[i];
                }
            }

            return -1;
        }
        #endregion

        #region 数学
        public static float Round(float value, int digit)
        {
            float vt = Mathf.Pow(10, digit);
            //1.乘以倍数 + 0.5
            float vx = value * vt + 0.5f;
            //2.向下取整
            float temp = Mathf.Floor(vx);
            //3.再除以倍数
            return (temp / vt);
        }

        /// <summary>
        /// 精确到小数后n位
        /// </summary>
        /// <param name="a"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static float SplitAndRound(float a, int n)
        {
            a = a * Mathf.Pow(10, n);
            return (Mathf.Round(a)) / (Mathf.Pow(10, n));
        }

        /// <summary>
        /// 向上取整
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetIntStringByMax(float value)
        {
            return Mathf.CeilToInt(value).ToString();
        }

        public static int GetIntByMax(float value)
        {
            return Mathf.CeilToInt(value);
        }

        public static int HpFloat2Int(float hp)
        {
            return GetIntByMax(hp);
        }

        public static int PropertyFloat2Int(float property)
        {
            return GetIntByMin(property);
        }

        /// <summary>
        /// 向下取整，最小数值为1
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetIntStringByMin(float value)
        {
            return GetIntByMin(value).ToString();
        }

        public static int GetIntByMin(float value)
        {
            if (Mathf.Abs(value) < 1 && value != 0)
            {
                return 1;
            }
            else
            {
                return (int)value;
            }
        }



        static float GetAngle(Vector3 forward, Vector3 position, Vector3 targetPosition)
        {
            Vector3 selfPos = new Vector3(position.x, 0f, position.z);
            Vector3 targetPos = new Vector3(targetPosition.x, 0f, targetPosition.z);
            Vector3 otherVec = targetPos - selfPos;
            otherVec.Normalize();
            return Vector3.Angle(forward, otherVec);
        }

        /// <summary>
        /// 属性数值精确到四位
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float PropertyRound(float value)
        {
            return (float)System.Math.Round(value, 4);
        }

        /// <summary>
        /// 获取圆形的布局点
        /// </summary>
        /// <param name="count">点的总数量</param>
        /// <param name="clearance">间隔</param>
        /// <param name="sequence">是否按照顺序生成</param>
        /// <param name="beginIndex">开始的索引，一般情况下为0</param>
        /// <returns></returns>
        public static void GetCirclePointsByCount(float clearance, bool sequence, int beginIndex, ref Vector3[] points)
        {
            points[0] = Vector3.zero;
            float radius = clearance;
            int endIndex = points.Length + beginIndex;
            for (int index = 1; index < endIndex;)
            {
                float totalLenght = Mathf.PI * radius * 2f;
                int step = (int)(totalLenght / clearance);
                float angleStep = 360f / step;
                float angle = 0f;

                if (sequence)
                {
                    //按照顺序生成点
                    while (angle <= 360f)
                    {
                        if (index >= beginIndex)
                        {
                            float hudu = (angle / 180f) * Mathf.PI;
                            float x = radius * Mathf.Cos(hudu);
                            float z = radius * Mathf.Sin(hudu);
                            points[index - beginIndex] = new Vector3(x, 0f, z);
                        }
                        
                        angle += angleStep;
                        index++;
                        if (index >= endIndex)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    //随机生成点

                    List<int> angleIndexList = CreateRandomList(1, step);
                    foreach (int angleIndex in angleIndexList)
                    {
                        angle = angleIndex * angleStep;
                        if (index >= beginIndex)
                        {
                            float hudu = (angle / 180f) * Mathf.PI;
                            float x = radius * Mathf.Cos(hudu);
                            float z = radius * Mathf.Sin(hudu);
                            points[index - beginIndex] = new Vector3(x, 0f, z);
                        }
                        index++;
                        if (index >= endIndex)
                        {
                            break;
                        }
                    }
                }

                radius += clearance;
            }
        }

        /// <summary>
        /// 获取圆形的布局点， 这个存在GC的问题，日后需要优化
        /// </summary>
        /// <param name="count">点的总数量</param>
        /// <param name="clearance">间隔</param>
        /// <param name="sequence">是否按照顺序生成</param>
        /// <returns></returns>
        public static Vector3[] GetCirclePointsByRange(float range, float clearance, bool sequence)
        {
            List<Vector3> list = new List<Vector3>(100) { Vector3.zero };
            float raduis = clearance;
            while (raduis <= range)
            {
                float totalLenght = Mathf.PI * raduis * 2f;
                int step = (int)(totalLenght / clearance);
                float angleStep = 360f / step;
                float angle = 0f;

                if (sequence)
                {
                    //按照顺序生成点
                    while (angle <= 360f)
                    {
                        float hudu = (angle / 180f) * Mathf.PI;
                        float x = raduis * Mathf.Cos(hudu);
                        float z = raduis * Mathf.Sin(hudu);
                        list.Add(new Vector3(x, 0f, z));
                        angle += angleStep;
                    }
                }
                else
                {
                    //随机生成点
                    List<int> angleIndexList = CreateRandomList(0, step - 1);
                    foreach (int angleIndex in angleIndexList)
                    {
                        angle = angleIndex * angleStep;
                        float hudu = (angle / 180f) * Mathf.PI;
                        float x = raduis * Mathf.Cos(hudu);
                        float z = raduis * Mathf.Sin(hudu);
                        list.Add(new Vector3(x, 0f, z));
                        angle += angleStep;
                    }
                }

                raduis += clearance;
            }
            return list.ToArray();
        }

        static public bool RectIntOverlap(RectInt r1, RectInt r2)
        {
            return Math.Abs((r1.xMin + r1.xMax) - (r2.xMin + r2.xMax)) < (Math.Abs(r1.xMax + r2.xMax - r1.xMin - r2.xMin))
                && Math.Abs((r1.yMin + r1.yMax) - (r2.yMin + r2.yMax)) < (Math.Abs(r1.yMax + r2.yMax - r1.yMin - r2.yMin));
        }
        #endregion

        #region 数值转换为字符
        /// <summary>
        /// 获取百分比的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FloatToPercentString(float value, int round = 0)
        {
            value = value > 0 ? value : -value;
            if (round == 0)
            {
                return string.Format("{0:0}%", value * 100);
            }
            else
            {
                value = SplitAndRound(value, round + 2) * 100;
                return value.ToString(string.Format("f{0}", round)) + "%";
            }            
        }

        public static string FloatToString(float value, int round = 0)
        {
            value = value > 0 ? value : -value;
            return value.ToString(string.Format("f{0}", round));
        }

        public static string GetItemCountString(int count)
        {
            if (count > 999999)
            {
                float value = count / 1000000f;
                value = (float)System.Math.Round(value, 1);
                return value + "M";
            }
            else if (count > 9999)
            {
                float value = count / 1000f;
                value = (float)System.Math.Round(value, 1);
                return value + "K";
            }
            else
            {
                return count.ToString();
            }
        }

        static public string GetChineseNumber(int num)
        {
            int hundred = num / 100;
            num = num % 100;

            int ten = num / 10;
            int one = num % 10;



            string str = "";
            if (hundred > 0)
            {
                str = GetNumber(hundred) + GetNumber(100);
            }

            if (ten > 0)
            {
                if (ten == 1)
                {
                    str += GetNumber(10);
                }
                else
                {
                    str += GetNumber(ten) + GetNumber(10);
                }
            }

            if (one > 0)
            {
                str += GetNumber(one);
            }
            return str;
        }

        /// <summary>
        /// 解析描述字符串
        /// </summary>
        /// <param name="describe"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        static public string ParseDescribe(string describe, params string[] param)
        {
            string desc = describe;
            int index = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < param.Length; i++)
            {
                string str = param[i];
                index++;
                builder.Remove(0, builder.Length);
                builder.AppendFormat("<param{0}>", index);
                string paramStr = builder.ToString();
                int beginPos = desc.IndexOf(paramStr);
                if (beginPos >= 0)
                {
                    if (StringIsNum(str))
                    {
                        //技能的字段统一颜色
                        desc = desc.Replace(paramStr, TextColor.paramColorHead + str + TextColor.colorEnd);
                    }
                    else
                    {
                        desc = desc.Replace(paramStr, str);
                    }
                }
            }
            return desc;
        }

        /// <summary>
        /// 解析描述字符串
        /// </summary>
        static public string ParseDescribe(string describe, ConfigDictParam configParam)
        {
            if (describe == null || describe == "")
            {
                return "";
            }
            string tempDescribe = describe;

            Dictionary<string, string> paramNameList = new Dictionary<string, string>();

            int endIndex;
            string paramFullName;
            string paramKey;
            while (TryGetDescribeParamName(tempDescribe, out paramKey, out paramFullName, out endIndex))
            {
                tempDescribe = tempDescribe.Remove(0, endIndex + 1);
                paramNameList.Add(paramKey, paramFullName);
            }

            string resultDescribe = describe;
            var e = paramNameList.GetEnumerator();
            while (e.MoveNext())
            {
                string key = e.Current.Key;
                string fullName = e.Current.Value;
                string value = configParam.GetParamToString(key);
                if (value == "" || value == null)
                {
                    value = fullName;
                }
                if (IsNumeric(value))
                {
                    //技能的字段统一颜色
                    resultDescribe = resultDescribe.Replace(fullName, TextColor.paramColorHead + value + TextColor.colorEnd);
                }
                else
                {
                    resultDescribe = resultDescribe.Replace(fullName, TextColor.paramColorHead + value + TextColor.colorEnd);
                    //resultDescribe = resultDescribe.Replace(fullName, value);
                }
            }
            paramNameList.Clear();
            return resultDescribe;
        }

        static public bool TryGetDescribeParamName(string describe, out string paramKey, out string paramName, out int endIndex)
        {
            int startIndex = describe.IndexOf(ConfigDictParam.paramDescribeStart);
            if (startIndex >= 0 && describe[startIndex + 1] == ConfigDictParam.paramSign)
            {
                endIndex = describe.IndexOf(ConfigDictParam.paramDescribeEnd);
                if (endIndex > startIndex + 1)
                {
                    paramName = describe.Substring(startIndex, endIndex - startIndex + 1);
                    paramKey = describe.Substring(startIndex + 2, endIndex - startIndex - 2);
                    return true;
                }
            }

            endIndex = -1;
            paramName = null;
            paramKey = null;
            return false;
        }

        static public bool IsNumeric(string value)
        {
            if (value == null || value.Length <= 0)
            {
                return false;
            }
            int len = value.Length;
            if (value[len - 1] == '%')
            {
                value = value.Remove(len - 1);
            }
            return Regex.IsMatch(value, @"^[-+]?\d+(\.\d+)?$");
        }

        static public bool StringIsNum(string str)
        {
            int len = str.Length;
            if (str[len - 1] == '%')
            {
                str = str.Remove(len - 1);
            }
            float value = 0;
            return float.TryParse(str, out value);
        }

        /// <summary>
        /// 获取个位数的中文
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        static string GetNumber(int num)
        {
            switch (num)
            {
                case 0:
                    return "零";
                case 1:
                    return "一";
                case 2:
                    return "二";
                case 3:
                    return "三";
                case 4:
                    return "四";
                case 5:
                    return "五";
                case 6:
                    return "六";
                case 7:
                    return "七";
                case 8:
                    return "八";
                case 9:
                    return "九";
                case 10:
                    return "十";
                case 100:
                    return "百";
                case 1000:
                    return "千";
                default:
                    return "";
            }
        }





        public static string TimeSpanToString(TimeSpan time)
        {
            return time.Hours.ToString("00") + ":" + time.Minutes.ToString("00") + ":" + time.Seconds.ToString("00");
        }

        public static string GetDateStringToAutoTest(DateTime time)
        {
            StringBuilder timeBuilder = new StringBuilder();
            timeBuilder.AppendFormat("{0}/{1}/{2} {3}:{4}:{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
            return timeBuilder.ToString();
        }
        #endregion

        

    }
}
