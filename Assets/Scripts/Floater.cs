using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    [SerializeField] private float boatHeight = 1f;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float depthBeforeSubmerged = 1f;
    [SerializeField] private float displacementAmount = 3f;

    private void FixedUpdate() {
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.x, transform.position.z);
        // Debug.Log(transform.position.y + " + " + waveHeight);
        if (transform.position.y < (waveHeight + boatHeight)) { // Under the water
            float displacementMultiplier = Mathf.Clamp01((waveHeight + boatHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            // Debug.Log("Correcting with displacement: " + displacementMultiplier);
            rigidbody.AddForce(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), ForceMode.Acceleration);
        }    
    }
}
