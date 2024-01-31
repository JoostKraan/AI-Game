using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct BuildingInfo
{
    public GameObject prefab;
    public float buildHeight;
    public Button button; // The UI button for this building
    public Vector3 offset;
    public int buildCost; // The cost in points to build this structure
    public TMP_Text buildCostText;
}

public class BuildingManager : MonoBehaviour
{
    public BuildingInfo[] buildingInfos; // The prefabs, build heights, and UI buttons of possible selected objects
    public LayerMask groundLayer;
    public LayerMask collisionLayer;
    public GameObject buildingUI;
    public PlayerMovement player;
    private GameObject currentBuilding; // The currently placed building
    private bool isPlacing = false; // Flag to check if a building is currently being placed
    public bool buildMode;
    private int currentIndex = 0; // The current index for the buildingInfos array
    private int playerPoints = 100; // Initial points for the player
    public float maxBuildDistance = 10f; // Maximum distance from the player to allow building

    // Variables for color management
    private Color originalColor;
    public Color canPlaceColor = Color.green;
    public Color cannotPlaceColor = Color.red;

    // Text for player points
    public TMP_Text playerPointsText;

    private void Start()
    {
        foreach (var info in buildingInfos)
        {
            info.button.interactable = false;
            info.buildCostText.SetText("Cost : " + info.buildCost.ToString());
        }

        buildingUI.SetActive(false);
        UpdatePlayerPointsText();
    }

    void UpdatePlayerPointsText()
    {
        playerPointsText.text = "Player Points: " + playerPoints;
    }

    void Update()
    {
        UpdatePlayerPointsText();
        HandleInput();

        if (isPlacing)
        {
            UpdateBuildingPosition();
            HandleRotationInput();

            if (currentBuilding)
            {
                if (CanPlaceBuilding())
                {
                    SetBuildingColor(canPlaceColor); // Change the color to green if it can be placed
                }
                else
                {
                    SetBuildingColor(cannotPlaceColor); // Change the color to red if it cannot be placed
                }
            }
        }
        else
        {
            UpdatePreviewPosition();
        }
    }

    void SetBuildingColor(Color color)
    {
        var renderers = currentBuilding.GetComponentsInChildren<Renderer>(true);

        foreach (var renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            Material[] materialsCopy = new Material[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                Material materialCopy = new Material(materials[i]); // Create a copy of the material
                materialsCopy[i] = materialCopy;

                if (materialCopy.HasProperty("_BaseColor"))
                {
                    materialCopy.SetColor("_BaseColor", color);
                }
            }

            renderer.sharedMaterials = materialsCopy;
        }
    }

    void CreatePreviewBuilding()
    {
        BuildingInfo currentBuildingInfo = buildingInfos[currentIndex];
        currentBuilding = Instantiate(currentBuildingInfo.prefab);
        currentBuilding.GetComponent<Collider>().isTrigger = true;
        originalColor = GetOriginalColor(currentBuilding);
        AdjustTransparency(currentBuilding, 0.5f);
    }

