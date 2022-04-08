using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorParticle : MonoBehaviour
{
    void Update ()
    {
        var mainModule = GetComponent<ParticleSystem>().main;
        
        // mainModule.startSize = transform.lossyScale.magnitude / 75;
        mainModule.scalingMode = ParticleSystemScalingMode.Hierarchy;
    }

}
