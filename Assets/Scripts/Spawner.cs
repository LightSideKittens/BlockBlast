using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Shape> shapes;
    public List<Color> colors;
        
    public Shape SpawnRandomShape()
    {
        var shape = shapes[Random.Range(0, shapes.Count)];
        var shapeInstance = Instantiate(shape, transform.position, Quaternion.identity);
        shapeInstance.SetColor(colors[Random.Range(0, colors.Count)]);
        return shapeInstance;
    }
    
}
