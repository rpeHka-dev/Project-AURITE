using UnityEngine;
using UnityEngine.EventSystems;

public class HexCell : MonoBehaviour, IPointerClickHandler
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public int S { get; private set; }
    public GameObject currentBuilding { get; set; }
   
    // Температура гекса (-2, -1, 0, 1, 2)
    public int temperature;

    public void Setup(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
        transform.position = HexMetrics.GetPositionFromCube(q, r, s);
        gameObject.name = $"HexCell_{q}_{r}_{s}";
       
        // Вызываем расчет температуры сразу при создании гекса
        CalculateTemperature();
    }

    // Логика расчета температуры в зависимости от расстояния до центра
    public void CalculateTemperature()
    {
        int distance = (Mathf.Abs(Q) + Mathf.Abs(R) + Mathf.Abs(S)) / 2;

        if (distance <= 2)
            temperature = 2;
        else if (distance == 3)
            temperature = 1;
        else
            temperature = -2;
    }

    // --- ЗАГЛУШКА ДЛЯ РАСПРЕДЕЛИТЕЛЯ ТЕПЛА ---
    // Этот метод будет вызываться при постройке распределителя тепла.
    // Пока он просто увеличивает температуру текущего гекса на 1.
    public void IncreaseTemperatureFromDistributor()
    {
        // TODO: Реализовать полноценную логику (увеличение температуры в радиусе, проверка на дубли и т.д.)
        temperature += 1;
       
        // Пока просто выводим в консоль для проверки
        Debug.Log($"Температура гекса ({Q},{R},{S}) увеличена распределителем до {temperature}");
    }

    private void OnMouseDown()
    {
        Debug.Log($"Clicked hex: ({Q}, {R}, {S})");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick: ({Q}, {R}, {S})");
    }
}