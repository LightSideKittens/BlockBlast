using UnityEngine;

public class MovingBlock : MonoBehaviour
{
    public float moveSpeed = 2f;
    private float leftLimit;
    private float rightLimit;

    private bool isActiveBlock = true; // ← Только активный блок двигается
    private bool isFalling = false;
    private bool landed = false;

    private Rigidbody2D rb;
    private BlockSpawner spawner;
    private Vector3 startPos;
    private float direction = 1f;

    public void Init(BlockSpawner spawner, Camera cam)
    {
        this.spawner = spawner;
        rb = GetComponent<Rigidbody2D>();
        rb.isKinematic = true;

        // Установка границ
        Vector3 leftWorld = cam.ViewportToWorldPoint(new Vector3(0.1f, 0, cam.nearClipPlane + 10f));
        Vector3 rightWorld = cam.ViewportToWorldPoint(new Vector3(0.9f, 0, cam.nearClipPlane + 10f));

        leftLimit = leftWorld.x;
        rightLimit = rightWorld.x;

        startPos = transform.position;
        isActiveBlock = true; // этот блок начинает как активный
    }

    void Update()
    {
        if (isActiveBlock && !isFalling)
        {
            // Движение по X
            transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

            if (transform.position.x <= leftLimit || transform.position.x >= rightLimit)
                direction *= -1f;

            if (Input.GetMouseButtonDown(0))
            {
                Drop();
            }
        }
    }

    void Drop()
    {
        isFalling = true;
        isActiveBlock = false;
        rb.isKinematic = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!landed && isFalling)
        {
            landed = true;
            spawner.OnBlockLanded();
        }
    }

    // Новый метод, вызывается извне, чтобы этот блок не был активен
    public void Deactivate()
    {
        isActiveBlock = false;
    }
}
