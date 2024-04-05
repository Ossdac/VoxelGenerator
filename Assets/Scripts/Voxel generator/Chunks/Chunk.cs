using System;
using UnityEngine;

public static class Chunk
{

    public static void LoopThroughTheBlocks(ChunkData chunkData, Action<int, int, int> actionToPerform)
    {
        for (int index = 0; index < chunkData.blocks.Length; index++)
        {
            var position = GetPostitionFromIndex(chunkData, index);
            actionToPerform(position.x, position.y, position.z);
        }
    }

    private static Vector3Int GetPostitionFromIndex(ChunkData chunkData, int index)
    {
        Vector3Int chunkSize = chunkData.chunkSize;
        
        int x = index % chunkSize.x;
        int y = (index /chunkSize.x) % chunkSize.y;
        int z = index / (chunkSize.x * chunkSize.y);
        return new Vector3Int(x, y, z);
    }

    private static bool InRange(ChunkData chunkData, Vector3 axisCoordinate)
    {
        Vector3Int chunkSize = chunkData.chunkSize;
        if ((axisCoordinate.x < 0 || axisCoordinate.x >= chunkSize.x) ||
            (axisCoordinate.y < 0 || axisCoordinate.y >= chunkSize.y) ||
            (axisCoordinate.z < 0 || axisCoordinate.z >= chunkSize.z))
            return false;

        return true;
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, Vector3Int chunkCoordinates)
    {
        return GetBlockFromChunkCoordinates(chunkData, chunkCoordinates.x, chunkCoordinates.y, chunkCoordinates.z);
    }

    public static BlockType GetBlockFromChunkCoordinates(ChunkData chunkData, int x, int y, int z)
    {
        if (InRange(chunkData, new Vector3Int(x, y, z)))
        {
            int index = GetIndexFromPosition(chunkData, x, y, z);
            return chunkData.blocks[index];
        }

        return chunkData.worldReference.GetBlockFromChunkCoordinates(chunkData.worldPosition.x + x, chunkData.worldPosition.y + y, chunkData.worldPosition.z + z);
    }

    public static void SetBlock(ChunkData chunkData, Vector3Int localPosition, BlockType block)
    {
        if (InRange(chunkData, localPosition))
        {
            int index = GetIndexFromPosition(chunkData, localPosition.x, localPosition.y, localPosition.z);
            chunkData.blocks[index] = block;
        }
        else
        {
            throw new Exception("Need to ask World for appropiate chunk");
        }
    }

    private static int GetIndexFromPosition(ChunkData chunkData, int x, int y, int z)
    {
        return x + chunkData.chunkSize.x * y + chunkData.chunkSize.x * chunkData.chunkSize.y * z;
    }

    public static Vector3Int GetBlockInChunkCoordinates(ChunkData chunkData, Vector3Int pos)
    {
        return new Vector3Int
        {
            x = pos.x - chunkData.worldPosition.x,
            y = pos.y - chunkData.worldPosition.y,
            z = pos.z - chunkData.worldPosition.z
        };
    }

    public static MeshData GetChunkMeshData(ChunkData chunkData)
    {
        MeshData meshData = new();

        LoopThroughTheBlocks(chunkData, (x, y, z) => meshData = BlockHelper.GetMeshData(chunkData, x, y, z, meshData, chunkData.blocks[GetIndexFromPosition(chunkData, x, y, z)]));


        return meshData;
    }

    internal static Vector3Int ChunkPositionFromBlockCoords(World world, int x, int y, int z)
    {
        Vector3Int chunkSize = world.chunkSize;
        
        Vector3Int pos = new()
        {
            x = Mathf.FloorToInt(x / (float)chunkSize.x) * chunkSize.x,
            y = Mathf.FloorToInt(y / (float)chunkSize.y) * chunkSize.y,
            z = Mathf.FloorToInt(z / (float)chunkSize.z) * chunkSize.z
        };
        return pos;
    }
}