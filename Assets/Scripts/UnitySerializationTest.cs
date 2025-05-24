using System.Collections.Generic;
using UnityEngine;

public class UnitySerializationTest : MonoBehaviour
{
    // 1. Dictionary эквивалент — список пар
    public List<StringIntPair> dictPairs = new()
    {
        new() { key = "k1", value = 10 },
        new() { key = "k2", value = 20 }
    };

    // 2. HashSet эквивалент — список строк
    public List<string> hashSetData = new() { "A", "B", "C" };

    // 3. Полиморфная ссылка (абстрактный класс)
    [SerializeReference] private Shape shape = new Circle { name = "Ball", radius = 1.5f };

    // 4. Список полиморфных объектов
    [SerializeReference] private List<Shape> shapeList = new()
    {
        new Circle    { name = "Small Ball", radius = 0.5f },
        new Rectangle { name = "Box", width = 2, height = 1 }
    };

    // 5. Поле‑интерфейс
    [SerializeReference] private IAbility ability = new Jump { height = 3 };

    // 6. Список интерфейсов
    [SerializeReference] private List<IAbility> abilityList = new()
    {
        new Jump { height = 1.2f },
        new Fly  { speed  = 8  }
    };
}