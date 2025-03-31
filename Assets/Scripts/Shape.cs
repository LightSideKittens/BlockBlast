using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public Vector2Int ratio;
    public List<SpriteRenderer> blocks;
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
    
    public Shape CreateGhost(Color ghostColor)
    {
        var ghost = Instantiate(this, transform.position, transform.rotation);

        // Принудительно обновим список блоков вручную:
        ghost.blocks = new List<SpriteRenderer>();
        for (var i = 0; i < ghost.transform.childCount; i++)
        {
            ghost.blocks.Add(ghost.transform.GetChild(i).GetComponent<SpriteRenderer>());
        }

        ghost.SetColor(ghostColor);
        foreach (var b in ghost.blocks)
            b.color = new Color(ghostColor.r, ghostColor.g, ghostColor.b, 0.4f);
        Destroy(ghost.GetComponent<Dragger>());
        return ghost;
    }

}