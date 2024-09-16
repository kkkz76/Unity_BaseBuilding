using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingSystem : MonoBehaviour
{
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 10;
    [SerializeField] private float cellSize = 10f;
    public float planeHeight = 0f;
    private Grid<GridObject> grid;

    [SerializeField] private PlacedObjectTypeSO placedObjectTypeSO;
    private PlacedObjectTypeSO.Dir currentDir = PlacedObjectTypeSO.Dir.Down;
    private void Awake()
    {
        grid = new Grid<GridObject>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObject> grid, int x, int z) => new GridObject(grid, x, z));
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition != Vector3.zero)
            {
                grid.GetXZ(mouseWorldPosition , out int x, out int z);
                GridObject gridObject = grid.GetGridObject(x, z);
                if (gridObject != null)
                {
                    List<Vector2Int> gridPositionList = placedObjectTypeSO.GetGridPositionList(new Vector2Int(x, z), currentDir);

                    bool canBuild = true;
                    foreach (Vector2Int gridPosition in gridPositionList)
                    {
                        if (!grid.GetGridObject(gridPosition.x, gridPosition.y).ChecKCanBuild())
                        {
                            //cannot build 
                            canBuild = false;
                            break;
                        }
                    }

                    if (canBuild)
                    {
                        Vector2Int rotationOffest = placedObjectTypeSO.GetRotationOffset(currentDir);
                        Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffest.x, 0, rotationOffest.y) * grid.GetCellSize();

                        Transform builtTransform = Instantiate(placedObjectTypeSO.prefab, placedObjectWorldPosition, Quaternion.Euler(0,placedObjectTypeSO.GetRotationAngle(currentDir),0));

                        foreach (Vector2Int gridPosition in gridPositionList)
                        {
                            grid.GetGridObject(gridPosition.x, gridPosition.y).SetTransform(builtTransform);
                        }

                    }
                    else
                    {
                        Debug.Log("Cannot Build a building");
                    }
                }
                
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        { 
            currentDir = PlacedObjectTypeSO.GetNextDir(currentDir);
            Debug.Log(currentDir);
        }
    }

    public class GridObject
    {
        private Grid<GridObject> grid;
        private int x;
        private int z;
        private Transform transform;

        public GridObject(Grid<GridObject> grid , int x, int z)
        {
            this.grid = grid;
            this.x = x;
            this.z = z;
        }
        public void SetTransform(Transform transform)
        {
            this.transform = transform;
            grid.TriggerGridObjectChanged(x, z);
        }

        public void ClearTransform()
        {
            transform = null;
            grid.TriggerGridObjectChanged(x, z);
        }
        public bool ChecKCanBuild()
        {
            return transform == null;
        }

        public override string ToString()
        {
            return x + "," + z +"\n"+transform;
        }
    }


    public Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0)); 
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }
}


