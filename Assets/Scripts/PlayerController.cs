using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float guiMessageOnScreenTime = 5f;
    [SerializeField] private Text gui;
    [SerializeField] private Image guiDamageOverlay;
    
    [SerializeField] private InputActionReference speedActionReference;

    public MeteorsGenerator meteorsGenerator;

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

    // Start is called before the first frame update
    void Start()
    {
        _xrOrigin = GetComponent<XROrigin>();
        _collider = GetComponent<CapsuleCollider>();
        _body = GetComponent<Rigidbody>();
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

        OnSpeed();
        UpdateGUI();
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
        if (elapsedTime > guiMessageOnScreenTime) {
            guiText = "Survived " + survivedMeteors + " meteor" + ((survivedMeteors != 1) ? "s" : "") + " so far.\n";
            if (midAirMeteors == 0) {
                guiText += "No meteors yet.\nWatch your back!\n";
            } else {
                guiText += "¡" + midAirMeteors + " meteor" + ((midAirMeteors != 1) ? "s" : "") + " in mid air!\n¡Watch out!\n";
            }
        }
        gui.text = $"Alive for: {elapsedTime:0.00}s\n{guiText}";
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

        if (_body.velocity.magnitude > boatSpeed)
        {
            _body.velocity = _body.velocity.normalized * boatSpeed;
        }

    }

    public void OnDeath() {
        Debug.Log("You're dead");
        new WaitForSeconds(5.0f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Reset scene
    }
}
