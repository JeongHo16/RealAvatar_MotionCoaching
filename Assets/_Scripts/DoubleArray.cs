using UnityEngine;

[System.Serializable]
public class DoubleArray
{
    [SerializeField]
    private int size;

    [SerializeField]
    private double[] array;

    public void SetSize()
    {
        size = array.Length;
    }

    public int Length
    {
        get { return array == null ? -1 : array.Length; }
    }

    public double this[int index]
    {
        get { return array == null ? -1 : array[index]; }
        set { array[index] = value; }
    }

    public void Add(double angle)
    {
        if (array == null)
        {
            array = new double[1];
            array[0] = angle;

            return;
        }

        double[] tempArray = new double[array.Length];
        for (int ix = 0; ix < tempArray.Length; ++ix)
        {
            tempArray[ix] = array[ix];
        }

        array = new double[array.Length + 1];
        for (int ix = 0; ix < tempArray.Length; ++ix)
        {
            array[ix] = tempArray[ix];
        }

        array[array.Length - 1] = angle;
    }

    public void RemoveAt(int index)
    {
        if (array == null) return;

        double[] tempArray = new double[array.Length - 1];
        int idx = 0;
        for (int ix = 0; ix < array.Length; ++ix)
        {
            if (ix == index) continue;
            tempArray[idx] = array[ix];
            ++idx;
        }

        array = new double[tempArray.Length];
        for (int ix = 0; ix < array.Length; ++ix)
        {
            array[ix] = tempArray[ix];
        }
    }
}