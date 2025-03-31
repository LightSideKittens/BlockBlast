using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public List<Spawner> spawners;
    private Dictionary<SpriteRenderer, Color> originalColors;
    public Vector2Int gridSize;
    private Vector3 gridOffset;
    private SpriteRenderer[,] grid;
    public SpriteRenderer back;
    public bool debug;
    private int _spawnShapeLock = 0;
    private bool _allShapesPlaced;
    private Color shapeColor;
    private Shape currentGhostShape;

    private void Awake()
    {
        originalColors = new Dictionary<SpriteRenderer, Color>();

        grid = new SpriteRenderer[gridSize.x, gridSize.y];
        gridOffset = new Vector3(back.size.x / 2, back.size.y / 2);
        CreateAndInitShape();
    }

    private async void CreateAndInitShape()
    {
        foreach (var spawner in spawners)
        {
            var shape = spawner.SpawnRandomShape();
            Debug.Log(await CanPlace());
            var dragger = shape.GetComponent<Dragger>();
            var startPos = dragger.transform.position;
            dragger.Ended += OnEnd;
            dragger.Started += OnStart;
            shape.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            void OnStart()
            {
                shape.transform.localScale = new Vector3(1f, 1f, 1f);
                //var ghost = shape.CreateGhost(Color.gray); // или другой "призрачный" цвет
                //dragger.ghostShape = ghost;
                dragger.fieldManager = this;
            }

            void OnEnd()
            {
                if (currentGhostShape != null)
                {
                    Destroy(currentGhostShape.gameObject);
                    currentGhostShape = null;
                }

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
                                         && gridIndex.y > -1 && gridIndex.y < grid.GetLength(1)) // исправлено GetLength(0) -> GetLength(1)
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
                        var gridIndex = gridIndices[i];
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

                    _spawnShapeLock++;
                    if (_spawnShapeLock >= spawners.Count)
                    {
                        _spawnShapeLock = 0;
                        CreateAndInitShape();
                    }
                }
                else
                {
                    dragger.transform.position = startPos;
                    shape.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
    }

    public void UpdateGhost(Transform shapeTransform)
{
    // Удалим предыдущего призрака, если он остался
    if (currentGhostShape != null)
    {
        Destroy(currentGhostShape.gameObject);
        currentGhostShape = null;
    }

    // Получаем Shape с драггера
    var shape = shapeTransform.GetComponent<Shape>();
    if (shape == null || shape.blocks == null || shape.blocks.Count == 0) return;

    // Создаём нового призрака
    var ghost = shape.CreateGhost(Color.gray);
    currentGhostShape = ghost;

    var gridIndices = new List<Vector2Int>();
    var canPlace = true;

    for (var i = 0; i < shape.blocks.Count; i++)
    {
        var block = shape.blocks[i];
        var localPos = back.transform.InverseTransformPoint(block.transform.position);
        localPos += gridOffset;
        var gridIndex = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));
        gridIndices.Add(gridIndex);

        // Проверяем, не заняты ли ячейки
        if (gridIndex.x > -1 && gridIndex.x < grid.GetLength(0)
            && gridIndex.y > -1 && gridIndex.y < grid.GetLength(1))
        {
            if (grid[gridIndex.x, gridIndex.y] == null)
            {
                continue;
            }
        }

        canPlace = false;
    }

    if (!canPlace)
    {
        // 🔄 Сброс подсветки, даже если размещение невозможно
        if (originalColors != null)
        {
            foreach (var kvp in originalColors)
            {
                if (kvp.Key != null)
                {
                    kvp.Key.color = kvp.Value;
                }
            }
            originalColors.Clear();
        }

        Destroy(ghost.gameObject);
        currentGhostShape = null;
        return;
    }

    // Размещаем блоки призрака поверх ячеек
    for (int i = 0; i < ghost.blocks.Count; i++)
    {
        var gridIndex = gridIndices[i];
        Vector2 worldPos = gridIndex;
        worldPos -= (Vector2)gridOffset;
        worldPos = back.transform.TransformPoint(worldPos);

        ghost.blocks[i].transform.position = worldPos;
    }

    if (shape.blocks.Count > 0)
    {
        shapeColor = shape.blocks[0].color;
    }
    
    HighlightDestroyableLines(gridIndices, shapeColor);
}

    private void HighlightDestroyableLines(List<Vector2Int> futureIndices, Color highlightColor)

{
    // 🔄 Сброс подсветки
    foreach (var kvp in originalColors)
    {
        if (kvp.Key != null)
        {
            kvp.Key.color = kvp.Value;
        }
    }
    originalColors.Clear();

    int width = grid.GetLength(0);
    int height = grid.GetLength(1);

    // 💡 создаём логическую сетку занятости, вместо реальных объектов
    bool[,] simulatedOccupied = new bool[width, height];

    foreach (var index in futureIndices)
    {
        if (index.x >= 0 && index.x < width && index.y >= 0 && index.y < height)
        {
            simulatedOccupied[index.x, index.y] = true;
        }
    }

    // Метод для подсветки ячеек
    void HighlightCell(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return;

        var sprite = grid[x, y];
        if (sprite != null)
        {
            if (!originalColors.ContainsKey(sprite))
            {
                originalColors[sprite] = sprite.color;
            }
            sprite.color = highlightColor; 
        }
        else if (currentGhostShape != null && currentGhostShape.blocks != null)
        {
            foreach (var block in currentGhostShape.blocks)
            {
                if (block == null) continue;

                var localPos = back.transform.InverseTransformPoint(block.transform.position) + gridOffset;
                var ghostIndex = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));
                if (ghostIndex == new Vector2Int(x, y))
                {
                    block.color = highlightColor;
                }
            }
        }
    }

    // 🔷 Подсветка строк
    for (int y = 0; y < height; y++)
    {
        bool fullRow = true;
        for (int x = 0; x < width; x++)
        {
            if (!simulatedOccupied[x, y] && grid[x, y] == null)
            {
                fullRow = false;
                break;
            }
        }

        if (fullRow)
        {
            for (int x = 0; x < width; x++)
            {
                HighlightCell(x, y);
            }
        }
    }

    // 🔷 Подсветка колонн
    for (int x = 0; x < width; x++)
    {
        bool fullColumn = true;
        for (int y = 0; y < height; y++)
        {
            if (!simulatedOccupied[x, y] && grid[x, y] == null)
            {
                fullColumn = false;
                break;
            }
        }

        if (fullColumn)
        {
            for (int y = 0; y < height; y++)
            {
                HighlightCell(x, y);
            }
        }
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