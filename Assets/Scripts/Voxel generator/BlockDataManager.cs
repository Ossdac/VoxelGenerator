using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockDataManager : MonoBehaviour
{
    public static float tileSizeX, tileSizeY;
    public static Dictionary<BlockType, TextureData> blockTextureDataDictionary = new();
    public BlockDataSO textureData;

    private void Awake()
    {
        foreach (TextureData item in textureData.textureDataList)
        {
            if (blockTextureDataDictionary.ContainsKey(item.blockType) == false)
            {
                blockTextureDataDictionary.Add(item.blockType, item);
            };
        }
        tileSizeX = 1f / textureData.textureSizeX;
        tileSizeY = 1f / textureData.textureSizeY;
    }
}
