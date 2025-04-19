using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    [SerializeField] private DifficultyManager difficultyManager;
    public List<Spawner> spawners;
    private List<Shape> activeShapes = new();
    private Dictionary<SpriteRenderer, Sprite> originalSprites;
    public Vector2Int gridSize;
    
    public float defaultShapeSize = 0.85f;
    
    private Vector3 gridOffset;
    private SpriteRenderer[,] grid;
    public SpriteRenderer back;
    public bool debug;
    private int spawnShapeLock = 0;
    private bool allShapesPlaced;
    private Color shapeColor;
    private Shape currentGhostShape;
    public ScoreManager scoreManager;
    private int? lastUsedSpriteIndex = null;

    private void Awake()
    {
        originalSprites = new Dictionary<SpriteRenderer, Sprite>();
        grid = new SpriteRenderer[gridSize.x, gridSize.y];
        gridOffset = new Vector3(back.size.x / 2, back.size.y / 2);
        CreateAndInitShape();
    }

    private async void CreateAndInitShape()
    {
        int totalSpawners = spawners.Count;

        // 1. Получаем текущий уровень сложности от 0 до 1
        float difficulty = difficultyManager.GetDifficultyValue();

        // 2. Собираем все префабы фигур
        List<Shape> allShapes = new();
        foreach (var spawner in spawners)
            allShapes.AddRange(spawner.GetAllShapePrefabs());

        // 3. Ищем фигуры, которые реально можно поставить
        List<Shape> validShapes = new();
        foreach (var shapePrefab in allShapes)
        {
            Shape temp = Instantiate(shapePrefab, transform.position, Quaternion.identity);
            bool canPlace = await CanPlace(temp);
            Destroy(temp.gameObject);
            if (canPlace)
                validShapes.Add(shapePrefab);
        }

        // 4. Выбираем спавнер, который гарантированно получит подходящую фигуру
        int guaranteedIndex = validShapes.Count > 0 ? Random.Range(0, totalSpawners) : -1;

        // 5. Спавним фигуры
        for (int i = 0; i < totalSpawners; i++)
        {
            var spawner = spawners[i];
            Shape shape;

            if (i == guaranteedIndex)
            {
                shape = spawner.SpawnSpecificShape(validShapes[Random.Range(0, validShapes.Count)],
                    ref lastUsedSpriteIndex);
                Debug.Log($"[SPAWN] Спавнер {i}: ГАРАНТИРОВАННАЯ фигура");
            }
            else
            {
                shape = spawner.SpawnRandomShape(ref lastUsedSpriteIndex);
                Debug.Log($"[SPAWN] Спавнер {i}: случайная фигура (сложность {difficulty:F2})");
            }

            activeShapes.Add(shape);

            var dragger = shape.GetComponent<Dragger>();
            var startPos = shape.transform.position;
            shape.transform.localScale = Vector3.one * defaultShapeSize;

            dragger.Started += () =>
            {
                shape.transform.localScale = Vector3.one;
                dragger.fieldManager = this;
            };

            dragger.Ended += () =>
            {
                if (currentGhostShape != null)
                {
                    Destroy(currentGhostShape.gameObject);
                    currentGhostShape = null;
                }

                var gridIndices = new List<Vector2Int>();
                var canPlace = true;

                for (var j = 0; j < shape.blocks.Count; j++)
                {
                    var block = shape.blocks[j];
                    var localPos = back.transform.InverseTransformPoint(block.transform.position);
                    localPos += gridOffset;
                    var gridIndex = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));
                    gridIndices.Add(gridIndex);

                    if (gridIndex.x >= 0 && gridIndex.x < grid.GetLength(0)
                                         && gridIndex.y >= 0 && gridIndex.y < grid.GetLength(1))
                    {
                        if (grid[gridIndex.x, gridIndex.y] == null)
                            continue;
                    }

                    canPlace = false;
                }

                if (canPlace)
                {
                    for (int j = 0; j < shape.blocks.Count; j++)
                    {
                        var gridIndex = gridIndices[j];
                        Vector2 worldPos = gridIndex - (Vector2)gridOffset;

                        var block = shape.blocks[j];
                        grid[gridIndex.x, gridIndex.y] = block;

                        block.transform.parent = null;
                        block.transform.position = back.transform.TransformPoint(worldPos);
                    }

                    activeShapes.Remove(shape);
                    Destroy(dragger);

                    difficultyManager.OnShapePlaced();
                    CheckAndDestroyBlocks();
                    CheckLoseCondition();

                    spawnShapeLock++;
                    if (spawnShapeLock >= spawners.Count)
                    {
                        spawnShapeLock = 0;
                        CreateAndInitShape();
                    }
                }
                else
                {
                    shape.transform.DOMove(Vector3.zero, 0.3f);

                    SpriteRenderer s = null;
                    
                    
                    
                    shape.transform.localScale = Vector3.one * defaultShapeSize;
                }
            };
        }
    }

    public async Task<bool> CanPlace(Shape shape)
    {
        var prevScale = shape.transform.localScale;
        var prevPos = shape.transform.position;

        var result = await CheckCanPlace();
        
        shape.transform.localScale = prevScale;
        shape.transform.position = prevPos;
        
        return result;
        
        async Task<bool> CheckCanPlace()
        {
            shape.transform.localScale = Vector3.one;

            Vector2 r = shape.ratio;
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

                    if (Check(shape))
                    {
                        return true;
                    }
                }
            }
        
            return false;
        }
    }

    private bool Check(Shape shape)
    {
        for (var i = 0; i < shape.blocks.Count; i++)
        {
            var block = shape.blocks[i];
            var localPos = back.transform.InverseTransformPoint(block.transform.position);
            localPos += gridOffset;
            var gridIndex = new Vector2Int(Mathf.RoundToInt(localPos.x), Mathf.RoundToInt(localPos.y));

            if (gridIndex.x < 0 || gridIndex.x >= grid.GetLength(0)
                                || gridIndex.y < 0 || gridIndex.y >= grid.GetLength(1)
                                || grid[gridIndex.x, gridIndex.y] != null)
            {
                return false;
            }
        }

        return true;
    }

    private async void CheckLoseCondition()
    {
        if (activeShapes.Count == 0)
            return;

        foreach (var shape in activeShapes)
        {
            if (shape == null) continue;

            if (await CanPlace(shape))
                return;
        }

        Debug.Log("Поражение: ни одна из оставшихся фигур не может быть размещена.");
        // TODO: проигрыш
    }

    private async Task<bool> SimulateCanPlace(Shape shape)
    {
        Vector2 r = shape.ratio;
        var originalPosition = shape.transform.position;
        var tp = back.transform.position - gridOffset + ((Vector3)r / 2);

        int xCount = grid.GetLength(0) - (shape.ratio.x - 1);
        int yCount = grid.GetLength(1) - (shape.ratio.y - 1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                shape.transform.position = tp + new Vector3(x, y);
                if (debug) await Task.Delay(100);

                if (Check(shape))
                {
                    shape.transform.position = originalPosition;
                    return true;
                }
            }
        }

        shape.transform.position = originalPosition;
        return false;
    }


    public void UpdateGhost(Transform shapeTransform)
    {
        if (currentGhostShape != null)
        {
            Destroy(currentGhostShape.gameObject);
            currentGhostShape = null;
        }

        var shape = shapeTransform.GetComponent<Shape>();
        if (shape == null || shape.blocks == null || shape.blocks.Count == 0) return;

        var sprite = shape.blocks[0].sprite;
        var ghost = shape.CreateGhost(sprite);
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
            if (originalSprites != null)
            {
                foreach (var kvp in originalSprites)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.sprite = kvp.Value;
                    }
                }

                originalSprites.Clear();
            }

            Destroy(ghost.gameObject);
            currentGhostShape = null;
            return;
        }

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
        foreach (var kvp in originalSprites)
        {
            if (kvp.Key != null)
            {
                kvp.Key.sprite = kvp.Value;
            }
        }

        originalSprites.Clear();

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        bool[,] simulatedOccupied = new bool[width, height];

        foreach (var index in futureIndices)
        {
            if (index.x >= 0 && index.x < width && index.y >= 0 && index.y < height)
            {
                simulatedOccupied[index.x, index.y] = true;
            }
        }

        void HighlightCell(int x, int y)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;

            var sprite = grid[x, y];
            if (sprite != null)
            {
                if (!originalSprites.ContainsKey(sprite))
                {
                    originalSprites[sprite] = sprite.sprite;
                }

                sprite.sprite = currentGhostShape.blocks[0].sprite;
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
        int totalDestroyed = 0;
        totalDestroyed += CheckAndDestroyBlocks(false);
        totalDestroyed += CheckAndDestroyBlocks(true);

        if (scoreManager != null)
        {
            bool wasLineBroken = totalDestroyed > 0;
            scoreManager.AddScore(totalDestroyed, wasLineBroken);
        }
    }

    private int CheckAndDestroyBlocks(bool checkColumns)
    {
        int x, y;
        int destroyedLines = 0;

        int xCount = checkColumns ? grid.GetLength(1) : grid.GetLength(0);
        int yCount = checkColumns ? grid.GetLength(0) : grid.GetLength(1);

        for (int i = 0; i < xCount; i++)
        {
            bool isFullLine = true;

            for (int j = 0; j < yCount; j++)
            {
                x = i;
                y = j;
                if (checkColumns) (x, y) = (y, x);
                if (grid[x, y] == null)
                {
                    isFullLine = false;
                    break;
                }
            }

            if (isFullLine)
            {
                for (int j = 0; j < yCount; j++)
                {
                    x = i;
                    y = j;
                    if (checkColumns) (x, y) = (y, x);
                    Destroy(grid[x, y].gameObject);
                    grid[x, y] = null;
                }

                destroyedLines++;
            }
        }

        return destroyedLines;
    }
}