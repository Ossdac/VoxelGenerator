using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LabyrinthReader : MonoBehaviour
{
    [SerializeField] private World world;

    void Start()
    {
        //CombineMeshes();
        GenerateCubePosition2DArray();
    }
    [ContextMenu("Reset")]
    public void GenerateCubePosition2DArray()
    {
        Transform[] cubePositions = new Transform[transform.childCount];

        for (int i = 0; i < cubePositions.Length; i++) 
        {
            cubePositions[i] = transform.GetChild(i);
        }

        float maxX = float.MinValue;
        float maxZ = float.MinValue;
        float minX = float.MaxValue;
        float minZ = float.MaxValue;
        float scale = cubePositions[0].localScale.x;

        foreach (Transform cube in cubePositions)
        {
            if (cube.position.x < minX)
                minX = cube.position.x;
            if (cube.position.x > maxX)
                maxX = cube.position.x;
            if (cube.position.z < minZ)
                minZ = cube.position.z;
            if (cube.position.z > maxZ)
                maxZ = cube.position.z;
          
        }
        int x = Mathf.CeilToInt((maxX - minX) / scale);
        int z = Mathf.CeilToInt((maxZ - minZ) / scale);

        bool[,] positions = new bool[x, z];

        foreach (Transform cube in cubePositions)
        {
            positions[Mathf.FloorToInt((cube.position.x - minX) /scale), 
                Mathf.FloorToInt((cube.position.z - minZ) / scale)] = true;
        }
        world.GenerateWorld(positions);
    }

    //private void CombineMeshes()
    //{
    //    MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
    //    CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];

    //    for (int i = 1; i < meshFilters.Length; i++)
    //    {
    //        combine[i - 1].mesh = meshFilters[i].sharedMesh;
    //        combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
    //    }

    //    GetComponent<MeshFilter>().mesh = new Mesh();
    //    GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);
    //    GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
    //    transform.DetachChildren();
    //    transform.SetPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
    //    foreach (MeshFilter mf in meshFilters)
    //    {
    //        mf.transform.parent = gameObject.transform;
    //    }
    //    gameObject.SetActive(true);
    //}
}

