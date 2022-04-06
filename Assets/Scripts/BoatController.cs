using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("XROrigin").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log(other.gameObject.tag);
        playerController.OnDeath();
    }
}
