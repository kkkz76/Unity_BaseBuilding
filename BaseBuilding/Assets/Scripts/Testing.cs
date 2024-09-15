using UnityEngine;

public class Testing : MonoBehaviour
{
    private Grid grid;
    public float planeHeight = 0f; // Height of the plane where the ray will intersect

    private void Start()
    {
        grid = new Grid(4, 4, 8f,Vector3.zero);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition != Vector3.zero) // Ensure valid position
            {
                grid.SetValue(mouseWorldPosition, 56);
            }
        }
        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = GetMouseWorldPosition();
            if (mouseWorldPosition != Vector3.zero) // Ensure valid position
            {
                Debug.Log(grid.GetValue(mouseWorldPosition));
            }
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
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

    //private void OnDrawGizmos()
    //{
    //    // Draw a line for the ray cast
    //    Gizmos.color = Color.green; // Set the color for the gizmo line

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    Plane plane = new Plane(Vector3.up, new Vector3(0, planeHeight, 0)); // Adjust plane height if needed

    //    float distance;
    //    if (plane.Raycast(ray, out distance))
    //    {
    //        Vector3 point = ray.GetPoint(distance);
    //        Gizmos.DrawLine(ray.origin, point);
    //    }
    //    else
    //    {
    //        // Draw a line from the ray origin to a point far away to visualize the ray even if it doesn't hit the plane
    //        Gizmos.DrawLine(ray.origin, ray.origin + ray.direction * 100f);
    //    }
    //}
}
