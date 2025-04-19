using System;
using UnityEngine;

public class Dragger : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private Vector3 previousGhostPosition;
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
                        var shape = GetComponent<Shape>();
                        if (shape != null)
                        {
                            foreach (var block in shape.blocks)
                            {
                                block.sortingOrder = 2;
                            }
                        }

                        previousGhostPosition = transform.position;
                    }

                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        transform.position = touchPosition + offset;
                        if (fieldManager != null &&
                            Vector3.Distance(transform.position, previousGhostPosition) > 0.001f)
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

                    var currentShape = GetComponent<Shape>();
                    if (currentShape != null)
                    {
                        foreach (var block in currentShape.blocks)
                        {
                            block.sortingOrder = 0;
                        }
                    }

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