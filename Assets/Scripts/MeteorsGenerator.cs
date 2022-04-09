using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class MeteorsGenerator : MonoBehaviour
{
    public float distanceFromPlayer = 10f;
    public float minTimeBetweenMeteors = 5f;
    public float maxTimeBetweenMeteors = 10f;

    public float minHeight = 10f;
    public float maxHeight = 20f;

    public float minScaling = 6f;
    public float maxScaling = 12f;
    public float maxAngVelocity = 6f;

    public float aimMargin = 10f;
    public float minHitTime = 10f;
    public float maxHitTime = 10f;
    public int hitTimeDecrease = 3;
    public int hitTimeDecreaseEvery = 5;

    public GameObject meteorsHolder;
    public GameObject prefab;
    public GameObject player;

    private Vector3 playerPosition;
    private float nextMeteorAt;
    private float maxMeteorDepth = -15f;
    private int disappearedMeteors = 0;

    private void Start() {
        nextMeteorAt = Time.time + Random.Range(minTimeBetweenMeteors, maxTimeBetweenMeteors);
    }

    private void Update() {
        FindPlayer();

        if (Time.time > nextMeteorAt) {
            Meteor();
            nextMeteorAt = Time.time + Random.Range(minTimeBetweenMeteors, maxTimeBetweenMeteors);
        }

        DestroyDistantMeteors();
    }

    private void AdjustDifficulty()
    {
        if (hitTimeDecrease >= 0 && disappearedMeteors % hitTimeDecreaseEvery == 0)
        {
            hitTimeDecrease--;
            minHitTime = Math.Max(minHitTime - 1, 1);
            maxHitTime = Math.Max(maxHitTime - 1, 1);
        }
    }

    private void FindPlayer() {
        playerPosition = player.transform.position;
        playerPosition.y = 0f;
    }

    private Vector3 GetSpawnPosition() {
        Vector3 forward = Camera.main.transform.forward.normalized;
        Vector3 backward = -forward;
        
        Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized;
        float angle = Random.Range(0, 2*Mathf.PI);
        Vector3 spawn = new Vector3(playerPosition.x + distanceFromPlayer * Mathf.Cos(angle), Random.Range(minHeight, maxHeight), playerPosition.z + distanceFromPlayer * Mathf.Cos(angle));
        spawn.y = Random.Range(minHeight, maxHeight);

        return spawn;
    }

    private GameObject GenerateRandomMeteor(Vector3 spawnPosition) {
        // Should not modify rotation if you want to add a tail to the meteor
        // GameObject meteor = Instantiate(prefab, spawnPosition, Quaternion.identity);
        GameObject meteor = Instantiate(prefab, spawnPosition, Random.rotation);

        Vector3 scalingFactor = new Vector3(Random.Range(minScaling, maxScaling), Random.Range(minScaling, maxScaling), Random.Range(minScaling, maxScaling));
        var lossyScale = meteor.transform.lossyScale;
        Vector3 scale = new Vector3(lossyScale.x * scalingFactor.x, lossyScale.y * scalingFactor.y, lossyScale.z * scalingFactor.z);
        meteor.transform.localScale = scale;
        meteor.transform.SetParent(meteorsHolder.transform);

        return meteor;
    }

    private Vector3 CalculateDestination(GameObject meteor) {
        Vector3 origin = meteor.transform.position;
        Vector3 forward = Camera.main.transform.forward.normalized;
        Vector3 perpendicular = Vector3.Cross(forward, Vector3.up).normalized;
        
        Vector3 destination = playerPosition + forward * Random.Range(-aimMargin/2, aimMargin) + perpendicular * Random.Range(-aimMargin, aimMargin);
        return destination;
    }

    private Vector3 CalculateInitialVelocity(GameObject meteor, Rigidbody body) {
        Vector3 destination = CalculateDestination(meteor);
        float hitTime = Random.Range(minHitTime, maxHitTime);
        
        Vector3 origin = meteor.transform.position;

        float x = Vector3.Distance(origin, destination);
        
        // x - x0 = v0x*t
        // v0x = (x - x0)/t
        float v0x = x/hitTime;
        
        // y - y0 = v0y*t + 1/2 * g * t^2
        // v0y = ((y - y0) - 1/2 * g * t^2)/t
        float y0 = origin.y;
        float y = destination.y;
        float g = Physics.gravity.y;
        float v0y = ((y - y0) - 0.5f * g * Mathf.Pow(hitTime, 2))/hitTime;
        
        return new Vector3(destination.x - origin.x, 0, destination.z - origin.z).normalized * v0x + new Vector3(0, v0y, 0);
    }

    private void ThrowMeteor(GameObject meteor) {
        Rigidbody body = meteor.GetComponent<Rigidbody>();
        Vector3 initialVelocity = CalculateInitialVelocity(meteor, body);
        body.velocity = initialVelocity;
        // Should not modify rotation if you want to add a tail to the meteor
        body.angularVelocity = new Vector3(Random.Range(0, maxAngVelocity), Random.Range(0, maxAngVelocity), Random.Range(0, maxAngVelocity)); // Add spin
    }

    private void Meteor() {
        Vector3 spawnPosition = GetSpawnPosition();
        // Debug.Log("Spawning meteor at " + spawnPosition);
        GameObject meteor = GenerateRandomMeteor(spawnPosition);
        ThrowMeteor(meteor);
    }

    private void DestroyDistantMeteors()
    {
        int previousMeteors = disappearedMeteors;
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.transform.position.y < maxMeteorDepth) {
                Object.Destroy(child);
                disappearedMeteors++;
            }
        }

        if (previousMeteors != disappearedMeteors)
        {
            AdjustDifficulty();
        }
    }

    // Public methods

    public int CountDisappearedMeteors() {
        return disappearedMeteors;
    }

    public int CountMidAirMeteors() {
        int count = 0;
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.transform.position.y > 1f) {
                count++;
            }
        }
        return count;
    }

}
