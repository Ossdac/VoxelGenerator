using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> vertices = new();
    public List<int> triangles = new();
    public List<Vector2> uv = new();

    public List<Vector3> colliderVertices = new();
    public List<int> colliderTriangles = new();


    public void AddVertex(Vector3 vertex)
    {
        vertices.Add(vertex);
        colliderVertices.Add(vertex);
    }

    public void AddQuadTriangles()
    {
        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 3);
        triangles.Add(vertices.Count - 2);

        triangles.Add(vertices.Count - 4);
        triangles.Add(vertices.Count - 2);
        triangles.Add(vertices.Count - 1);

        colliderTriangles.Add(colliderVertices.Count - 4);
        colliderTriangles.Add(colliderVertices.Count - 3);
        colliderTriangles.Add(colliderVertices.Count - 2);

        colliderTriangles.Add(colliderVertices.Count - 4);
        colliderTriangles.Add(colliderVertices.Count - 2);
        colliderTriangles.Add(colliderVertices.Count - 1);


    }
}
