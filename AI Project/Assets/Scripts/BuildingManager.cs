using UnityEngine;
using UnityEngine.UI;

public class BuildingManager : MonoBehaviour
{
    public GameObject[] buildingPrefabs;  // The prefabs of possible selected objects
    public Button[] buttons; // The buttons for UI
    public GameObject buildingUI; // The UI for building selection
    public LayerMask groundLayer; // The layer for any objects representing ground
    public LayerMask collisionLayer;
    public float buildHeight; // The height of the currently placed building
    public bool buildMode; // If the game is currently in buildMode
    public float currentIndex; // The current index for the building prefabs
    private GameObject currentBuilding; // The currently placed building
    private bool isPlacing = false; // Flag to check if a building is currently being placed

    private void Start()
    {
        buildingUI.SetActive(false); // Hide building UI on start
    }

    void Update()
    {
        HandleInput();

        if (isPlacing)
        {
            UpdateBuildingPosition();
            HandleRotationInput();
            
            if (currentBuilding.GetComponent<Building>() is Turret turret)
            {
                turret.canShoot = false;
            }
            else if (currentBuilding.GetComponent<Building>() is Mine mine)
            {
                mine.canExplode = false;
            }
        }
    }

    void HandleInput()
    {
        // Toggle build mode and show/hide building UI
        if (Input.GetKeyDown(KeyCode.F))
        {
            buildMode = !buildMode;
            buildingUI.SetActive(buildMode);

            if (!buildMode)
            {
                DestroyPlacingBuilding();
            }
            else
            {
                currentIndex = 0;
            }
        }

        if (!buildMode) return;

        // Left mouse button to place or confirm the building
        if (Input.GetMouseButtonDown(0))
        {
            if (!isPlacing)
            {
                StartPlacingBuilding();
            }
            else
            {
                if (CanPlaceBuilding())
                {
                    StopPlacingBuilding();
                }
            }
        }

        // Cancel building placement on escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyPlacingBuilding();
        }

        // Change the current building prefab using the mouse scroll wheel
        currentIndex += Input.mouseScrollDelta.y;
        currentIndex = Mathf.Clamp(currentIndex, 0, buildingPrefabs.Length - 1);

        // Update UI button interactivity based on the current index
        foreach (var button in buttons)
        {
            button.interactable = false;
        }

        buttons[(int)currentIndex].interactable = true;
    }

    void StartPlacingBuilding()
    {
        isPlacing = true;
        currentBuilding = Instantiate(buildingPrefabs[(int)currentIndex]);
    }

    void StopPlacingBuilding()
    {
        isPlacing = false;

        if (currentBuilding.GetComponent<Building>() is Turret turret)
        {
            turret.canShoot = true;
        }
        else if (currentBuilding.GetComponent<Building>() is Mine mine)
        {
            mine.canExplode = true;
        }

        currentBuilding = null;
    }

    void DestroyPlacingBuilding()
    {
        Destroy(currentBuilding);
        isPlacing = false;
        currentBuilding = null;
    }

    // Snap a position to a grid
    Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1.0f; // Adjust this value based on your grid size

        float x = Mathf.Floor(position.x / gridSize) * gridSize + gridSize / 2.0f;
        float y = position.y;
        float z = Mathf.Floor(position.z / gridSize) * gridSize + gridSize / 2.0f;

        return new Vector3(x, y, z);
    }

    // Check if the building can be placed at the current position
    bool CanPlaceBuilding()
    {
        Collider collider = currentBuilding.GetComponent<Collider>();
        Collider[] colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents / 2, currentBuilding.transform.rotation, collisionLayer);

        foreach (Collider collider2 in colliders)
        {
            // Adjust this condition based on your specific requirements
            if (collider2.gameObject != currentBuilding)
            {
                Debug.Log("Cannot place building: overlapping with " + collider2.gameObject.name);
                return false;
            }
        }

        return true;
    }

    // Update the position of the building being placed based on mouse position
    void UpdateBuildingPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 snappedPosition = SnapToGrid(hit.point);
            snappedPosition.y += buildHeight;
            currentBuilding.transform.position = snappedPosition;
        }
    }

    // Rotate the building when 'R' key is pressed
    void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentBuilding.transform.Rotate(Vector3.up, 90f);
        }
    }
}
