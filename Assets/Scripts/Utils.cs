using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Utils
{
    public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float spatialBlend)
    {
        var audioGameObject = new GameObject("TempAudio");
        audioGameObject.transform.position = position;
        
        var audioSource = audioGameObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.spatialBlend = spatialBlend;
        audioSource.Play();
        Object.Destroy(audioGameObject, clip.length);
        return audioSource;
    }
}
