using UnityEngine;

public static class HexMetrics
{
    // Ўирина и высота гекса (дл€ плоской сетки)
    public const float hexWidth = 2.15f;
    public const float hexHeight = 1.3f;

    // ѕолучить позицию гекса по кубическим координатам
    public static Vector3 GetPositionFromCube(int q, int r, int s)
    {
        float x = (q + r * 0.5f) * hexWidth;
        float y = r * hexHeight;
        return new Vector3(x, y, 0f);
    }
}