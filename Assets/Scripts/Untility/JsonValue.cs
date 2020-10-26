
using System.Collections.Generic;

public struct JIntRange
{
    public JIntRange(int min, int max)
    {
        this.min = min;
        this.max = max;
    }

    public int GetValue(System.Random random)
    {
        return random.Range(min, max);
    }

    public int min;
    public int max;
}

public struct JFloatRange
{
    public JFloatRange(float min, float max)
    {
        this.min = min;
        this.max = max;
    }

    public float GetValue(System.Random random)
    {
        return random.Range(min, max);
    }

    public float min;
    public float max;
}

public struct IntVector2
{
    public static IntVector2 zero = new IntVector2();

    public IntVector2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public int x;
    public int y;

    static public IntVector2 operator *(IntVector2 v, int f)
    {
        return new IntVector2(v.x * f, v.y * f);
    }

}

public class JWeightRange<T>
{
    public JWeight<T>[] weights;

    public List<T> GetAllValues()
    {
        List<T> values = new List<T>();
        foreach (JWeight<T> w in weights)
        {
            values.Add(w.value);
        }
        return values;
    }

    public T GetValue(System.Random random)
    {
        int index = 0;
        return GetValueFromWeightList(weights, random, out index);
    }

    T GetValueFromWeightList(JWeight<T>[] weights, System.Random random, out int index)
    {
        index = -1;
        if (weights != null && weights.Length > 0)
        {
            if (weights.Length == 1)
            {
                index = 0;
                return weights[0].value;
            }
            else
            {
                int[] ws = new int[weights.Length];
                for (int i = 0; i < ws.Length; i++)
                {
                    ws[i] = weights[i].weight;
                }
                index = Untility.Tool.RandomIndexByWeight(ws, random);
                return weights[index].value;
            }
        }
        else
        {
            return default(T);
        }
    }

    public List<T> GetValues(int count, System.Random random)
    {
        List<T> values = new List<T>();
        if (count >= 0 && count < weights.Length)
        {
            List<JWeight<T>> tempList = new List<JWeight<T>>(weights);
            for (int i = 0; i < count; i++)
            {
                int removeIndex;
                T value = GetValueFromWeightList(tempList.ToArray(), random, out removeIndex);
                values.Add(value);
                tempList.RemoveAt(removeIndex);
            }
            tempList.Clear();
        }
        else if (count > 0)
        {
            foreach (JWeight<T> e in weights)
            {
                values.Add(e.value);
            }
        }
        return values;
    }
}

public class JWeight<T>
{
    public JWeight(int weight, T value)
    {
        this.weight = weight;
        this.value = value;
    }

    public int weight;
    public T value;
}

public class JRPNWeight<T>
{
    public JRPNWeight(string weight, T value)
    {
        this.weight = weight;
        this.value = value;
    }

    public string weight;
    public T value;
}
