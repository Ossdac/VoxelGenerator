using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider meshCollider;
    Mesh mesh;
    public bool showGizmo = false;

    public ChunkData ChunkData { get; private set; }


    private void Awake()
    {
        AssignComponents();
    }

    public void AssignComponents()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
        mesh = meshFilter.mesh;
    }

    public void InitializeChunk(ChunkData data)
    {
        ChunkData = data;
    }

    public void RenderMesh(MeshData meshData)
    {
        mesh.Clear();

        mesh.vertices = meshData.vertices.ToArray();

        mesh.SetTriangles(meshData.triangles.ToArray(), 0);

        mesh.uv = meshData.uv.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        Mesh collisionMesh = new();
        collisionMesh.vertices = meshData.colliderVertices.ToArray();
        collisionMesh.triangles = meshData.colliderTriangles.ToArray();

        meshCollider.sharedMesh = collisionMesh;
    }

    public void UpdateChunk()
    {
        RenderMesh(Chunk.GetChunkMeshData(ChunkData));
    }

    public void UpdateChunk(MeshData data)
    {
        RenderMesh(data);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            if (Application.isPlaying && ChunkData != null)
            {
                if (Selection.activeObject == gameObject)
                    Gizmos.color = new Color(0, 1, 0, 0.4f);
                else
                    Gizmos.color = new Color(1, 0, 1, 0);

                Vector3 chunkSize = (Vector3)ChunkData.chunkSize;
                Vector3 blockSize = ChunkData.blockSize;
                float xPos = chunkSize.x * blockSize.x;
                float yPos = chunkSize.y * blockSize.y;
                float zPos = chunkSize.z * blockSize.z;

                Gizmos.DrawCube(transform.position +
                    new Vector3((xPos / 2f) - (blockSize.x / 2), (yPos / 2f) - (blockSize.y /2), (zPos / 2f) - (blockSize.z / 2)),
                    new Vector3(xPos, yPos, zPos));
            }
        }
    }
#endif
}