using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapGridObject 
{
    private int value;
    private const int MIN = 0;
    private const int MAX = 100;
    private int x;
    private int z;

    private Grid<HeatMapGridObject> grid;

    public HeatMapGridObject(Grid<HeatMapGridObject> grid, int x, int z)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
    }


    public void AddValue(int value)
    {
        this.value += value;
        value = Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGridObjectChanged(x,z);
    }

    public float GetValueNormalized()
    {
        return (float)value/MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }

}
