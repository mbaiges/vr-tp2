using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private InputActionReference jumpActionReference;
    [SerializeField] private float jumpForce = 100.0f;
    [SerializeField] private float yJumpForce = 300.0f;
    [SerializeField] private float killingAcceleration = 10.0f;
    [SerializeField] private float graceFactor = 1f;
    [SerializeField] private Text guiHealth;
    [SerializeField] private Image guiDamageOverlay;
    [SerializeField] private LayerMask whatIsGrappleable;

    private XROrigin _xrOrigin;
    private CapsuleCollider _collider;
    private Rigidbody _body;

    private Vector3 spawn; 

    // Jump
    private Vector3 lastPosition;
    private Vector3 currentPosition;

    // Deaths
    private Vector3 lastVelocity;
    private Vector3 currentVelocity;
    private float health = 100.0f;

    public AudioSource audioSourceHit;
    public AudioSource audioSourceDeath;


    private bool IsJumpable => Physics.Raycast(
        new Vector2(transform.position.x, transform.position.y + _collider.height), 
        Vector3.down, 
        2.1f, 
        whatIsGrappleable
    );

    // Start is called before the first frame update
    void Start()
    {
        _xrOrigin = GetComponent<XROrigin>();
        _collider = GetComponent<CapsuleCollider>();
        _body = GetComponent<Rigidbody>();
        spawn = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var center = _xrOrigin.CameraInOriginSpacePos;
        _collider.center = new Vector3(center.x, _collider.center.y, center.z);
        _collider.height = _xrOrigin.CameraInOriginSpaceHeight;

        currentPosition = transform.position;
        currentVelocity = _body.velocity;
        Jump();
        UpdatePlayerHealth();
        lastPosition = currentPosition;
        lastVelocity = currentVelocity;

        UpdateGUIHealth();
    }

    // Updaters

    private void UpdatePlayerHealth() {
        // Debug.Log("Last Velocity: " + lastVelocity.magnitude);
        // Debug.Log("Current Velocity: " + currentVelocity.magnitude);
        float accChange = Mathf.Abs(lastVelocity.magnitude - currentVelocity.magnitude);
        if (accChange > killingAcceleration) {
            Debug.Log(accChange);
            if (health > 0) {
                health -= accChange * graceFactor;
                audioSourceHit.Play();
            }
            if (health <= 0) {
                health = 0f;
                OnDeath();
            }
        }
    }

    private void UpdateGUIHealth() {
        guiHealth.text = "Health: " + ((float)(int) health) + "/100";

        float alpha = 1 - health/100f;
        guiDamageOverlay.canvasRenderer.SetAlpha(alpha);
    }

    // Action handlers

    private void Jump() {
        float gripValue = jumpActionReference.action.ReadValue<float>();
        if (gripValue > 0) {
            OnJump();
        }
    }
    private void OnJump() {
        RaycastHit hit;
        Vector3 headPosition = new Vector3(transform.position.x, transform.position.y +_collider.height, transform.position.z);
        Vector3 direction = Vector3.down;
        float maxDistance = 2.1f;
        Debug.Log(headPosition);
        Debug.Log(direction);
        Debug.Log(maxDistance);
        if (Physics.Raycast(headPosition, direction, out hit, maxDistance, whatIsGrappleable)) {
            Debug.Log("Velocity " + _body.velocity);
            Vector3 vel = (currentPosition - lastPosition) * jumpForce;
            Vector3 jump = new Vector3(vel.x, vel.y + yJumpForce, vel.z);
            Debug.Log("Jumping at " + jump);
            _body.AddForce(jump);
        };
    }

    private void OnDeath() {
        Debug.Log("DEAD LITTLE BOY SCOUT! ");
        audioSourceDeath.Play();
        _body.velocity = Vector3.zero;
        new WaitForSeconds(2f); 
        transform.position = spawn;
        currentPosition = spawn;
        lastPosition = currentPosition;
        currentVelocity = _body.velocity;
        lastVelocity = currentPosition;
        health = 100;
        UpdateGUIHealth();
    }

}
