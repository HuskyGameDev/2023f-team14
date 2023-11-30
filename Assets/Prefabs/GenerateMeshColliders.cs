using UnityEngine;

public class GenerateMeshColliders : MonoBehaviour
{
    void Awake()
    {
        // Loop through all children of the current GameObject
        foreach (Transform child in transform)
        {
            // Check if the child has a MeshRenderer component
            MeshRenderer renderer = child.GetComponent<MeshRenderer>();

            if (renderer != null)
            {
                // Check if the child already has a MeshCollider
                MeshCollider collider = child.GetComponent<MeshCollider>();

                if (collider == null)
                {
                    // Add a MeshCollider to the child
                    collider = child.gameObject.AddComponent<MeshCollider>();

                    // Optionally, you can set properties for the collider here
                    // For example, you can enable or disable convex, or set cooking options.
                    // collider.convex = true;
                    // collider.cookingOptions = MeshColliderCookingOptions.CookForFasterSimulation;
                }

                // Set the MeshCollider's mesh to the same mesh as the MeshRenderer
                collider.sharedMesh = renderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
            }
        }
    }
}
