using UnityEngine;
using System.Collections;

public class GemScript : MonoBehaviour
{
    public float lifetime;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public Vector3 angularVelocity;
    public Space space = Space.Self;
    void Update()
    {
        transform.Rotate(angularVelocity * Time.deltaTime, space);
    }
}