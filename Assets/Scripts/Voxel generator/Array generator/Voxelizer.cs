using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    public GameObject targetObject;
    public Vector3Int size = new(50, 50, 50);
    private bool[,,] voxelGrid;

    [SerializeField] private World world;
      
    [SerializeField] private bool matchBlockSize;

    [SerializeField] private bool autoSize;
    [SerializeField] private bool scale;
    [SerializeField] private float scaleFactor;

    [SerializeField] private bool solid;

    [ContextMenu("Reset")]
    void Start()
    {
        if (targetObject != null)
        {
            GameObject instantiatedObject =
                Instantiate(targetObject,
                transform.position,
                Quaternion.identity);

            VoxelizeObject(instantiatedObject);
            if (solid)
                world.GenerateWorld(FillSpaces.Bool3D(voxelGrid));
            else
                world.GenerateWorld(voxelGrid);

            Destroy(instantiatedObject);
        }
    }

    void VoxelizeObject(GameObject obj)
    {
        Bounds bounds = CalculateBounds(obj);
        CalculateArrayDimensions(bounds);
        voxelGrid = new bool[size.x,
            size.y, size.z];
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
        if (autoSize)
        {
            size = new Vector3Int(
                Mathf.CeilToInt(bounds.size.x),
                Mathf.CeilToInt(bounds.size.y),
                Mathf.CeilToInt(bounds.size.z));
            if (scale)
            {
                size = new Vector3Int(
                Mathf.CeilToInt(size.x * scaleFactor),
                Mathf.CeilToInt(size.y * scaleFactor),
                Mathf.CeilToInt(size.z * scaleFactor));
            }
        }
        if (matchBlockSize)
        {
            size = new Vector3Int(
                Mathf.CeilToInt(size.x / world.blockSize.x),
                Mathf.CeilToInt(size.y / world.blockSize.y),
                Mathf.CeilToInt(size.z / world.blockSize.z));
        }
    }

    void FillVoxelGrid(Bounds bounds)
    {
        int arrayDimensionsX = size.x;
        int arrayDimensionsY = size.y;
        int arrayDimensionsZ = size.z;
        float voxelSizeX = bounds.size.x / arrayDimensionsX;
        float voxelSizeY = bounds.size.y / arrayDimensionsY;
        float voxelSizeZ = bounds.size.z / arrayDimensionsZ;
        Vector3 voxelHalfSize = new Vector3(voxelSizeX,
            voxelSizeY, voxelSizeZ) * .5f;
        Debug.Log(voxelHalfSize);

        for (int x = 0; x < arrayDimensionsX; x++)
        {
            for (int y = 0; y < arrayDimensionsY; y++)
            {
                for (int z = 0; z < arrayDimensionsZ; z++)
                {
                    Vector3 center = bounds.min + new Vector3(
                        voxelSizeX * (x + 0.5f),
                        voxelSizeY * (y + 0.5f),
                        voxelSizeZ * (z + 0.5f));
                    bool occupied = Physics.CheckBox(center,
                        voxelHalfSize, Quaternion.identity);
                    voxelGrid[x, y, z] = occupied;
                }
            }
        }
    }
}
