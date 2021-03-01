using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamWiggle : MonoBehaviour
{

    public float frequency = 1f;
    public float intensity = 1f;

    Quaternion startRot;
    // Start is called before the first frame update
    void Start()
    {
        startRot = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float rotX = Mathf.PerlinNoise(21.4752f + Time.time * frequency, 74.124f + Time.time * frequency) * intensity;
        float rotY = Mathf.PerlinNoise(5.6491f + Time.time * frequency, 931.978123f + Time.time * frequency) * intensity;

        Quaternion a = Quaternion.Euler(rotX, rotY, 0f);

        this.transform.rotation = startRot * a;
    }
}
