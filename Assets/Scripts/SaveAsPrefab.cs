using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class SaveToPrefab : MonoBehaviour
{
    [SerializeField] private GameObject[] chunks;
    [SerializeField] private string prefabFolder;
    [SerializeField] private string prefabName;

    private string prefabPath;
    private string meshPath;
    
    [ContextMenu("Go")]
    public void CreatePrefab()
    {
        GameObject prefab = new();
        prefabPath = Path.Combine("Assets", prefabFolder);
        meshPath = Path.Combine(prefabPath, prefabName + "Parts");
        CheckDirectorys();
        int counter = 0;
        foreach (GameObject chunk in chunks) 
        {
            Mesh mesh = chunk.GetComponent<MeshFilter>().mesh;
            if (mesh.vertices.Length > 0)
            {
                AssetDatabase.CreateAsset(mesh, Path.Combine(meshPath,
                    "Part" + counter++ + ".asset"));
                Destroy(chunk.GetComponent<ChunkRenderer>());
                chunk.transform.parent = prefab.transform;
            }
        }
        PrefabUtility.SaveAsPrefabAsset(prefab, Path.Combine(prefabPath,
            prefabName + ".prefab"));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void CheckDirectorys()
    {
        if (!Directory.Exists(prefabPath))
        {
            Directory.CreateDirectory(prefabPath);
        }
        if (!Directory.Exists(meshPath))
        {
            Directory.CreateDirectory(meshPath);
        }
    }
}
