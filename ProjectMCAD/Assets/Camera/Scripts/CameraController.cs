using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public string targetTag = "Player";

    protected Vector3 velocity;

    Transform Target { get; set; }

    void Start()
    {
        Target = GameObject.FindGameObjectWithTag(targetTag).transform;   
    }

    void LateUpdate()
    {
        if (Target == null) return;
        transform.position = Vector3.SmoothDamp(transform.position, Target.position, ref velocity, 0.15f) - Vector3.forward;
    }
}
