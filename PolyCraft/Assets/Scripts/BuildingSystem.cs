using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private GameObject ghostWallPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject ghostFloorPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject ghostRampPrefab;
    [SerializeField] private GameObject rampPrefab;
    [SerializeField] private float gridSize = 1.0f; // Size of each cell in the grid
    [SerializeField] private Transform player; // Reference to the player transform
    [SerializeField] private BuildType selectedBuildType; // Enum to select what to build
    [SerializeField] private LayerMask snapLayerMask; // Layer mask for snapping to structures
    
    private GameObject currentGhost;
    private bool isBuildMode = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildMode = !isBuildMode;
            ToggleGhost(isBuildMode);
        }

        if (isBuildMode)
        {
            UpdateGhostPosition();

            if (Input.GetMouseButtonDown(1)) // Right mouse button
            {
                PlaceStructure();
            }
        }
    }

    void ToggleGhost(bool isActive)
    {
        if (isActive)
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
            }

            switch (selectedBuildType)
            {
                case BuildType.Wall:
                    currentGhost = Instantiate(ghostWallPrefab);
                    break;
                case BuildType.Floor:
                    currentGhost = Instantiate(ghostFloorPrefab);
                    break;
                case BuildType.Ramp:
                    currentGhost = Instantiate(ghostRampPrefab);
                    break;
            }

            currentGhost.SetActive(true);
        }
        else
        {
            if (currentGhost != null)
            {
                currentGhost.SetActive(false);
            }
        }
    }

    void UpdateGhostPosition()
    {
        // Get the center point of the screen
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition = SnapToGrid(targetPosition);
            currentGhost.transform.position = targetPosition;
            currentGhost.transform.rotation = player.rotation; // Match the player's rotation
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        // Snap to the ground
        position.x = Mathf.Round(position.x / gridSize) * gridSize;
        position.y = Mathf.Round(position.y / gridSize) * gridSize;
        position.z = Mathf.Round(position.z / gridSize) * gridSize;

        // Snap to nearby structures
        Collider[] colliders = Physics.OverlapSphere(position, gridSize, snapLayerMask);
        if (colliders.Length > 0)
        {
            foreach (Collider col in colliders)
            {
                Vector3 closestPoint = col.ClosestPoint(position);
                position = new Vector3(
                    Mathf.Round(closestPoint.x / gridSize) * gridSize,
                    Mathf.Round(closestPoint.y / gridSize) * gridSize,
                    Mathf.Round(closestPoint.z / gridSize) * gridSize
                );
                break;
            }
        }

        return position;
    }

    void PlaceStructure()
    {
        switch (selectedBuildType)
        {
            case BuildType.Wall:
                Instantiate(wallPrefab, currentGhost.transform.position, currentGhost.transform.rotation);
                break;
            case BuildType.Floor:
                Instantiate(floorPrefab, currentGhost.transform.position, currentGhost.transform.rotation);
                break;
            case BuildType.Ramp:
                Instantiate(rampPrefab, currentGhost.transform.position, currentGhost.transform.rotation);
                break;
        }
    }
}

public enum BuildType
{
    Wall,
    Floor,
    Ramp
}

