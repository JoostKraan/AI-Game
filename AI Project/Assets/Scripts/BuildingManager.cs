using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public GameObject buildingPrefab;  // The prefab of the building object
    public LayerMask groundLayer; // The layer for any objects representing ground
    public float buildHeight; // The height of the currently placed building
    public bool buildMode; // If the game is currently in buildMode
    private GameObject currentBuilding; // The currently placed building
    private bool isPlacing = false;      // Flag to check if a building is currently being placed

    void Update()
    {
        HandleInput();

        if (isPlacing)
        {
            UpdateBuildingPosition();
            HandleRotationInput();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            buildMode = !buildMode;

            if (!buildMode)
            {
                DestroyPlacingBuilding();
            }
        }

        if (!buildMode) return;

        if (Input.GetMouseButtonDown(0)) // Left mouse button to place the building
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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyPlacingBuilding();
        }
    }

    void StartPlacingBuilding()
    {
        isPlacing = true;
        currentBuilding = Instantiate(buildingPrefab);
    }

    void StopPlacingBuilding()
    {
        isPlacing = false;
        currentBuilding = null;
    }

    void DestroyPlacingBuilding()
    {
        Destroy(currentBuilding);
        isPlacing = false;
        currentBuilding = null;
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1.0f; // Adjust this value based on your grid size

        float x = Mathf.Floor(position.x / gridSize) * gridSize + gridSize / 2.0f;
        float y = position.y;
        float z = Mathf.Floor(position.z / gridSize) * gridSize + gridSize / 2.0f;

        return new Vector3(x, y, z);
    }

    bool CanPlaceBuilding()
    {
        Collider collider = currentBuilding.GetComponent<Collider>();
        Collider[] colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents / 2);

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

    void HandleRotationInput()
    {
        float rotationSpeed = 5f;

        if (Input.GetKeyDown(KeyCode.R)) // Rotate the building when 'R' key is pressed
        {
            currentBuilding.transform.Rotate(Vector3.up, 90f);
        }
    }
}