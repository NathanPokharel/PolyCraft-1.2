using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildSystem : MonoBehaviour
{
    [SerializeField] private int materials = 0; 
    [SerializeField] private GameObject ghostWallPrefab;
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject ghostFloorPrefab;
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject ghostRampPrefab;
    [SerializeField] private GameObject rampPrefab;

    [SerializeField] private GameObject ghostErrorWallPrefab;
    [SerializeField] private GameObject ghostErrorFloorPrefab;
    [SerializeField] private GameObject ghostErrorRampPrefab;

    [SerializeField] private float gridSize = 1.0f; // Size of each cell in the grid
    [SerializeField] private Transform player; // Reference to the player transform
    [SerializeField] private BuildType selectedBuildType; // Enum to select what to build

    private GameObject currentGhost;
    private bool isBuildMode = false;
    private bool hasEnoughMaterials = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            isBuildMode = !isBuildMode;
            ToggleGhost(isBuildMode);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            SelectNextBuildType();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            SelectPreviousBuildType();
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

            hasEnoughMaterials = materials >= 5;

            switch (selectedBuildType)
            {
                case BuildType.Wall:
                    currentGhost = Instantiate(hasEnoughMaterials ? ghostWallPrefab : ghostErrorWallPrefab);
                    break;
                case BuildType.Floor:
                    currentGhost = Instantiate(hasEnoughMaterials ? ghostFloorPrefab : ghostErrorFloorPrefab);
                    break;
                case BuildType.Ramp:
                    currentGhost = Instantiate(hasEnoughMaterials ? ghostRampPrefab : ghostErrorRampPrefab);
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

            // Snap the rotation to the nearest 90 degrees
            Vector3 playerRotation = player.eulerAngles;
            playerRotation.y = Mathf.Round(playerRotation.y / 90) * 90;
            currentGhost.transform.rotation = Quaternion.Euler(playerRotation);
        }
    }

    Vector3 SnapToGrid(Vector3 position)
    {
        // Snap to the ground
        position.x = Mathf.Round(position.x / gridSize) * gridSize;
        position.y = Mathf.Round(position.y / gridSize) * gridSize;
        position.z = Mathf.Round(position.z / gridSize) * gridSize;

        return position;
    }

    void PlaceStructure()
    {
        if (materials < 5)
        {
            Debug.Log("Not enough materials to build!");
            return;
        }

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

        materials -= 5; // Deduct materials after placing the structure
        ToggleGhost(isBuildMode); // Refresh the ghost preview
    }

    public void AddMaterials(int amount)
    {
        materials += amount;
        Debug.Log($"Materials added: {amount}. Total materials: {materials}");
    }

    private void SelectNextBuildType()
    {
        selectedBuildType++;
        if ((int)selectedBuildType >= System.Enum.GetValues(typeof(BuildType)).Length)
        {
            selectedBuildType = 0;
        }
        ToggleGhost(isBuildMode);
    }

    private void SelectPreviousBuildType()
    {
        selectedBuildType--;
        if (selectedBuildType < 0)
        {
            selectedBuildType = (BuildType)(System.Enum.GetValues(typeof(BuildType)).Length - 1);
        }
        ToggleGhost(isBuildMode);
    }
}

public enum BuildType
{
    Wall,
    Floor,
    Ramp
}


