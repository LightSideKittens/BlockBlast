using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Shape> shapes;
    public List<Sprite> blockSprites;

    public Shape SpawnRandomShape()
    {
        if (shapes.Count == 0 || blockSprites.Count == 0)
            return null;

        var shapePrefab = shapes[Random.Range(0, shapes.Count)];
        var shapeInstance = Instantiate(shapePrefab, transform.position, Quaternion.identity);

        var sprite = blockSprites[Random.Range(0, blockSprites.Count)];
        shapeInstance.SetSprite(sprite);

        return shapeInstance;
    }
}