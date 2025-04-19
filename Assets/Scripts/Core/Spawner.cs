using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Shape> shapes;
    public List<Sprite> blockSprites;
    private Shape currentShape;

    public Shape SpawnRandomShape(ref int? lastUsedSpriteIndex)
    {
        
        if (shapes.Count == 0 || blockSprites.Count == 0)
            return null;

        var shapePrefab = shapes[Random.Range(0, shapes.Count)];
        var shapeInstance = Instantiate(shapePrefab, transform.position, Quaternion.identity);

        int newSpriteIndex;
        do
        {
            newSpriteIndex = Random.Range(0, blockSprites.Count);
        }
        while (blockSprites.Count > 1 && lastUsedSpriteIndex.HasValue && newSpriteIndex == lastUsedSpriteIndex.Value);

        var sprite = blockSprites[newSpriteIndex];
        shapeInstance.SetSprite(sprite);

        lastUsedSpriteIndex = newSpriteIndex;
        return shapeInstance;
    }
    
    public List<Shape> GetAllShapePrefabs()
    {
        return shapes;
    }

    public Shape SpawnSpecificShape(Shape shapePrefab, ref int? lastUsedSpriteIndex)
    {
        var shapeInstance = Instantiate(shapePrefab, transform.position, Quaternion.identity);

        int newSpriteIndex;
        do
        {
            newSpriteIndex = Random.Range(0, blockSprites.Count);
        }
        while (blockSprites.Count > 1 && lastUsedSpriteIndex.HasValue && newSpriteIndex == lastUsedSpriteIndex.Value);

        var sprite = blockSprites[newSpriteIndex];
        shapeInstance.SetSprite(sprite);
        lastUsedSpriteIndex = newSpriteIndex;

        return shapeInstance;
    }

}