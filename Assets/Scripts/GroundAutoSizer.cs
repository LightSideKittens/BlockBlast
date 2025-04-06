using UnityEngine;

public class GroundAutoSizer : MonoBehaviour
{
    public float height = 0.5f; // Толщина земли

    void Start()
    {
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col == null)
        {
            col = gameObject.AddComponent<BoxCollider2D>();
        }

        Camera cam = Camera.main;
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0f, 0f, 10f));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1f, 0f, 10f));
        Vector3 bottom = cam.ViewportToWorldPoint(new Vector3(0.5f, 0f, 10f));

        float width = right.x - left.x;

        col.size = new Vector2(width, height);
        transform.position = new Vector2(bottom.x, bottom.y - height / 2f); // чуть ниже экрана
    }
}
