using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float guiMessageOnScreenTime = 5f;
    [SerializeField] private Text gui;
    [SerializeField] private Image guiDamageOverlay;
    
    [SerializeField] private InputActionReference moveActionReference;
    [SerializeField] private InputActionReference speedActionReference;

    [SerializeField] private BoatController boatController;

    public AudioClip deadAudioClip;
    public AudioSource engineAudioSource;
    public MeteorsGenerator meteorsGenerator;
    public GameObject playerContainer;
    public float rotSpeed = 1f;

    public float boatSpeed = 10f;
    private bool speeding = false;
    private float speedStartTime;

    private XROrigin _xrOrigin;
    private CapsuleCollider _collider;
    private Rigidbody _body;

    private Vector3 spawn; 

    private float startTime;
    private float elapsedTime;

    private string guiText;

    // Meteors

    private int survivedMeteors = 0;
    private int midAirMeteors = 0;

    // Death

    private float resetAt = 0f;

    // Start is called before the first frame update
    void Start()
    {
        _xrOrigin = GetComponent<XROrigin>();
        _collider = GetComponent<CapsuleCollider>();
        _body = playerContainer.GetComponent<Rigidbody>();
        spawn = transform.position;
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - startTime;

        var center = _xrOrigin.CameraInOriginSpacePos;
        _collider.center = new Vector3(center.x, _collider.center.y, center.z);
        _collider.height = _xrOrigin.CameraInOriginSpaceHeight;

        OnTurn();
        OnSpeed();
        UpdateGUI();

        if (resetAt > 0 && elapsedTime > resetAt) {
            ResetScene();
        }
    }

    // Updaters

    public void RevealSurvivedMeteors() {
        survivedMeteors = meteorsGenerator.CountDisappearedMeteors();
    }
    public void RevealMidAirMeteors() {
        midAirMeteors = meteorsGenerator.CountMidAirMeteors();
    }

    private void UpdateGUI() {
        RevealSurvivedMeteors();
        RevealMidAirMeteors();
        Debug.Log("midAir: " + midAirMeteors);
        Debug.Log("survived: " + survivedMeteors);
        Debug.Log("elapsedTime: " + elapsedTime);
        if (elapsedTime > guiMessageOnScreenTime) {
            Debug.Log("guiText: " + guiText);
            guiText = "Survived " + survivedMeteors + " meteor" + ((survivedMeteors != 1) ? "s" : "") + " so far.\n";
            if (midAirMeteors == 0) {
                guiText += "No meteors yet.\nWatch your back!\n";
            } else {
                guiText += "¡" + midAirMeteors + " meteor" + ((midAirMeteors != 1) ? "s" : "") + " in mid air!\n¡Watch out!\n";
            }
            gui.text = guiText;
        }
        // gui.text = $"Alive for: {elapsedTime:0.00}s\n{guiText}";
    }

    // Action handlers

    private void OnTurn() {
        Vector2 axisValue = moveActionReference.action.ReadValue<Vector2>();
        float turned = boatController.Turn(axisValue.x);
        Turn(turned);
    }

    private void Turn(float turn) {
        Vector3 rotation = playerContainer.transform.rotation.eulerAngles;
        rotation.y += turn * rotSpeed;
        playerContainer.transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }

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
            float angle = boatController.LookingAngle();
            angle = Mathf.Deg2Rad * angle;
            Debug.Log("Angle: " + angle);
            Vector3 direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
            Vector3 force = direction * boatSpeed;
            force.y = 0.5f;
            Debug.Log("Direction " + direction);
            _body.AddForce(force, ForceMode.Acceleration);
            engineAudioSource.pitch = 1 + Mathf.Clamp(gripValue, 0, 1);
        }

        if (_body.velocity.magnitude > boatSpeed)
        {
            _body.velocity = _body.velocity.normalized * boatSpeed;
        }

    }

    private void ResetScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnDeath()
    {
        Utils.PlayClipAtPoint(deadAudioClip, transform.position, 0f);
        resetAt = elapsedTime + 0.2f;
    }
}
