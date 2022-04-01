using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

public class RopeController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    [SerializeField] private InputActionReference triggerActionReference;
    private GameObject line = null;
    private LineRenderer lineRenderer = null;
    private bool secondaryHandTriggering = false;
    private Vector3 hitPoint;
    private Vector3 lastHand;
    private Vector3 currentHand;
    private bool addedForce = false;

    public Rigidbody player;

    void FixedUpdate()
    {
        currentHand = transform.position;

        float gripValue = triggerActionReference.action.ReadValue<float>();
        if (gripValue > 0) {
            OnTrigger();
        } else {
            secondaryHandTriggering = false;
            addedForce = false;
        }

        lastHand = currentHand;
    }

    public float threshold = 0.1F;
    public float forceIntensity = 1F;

    private void OnTrigger() {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        layerMask = ~layerMask;

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        
        bool collided = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask);
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

        if (collided) {
            if (!secondaryHandTriggering) {
                secondaryHandTriggering = true;
                hitPoint = hit.point;
            }
            DrawLine(transform.position, hitPoint);
            if (!addedForce) {
                AddForceToPlayer(currentHand, lastHand, hitPoint - transform.position);
            }
        } else {
            // Pipi sound
        }
    }

    void AddForceToPlayer(Vector3 currentHand, Vector3 lastHand, Vector3 direction) {
        if (Vector3.Distance(lastHand, currentHand) > threshold) {
            addedForce = true;
            Debug.Log("Fiuuuummmmmmmm");
            Debug.Log("" + Time.time + " " + direction + " " + forceIntensity * direction);
            player.AddForce(forceIntensity * direction);
        }
    }

    void DrawLine(Vector3 origin, Vector3 end) {
        //For creating line renderer object
        if (lineRenderer == null) {
            line = new GameObject("Line");
            lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.black;
            lineRenderer.endColor = Color.black;
            lineRenderer.startWidth = 0.01f;
            lineRenderer.endWidth = 0.01f;
            lineRenderer.positionCount = 2;
            lineRenderer.useWorldSpace = true;
        }

        //For drawing line in the world space, provide the x,y,z values
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, origin); //x,y and z position of the starting point of the line
        lineRenderer.SetPosition(1, end); //x,y and z position of the end point of the line
    }

    void RemoveLine() {
        lineRenderer.enabled = false;
        // Destroy(line);
        // line = null;
        // lineRenderer = null;
    }
}
