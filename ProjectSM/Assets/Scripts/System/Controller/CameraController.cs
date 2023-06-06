using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform trackingObj;
    [Range(0.01f,10f)]
    public float lerpTime;
    [Range(-10.0f, 10.0f)]
    public float yOffset = 0.0f;
    [Range(-10.0f, 10.0f)]
    public float zOffset = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, trackingObj.position.y + yOffset, trackingObj.position.z), Time.deltaTime * lerpTime);
    }
}
