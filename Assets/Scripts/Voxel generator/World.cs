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
    [SerializeField] private bool renderDown;

    Dictionary<Vector3Int, ChunkData> chunkDataDictionary = new Dictionary<Vector3Int, ChunkData>();
    Dictionary<Vector3Int, ChunkRenderer> chunkDictionary = new Dictionary<Vector3Int, ChunkRenderer>();

    private void Awake()
    {
        BlockHelper.RenderDown = renderDown;
    }

    [ContextMenu("Generate New World")]
    public void GenerateWorld()
    {
        ClearWorld();

        for (int x = 0; x < mapSizeInChunks; x++)
        {
            for (int z = 0; z < mapSizeInChunks; z++)
            {

                ChunkData data = new ChunkData(chunkSize, chunkHeight, this, new Vector3Int(x * chunkSize, 0, z * chunkSize));
                GenerateVoxels(data);
                chunkDataDictionary.Add(data.worldPosition, data);
            }
        }

        foreach (ChunkData data in chunkDataDictionary.Values)
        {
            MeshData meshData = Chunk.GetChunkMeshData(data);
            GameObject chunkObject = Instantiate(chunkPrefab, data.worldPosition, Quaternion.identity);
            ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
            chunkDictionary.Add(data.worldPosition, chunkRenderer);
            chunkRenderer.InitializeChunk(data);
            chunkRenderer.UpdateChunk(meshData);

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

        for (int x = 0; x <= chunkCountX; x++)
        {
            for (int z = 0; z <= chunkCountZ; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                ChunkData chunkData = new ChunkData(chunkSize, chunksY, this, chunkPosition);
                bool[,,] chunkArray = ExtractChunkArray(worldArray, x, z, chunkSize, chunksY, lengthX, lengthY, lengthZ);
                GenerateVoxelsFrom3DBoolArray(chunkData, chunkArray);
                CreateAndRegisterChunk(chunkData);
            }
        }
    }


    public void GenerateWorld(bool[,] worldArray)
    {
        ClearWorld(); 

        int chunkCountX = worldArray.GetLength(0) / chunkSize;
        int chunkCountZ = worldArray.GetLength(1) / chunkSize;

        for (int x = 0; x < chunkCountX; x++)
        {
            for (int z = 0; z < chunkCountZ; z++)
            {
                Vector3Int chunkPosition = new Vector3Int(x * chunkSize, 0, z * chunkSize);
                ChunkData chunkData = new ChunkData(chunkSize, chunkHeight, this, chunkPosition);
                GenerateVoxelsFrom2DBoolArray(chunkData, worldArray); 
                CreateAndRegisterChunk(chunkData);
            }
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
    }

    private void GenerateVoxels(ChunkData data)
    {
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int z = 0; z < data.chunkSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise((data.worldPosition.x + x) * noiseScale, (data.worldPosition.z + z) * noiseScale);
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



    private void GenerateVoxelsFrom3DBoolArray(ChunkData data, bool[,,] preMadeArray)
    {
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int y = 0; y < data.chunkHeight; y++)
            {
                for (int z = 0; z < data.chunkSize; z++)
                {
                    BlockType voxelType = preMadeArray[x, y, z] ? BlockType.Wall : BlockType.Air;
                    Chunk.SetBlock(data, new Vector3Int(x, y, z), voxelType);
                }
            }
        }
    }

    private void GenerateVoxelsFrom2DBoolArray(ChunkData data, bool[,] preMadeArray)
    {
        for (int x = 0; x < data.chunkSize; x++)
        {
            for (int y = 0; y < data.chunkHeight; y++)
            {
                for (int z = 0; z < chunkHeight; z++)
                {
                    BlockType voxelType = preMadeArray[x, y] ? BlockType.Wall : BlockType.Air;
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

    private void CreateAndRegisterChunk(ChunkData chunkData)
    {
        GameObject chunkObject = Instantiate(chunkPrefab, chunkData.worldPosition, Quaternion.identity);
        ChunkRenderer chunkRenderer = chunkObject.GetComponent<ChunkRenderer>();
        chunkRenderer.InitializeChunk(chunkData);
        chunkRenderer.UpdateChunk();
        chunkDataDictionary.Add(chunkData.worldPosition, chunkData);
        chunkDictionary.Add(chunkData.worldPosition, chunkRenderer);
    }

    private bool[,,] ExtractChunkArray(bool[,,] worldArray, int chunkX, int chunkZ, int chunkSize, int chunkHeight, int lengthX, int lengthY, int lengthZ)
    {
        // Calculate the start indices for X, Y, and Z dimensions.
        int startX = chunkX * chunkSize;
        int startY = 0; // Assuming Y starts from 0 for every chunk.
        int startZ = chunkZ * chunkSize;
        

        // Initialize the new array for the chunk.
        bool[,,] chunkArray = new bool[chunkSize, chunkHeight, chunkSize];

        // Iterate over each dimension and copy the values.
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    // Ensure we don't exceed the worldArray's bounds.
                    if ((startX + x) < lengthX && (startY + y) < lengthY && (startZ + z) < lengthZ)
                    {
                        chunkArray[x, y, z] = worldArray[startX + x, startY + y, startZ + z];
                    }
                    else
                    {
                        // Optionally handle out-of-bounds with a default value, e.g., false for no block.
                        chunkArray[x, y, z] = false;
                    }
                }
            }
        }

        return chunkArray;
    }


}