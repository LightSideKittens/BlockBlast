using System;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 previousGhostPosition; // Для проверки изменения позиции
    public Shape ghostShape;
    public FieldManager fieldManager;
    public event Action Started;
    public event Action Ended;

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    CheckTouch(touchPosition);
                    if (isDragging)
                    {
                        Started?.Invoke();

                        // Поднимаем порядок отрисовки всех блоков перетаскиваемой формы
                        var shape = GetComponent<Shape>();
                        if (shape != null)
                        {
                            foreach (var block in shape.blocks)
                            {
                                block.sortingOrder = 1;
                            }
                        }
                        // Инициализируем предыдущую позицию
                        previousGhostPosition = transform.position;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        transform.position = touchPosition + offset;
                        // Вызываем UpdateGhost только если позиция изменилась (чтобы не спамить вызовами)
                        if (fieldManager != null && Vector3.Distance(transform.position, previousGhostPosition) > 0.001f)
                        {
                            fieldManager.UpdateGhost(this.transform);
                            previousGhostPosition = transform.position;
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDragging)
                    {
                        Ended?.Invoke();
                        isDragging = false;
                    }
                    // Сбрасываем порядок отрисовки блоков до базового (0)
                    var currentShape = GetComponent<Shape>();
                    if (currentShape != null)
                    {
                        foreach (var block in currentShape.blocks)
                        {
                            block.sortingOrder = 0;
                        }
                    }
                    // Удаляем призрак, если он существует
                    if (ghostShape != null)
                    {
                        Destroy(ghostShape.gameObject);
                        ghostShape = null;
                    }
                    break;
            }
        }
    }

    private void CheckTouch(Vector3 touchPosition)
    {
        Collider2D hitCollider = Physics2D.OverlapPoint(touchPosition);
        if (hitCollider != null)
        {
            var go = hitCollider.gameObject;
            if (!Check())
            {
                if (hitCollider.transform.parent != null)
                {
                    go = hitCollider.transform.parent.gameObject;
                    Check();
                }
            }

            bool Check()
            {
                if (go == gameObject)
                {
                    isDragging = true;
                    offset = transform.position - touchPosition;
                    return true;
                }
                return false;
            }
        }
    }
}
