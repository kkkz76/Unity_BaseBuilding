using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMapGenericVisual : MonoBehaviour
{
    private Grid<HeatMapGridObject> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake()
    {
        // Initialize the mesh variable correctly
        mesh = new Mesh();
        // Ensure MeshFilter component exists
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter component is missing from this GameObject.");
            return;
        }
        // Assign the mesh to the MeshFilter
        meshFilter.mesh = mesh;
    }
    public void SetGrid(Grid<HeatMapGridObject> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();
        grid.OnGridValueChanged += Grid_OnGridValueChanged;
    }

    private void Grid_OnGridValueChanged(object sender, Grid<HeatMapGridObject>.OnGridValueChangedEventArgs e)
    {
        updateMesh = true;
    }

    private void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            UpdateHeatMapVisual();
        }
    }

    private void UpdateHeatMapVisual()
    {
        int width = grid.GetWidth();
        int height = grid.GetLength();
        float cellSize = grid.GetCellSize();

        // Create empty mesh arrays
        MeshUtils.CreateEmptyMeshArrays(width * height, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        int index = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Calculate the position and size of the quad
                Vector3 quadSize = new Vector3(cellSize, 0, cellSize);
                Vector3 position = grid.GetWorldPosition(x, z) + quadSize * 0.5f;
                float gridValueNormalized = (grid.GetGridObject(x, z)).GetValueNormalized();
                Vector2 gridValueUV = new Vector2(gridValueNormalized, 0);



                // Add quad to mesh arrays
                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, position, 0f, quadSize, gridValueUV, gridValueUV);

                index++;
            }
        }

        // Assign the mesh data
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

}
