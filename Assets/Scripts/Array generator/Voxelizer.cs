using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    public GameObject targetObject;
    public int sizeLongestAxis = 100; // Size for the longest axis in the voxel grid
    private Vector3Int arrayDimensions;
    private bool[,,] voxelGrid;

    [SerializeField] private World world;

    [SerializeField] private bool autoSize;

    [ContextMenu("Reset")]
    void Start()
    {
        if (targetObject != null)
        {
            VoxelizeObject(targetObject);
            world.GenerateWorld(voxelGrid);
        }
    }

    void VoxelizeObject(GameObject obj)
    {
        Bounds bounds = CalculateBounds(obj);
        CalculateArrayDimensions(bounds);
        voxelGrid = new bool[arrayDimensions.x, arrayDimensions.y, arrayDimensions.z];
        FillVoxelGrid(bounds);
    }

    Bounds CalculateBounds(GameObject obj)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(obj.transform.position, Vector3.zero);
        foreach (var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        return bounds;
    }

    void CalculateArrayDimensions(Bounds bounds)
    {
        float longestSide = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);
        float xRatio = bounds.size.x / longestSide;
        float yRatio = bounds.size.y / longestSide;
        float zRatio = bounds.size.z / longestSide;

        if (autoSize)
            sizeLongestAxis = Mathf.CeilToInt(longestSide);
        arrayDimensions = new Vector3Int(
            Mathf.CeilToInt(sizeLongestAxis * xRatio),
            Mathf.CeilToInt(sizeLongestAxis * yRatio),
            Mathf.CeilToInt(sizeLongestAxis * zRatio));
    }

    void FillVoxelGrid(Bounds bounds)
    {
        Vector3 voxelSize = new Vector3(bounds.size.x / arrayDimensions.x, bounds.size.y / arrayDimensions.y, bounds.size.z / arrayDimensions.z);
        for (int x = 0; x < arrayDimensions.x; x++)
        {
            for (int y = 0; y < arrayDimensions.y; y++)
            {
                for (int z = 0; z < arrayDimensions.z; z++)
                {
                    Vector3 center = bounds.min + new Vector3(voxelSize.x * (x + 0.5f), voxelSize.y * (y + 0.5f), voxelSize.z * (z + 0.5f));
                    bool occupied = Physics.CheckBox(center, voxelSize * 0.5f, Quaternion.identity);
                    voxelGrid[x, y, z] = occupied;
                }
            }
        }
    }
}
