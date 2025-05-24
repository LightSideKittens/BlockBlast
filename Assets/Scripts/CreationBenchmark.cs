using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

public class CreationBenchmark : MonoBehaviour
{
     public AnimationCurve timeOffsetPerTarget = AnimationCurve.Constant(0, 0, 1623);
    public int iterations = 1000;
    public UnitySerializationTest unitySerializationTest;
    public OdinSerializationTest odinSerializationTest;

    private void Start() => StartCoroutine(Run());

    [Button]
    public void Test()
    {
        Debug.Log(timeOffsetPerTarget.Evaluate(0));
    }

    private IEnumerator Run()
    {
        // 1. Прогрев JIT — создаём по одному компоненту до старта таймера
        Instantiate(unitySerializationTest);
#if ODIN_INSPECTOR
        Instantiate(odinSerializationTest);
#endif
        yield return null;   // ждём конца кадра, чтобы JIT завершил работу

        // 2. Чистим GC, чтобы далее считать только новые аллокации
        ForceGC();

        // 3. Замер Unity‑варианта
        Measure(unitySerializationTest, "Unity‑serialization");

#if ODIN_INSPECTOR
        // 4. Дать движку один кадр «подышать» и снова чистим GC
        yield return null;
        ForceGC();

        // 5. Замер Odin‑варианта
        Measure(odinSerializationTest, "Odin Serializer");
#endif

        Debug.Log("<b>Benchmark completed.</b>");
    }

    // ---------------- helpers ----------------

    private void Measure(Object target, string label)
    {
        // Родитель‑контейнер, чтобы легко уничтожить все дочерние объекты после теста
        var parent = new GameObject($"{label}_Parent");

        long memBefore = GC.GetTotalMemory(true);
        var sw = Stopwatch.StartNew();

        for (int i = 0; i < iterations; i++)
        {
            Instantiate(target);
        }

        sw.Stop();
        long memAfter = GC.GetTotalMemory(true);

        // Вывод результатов
        UnityEngine.Debug.LogFormat(
            "<b>{0}</b> → {1}×AddComponent: {2:F2} мс   GC: {3:F1} KB",
            label,
            iterations,
            sw.Elapsed.TotalMilliseconds,
            (memAfter - memBefore) / 1024f);

        Destroy(parent);   // прибираемся
    }

    private static void ForceGC()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}
