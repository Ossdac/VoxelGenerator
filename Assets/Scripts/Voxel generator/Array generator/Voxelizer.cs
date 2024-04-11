using UnityEngine;

public class Voxelizer : MonoBehaviour
{
    public GameObject targetObject;
    public int sizeLongestAxis = 100;
    private Vector3Int arrayDimensions;
    private bool[,,] voxelGrid;

    [SerializeField] private World world;

    [SerializeField] private Vector3 scanDimentions;

    [SerializeField] private bool matchBlockSize;

    [SerializeField] private bool autoSize;

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
        voxelGrid = new bool[arrayDimensions.x,
            arrayDimensions.y, arrayDimensions.z];
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
        float longestSide = Mathf.Max(bounds.size.x,
            bounds.size.y, bounds.size.z);
        float xRatio = bounds.size.x / longestSide;
        float yRatio = bounds.size.y / longestSide;
        float zRatio = bounds.size.z / longestSide;

        if (autoSize)
            sizeLongestAxis = Mathf.CeilToInt(longestSide);

        if (matchBlockSize)
            scanDimentions = world.blockSize;

        arrayDimensions = new Vector3Int(
            Mathf.CeilToInt(sizeLongestAxis * xRatio * scanDimentions.x),
            Mathf.CeilToInt(sizeLongestAxis * yRatio * scanDimentions.y),
            Mathf.CeilToInt(sizeLongestAxis * zRatio * scanDimentions.z));
    }

    void FillVoxelGrid(Bounds bounds)
    {
        int arrayDimensionsX = arrayDimensions.x;
        int arrayDimensionsY = arrayDimensions.y;
        int arrayDimensionsZ = arrayDimensions.z;
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
