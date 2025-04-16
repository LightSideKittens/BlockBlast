using UnityEngine;
using System.Collections.Generic;

public class BlockSpawner : MonoBehaviour
{
    [Header("Список префабов блоков")]
    public List<GameObject> blockPrefabs = new List<GameObject>();

    [Header("Настройки спавна")]
    public Camera mainCamera;
    [Range(0f, 1f)] public float verticalOffsetPercent = 0.1f;
    [Range(0f, 0.5f)] public float horizontalMarginPercent = 0.1f; // по 10% слева и справа

    private MovingBlock lastBlock;

    void Start()
    {
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        if (lastBlock != null)
            lastBlock.Deactivate();

        if (blockPrefabs.Count == 0)
        {
            Debug.LogError("Список префабов блоков пуст!");
            return;
        }

        // Случайный блок из списка
        GameObject selectedPrefab = blockPrefabs[Random.Range(0, blockPrefabs.Count)];

        // Случайная X-позиция внутри "чистой" области (без краёв)
        float minX = horizontalMarginPercent;
        float maxX = 1f - horizontalMarginPercent;
        float randomX = Random.Range(minX, maxX);

        Vector3 randomPoint = mainCamera.ViewportToWorldPoint(
            new Vector3(randomX, 1f - verticalOffsetPercent, mainCamera.nearClipPlane + 10f)
        );

        Vector3 spawnPosition = new Vector3(randomPoint.x, randomPoint.y, 0f);

        GameObject block = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
        lastBlock = block.GetComponent<MovingBlock>();
        lastBlock.Init(this, mainCamera);
    }

    public void OnBlockLanded()
    {
        Invoke(nameof(SpawnNewBlock), 0.3f);
    }

#if UNITY_EDITOR
    // Визуальные границы спавна (только в редакторе)
    void OnDrawGizmos()
    {
        if (!mainCamera) return;

        Vector3 left = mainCamera.ViewportToWorldPoint(new Vector3(horizontalMarginPercent, 0.5f, 10f));
        Vector3 right = mainCamera.ViewportToWorldPoint(new Vector3(1f - horizontalMarginPercent, 0.5f, 10f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(left + Vector3.down * 20f, left + Vector3.up * 20f);
        Gizmos.DrawLine(right + Vector3.down * 20f, right + Vector3.up * 20f);
    }
#endif
}
