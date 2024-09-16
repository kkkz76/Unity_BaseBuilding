using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid<HeatMapGridObject> grid;
    public float planeHeight = 0f;
    [SerializeField] private HeatMapGenericVisual HeatMapGenericVisual;

    private void Start()
    {
        grid = new Grid<HeatMapGridObject>(10, 10, 10f, Vector3.zero , (Grid<HeatMapGridObject> grid, int x , int z ) => new HeatMapGridObject(grid,x, z));
        HeatMapGenericVisual.SetGrid(grid);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition != Vector3.zero) // Ensure valid position
            {
                HeatMapGridObject heatMapGridObject = grid.GetGridObject(mouseWorldPosition);
                if(heatMapGridObject != null)
                {
                    heatMapGridObject.AddValue(10);
                  
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition != Vector3.zero) // Ensure valid position
            {
                Debug.Log(grid.GetGridObject(mouseWorldPosition));
            }
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found.");
            return Vector3.zero;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0)); // Adjust plane height if needed

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }

        // Return zero vector if ray doesn't intersect the plane
        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main camera not found.");
            return;
        }

        // Draw a line for the ray cast
        Gizmos.color = Color.green; // Set the color for the gizmo line

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0)); // Adjust plane height if needed

        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 point = ray.GetPoint(distance);
            Gizmos.DrawLine(ray.origin, point);
        }
        else
        {
            // Draw a line from the ray origin to a point far away to visualize the ray even if it doesn't hit the plane
            Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100f);
        }
    }
}

