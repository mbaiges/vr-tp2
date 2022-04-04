using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorsGenerator : MonoBehaviour
{
    public GameObject meteorsHolder;
    public GameObject prefab;
    public GameObject player;

    private float nextMeteorAt = 2f;
    private float maxMeteorDepth = -15f;

    private void Update() {
        if (Time.time > nextMeteorAt) {
            ThrowMeteor();
            nextMeteorAt = Time.time + Random.Range(0f, 5f);
        }

        DestroyDistantMeteors();
    }

    private Vector3 GetSpawnPosition() {
        return new Vector3(0, 4, 0);
    }

    private void ThrowMeteor() {
        Vector3 spawnPosition = GetSpawnPosition();
        Debug.Log("Spawning meteor at " + spawnPosition);
        GameObject meteor = GenerateRandomMeteor(spawnPosition);
        meteor.transform.parent = meteorsHolder.transform;
    }

    private GameObject GenerateRandomMeteor(Vector3 spawnPosition) {
        GameObject meteor = Instantiate(prefab, spawnPosition, Quaternion.identity);

        MeshFilter meshFilter = meteor.GetComponent<MeshFilter>();
        Debug.Log(meshFilter);

        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++) {
            if (Random.Range(0, 1) < 0.1f) {
                Vector3 vertex = vertices[i];

                float change = Random.Range(0, 0.003f);
                Vector3 normal = Vector3.Normalize(vertex - meteor.transform.position);
                int direction = Random.Range(0,1) < 0.5f ? 1 : -1;
                vertices[i] = vertex + direction * normal * change;
            }
        }

        meshFilter.mesh.vertices = vertices;

        return meteor;
    }

    private void DestroyDistantMeteors() {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.transform.position.y < maxMeteorDepth) {
                Object.Destroy(child);
            }
        }
    }

}
