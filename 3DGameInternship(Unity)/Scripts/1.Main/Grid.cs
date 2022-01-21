using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject plane;

    [SerializeField]
    private float size = 1f;

    private void Start()
    {
        float width = PlayerPrefs.GetFloat("width");
        float length = PlayerPrefs.GetFloat("length");

        //plane.transform.localScale = new Vector3(width, 1, length);
    }

    public Vector3 GetNearestPointOnGrid(Vector3 position) // Convert to Position at closest Grid Coordinate
    {
        position -= transform.position; // Grid is lower than Plane

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        Vector3 result = new Vector3(
            (float)xCount * size,
            (float)yCount * size,
            (float)zCount * size);

        result += transform.position; // Plane is higher than Grid

        return result;
    }

    public Vector3 CoordinatesToGridPoint (Vector3 position) // Convert to Position
    {

        Vector3 result = new Vector3(
            (float)position.x * size,
            (float)position.y * size,
            (float)position.z * size);

        result += transform.position; // Plane is higher than Grid

        return result;
    }

    public Vector3 GetGridCoordinates(Vector3 position) // Convert to Grid Coordinate at closest Grid Coordinate
    {
        position -= transform.position; // Grid is lower than Plane

        int xCount = Mathf.RoundToInt(position.x / size);
        int yCount = Mathf.RoundToInt(position.y / size);
        int zCount = Mathf.RoundToInt(position.z / size);

        return new Vector3(xCount, yCount, zCount);
    }

    public Vector3 GetGridPoint(Vector3 position) // Convert to Grid Coordinate at closest Grid Coordinate
    {
        position -= transform.position; // Grid is lower than Plane

        float xCount = position.x / size;
        float yCount = position.y / size;
        float zCount = position.z / size;

        return new Vector3(xCount, yCount, zCount);
    }

    public float ConvertToGridDistance(float distance)
    {
        float newDistance = distance / size;

        return newDistance;
    }
}