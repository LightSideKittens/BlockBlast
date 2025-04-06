using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public GameObject blockPrefab;
    public Camera mainCamera;
    [Range(0f, 1f)] public float verticalOffsetPercent = 0.1f;

    private MovingBlock lastBlock;

    void Start()
    {
        SpawnNewBlock();
    }

    public void SpawnNewBlock()
    {
        // Деактивируем предыдущий, если есть
        if (lastBlock != null)
        {
            lastBlock.Deactivate();
        }

        Vector3 topCenter = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f - verticalOffsetPercent, mainCamera.nearClipPlane + 10f));
        Vector3 spawnPosition = new Vector3(topCenter.x, topCenter.y, 0f);

        GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        lastBlock = block.GetComponent<MovingBlock>();
        lastBlock.Init(this, mainCamera);
    }

    public void OnBlockLanded()
    {
        Invoke(nameof(SpawnNewBlock), 0.3f);
    }
}
