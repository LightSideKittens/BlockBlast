using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    public List<SpriteRenderer> blocks = new List<SpriteRenderer>();
    public Vector2Int ratio;
    public GameObject usedBlockPrefab;
    public Dictionary<SpriteRenderer, GameObject> blockPrefabMap = new();

    private void Awake()
    {
        blocks.Clear();
        foreach (Transform child in transform)
        {
            var sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
                blocks.Add(sr);
        }
    }

    public Shape CreateGhost(Sprite sprite)
    {
        var ghostObj = new GameObject("GhostShape");
        var ghost = ghostObj.AddComponent<Shape>();
        ghost.ratio = this.ratio;

        ghost.blocks = new List<SpriteRenderer>();

        foreach (var original in blocks)
        {
            var newBlockObj = new GameObject("GhostBlock");
            newBlockObj.transform.position = original.transform.position;
            newBlockObj.transform.SetParent(ghostObj.transform);

            var sr = newBlockObj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.color = new Color(1f, 1f, 1f, 0.4f); // прозрачный
            sr.sortingOrder = -1;

            ghost.blocks.Add(sr);
        }

        return ghost;
    }


    public void SetSprite(Sprite newSprite)
    {
        foreach (var block in blocks)
        {
            block.sprite = newSprite;
        }
    }

    
    public void ReplaceBlocks(GameObject newBlockPrefab)
    {
        blockPrefabMap.Clear();
        usedBlockPrefab = newBlockPrefab;

        List<SpriteRenderer> newBlocks = new();
        foreach (var old in blocks)
        {
            var newBlock = Instantiate(newBlockPrefab, old.transform.position, Quaternion.identity, transform);
            newBlocks.Add(newBlock.GetComponent<SpriteRenderer>());
            blockPrefabMap[newBlocks[^1]] = newBlockPrefab;
            Destroy(old.gameObject);
        }

        blocks = newBlocks;
    }
}
