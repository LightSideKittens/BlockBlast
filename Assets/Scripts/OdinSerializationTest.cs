/*************************************************************
 *  Unity6_vs_Odin_ComplexTypes.cs
 *  Полиморфия через SerializeReference + эквиваленты словаря
 *************************************************************/

using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
#region ---------- ОБЩИЕ ТИПЫ ----------

// --- базовая и производные фигуры (абстрактная и конкретные) ---
[System.Serializable] public abstract class Shape { public string name; }
[System.Serializable] public class Circle    : Shape { public float radius; }
[System.Serializable] public class Rectangle : Shape { public float width, height; }

// --- интерфейс + реализации ---
public interface IAbility { }
[System.Serializable] public class Jump : IAbility { public float height; }
[System.Serializable] public class Fly  : IAbility { public float speed; }

// --- пара «ключ‑значение» для словаря Unity ---
[System.Serializable] public struct StringIntPair { public string key; public int value; }

#endregion


/*--------------------------------------------------------------
 *  UNITY 6 ВАРИАНТ  — SerializeReference + эквиваленты
 *------------------------------------------------------------*/


/*--------------------------------------------------------------
 *  ODIN ВАРИАНТ  — настоящие сложные коллекции
 *------------------------------------------------------------*/
#if ODIN_INSPECTOR

/// <summary>
/// Те же логические данные, но без ограничений Unity.
/// Все поля сериализуются через бинарный протокол Odin.
/// </summary>
public class OdinSerializationTest : SerializedMonoBehaviour
{
    [OdinSerialize, NonSerialized, ShowInInspector]
    private Dictionary<string,int> dict = new() { { "k1", 10 }, { "k2", 20 } };

    [OdinSerialize, NonSerialized, ShowInInspector]
    private HashSet<string> hashSet = new() { "A", "B", "C" };

    [OdinSerialize, NonSerialized, ShowInInspector]
    private Shape shape = new Circle { name = "Ball", radius = 1.5f };

    [OdinSerialize, NonSerialized, ShowInInspector]
    private List<Shape> shapeList = new()
    {
        new Circle    { name = "Small Ball", radius = 0.5f },
        new Rectangle { name = "Box",        width = 2, height = 1 }
    };

    [OdinSerialize, NonSerialized, ShowInInspector]
    private IAbility ability = new Jump { height = 3 };

    [OdinSerialize, NonSerialized, ShowInInspector]
    private List<IAbility> abilityList = new()
    {
        new Jump { height = 1.2f },
        new Fly  { speed  = 8  }
    };
}
#endif
