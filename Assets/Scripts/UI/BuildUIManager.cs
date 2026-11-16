using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BuildUIManager : MonoBehaviour
{
    [Header("Панель")]
    public Button btnOpenBuild;
    public Button btnCloseBuild;
    public GameObject buildPanel;

    [Header("Карточки")]
    public Transform cardsContent; // Content внутри BuildScroll
    public GameObject buildingCardPrefab;

    [Header("Здания")]
    public BuildingData[] buildings;

    [Header("Подсказка (необязательно)")]
    public GameObject hintPanel;
    public TMP_Text hintText;

    private BuildingData selectedBuilding;

    void Start()
    {
        btnOpenBuild.onClick.AddListener(TogglePanel);
        if (btnCloseBuild != null)
            btnCloseBuild.onClick.AddListener(ClosePanel);

        buildPanel.SetActive(false);
        ShowHint("");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && selectedBuilding != null)
            CancelSelection();

        // Если мышь над интерфейсом, гексы не трогаем
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // ЛКМ - построить выбранное здание
        if (Input.GetMouseButtonDown(0))
        {
            HexCell cell = GetCellUnderMouse();
            if (cell != null)
                TryBuild(cell);
        }

        // ПКМ - снести здание и вернуть ресурсы
        if (Input.GetMouseButtonDown(1))
        {
            HexCell cell = GetCellUnderMouse();
            if (cell != null && cell.buildingData != null)
                DeleteBuilding(cell);
        }
    }

    // === Панель ===

    public void TogglePanel()
    {
        if (buildPanel.activeSelf)
            ClosePanel();
        else
            OpenPanel();
    }

    void OpenPanel()
    {
        buildPanel.SetActive(true);
        CreateCards();
    }

    void ClosePanel()
    {
        buildPanel.SetActive(false);
    }

    void CreateCards()
    {
        // Удаляем старые карточки, чтобы не было дублей
        foreach (Transform child in cardsContent)
            Destroy(child.gameObject);

        foreach (BuildingData building in buildings)
        {
            GameObject card = Instantiate(buildingCardPrefab, cardsContent);
            card.GetComponent<BuildingCard>().Setup(building, SelectBuilding);
        }
    }

    void SelectBuilding(BuildingData building)
    {
        selectedBuilding = building;
        ClosePanel();
        ShowHint($"{building.buildingName}: кликните по гексу, чтобы построить. Esc - отмена");
        Debug.Log($"Выбрано: {building.buildingName}. Кликните по гексу, чтобы построить");
    }

    void CancelSelection()
    {
        selectedBuilding = null;
        ShowHint("");
    }

    void ShowHint(string text)
    {
        if (hintPanel == null || hintText == null) return;

        hintText.text = text;
        hintPanel.SetActive(text != "");
    }

    // === Постройка и удаление ===

    HexCell GetCellUnderMouse()
    {
        Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
        if (hit.collider == null) return null;

        return hit.collider.GetComponent<HexCell>();
    }

    void TryBuild(HexCell cell)
    {
        Debug.Log($"Selected hex: ({cell.Q}, {cell.R}, {cell.S})");

        if (selectedBuilding == null)
            return;

        if (cell.buildingData != null)
        {
            Debug.Log("На этом гексе уже есть здание!");
            return;
        }

        if (!ResourceManager.Instance.HasResources(selectedBuilding))
        {
            Debug.Log("Не хватает ресурсов!");
            return;
        }

        ResourceManager.Instance.SpendResources(selectedBuilding);
        cell.PlaceBuilding(selectedBuilding);

        if (selectedBuilding.name == "HeatDistributor")
            cell.IncreaseTemperatureFromDistributor();

        Debug.Log($"Построено: {selectedBuilding.buildingName} на ({cell.Q}, {cell.R}, {cell.S})");
        CancelSelection();
    }

    void DeleteBuilding(HexCell cell)
    {
        BuildingData building = cell.buildingData;

        ResourceManager.Instance.RefundResources(building);
        cell.RemoveBuilding();

        Debug.Log($"Удалено: {building.buildingName} на ({cell.Q}, {cell.R}, {cell.S})");
    }
}
