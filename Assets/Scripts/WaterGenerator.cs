using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterGenerator : MonoBehaviour
{
    private float epsilon = 0.1f;
    public float radius = 15f;
    public int blockSize = 10;
    public GameObject origin;
    public GameObject prefab;
    public GameObject player;

    private void FixedUpdate() {
        GenerateWater();
        DestroyDistantWater();
    }

    private void GenerateWater() {
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = 0;
        
        int minX, maxX, minZ, maxZ;
        minX = (int) (playerPosition.x - radius)/blockSize;
        maxX = (int) (playerPosition.x + radius)/blockSize;
        minZ = (int) (playerPosition.z - radius)/blockSize;
        maxZ = (int) (playerPosition.z + radius)/blockSize;

        // Debug.Log("minX: " + minX + ", maxX: " + maxX);
        // Debug.Log("minZ: " + minZ + ", maxZ: " + maxZ);

        for (int x = minX; x <= maxX; x++) {
            for (int z = minZ; z <= maxZ; z++) {
                float xPos = (x*blockSize) + (blockSize/2.0f);
                float zPos = (z*blockSize) + (blockSize/2.0f);
                Vector3 generationPoint = new Vector3(xPos, 0, zPos);
                if (Vector3.Distance(generationPoint, playerPosition) <= radius) {
                    if (!WaterExists(generationPoint)) {
                        // Debug.Log("Instantiating at " + generationPoint);
                        GameObject water = Instantiate(prefab, generationPoint, Quaternion.identity);
                        water.transform.position = generationPoint;
                        water.transform.parent = origin.transform;
                    }
                }
            }
        }
    }

    private void DestroyDistantWater() {
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = 0;
        
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (Vector3.Distance(child.transform.position, playerPosition) > radius) {
                Object.Destroy(child);
            }
        }
    }

    private bool WaterExists(Vector3 waterPosition) {
        for (int i = 0; i < transform.childCount; i++) {    
            GameObject child = transform.GetChild(i).gameObject;
            if (Vector3.Distance(waterPosition, child.transform.position) <= epsilon) {
                // Debug.Log("Found Existing Children");
                return true;
            }
        }
        return false;
    }

}
