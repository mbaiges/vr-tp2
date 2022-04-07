using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public AudioClip splashAudioClip;
    
    [Range(0, 1)] public float splash3DBlend;

    private bool hasCollided = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasCollided && other.gameObject.CompareTag("Water"))
        {
            hasCollided = true;
            Utils.PlayClipAtPoint(splashAudioClip, transform.position, Mathf.Clamp(splash3DBlend, 0, 1));
        }
    }
}
