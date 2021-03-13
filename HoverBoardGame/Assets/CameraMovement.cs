using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Transform _target;
    [SerializeField] Vector3 relativePosition=new Vector3(0f,-1.2f,4f);
    Vector3 _currentRelativePosition;
    Vector3 _cameraVelocity = new Vector3(0, 0, 0);
    public float velocity = 0f;
    [SerializeField] float camOffset = 4f;
    public float smoothTime = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        _currentRelativePosition = relativePosition;
        _target = GameObject.Find("hoverboard").transform;
        this.transform.position = _target.TransformPoint(relativePosition);
        this.transform.LookAt(_target, _target.up);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.position = Vector3.SmoothDamp(transform.position, _target.TransformPoint(relativePosition), ref _cameraVelocity, smoothTime);
        this.transform.LookAt(_target,_target.up);
        
    }
}
