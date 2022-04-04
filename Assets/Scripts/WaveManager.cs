using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    public float amplitude1 = 0.2f;
    public float length1 = 1.6f;
    public float speed1 = 1f;
    public float amplitude2 = 0.6f;
    public float length2 = 0.8f;
    public float speed2 = 1.2f;
    public float amplitude3 = 0.8f;
    public float length3 = 2f;
    public float speed3 = 2f;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Debug.Log("Instance already exists, destroying objects!");
            Destroy(this);
        }
    }

    private float Wave1(float _x, float _z) {
        float offset = Time.time * speed1;
        return amplitude1 * (Mathf.Sin((_x + Time.time) / length1 + offset) + Mathf.Cos(_z / length1 + offset));
    }

    private float Wave2(float _x, float _z) {
        float offset = Time.time * speed2;
        return amplitude2 * (Mathf.Sin((_x+_z) / length2 + offset));
    }

    private float Wave3(float _x, float _z) {
        float offset = Time.time * speed3;
        return amplitude3 * (Mathf.Sin(_z / length3 + offset));
    }

    public float GetWaveHeight(float _x, float _z) {
        return Wave1(_x, _z) + Wave2(_x, _z) + Wave3(_x, _z);
    }
}
