using System.Collections.Generic;
using UnityEngine;

public static class BlockHelper
{

    private static bool renderNothing;
    private static bool renderDown;

    public static bool RenderNothing { set { renderNothing = value; } }
    public static bool RenderDown { set { renderDown = value; } }

    private static Dictionary<Vector2, Vector2[]> uvCache = new Dictionary<Vector2, Vector2[]>();

    private static readonly Direction[] directions =
    {
        Direction.backwards,
        Direction.down,
        Direction.foreward,
        Direction.left,
        Direction.right,
        Direction.up
    };

    public static MeshData GetMeshData(ChunkData chunk, int x, int y, int z, MeshData meshData, BlockType blockType)
    {

        if (blockType == BlockType.Nothing || blockType == BlockType.Air)
            return meshData;



        foreach (Direction direction in directions)
        {
            var neighbourBlockCoordinates = new Vector3Int(x, y, z) + direction.GetVector();
            var neighbourBlockType = Chunk.GetBlockFromChunkCoordinates(chunk, neighbourBlockCoordinates);

            if (neighbourBlockType == BlockType.Air || neighbourBlockType == BlockType.Nothing && renderNothing &&
                (renderDown || !direction.Equals(Direction.down)))
            {

                meshData = GetFaceDataIn(direction, x, y, z, meshData, blockType, chunk);
            }
        }

        return meshData;
    }

    public static MeshData GetFaceDataIn(Direction direction,
        int x, int y, int z, MeshData meshData, BlockType blockType, ChunkData chunkData)
    {
        GetFaceVertices(direction, x, y, z, meshData, chunkData);
        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(direction, blockType));


        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData, ChunkData chunkData)
    {
        Vector3 blockSize = chunkData.blockSize;
        float xCenter = x * blockSize.x;
        float yCenter = y * blockSize.y;
        float zCenter = z * blockSize.z;
        float xCorner = blockSize.x / 2;
        float yCorner = blockSize.y / 2;
        float zCorner = blockSize.z / 2;
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter - yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter + yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter + yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter - yCorner, zCenter - zCorner));
                break;
            case Direction.foreward:
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter - yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter + yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter + yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter - yCorner, zCenter + zCorner));
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter - yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter + yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter + yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter - yCorner, zCenter - zCorner));
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter - yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter + yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter + yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter - yCorner, zCenter + zCorner));
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter - yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter - yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter - yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter - yCorner, zCenter + zCorner));
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter + yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter + yCorner, zCenter + zCorner));
                meshData.AddVertex(new Vector3(xCenter + xCorner, yCenter + yCorner, zCenter - zCorner));
                meshData.AddVertex(new Vector3(xCenter - xCorner, yCenter + yCorner, zCenter - zCorner));
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        Vector2 texturePosition = TexturePosition(direction, blockType);

        if (uvCache.TryGetValue(texturePosition, out Vector2[] cachedUVs))
        {
            return cachedUVs;
        }

        float tilePosX = texturePosition.x;
        float tilePosY = texturePosition.y;
        float tileSizeX = BlockDataManager.tileSizeX;
        float tileSizeY = BlockDataManager.tileSizeY;

        Vector2[] UVs = new Vector2[]
        {
        new(tileSizeX * tilePosX + tileSizeX, tileSizeY * tilePosY),
        new(tileSizeX * tilePosX + tileSizeX, tileSizeY * tilePosY + tileSizeY),
        new(tileSizeX * tilePosX, tileSizeY * tilePosY + tileSizeY),
        new(tileSizeX * tilePosX, tileSizeY * tilePosY)
        };

        uvCache.Add(texturePosition, UVs);

        return UVs;
    }


    public static Vector2 TexturePosition(Direction direction, BlockType blockType)
    {
        return direction switch
        {
            Direction.up => BlockDataManager.blockTextureDataDictionary[blockType].up,
            _ => BlockDataManager.blockTextureDataDictionary[blockType].side
        };
    }
}
