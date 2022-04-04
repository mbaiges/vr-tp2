using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float guiMessageOnScreenTime = 5f;
    [SerializeField] private Text gui;
    [SerializeField] private Image guiDamageOverlay;
    
    [SerializeField] private InputActionReference speedActionReference;

    public float boatSpeed = 10f;
    private bool speeding = false;
    private float speedStartTime;

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

        lastPosition = currentPosition;
        lastVelocity = currentVelocity;

        OnSpeed();
        UpdateGUI();
    }

    // Updaters

    private void UpdateGUI() {
        gui.text = (Time.time < guiMessageOnScreenTime) ? "¡Avoid the meteors and reach the Island!" : "";
    }

    // Action handlers

    private void OnSpeed() {
        float gripValue = speedActionReference.action.ReadValue<float>();
        if (gripValue <= 0) {
            speeding = false;
            return;
        }

        if (!speeding) {
            speedStartTime = Time.time;
            speeding = true;
        }

        if (speeding) {
            Vector3 direction = Camera.main.transform.forward;
            _body.AddForce(Vector3.Normalize(new Vector3(direction.x, 0.5f, direction.z)), ForceMode.Acceleration);
        }

        if(_body.velocity.magnitude > boatSpeed)
        {
            _body.velocity = _body.velocity.normalized * boatSpeed;
        }

    }

    private void OnDeath() {
        Debug.Log("You're dead");
        _body.velocity = Vector3.zero;
        new WaitForSeconds(2f); 
        transform.position = spawn;
        currentPosition = spawn;
        lastPosition = currentPosition;
        currentVelocity = _body.velocity;
        lastVelocity = currentPosition;
        UpdateGUI();
    }

}
