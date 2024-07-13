using UnityEngine;

public class RayCast : MonoBehaviour
{
    public Camera playerCamera;
    public Transform playerTransform;
    public float maxRayDistance = 100f;
    public float hitCooldown = 0.5f; // Time between hits in seconds

    private float nextHitTime = 0f;

    void Update()
    {
        if (playerCamera == null || playerTransform == null)
        {
            Debug.LogError("Player camera or player transform is not assigned.");
            return;
        }

        if (Input.GetMouseButton(0) && Time.time >= nextHitTime) // Check for left mouse button held down and cooldown
        {
            Vector3 rayOrigin = playerCamera.transform.position;
            Vector3 rayDirection = playerCamera.transform.forward;

            Ray ray = new Ray(rayOrigin, rayDirection);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, maxRayDistance))
            {
                Debug.Log("Ray hit: " + hitInfo.collider.gameObject.name);
                Debug.DrawRay(rayOrigin, rayDirection * hitInfo.distance, Color.red, 2f);

                // Check if the hit object has the HitReplace component
                HitReplace hitReplace = hitInfo.collider.GetComponent<HitReplace>();
                if (hitReplace != null)
                {
                    // Register the hit
                    hitReplace.RegisterHit();
                    nextHitTime = Time.time + hitCooldown; // Set the next allowed hit time
                }
            }
            else
            {
                Debug.Log("Ray did not hit anything.");
                Debug.DrawRay(rayOrigin, rayDirection * maxRayDistance, Color.green, 2f);
            }
        }
    }
}


