using UnityEngine;
using UnityEngine.EventSystems;

public class HexCell : MonoBehaviour, IPointerClickHandler
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public int S { get; private set; }
    public GameObject currentBuilding { get; set; }

    public void Setup(int q, int r, int s)
    {
        Q = q;
        R = r;
        S = s;
        transform.position = HexMetrics.GetPositionFromCube(q, r, s);
        gameObject.name = $"HexCell_{q}_{r}_{s}";
    }

    // Работает через старую систему ввода (не требует EventSystem)
    private void OnMouseDown()
    {
        Debug.Log($"Clicked hex: ({Q}, {R}, {S})");
    }

    // Работает через новую систему (требует EventSystem)
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"OnPointerClick: ({Q}, {R}, {S})");
    }
}