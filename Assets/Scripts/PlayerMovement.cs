using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")] 
    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(moveX, moveY).normalized;

        // Перемещение
        transform.position += (Vector3)moveDir * moveSpeed * Time.deltaTime;

        // Поворот по направлению движения
        if (moveDir != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f); // -90, если спрайт изначально "смотрит вверх"
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}