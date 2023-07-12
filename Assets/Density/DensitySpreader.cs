using System.Collections.Generic;
using UnityEngine;

public class DensitySpreader : MonoBehaviour {
    public List<Transform> objects;
    public float InitialSpreadMax = 1;
    public float someFactor = 1.0f; // Adjust this as needed
    public float speed = 1.0f; // Movement speed of objects
    public float dampening = 1.0f;

    // Keep track of velocities
    private Dictionary<Transform, Vector3> velocities = new Dictionary<Transform, Vector3>();

    void Start() {
      for (var i = 0; i < transform.childCount; i++) {
        var child = transform.GetChild(i);
        child.transform.position = InitialSpreadMax * Random.onUnitSphere.XZ();
        objects.Add(child);

        // Initialize velocity
        velocities[child] = Vector3.zero;
      }
    }

    void FixedUpdate()
    {
        int[,] densityGrid = new int[32, 24];

        // Clear the density grid
        for (int i = 0; i < 32; i++)
            for (int j = 0; j < 24; j++)
                densityGrid[i, j] = 0;

        // Populate the density grid
        foreach (Transform obj in objects)
        {
            int x = Mathf.Clamp(Mathf.FloorToInt(obj.position.x), 0, 31);
            int z = Mathf.Clamp(Mathf.FloorToInt(obj.position.z), 0, 23);
            densityGrid[x, z]++;
        }

        // Apply density forces
        foreach (Transform obj in objects)
        {
            int x = Mathf.Clamp(Mathf.FloorToInt(obj.position.x), 0, 31);
            int z = Mathf.Clamp(Mathf.FloorToInt(obj.position.z), 0, 23);

            // Calculate the average density of the cell and its surrounding cells
            int totalDensity = 0;
            int numCells = 0;
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = Mathf.Clamp(x + dx, 0, 31);
                    int nz = Mathf.Clamp(z + dz, 0, 23);
                    totalDensity += densityGrid[nx, nz];
                    numCells++;
                }
            }
            float averageDensity = (float)totalDensity / numCells;

            // Apply a force proportional to the difference between the cell's density and the average density
            // Only apply if more than one entity is in the cell
            if (densityGrid[x, z] > 1) {
                float forceMagnitude = (Mathf.Log(densityGrid[x, z]) - averageDensity) * someFactor;

                // In this case, the force is applied in a random direction within the XZ plane
                Vector3 forceDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
                Vector3 force = forceDirection * forceMagnitude;

                // dampen the velocity
                velocities[obj] *= dampening;

                // Apply the force to the velocity
                velocities[obj] += force;
            }

            // Update the position based on the velocity
            obj.position += velocities[obj] * Time.deltaTime;
        }
    }
}
