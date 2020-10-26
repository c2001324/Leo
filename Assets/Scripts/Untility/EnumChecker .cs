using UnityEngine;
using UnityEditor;
using System;

namespace Untility
{
    /// <summary>
    /// 用于数量大于32个的Enum
    /// </summary>
    public static class EnumChecker
    {
        public static bool Check<T>(T[] sources, params T[] targets)
        {
            if (sources == null || sources.Length == 0 || targets.Length == 0)
            {
                return false;
            }
            foreach (T target in targets)
            {
                bool find = false;
                foreach (T source in sources)
                {
                    if (source.Equals(target))
                    {
                        find = true;
                        break;
                    }
                }

                if (!find)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 多个中是否有一个
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public static bool CheckByOne<T>(T[] sources, params T[] targets)
        {
            if (sources == null || sources.Length == 0 || targets.Length == 0)
            {
                return false;
            }
            foreach (T target in targets)
            {
                foreach (T source in sources)
                {
                    if (source.Equals(target))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}

