using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitReplace : MonoBehaviour
{
    [SerializeField] private GameObject replacementPrefab;
    [SerializeField] private int hitsToReplace = 5;
    [SerializeField] private float scaleDuration = 0.1f; // Duration of the scaling animation
    [SerializeField] private float scaleAmount = 1.2f; // Amount to scale up
    [SerializeField] private int materialsReward = 5; // Amount of materials to add upon replacement

    private int currentHits = 0;
    private bool isScaling = false;
    private BuildSystem buildSystem;

    void Start()
    {
        // Find the BuildSystem in the scene
        buildSystem = FindObjectOfType<BuildSystem>();
        if (buildSystem == null)
        {
            Debug.LogError("BuildSystem not found in the scene.");
        }
    }

    public void RegisterHit()
    {
        // Increase hit count when a hit is registered
        currentHits++;
        Debug.Log("I've Been Hit");

        // Trigger the scaling animation
        if (!isScaling)
        {
            StartCoroutine(ScaleUpDown());
        }

        if (currentHits >= hitsToReplace)
        {
            ReplaceObject();
        }
    }

    private void ReplaceObject()
    {
        // Instantiate the replacement prefab at the same position and rotation
        Instantiate(replacementPrefab, transform.position, transform.rotation);
        
        // Increase materials in the BuildSystem
        if (buildSystem != null)
        {
            buildSystem.AddMaterials(materialsReward);
        }

        // Destroy the current object
        Destroy(gameObject);
    }

    private IEnumerator ScaleUpDown()
    {
        isScaling = true;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * scaleAmount;

        // Scale up
        float elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // Scale down
        elapsedTime = 0f;
        while (elapsedTime < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsedTime / scaleDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localScale = originalScale;

        isScaling = false;
    }
}


