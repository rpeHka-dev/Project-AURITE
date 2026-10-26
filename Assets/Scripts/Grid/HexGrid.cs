using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private GameObject hexCellPrefab;
    [SerializeField] private int gridRadius = 3;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        GameObject gridParent = new GameObject("HexGrid");

        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            for (int r = -gridRadius; r <= gridRadius; r++)
            {
                int s = -q - r;
                if (Mathf.Abs(s) > gridRadius) continue;

                GameObject hexObject = Instantiate(hexCellPrefab, gridParent.transform);
                HexCell cell = hexObject.GetComponent<HexCell>();
                cell.Setup(q, r, s);
                Debug.Log($"Created hex ({q},{r},{s}) at {cell.transform.position}");
            }
        }
    }
}