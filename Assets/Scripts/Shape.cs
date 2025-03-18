using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public Vector2Int ratio;
    public List<SpriteRenderer> blocks = new();
    
    private void Awake()
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            blocks.Add(transform.GetChild(i).GetComponent<SpriteRenderer>());
        }
    }
    
    public void SetColor(Color color)
    {
        for (var i = 0; i < blocks.Count; i++)
        {
            blocks[i].color = color;
        }
    }
}