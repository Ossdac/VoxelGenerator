using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class World : MonoBehaviour
{
    public int mapSizeInChunks = 6;
    public int chunkSize = 16, chunkHeight = 100;
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

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {
                ChunkData chunkData = new ChunkData(chunkSize, chunkHeight, 
                    this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(chunkData);
                chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
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
        int chunkCountX = Mathf.CeilToInt((float)lengthX / chunkSize);
        int chunkCountZ = Mathf.CeilToInt((float)lengthZ / chunkSize);

        for (int x = 0; x < chunkCountX; x++)
        {
            for (int z = 0; z < chunkCountZ; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                ChunkData chunkData = new ChunkData(chunkSize, chunkHeight, this, chunkPosition);
                bool[,] chunkArray = ExtractChunkArray(worldArray, x, z, chunkSize, lengthX, lengthZ);
                GenerateVoxelsFrom2DBoolArray(chunkData, chunkArray);
                chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
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
        int chunkCountX = lengthX / chunkSize;
        int chunksY = lengthY;
        int chunkCountZ = lengthZ / chunkSize;

        chunkHeight = chunksY;

        for (int x = 0; x <= chunkCountX; x++)
        {
            for (int z = 0; z <= chunkCountZ; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                ChunkData chunkData = new ChunkData(chunkSize, chunksY, this, chunkPosition);
                bool[,,] chunkArray = ExtractChunkArray(worldArray, x, z, chunkSize, chunksY, lengthX, lengthY, lengthZ);
                GenerateVoxelsFrom3DBoolArray(chunkData, chunkArray);
                chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
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
        int chunkCountX = Mathf.CeilToInt((float)lengthX / chunkSize);
        int chunkCountZ = Mathf.CeilToInt((float)lengthZ / chunkSize);

        for (int x = 0; x < chunkCountX; x++)
        {
            for (int z = 0; z < chunkCountZ; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                ChunkData chunkData = new ChunkData(chunkSize, chunkHeight, this, chunkPosition);
                int[,] chunkArray = ExtractChunkArray(worldArray, x, z, chunkSize, lengthX, lengthZ);
                GenerateVoxelsFrom2DIntArray(chunkData, chunkArray);
                chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
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
        int chunkCountX = worldArray.GetLength(0) / chunkSize;
        int chunkCountZ = worldArray.GetLength(1) / chunkSize;

        for (int x = 0; x <= chunkCountX; x++)
        {
            for (int z = 0; z <= chunkCountZ; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                ChunkData chunkData = new ChunkData(chunkSize, chunkHeight, this, chunkPosition);
                float[,] chunkArray = ExtractChunkArray(worldArray, x, z, chunkSize, lengthX, lengthZ);
                GenerateVoxelsFrom2DFloatArray(chunkData, chunkArray);
                chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
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
            Destroy(chunk.gameObject);
        }
        chunkDictionary.Clear();
        BlockHelper.RenderNothing = renderNothing;
        BlockHelper.RenderDown = renderDown;
    }

    private void GenerateVoxels(ChunkData data)
    {
        int chunkSize = data.chunkSize;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPosition.x + x) * 
                    noiseScale, (data.worldPosition.z + z) * noiseScale);
                int groundPosition = Mathf.RoundToInt(noiseValue * chunkHeight);
                for (int y = 0; y < chunkHeight; y++)
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
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxelsFrom2DBoolArray(ChunkData data, bool[,] preMadeArray)
    {
        int chunkSize = data.chunkSize;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                BlockType voxelType = preMadeArray[x, z] ? BlockType.Wall : BlockType.Air;
                for (int y = 0; y < chunkHeight; y++)
                {
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxelsFrom3DBoolArray(ChunkData data, bool[,,] preMadeArray)
    {
        int chunkSize = data.chunkSize;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    BlockType voxelType = preMadeArray[x, y, z] ? BlockType.Wall : BlockType.Air;
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    
    private void GenerateVoxelsFrom2DFloatArray(ChunkData data, float[,] preMadeArray)
    {
        int chunkSize = data.chunkSize;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int groundPosition = Mathf.RoundToInt(preMadeArray[x,z] * chunkHeight);
               
                for (int y = 0; y < chunkHeight; y++)
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
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxelsFrom2DIntArray(ChunkData data, int[,] preMadeArray)
    {
        int chunkSize = data.chunkSize;
        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int groundPosition = preMadeArray[x, z];
                BlockType voxelType = BlockType.Air;
                for (int y = chunkHeight - 1; y >= 0; y--)
                {
                    if (groundPosition != 0 && y <= groundPosition)
                        voxelType = BlockType.Wall;
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);                    
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
        GameObject chunkObject = Instantiate(chunkPrefab, chunkData.worldPosition, Quaternion.identity);
        ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        chunkDictionary.Add(chunkData.worldPosition, chunkRenderer);
        chunkRenderer.InitializeChunk(chunkData);
        chunkRenderer.RenderMesh(meshData);
    }

    private bool[,,] ExtractChunkArray(bool[,,] worldArray, int chunkX, int chunkZ, int chunkSize, int chunkHeight, int lengthX, int lengthY, int lengthZ)
    {
       
        int startX = chunkX * chunkSize;
        int startY = 0; 
        int startZ = chunkZ * chunkSize;
        

        bool[,,] chunkArray = new bool[chunkSize, chunkHeight, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
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

    private bool[,] ExtractChunkArray(bool[,] worldArray, int chunkX, int chunkZ, int chunkSize, int lengthX, int lengthZ)
    {

        int startX = chunkX * chunkSize;
        int startZ = chunkZ * chunkSize;

        bool[,] chunkArray = new bool[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
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

    private float[,] ExtractChunkArray(float[,] worldArray, int chunkX, int chunkZ, int chunkSize, int lengthX, int lengthZ)
    {

        int startX = chunkX * chunkSize;
        int startZ = chunkZ * chunkSize;

        float[,] chunkArray = new float[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
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

    private int[,] ExtractChunkArray(int[,] worldArray, int chunkX, int chunkZ, int chunkSize, int lengthX, int lengthZ)
    {

        int startX = chunkX * chunkSize;
        int startZ = chunkZ * chunkSize;

        int[,] chunkArray = new int[chunkSize, chunkSize];

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
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