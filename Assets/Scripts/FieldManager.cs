using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
public Spawner spawner;
public Vector2Int gridSize;
private Vector3 gridOffset;
private SpriteRenderer[,] grid;
public SpriteRenderer back;
public bool debug;
private void Awake()
{
    grid = new SpriteRenderer[gridSize.x, gridSize.y];
    gridOffset = new Vector3(back.size.x / 2, back.size.y / 2);
    CreateAndInitShape();
}

private async void CreateAndInitShape()
{
    var shape = spawner.SpawnRandomShape();
    Debug.Log(await CanPlace());
    var dragger = shape.GetComponent<Dragger>();
    var startPos = dragger.transform.position;
    dragger.Ended += OnEnd;

    void OnEnd()
    {
        var gridIndices = new List<Vector2Int>();
        var canPlace = true;
        for (var i = 0; i < shape.blocks.Count; i++)
        {
            var block = shape.blocks[i];
            var localPos = back.transform.InverseTransformPoint(block.transform.position);
            localPos += gridOffset;
            var gridIndex = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));
            gridIndices.Add(gridIndex);
            if (gridIndex.x > -1 && gridIndex.x < grid.GetLength(0) 
              && gridIndex.y > -1 && gridIndex.y < grid.GetLength(0))
            {
                if (grid[gridIndex.x, gridIndex.y] == null)
                {
                    continue;
                }
            }
            canPlace = false;
        }
        if (canPlace)
        {
            for (var i = 0; i < shape.blocks.Count; i++)
            {
                var gridIndex = gridIndices[i];;
                Vector2 worldPos = gridIndex;
                worldPos -= (Vector2)gridOffset;
                
                var block = shape.blocks[i];
                
                grid[gridIndex.x, gridIndex.y] = block;
                
                block.transform.parent = null;
                worldPos = back.transform.TransformPoint(worldPos);
                block.transform.position = worldPos;
            }
            Destroy(dragger);
            CheckAndDestroyBlocks();
            CreateAndInitShape();
        }
        else
        {
           dragger.transform.position = startPos;
        }
    }

    async Task<bool> CanPlace()
    {
        Vector2 r = shape.ratio;
        var sp = shape.transform.position;
        var tp = back.transform.position - gridOffset + ((Vector3)r / 2);
        shape.transform.position = tp;
            
        int xCount = grid.GetLength(0) - (shape.ratio.x - 1);
        int yCount = grid.GetLength(1) - (shape.ratio.y - 1);
            
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                shape.transform.position = tp + new Vector3(x, y);
                if (debug)
                {
                    await Task.Delay(100);
                }
                var check = Check();
                shape.transform.position = sp;
                if (check)
                {
                    return true;
                }
            }
        }
            
        shape.transform.position = sp;
        return false;
    }

    bool Check()
    {
        var canPlace = true;
        for (var i = 0; i < shape.blocks.Count; i++)
        {
            var block = shape.blocks[i];
            var localPos = back.transform.InverseTransformPoint(block.transform.position);
            localPos += gridOffset;
            var gridIndex = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));
            if (gridIndex.x > -1 && gridIndex.x < grid.GetLength(0) 
                                 && gridIndex.y > -1 && gridIndex.y < grid.GetLength(0))
            {
                if (grid[gridIndex.x, gridIndex.y] == null)
                {
                    continue;
                }
            }
            canPlace = false;
            break;
        }
        return canPlace;
    }
}

private void CheckAndDestroyBlocks()
{
    CheckAndDestroyBlocks(false);
    CheckAndDestroyBlocks(true);
}

private void CheckAndDestroyBlocks(bool checkColumns)
{
    int x;
    int y;
    int xCount = checkColumns ? grid.GetLength(1) : grid.GetLength(0);
    int yCount = checkColumns ? grid.GetLength(0) : grid.GetLength(1);
    
    for (int i = 0; i < xCount; i++)
    {
        x = i;
        bool isAll = true;
        
        for (int j = 0; j < yCount; j++)
        {
            x = i;
            y = j;
            if (checkColumns) (x, y) = (y, x);
            if (grid[x, y] == null)
            {
                isAll = false;
                break;
            }
        }

        if (isAll)
        {
            for (int j = 0; j < yCount; j++)
            {
                x = i;
                y = j;
                if (checkColumns) (x, y) = (y, x);
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }
}
}