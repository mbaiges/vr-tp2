using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private float turnMagnitude = 1f;
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            playerController.OnDeath();
        }
    }

    // Public

    public float LookingAngle() {
        float ret = -transform.rotation.eulerAngles.y + 270;
        if (ret < 0) {
            ret = 360 + ret;
        }
        return ret;
    }

    public float Turn(float amount) {
        // Debug.Log("Turning " + amount + " To The " + ((amount > 0) ? "Right" : "Left"));
        Vector3 rotation = transform.rotation.eulerAngles;
        float turn = amount * turnMagnitude;
        rotation.y += turn;
        // transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
        return turn;
    }
}
