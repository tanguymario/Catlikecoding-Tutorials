using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum OptionalToggle
{
    Ignore,
    Yes, 
    No
}

public class HexMapEditor : MonoBehaviour 
{
    #region Properties

    public Color[] colors;

    public HexGrid hexGrid;

    public ToggleGroup toggleGroupColors;

    public ToggleGroup toggleGroupRiverModes;

    public Toggle toggleColorPrefab;

    private bool applyColor = true;

    private bool applyElevation = true;

    private bool applyRiverMode = false;

    private Color activeColor;

    private int activeElevation;

    private int activeBrushSize;

    private OptionalToggle activeRiverMode = OptionalToggle.Yes;

    private bool isDrag;

    private HexDirection dragDirection;

    private HexCell previousCell;

    #endregion

    #region Unity Callbacks

	private void Awake() 
    {
        if (colors.Length > 0)
            InitializeToggleColors();
        else
            Destroy(gameObject);
	}
	
	private void Update() 
    {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
            HandleInput();
        else
            previousCell = null;
	}

    #endregion

    #region Methods

    public void SetApplyColor(bool toggle)
    {
        applyColor = toggle;
    }

    public void SelectColor(int index)
    {
        activeColor = colors[index];
    }

    public void SetApplyElevation(bool toggle)
    {
        applyElevation = toggle;
    }

    public void SetElevation(float elevation)
    {
        activeElevation = (int)elevation;
    }

    public void SetRiverMode(int mode)
    {
        activeRiverMode = (OptionalToggle)mode;
    }

    public void SetApplyRiverMode(bool toggle)
    {
        applyRiverMode = toggle;
    }

    public void SetBrushSize(float brushSize)
    {
        activeBrushSize = (int) brushSize;
    }

    public void ShowUI(bool visible)
    {
        hexGrid.ShowUI(visible);
    }

    private void InitializeToggleColors()
    {
        for (int i = 0; i < colors.Length; i++)
        {
            Toggle toggleColor = Instantiate<Toggle>(toggleColorPrefab);
            toggleColor.transform.SetParent(toggleGroupColors.transform, false);
            toggleColor.group = toggleGroupColors;
            toggleColor.transform.GetChild(1).GetComponent<Image>().color = colors[i];
            if (i != 0)
                toggleColor.isOn = false;
            else
                toggleColor.isOn = true;

            toggleColor.onValueChanged.AddListener((bool on) =>
                {
                    if(on)
                    {
                        SelectColor(toggleGroupColors.GetActive().transform.GetSiblingIndex() - 1);
                    }
                });
        }

        SelectColor(0);
    }

    private void HandleInput()
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit))
        {
            HexCell currCell = hexGrid.GetCell(hit.point);

            if (previousCell && previousCell != currCell)
                ValidateDrag(currCell);
            else
                isDrag = false;

            EditCells(hexGrid.GetCell(hit.point));

            previousCell = currCell;
        }
        else
        {
            previousCell = null;
        }
    }

    private void ValidateDrag(HexCell cell)
    {
        for (dragDirection = HexDirection.NE; dragDirection <= HexDirection.NW; dragDirection++)
        {
            if (previousCell.GetNeighbor(dragDirection) == cell)
            {
                isDrag = true;
                return;
            }
        }

        isDrag = false;
    }

    private void EditCells(HexCell center)
    {
        int centerX = center.coordinates.X;
        int centerZ = center.coordinates.Z;

        for (int r = 0, z = centerZ - activeBrushSize; z <= centerZ; z++, r++)
        {
            for (int x = centerX - r; x <= centerX + activeBrushSize; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }

        for (int r = 0, z = centerZ + activeBrushSize; z > centerZ; z--, r++)
        {
            for (int x = centerX - activeBrushSize; x <= centerX + r; x++)
            {
                EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
            }
        }
    }

    private void EditCell(HexCell cell)
    {
        if (cell)
        {
            if (applyColor)
            {
                if (cell.Color != activeColor)
                {
                    cell.Color = activeColor;
                }
            }

            if (applyElevation)
            {
                if (cell.Elevation != activeElevation)
                {
                    cell.Elevation = activeElevation;
                }
            }

            if (applyRiverMode)
            {
                if (activeRiverMode == OptionalToggle.No)
                {
                    cell.RemoveRiver();
                }
                else if (isDrag && activeRiverMode == OptionalToggle.Yes)
                {
                    HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
                    if (otherCell)
                        otherCell.SetOutgoingRiver(dragDirection);
                }
            }
        }
    }

    #endregion
}
