using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{
    public Vector3Int mapSizeInChunks = new(6, 6, 6);
    public Vector3Int chunkSize = new(16, 16, 16);
    public Vector3 blockSize = new(1f, 1f, 1f);
    public float noiseScale = 0.03f;
    public GameObject chunkPrefab;
    [SerializeField] private bool renderNothing;
    [SerializeField] private bool renderDown;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    private void Awake()
    {
        BlockHelper.RenderNothing = renderNothing;
    }

    [ContextMenu("Generate New World")]
    public void GenerateWorld()
    {
        ClearWorld();

        for (int x = 0; x < mapSizeInChunks.x; x++)
        {
            for (int y = 0; y < mapSizeInChunks.y; y++)
            {
                for (int z = 0; z < mapSizeInChunks.z; z++)
                {
                    ChunkData chunkData = new(chunkSize, blockSize, this,
                        new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z));
                    GenerateVoxels(chunkData);
                    chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
                }
            }
        }
        foreach (ChunkData chunkData in chunkDataDictionary.Values)
        {
            RenderChunk(chunkData);
        }
    }

    public void GenerateWorld(bool[,] worldArray)
    {
        ClearWorld();

        int lengthX = worldArray.GetLength(0);
        int lengthZ = worldArray.GetLength(1);
        int chunkCountX = Mathf.CeilToInt((float)lengthX / chunkSize.x);
        int chunkCountZ = Mathf.CeilToInt((float)lengthZ / chunkSize.z);
        mapSizeInChunks = new(chunkCountX, mapSizeInChunks.y, chunkCountZ);

        for (int x = 0; x < chunkCountX; x++)
        {
            for (int y = 0; y < mapSizeInChunks.y; y++)
            {
                for (int z = 0; z < chunkCountZ; z++)
                {
                    Vector3Int chunkPosition = new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z);
                    ChunkData chunkData = new ChunkData(chunkSize, blockSize, this, chunkPosition);
                    bool[,] chunkArray = ExtractChunkArray(worldArray, x, z, lengthX, lengthZ);
                    GenerateVoxels(chunkData, chunkArray);
                    chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
                }
            }
        }
        foreach (ChunkData chunkData in chunkDataDictionary.Values)
        {
            RenderChunk(chunkData);
        }
    }

    public void GenerateWorld(bool[,,] worldArray)
    {
        ClearWorld();

        int lengthX = worldArray.GetLength(0);
        int lengthY = worldArray.GetLength(1);
        int lengthZ = worldArray.GetLength(2);
        int chunkCountX = Mathf.CeilToInt((float)lengthX / chunkSize.x);
        int chunkCountY = Mathf.CeilToInt((float)lengthY / chunkSize.y);
        int chunkCountZ = Mathf.CeilToInt((float)lengthZ / chunkSize.z);

        mapSizeInChunks = new(chunkCountX, chunkCountY, chunkCountZ);

        for (int x = 0; x < chunkCountX; x++)
        {
            for (int y = 0; y < chunkCountY; y++)
            {
                for (int z = 0; z < chunkCountZ; z++)
                {
                    ChunkData chunkData = new(chunkSize, blockSize, this,
                        new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z));
                    bool[,,] chunkArray = ExtractChunkArray(worldArray, x, y, z, lengthX, lengthY, lengthZ);
                    GenerateVoxels(chunkData, chunkArray);
                    chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
                }
            }
        }
        foreach (ChunkData chunkData in chunkDataDictionary.Values)
        {
            RenderChunk(chunkData);
        }
    }


    

    public void GenerateWorld(int[,] worldArray)
    {
        ClearWorld();

        int lengthX = worldArray.GetLength(0);
        int lengthZ = worldArray.GetLength(1);
        int chunkCountX = Mathf.CeilToInt((float)lengthX / chunkSize.x);
        int chunkCountZ = Mathf.CeilToInt((float)lengthZ / chunkSize.z);

        for (int x = 0; x < chunkCountX; x++)
        {
            for (int y = 0; y < mapSizeInChunks.y; y++)
            {
                for (int z = 0; z < chunkCountZ; z++)
                {
                    Vector3Int chunkPosition = new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z);
                    ChunkData chunkData = new ChunkData(chunkSize, blockSize, this, chunkPosition);
                    int[,] chunkArray = ExtractChunkArray(worldArray, x, z, lengthX, lengthZ);
                    GenerateVoxels(chunkData, chunkArray);
                    chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
                }
            }
        }
        foreach (ChunkData chunkData in chunkDataDictionary.Values)
        {
            RenderChunk(chunkData);
        }
    }

    public void GenerateWorld(float[,] worldArray)
    {
        ClearWorld();

        int lengthX = worldArray.GetLength(0);
        int lengthZ = worldArray.GetLength(1);
        int chunkCountX = worldArray.GetLength(0) / chunkSize.x;
        int chunkCountZ = worldArray.GetLength(1) / chunkSize.z;

        for (int x = 0; x <= chunkCountX; x++)
        {
            for (int y = 0; y < mapSizeInChunks.y; y++)
            {
                for (int z = 0; z <= chunkCountZ; z++)
                {
                    Vector3Int chunkPosition = new Vector3Int(x * chunkSize.x, y * chunkSize.y, z * chunkSize.z);
                    ChunkData chunkData = new ChunkData(chunkSize, blockSize, this, chunkPosition);
                    float[,] chunkArray = ExtractChunkArray(worldArray, x, z, lengthX, lengthZ);
                    GenerateVoxels(chunkData, chunkArray);
                    chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
                }
            }
        }
        foreach (ChunkData chunkData in chunkDataDictionary.Values)
        {
            RenderChunk(chunkData);
        }
    }



    private void ClearWorld()
    {
        chunkDataDictionary.Clear();
        foreach (ChunkRenderer chunk in chunkDictionary.Values)
        {
            if (chunk != null)
                Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();
        BlockHelper.RenderNothing = renderNothing;
        BlockHelper.RenderDown = renderDown;
    }

    private void GenerateVoxels(ChunkData data)
    {
        Vector3Int chunkSize = data.chunkSize;
        int startX = data.worldPosition.x;
        int startY = data.worldPosition.y;
        int startZ = data.worldPosition.z;
        int chunkXSize = chunkSize.x;
        int chunkYSize = chunkSize.y;
        int chunkZSize = chunkSize.z;
        int totalHeitht = chunkYSize * mapSizeInChunks.y;
        for (int x = 0; x < chunkXSize; x++)
        {
            for (int z = 0; z < chunkZSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((startX + x) * 
                    noiseScale, (startZ + z) * noiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * totalHeitht);
                for (int y = startY; y < chunkYSize + startY; y++)
                {
                    BlockType voxelType = BlockType.Wall;
                    if (y > groundPosition)
                    {
                        voxelType = BlockType.Air;
                    }
                    else if (y == groundPosition)
                    {
                        voxelType = BlockType.Ground;
                    }
                    Chunk.SetBlock(data, new Vector3Int(x, y-startY, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxels(ChunkData data, bool[,] preMadeArray)
    {
        Vector3Int chunkSize = data.chunkSize;
        int chunkXSize = chunkSize.x;
        int chunkYSize = chunkSize.y;
        int chunkZSize = chunkSize.z;
        for (int x = 0; x < chunkXSize; x++)
        {
            for (int z = 0; z < chunkZSize; z++)
            {
                BlockType voxelType = preMadeArray[x, z] ? BlockType.Wall : BlockType.Air;
                for (int y = 0; y < chunkYSize; y++)
                {
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxels(ChunkData data, bool[,,] preMadeArray)
    {
        Vector3Int chunkSize = data.chunkSize;
        int chunkXSize = chunkSize.x;
        int chunkYSize = chunkSize.y;
        int chunkZSize = chunkSize.z;
        for (int x = 0; x < chunkXSize; x++)
        {
            for (int y = 0; y < chunkYSize; y++)
            {
                for (int z = 0; z < chunkZSize; z++)
                {
                    BlockType voxelType = preMadeArray[x, y, z] ? BlockType.Wall : BlockType.Air;
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    
    private void GenerateVoxels(ChunkData data, float[,] preMadeArray)
    {

        Vector3Int chunkSize = data.chunkSize;
        
        int startY = data.worldPosition.y;
        int chunkXSize = chunkSize.x;
        int chunkYSize = chunkSize.y;
        int chunkZSize = chunkSize.z;
        int totalHeitht = chunkYSize * mapSizeInChunks.y;

        for (int x = 0; x < chunkXSize; x++)
        {
            for (int z = 0; z < chunkZSize; z++)
            {
                int groundPosition = Mathf.RoundToInt(preMadeArray[x,z] * totalHeitht);
               
                for (int y = startY; y < chunkYSize + startY; y++)
                {
                    BlockType voxelType = BlockType.Air;
                    if (y < groundPosition)
                    {
                        voxelType = BlockType.Wall;
                    }
                     else if (groundPosition != 0 && y == groundPosition)
                    {
                        voxelType = BlockType.Ground;
                    }
                    Chunk.SetBlock(data, new Vector3Int(x, y - startY, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxels(ChunkData data, int[,] preMadeArray)
    {
        Vector3Int chunkSize = data.chunkSize;
        int startY = data.worldPosition.y;
        int chunkXSize = chunkSize.x;
        int chunkYSize = chunkSize.y;
        int chunkZSize = chunkSize.z;
        for (int x = 0; x < chunkXSize; x++)
        {
            for (int z = 0; z < chunkZSize; z++)
            {
                int groundPosition = preMadeArray[x, z];
                BlockType voxelType = BlockType.Air;
                for (int y = startY + chunkYSize - 1; y >= startY; y--)
                {
                    if (groundPosition != 0 && y <= groundPosition)
                        voxelType = BlockType.Wall;
                    Chunk.SetBlock(data, new Vector3Int(x, y - startY, z), voxelType);                    
                }
            }
        }
    }


    internal BlockType GetBlockFromChunkCoordinates(int x, int y, int z)
    {
        Vector3Int pos = Chunk.ChunkPositionFromBlockCoords(this, x, y, z);
        ChunkData containerChunk;

        chunkDataDictionary.TryGetValue(pos, out containerChunk);

        if (containerChunk == null)
            return BlockType.Nothing;
        Vector3Int blockInCHunkCoordinates = Chunk.GetBlockInChunkCoordinates(containerChunk, new Vector3Int(x, y, z));
        return Chunk.GetBlockFromChunkCoordinates(containerChunk, blockInCHunkCoordinates);
    }

    private void RenderChunk(ChunkData chunkData)
    {
        MeshData meshData = Chunk.GetChunkMeshData(chunkData);
        GameObject chunkObject = Instantiate(chunkPrefab, Vector3.Scale(blockSize,
            chunkData.worldPosition), Quaternion.identity);
        ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        chunkDictionary.Add(chunkData.worldPosition, chunkRenderer);
        chunkRenderer.InitializeChunk(chunkData);
        chunkRenderer.RenderMesh(meshData);
    }

    private bool[,,] ExtractChunkArray(bool[,,] worldArray, int chunkX, int chunkY, int chunkZ, 
        int lengthX, int lengthY, int lengthZ)
    {
       
        int startX = chunkX * chunkSize.x;
        int startY = chunkY * chunkSize.y; 
        int startZ = chunkZ * chunkSize.z;
        

        bool[,,] chunkArray = new bool[chunkSize.x, chunkSize.y, chunkSize.z];

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int y = 0; y < chunkSize.y; y++)
            {
                for (int z = 0; z < chunkSize.z; z++)
                {
                    if ((startX + x) < lengthX && (startY + y) < lengthY && (startZ + z) < lengthZ)
                    {
                        chunkArray[x, y, z] = worldArray[startX + x, startY + y, startZ + z];
                    }
                    else
                    {
                        chunkArray[x, y, z] = false;
                    }
                }
            }
        }
                return chunkArray;
    }

    private bool[,] ExtractChunkArray(bool[,] worldArray, int chunkX, int chunkZ, int lengthX, int lengthZ)
    {

        int startX = chunkX * chunkSize.x;
        int startZ = chunkZ * chunkSize.z;

        bool[,] chunkArray = new bool[chunkSize.x, chunkSize.z];

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int z = 0; z < chunkSize.z; z++)
            {
                if ((startX + x) < lengthX  && (startZ + z) < lengthZ)
                {
                     chunkArray[x, z] = worldArray[startX + x, startZ + z];
                }
                else
                {
                    chunkArray[x, z] = false;
                }
            }
            
        }

        return chunkArray;
    }

    private float[,] ExtractChunkArray(float[,] worldArray, int chunkX, int chunkZ, int lengthX, int lengthZ)
    {

        int startX = chunkX * chunkSize.x;
        int startZ = chunkZ * chunkSize.z;

        float[,] chunkArray = new float[chunkSize.x, chunkSize.z];

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int z = 0; z < chunkSize.z; z++)
            {
                if ((startX + x) < lengthX && (startZ + z) < lengthZ)
                {
                    chunkArray[x, z] = worldArray[startX + x, startZ + z];
                }
                else
                {
                    chunkArray[x, z] = 0f;
                }
            }

        }

        return chunkArray;
    }

    private int[,] ExtractChunkArray(int[,] worldArray, int chunkX, int chunkZ, int lengthX, int lengthZ)
    {

        int startX = chunkX * chunkSize.x;
        int startZ = chunkZ * chunkSize.z;

        int[,] chunkArray = new int[chunkSize.x, chunkSize.z];

        for (int x = 0; x < chunkSize.x; x++)
        {
            for (int z = 0; z < chunkSize.z; z++)
            {
                if ((startX + x) < lengthX && (startZ + z) < lengthZ)
                {
                    chunkArray[x, z] = worldArray[startX + x, startZ + z];
                }
                else
                {
                    chunkArray[x, z] = 0;
                }
            }

        }

        return chunkArray;
    }


}