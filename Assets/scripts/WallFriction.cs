using UnityEngine;

public class SetWallFriction : MonoBehaviour
{
    // Reference to the material with zero friction
    public Material zeroFrictionMaterial;

    private void Start()
    {
        // Find all objects with colliders tagged as "Wall"
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        // Loop through each wall and set its collider's physics material to zero friction
        foreach (GameObject wall in walls)
        {
            Collider collider = wall.GetComponent<Collider>();
            if (collider != null)
            {
                // Create a new PhysicMaterial and set its friction to 0
                PhysicMaterial zeroFrictionPhysicMaterial = new PhysicMaterial();
                zeroFrictionPhysicMaterial.dynamicFriction = 0f;
                zeroFrictionPhysicMaterial.staticFriction = 0f;

                // Assign the physics material to the collider
                collider.material = zeroFrictionPhysicMaterial;
            }
        }
    }
}