    Color GetOriginalColor(GameObject obj)
    {
        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial.HasProperty("_BaseColor"))
        {
            return renderer.sharedMaterial.GetColor("_BaseColor");
        }
        return Color.white; // Default color if no original color found
    }

    void AdjustTransparency(GameObject obj, float alpha)
    {
        var renderers = obj.GetComponentsInChildren<Renderer>(true);

        foreach (var renderer in renderers)
        {
            Material[] materials = renderer.sharedMaterials;
            Material[] materialsCopy = new Material[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                Material materialCopy = new Material(materials[i]); // Create a copy of the material
                materialsCopy[i] = materialCopy;

                if (materialCopy.HasProperty("_BaseColor"))
                {
                    Color baseColor = materialCopy.GetColor("_BaseColor");
                    baseColor.a = alpha;
                    materialCopy.SetColor("_BaseColor", baseColor);
                }
            }

            renderer.sharedMaterials = materialsCopy;
        }
    }

    void UpdatePreviewPosition()
    {
        if (currentBuilding != null)
        {
            currentBuilding.SetActive(true);
            Vector3 mousePosition = GetMousePositionOnGround();
            currentBuilding.transform.position = SnapToGrid(mousePosition);
        }
    }

    Vector3 GetMousePositionOnGround()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleBuildMode();
        }

        if (!buildMode) return;

        if (!isPlacing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartPlacingBuilding();
            }

            currentIndex = (int)Mathf.Clamp(currentIndex + Input.mouseScrollDelta.y, 0, buildingInfos.Length - 1);

            foreach (var info in buildingInfos)
            {
                info.button.interactable = false;
            }

            buildingInfos[currentIndex].button.interactable = true;

            for (int i = 0; i < buildingInfos.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    currentIndex = i;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && CanPlaceBuilding())
            {
                StopPlacingBuilding();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyPlacingBuilding();
            }
        }
    }

    void ToggleBuildMode()
    {
        buildMode = !buildMode;

        buildingUI.SetActive(buildMode);

        player.canShoot = !buildMode;

        foreach (var info in buildingInfos)
        {
            info.button.interactable = buildMode;
        }

        if (!buildMode)
        {
            DestroyPlacingBuilding();
        }
    }

    void StopPlacingBuilding()
    {
        SetBuildingColor(originalColor); // Return to the original color
        currentBuilding.GetComponent<Collider>().isTrigger = false;
        playerPoints -= buildingInfos[currentIndex].buildCost;

        if (currentBuilding.TryGetComponent(out Turret turret))
        {
            turret.canShoot = true;
            print(turret.canShoot);
        }
        else if (currentBuilding.TryGetComponent(out Mine mine))
        {
            mine.GetComponent<SphereCollider>().isTrigger = true;
            mine.canExplode = true;
        }

        isPlacing = false;
        currentBuilding = null;
        return;
    }

    void StartPlacingBuilding()
    {
        CreatePreviewBuilding();

        if (currentBuilding.GetComponent<Building>() is Turret turret)
        {
            turret.canShoot = false;
        }
        else if (currentBuilding.GetComponent<Building>() is Mine mine)
        {
            mine.canExplode = false;
        }

        isPlacing = true;
    }

    void DestroyPlacingBuilding()
    {
        Destroy(currentBuilding);
        isPlacing = false;
        currentBuilding = null;
    }

    bool CanPlaceBuilding()
    {
        if (playerPoints < buildingInfos[currentIndex].buildCost) return false;

        Collider collider = currentBuilding.GetComponent<Collider>();

        // Use an axis-aligned bounding box that encompasses the entire rotated object
        Bounds bounds = collider.bounds;

        Vector3 center = bounds.center;
        Vector3 halfExtents = bounds.extents;

        // Check the distance between the player and the building
        float distanceToPlayer = Vector3.Distance(player.transform.position, center);

        // Check if the distance is within the allowed range
        if (distanceToPlayer <= maxBuildDistance)
        {
            Collider[] colliders = Physics.OverlapBox(center, halfExtents, Quaternion.identity, collisionLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != currentBuilding)
                {
                    Debug.LogWarning(colliders[i].gameObject.name);
                    return false;
                }
            }

            return true;
        }

        return false;
    }

    void UpdateBuildingPosition()
    {
        if (currentBuilding != null)
        {
            Vector3 mousePosition = GetMousePositionOnGround();
            currentBuilding.transform.position = SnapToGrid(mousePosition);
        }
    }

    void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentBuilding.transform.Rotate(Vector3.up, 90f);
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        float gridSize = 1.0f;
        float x = Mathf.Floor(position.x / gridSize) * gridSize + gridSize / 2.0f + buildingInfos[currentIndex].offset.x;
        float y = position.y + buildingInfos[currentIndex].buildHeight;
        float z = Mathf.Floor(position.z / gridSize) * gridSize + gridSize / 2.0f + buildingInfos[currentIndex].offset.z;
        return new Vector3(x, y, z);
    }
}
