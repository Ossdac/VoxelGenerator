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

                    meshData = GetFaceDataIn(direction, x, y, z, meshData, blockType);                
            }
        }

        return meshData;
    }

    public static MeshData GetFaceDataIn(Direction direction,
        int x, int y, int z, MeshData meshData, BlockType blockType)
    {
        GetFaceVertices(direction, x, y, z, meshData);
        meshData.AddQuadTriangles();
        meshData.uv.AddRange(FaceUVs(direction, blockType));


        return meshData;
    }

    public static void GetFaceVertices(Direction direction, int x, int y, int z, MeshData meshData)
    {
        switch (direction)
        {
            case Direction.backwards:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                break;
            case Direction.foreward:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                break;
            case Direction.left:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                break;

            case Direction.right:
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                break;
            case Direction.down:
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y - 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y - 0.5f, z + 0.5f));
                break;
            case Direction.up:
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z + 0.5f));
                meshData.AddVertex(new Vector3(x + 0.5f, y + 0.5f, z - 0.5f));
                meshData.AddVertex(new Vector3(x - 0.5f, y + 0.5f, z - 0.5f));
                break;
            default:
                break;
        }
    }

    public static Vector2[] FaceUVs(Direction direction, BlockType blockType)
    {
        Vector2[] UVs = new Vector2[4];
        var tilePos = TexturePosition(direction, blockType);

        UVs[0] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX,
            BlockDataManager.tileSizeY * tilePos.y);

        UVs[1] = new Vector2(BlockDataManager.tileSizeX * tilePos.x + BlockDataManager.tileSizeX,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY);

        UVs[2] = new Vector2(BlockDataManager.tileSizeX * tilePos.x,
            BlockDataManager.tileSizeY * tilePos.y + BlockDataManager.tileSizeY);

        UVs[3] = new Vector2(BlockDataManager.tileSizeX * tilePos.x,
            BlockDataManager.tileSizeY * tilePos.y);

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
