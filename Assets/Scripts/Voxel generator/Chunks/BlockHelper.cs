using UnityEngine;

public static class BlockHelper
{

    private static bool renderNothing;
    private static bool renderDown;

    public static bool RenderNothing {set { renderNothing = value;}}
    public static bool RenderDown { set { renderDown = value; } }

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
        float xCorner = blockSize.x / 2;
        float yCorner = blockSize.y / 2;
        float zCorner = blockSize.z / 2;
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - xCorner, y - yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y + yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y + yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y - yCorner, z - zCorner));
                break;
            case Direction.foreward:
                meshData.AddVertex(new Vector3(x + xCorner, y - yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y + yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y + yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y - yCorner, z + zCorner));
                break; 
            case Direction.left:
                meshData.AddVertex(new Vector3(x - xCorner, y - yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y + yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y + yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y - yCorner, z - zCorner));
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + xCorner, y - yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y + yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y + yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y - yCorner, z + zCorner));
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - xCorner, y - yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y - yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y - yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y - yCorner, z + zCorner));
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - xCorner, y + yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y + yCorner, z + zCorner));
                meshData.AddVertex(new Vector3(x + xCorner, y + yCorner, z - zCorner));
                meshData.AddVertex(new Vector3(x - xCorner, y + yCorner, z - zCorner));
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        Vector2 tilePos = TexturePosition(direction, blockType);
        float tilePosX = tilePos.x;
        float tilePosY = tilePos.y;
        float tileSizeX = BlockDataManager.tileSizeX;
        float tileSizeY = BlockDataManager.tileSizeY;

        UVs[0] = new Vector2(tileSizeX * tilePosX + tileSizeX,
            tileSizeY * tilePosY);

        UVs[1] = new Vector2(tileSizeX * tilePosX + tileSizeX,
            tileSizeY * tilePosY + tileSizeY);

        UVs[2] = new Vector2(tileSizeX * tilePosX,
            tileSizeY * tilePosY + tileSizeY);

        UVs[3] = new Vector2(tileSizeX * tilePosX,
            tileSizeY * tilePosY);

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
