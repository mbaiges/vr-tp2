using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorsGenerator : MonoBehaviour
{
    public float distanceFromPlayer = 10f;
    public float sidesDistance = 10f;
    public float minHeight = 10f;
    public float maxHeight = 20f;
    public float minScaling = 6f;
    public float maxScaling = 12f;

    public float aimMargin = 10f;
    public float hitTime = 10f;

    public GameObject meteorsHolder;
    public GameObject prefab;
    public GameObject player;

    private Vector3 playerPosition;
    private float nextMeteorAt = 2f;
    private float maxMeteorDepth = -15f;

    private void Update() {
        FindPlayer();

        if (Time.time > nextMeteorAt) {
            Meteor();
            nextMeteorAt = Time.time + Random.Range(0f, 5f);
        }

        DestroyDistantMeteors();
    }

    private void FindPlayer() {
        playerPosition = player.transform.position;
        playerPosition.y = 0f;
    }

    private Vector3 GetSpawnPosition() {
        Vector3 forward = Camera.main.transform.forward.normalized;
        Vector3 backward = -forward;
        
        Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized;
        Debug.Log(perpendicular);
        Vector3 spawn = playerPosition + backward * distanceFromPlayer + Random.Range(-sidesDistance, sidesDistance) * perpendicular;
        spawn.y = Random.Range(minHeight, maxHeight);

        return spawn;
    }

    private GameObject GenerateRandomMeteor(Vector3 spawnPosition) {
        GameObject meteor = Instantiate(prefab, spawnPosition, Random.rotation);

        Vector3 scaling = new Vector3(Random.Range(minScaling, maxScaling), Random.Range(minScaling, maxScaling), Random.Range(minScaling, maxScaling));
        Vector3 scale = new Vector3(meteor.transform.lossyScale.x * scaling.x, meteor.transform.lossyScale.y * scaling.y, meteor.transform.lossyScale.z * scaling.z);
        meteor.transform.localScale = scale;
        meteor.transform.SetParent(meteorsHolder.transform);

        return meteor;
    }

    private Vector3 CalculateThrowingForce(GameObject meteor, Rigidbody body) {
        float mass = body.mass;
        Vector3 origin = meteor.transform.position;
        Vector3 forward = Camera.main.transform.forward.normalized;
        Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized;
        
        Vector3 destination = playerPosition + forward * Random.Range(0, aimMargin) + perpendicular * Random.Range(-aimMargin, aimMargin);
        float timeToDestination = hitTime;

        // Calculate initial Force using origin, destination and timeToDestination

        return Vector3.zero;
    }

    private void ThrowMeteor(GameObject meteor) {
        Rigidbody body = meteor.GetComponent<Rigidbody>();
        Vector3 force = CalculateThrowingForce(meteor, body);
        body.AddForce(force, ForceMode.Force);
    }

    private void Meteor() {
        Vector3 spawnPosition = GetSpawnPosition();
        Debug.Log("Spawning meteor at " + spawnPosition);
        GameObject meteor = GenerateRandomMeteor(spawnPosition);
        ThrowMeteor(meteor);
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
