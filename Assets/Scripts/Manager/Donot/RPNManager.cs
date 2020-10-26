using System.Collections.Generic;
using UnityEngine;
using Untility;

/// <summary>
/// 逆波兰式 管理器
/// </summary>
public class RPNManager : Singleton<RPNManager>, IDonotInitManager
{
    RPN m_RPN = new RPN();

    readonly static public string deep = "deep";

    /// <summary>
    /// 根据房间的深度来计算权重
    /// </summary>
    /// <param name="exp">表达式</param>
    /// <param name="depth">深度</param>
    /// <returns></returns>
    public bool TryCalRoomWeightByDepth(string exp, int depth, out int weight)
    {
        exp = exp.Replace(RPNManager.deep, depth.ToString());
        return TryEvaluate(exp, out weight);
    }

    public bool TryEvaluate(string exp, out int value)
    {
        if (m_RPN.Parse(exp))
        {
            object result = m_RPN.Evaluate();
            if (result != null && int.TryParse(result.ToString(), out value))
            {
                return true;
            }
        }

        Debug.LogError("解析表达式 " + exp + " 失败");
        value = 0;
        return false;
    }

    public bool TryEvaluate(string exp, out float value)
    {
        if (m_RPN.Parse(exp))
        {
            object result = m_RPN.Evaluate();
            if (result != null && float.TryParse(result.ToString(), out value))
            {
                return true;
            }
        }

        Debug.LogError("解析表达式 " + exp + " 失败");
        value = 0f;
        return false;
    }
}