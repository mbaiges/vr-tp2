using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGunScript : MonoBehaviour {

    [SerializeField] private InputActionReference triggerActionReference;
    private bool gripTriggering = false;

    public LayerMask whatIsGrappleable;
    public Transform gunTip;
    public GameObject playerGameobject;
    public float threshold = 0.1F;
    public float forceIntensity = 1F;

    private Rigidbody _playerBody;
    private Transform _playerTransform;
    private LineRenderer lr;
    private Vector3 grapplePoint;
    private bool grappled = false;
    private float maxDistance = 100f;
    private SpringJoint joint;
    private bool addedForce = false;
    private Vector3 lastHand;
    private Vector3 currentHand;
    private Vector3 lastBody;
    private Vector3 currentBody;

    void Awake() {
        lr = GetComponent<LineRenderer>();
        _playerBody = playerGameobject.GetComponent<Rigidbody>();
        _playerTransform = playerGameobject.GetComponent<Transform>();
    }

    void Update() {
        currentHand = transform.position;
        currentBody = _playerTransform.position;

        float gripValue = triggerActionReference.action.ReadValue<float>();
        if (gripValue > 0) {
            if (!gripTriggering) {
                gripTriggering = true;
                StartGrapple();
            }
            if (grappled && !addedForce) {
                AddForceToPlayer(grapplePoint - transform.position);
            }
        } else {
            if (gripTriggering) {
                StopGrapple();
                addedForce = false;
                gripTriggering = false;
            }
        }

        lastHand = currentHand;
        lastBody = currentBody;
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(gunTip.position, gunTip.forward, out hit, maxDistance, whatIsGrappleable)) {
            grappled = true;
            grapplePoint = hit.point;
            joint = playerGameobject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(_playerTransform.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        grappled = false;
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }

    private void AddForceToPlayer(Vector3 direction) {
        Vector3 normalizedDirection = Vector3.Normalize(direction);
        if (Vector3.Distance(lastHand - lastBody, currentHand - currentBody) > threshold) {
            addedForce = true;
            // Debug.Log("Fiuuuummmmmmmm");
            // Debug.Log("" + Time.time + " " + normalizedDirection + " " + forceIntensity * normalizedDirection);
            _playerBody.AddForce(forceIntensity * normalizedDirection);
            new WaitForSeconds(0.8f);
            StopGrapple();
        }
    }
}