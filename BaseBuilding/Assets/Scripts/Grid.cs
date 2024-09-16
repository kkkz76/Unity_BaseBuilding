using System;
using UnityEngine;

public class Grid
{
    public const int HEAT_MAP_MAX_VALUE = 100;
    public const int HEAT_MAP_MIN_VALUE = 0;

    public event EventHandler<OnGridValueChangedEventArgs> OnGridValueChanged;
    public class OnGridValueChangedEventArgs : EventArgs
    {
        public int x;
        public int z;
    }

    private bool showDebug = false;
    private int width;
    private int length;
    private int[,] gridArray;
    private TextMesh[,] debugTextArray;
    private float cellSize;
    private Vector3 originPosition;
    public const int sortingOrderDefault = 5000;

    public Grid(int width, int length, float cellSize, Vector3 originPosition)
    {
        this.width = width;
        this.length = length;
        this.cellSize = cellSize;
        this.originPosition = originPosition;

        gridArray = new int[width, length];
        debugTextArray = new TextMesh[width, length];

      
        if (showDebug)
        {
            // Initialize the debugTextArray with world text and draw debug lines
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < length; z++)
                {
                    Vector3 position = GetWorldPosition(x, z) + new Vector3(cellSize, 0, cellSize) * 0.5f;
                    debugTextArray[x, z] = CreateWorldText(gridArray[x, z].ToString(), null, position, 30, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.red, 100f);
                    Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.red, 100f);
                }
            }

            // Draw the grid boundary lines
            Debug.DrawLine(GetWorldPosition(0, length), GetWorldPosition(width, length), Color.red, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, length), Color.red, 100f);

            // Subscribe to grid value change event
            OnGridValueChanged += (object sender, OnGridValueChangedEventArgs eventArgs) =>
            {
                debugTextArray[eventArgs.x, eventArgs.z].text = gridArray[eventArgs.x, eventArgs.z].ToString();
            };
        }
    }

    public Vector3 GetWorldPosition(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    private void GetXZ(Vector3 worldPosition, out int x, out int z)
    {
        Vector3 relativePosition = worldPosition - originPosition;
        x = Mathf.FloorToInt(relativePosition.x / cellSize);
        z = Mathf.FloorToInt(relativePosition.z / cellSize);
    }

    public int GetWidth()
    {
        return width;
    }
    public int GetLength()
    {
       return length;
    }
    public float GetCellSize()
    {
        return cellSize;
    }

    private void SetValue(int x, int z, int value)
    {
        if (x >= 0 && z >= 0 && x < width && z < length)
        {
            gridArray[x, z] = Mathf.Clamp(value, HEAT_MAP_MIN_VALUE, HEAT_MAP_MAX_VALUE);
            if (debugTextArray != null && showDebug) // ensure debugtextarray is initialized
            {
                debugTextArray[x, z].text = gridArray[x, z].ToString();
            }
            OnGridValueChanged?.Invoke(this, new OnGridValueChangedEventArgs { x = x, z = z });
        }
    }

    public void SetValue(Vector3 worldPosition, int value)
    {
        GetXZ(worldPosition, out int x, out int z);
        SetValue(x, z, value);
    }

    public int GetValue(int x, int z)
    {
        if (x >= 0 && z >= 0 && x < width && z < length)
        {
            return gridArray[x, z];
        }
        else
        {
            return 0;
        }
    }

    public int GetValue(Vector3 worldPosition)
    {
        GetXZ(worldPosition, out int x, out int z);
        return GetValue(x, z);
    }

    public void AddValue(int x, int z, int value)
    {
        SetValue(x, z, GetValue(x, z) + value);
    }

    public void AddValue(Vector3 worldPosition, int value, int fullValueRange, int totalRange)
    {
        int lowerValueAmount = Mathf.RoundToInt((float)value / (totalRange - fullValueRange));
        GetXZ(worldPosition, out int originX, out int originY);

        for (int x = 0; x < totalRange; x++)
        {
            for (int z = 0; z < totalRange - x; z++)
            {
                int radius = x + z;
                int addValueAmount = value;

                if (radius >= fullValueRange)
                {
                    addValueAmount -= lowerValueAmount * (radius - fullValueRange);
                }

                AddValue(originX + x, originY + z, addValueAmount);

                if (x != 0)
                {
                    AddValue(originX - x, originY + z, addValueAmount);
                }
                if (z != 0)
                {
                    AddValue(originX + x, originY - z, addValueAmount);
                    if (x != 0)
                    {
                        AddValue(originX - x, originY - z, addValueAmount);
                    }
                }
            }
        }
    }

    //----------------------------- text generator -----------------------

    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, Color? color = null, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = sortingOrderDefault)
    {
        if (color == null) color = Color.white;
        return CreateWorldText(parent, text, localPosition, fontSize, (Color)color, textAnchor, textAlignment, sortingOrder);
    }

    // Create Text in the World
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize, Color color, TextAnchor textAnchor, TextAlignment textAlignment, int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        transform.rotation = Quaternion.Euler(90f, 0f, 0f); // Adjust rotation as needed
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    //---------------------------------------------------------------------------

}
