using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class ChunkRenderer : MonoBehaviour
{
    [SerializeField]MeshFilter meshFilter;
    [SerializeField] MeshCollider meshCollider;
    [SerializeField] Mesh mesh;
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

    private void RenderMesh(MeshData meshData)
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
        collisionMesh.RecalculateNormals();

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
                    Gizmos.color = new Color(1, 0, 1, 0.4f);

                int chunkSize = ChunkData.chunkSize;
                int chunkHeight = ChunkData.chunkHeight;

                Gizmos.DrawCube(transform.position + new Vector3(chunkSize / 2f, chunkHeight / 2f, chunkSize / 2f), new Vector3(chunkSize, chunkHeight, chunkSize));
            }
        }
    }
#endif
}