using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkData
{
    public BlockType[] blocks;
    public Vector3 blockSize = new(1, 1, 1);
    public Vector3Int chunkSize = new(16, 16, 16);
    public World worldReference;
    public Vector3Int worldPosition;


    public ChunkData(Vector3Int chunkSize, Vector3 blockSize, World world, Vector3Int worldPosition)
    {
        this.blockSize = blockSize;
        this.chunkSize = chunkSize;
        this.worldReference = world;
        this.worldPosition = worldPosition;
        blocks = new BlockType[chunkSize.x * chunkSize.y * chunkSize.z];
    }

}
