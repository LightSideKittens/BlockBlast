// using UnityEditor;
// using UnityEngine;
//
// [CustomEditor(typeof(Shape))]
// public class ShapeEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//
//         Shape shape = (Shape)target;
//
//         if (GUILayout.Button("🔄 Обновить координаты блоков"))
//         {
//             shape.cells.Clear();
//             shape.blocks.Clear();
//
//             for (int i = 0; i < shape.transform.childCount; i++)
//             {
//                 var child = shape.transform.GetChild(i);
//                 Vector2Int cellPos = Vector2Int.RoundToInt(child.localPosition);
//                 shape.cells.Add(cellPosition);
//                 shape.blocks.Add(child.GetComponent<SpriteRenderer>());
//
//                 EditorUtility.SetDirty(shape);
//             }
//         }
//     }
// }