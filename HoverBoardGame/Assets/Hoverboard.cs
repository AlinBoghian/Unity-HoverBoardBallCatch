using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Scenes;
using UnityEngine;
using UnityEngine.UIElements;

public class Hoverboard : MonoBehaviour
{
    Rigidbody rigidbody;
    [SerializeField] private float spinSpeed = 20f;
    [SerializeField] private float dragFactor = 0.1f;
    [SerializeField] private float torqueFactor=50f;
    [SerializeField] private float normalspeed=10f;
    [SerializeField] private float boostSpeed = 20f;
    [SerializeField] private float angularSpeed=10f;
    [SerializeField] private float timeBetweenDashes = 0f;
    [SerializeField] private float deltaDashTime=1f;
    private int _lastDashInput;
    private int _dashInput;
    private float _timeSinceLast;
    private float _timeUntilDashAvailable;
    private float _forwardInput;
    private float _spinInput;
    private bool _leftClickInput;
    private bool _clickRotateToggle = false;
    private bool _boostInput;
    private Vector3 _firstMousePosition;
    private Vector3 _appliedTorque;
    private Vector3 _appliedForce;
    private Vector3 _torqueDirection = Vector3.zero;

    private IHoverBoardState currentState;
    private int _didItChange=1;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.maxAngularVelocity = float.MaxValue;
        rigidbody.useGravity = false;
        currentState = new ControlledState();
    }

    // Update is called once per frame
    private void Update()
    {
        ProcessInput();
    }
    private void FixedUpdate()
    {
        currentState.ComputeMovement(this);
        ComputeDrag();
    }

    private void ChangeState(IHoverBoardState state)
    {
        currentState = state;
    }
    public void ApplyPhysicsFromInput()
    {
        if (_forwardInput == 1)
        {
            float desiredSpeed = (_boostInput) ? boostSpeed : normalspeed;
            _appliedForce = Mathf.Pow(desiredSpeed, 2) * dragFactor * Vector3.forward;
            rigidbody.AddRelativeForce(_appliedForce * Time.fixedDeltaTime ,ForceMode.Impulse);
        }

        _appliedTorque = Mathf.Pow(angularSpeed, 2) * torqueFactor * _torqueDirection;
        rigidbody.AddRelativeTorque(Time.fixedDeltaTime * _appliedTorque, ForceMode.Impulse);
        rigidbody.AddRelativeTorque(Time.fixedDeltaTime * _spinInput * Vector3.back, ForceMode.Impulse);
        rigidbody.AddRelativeForce(Vector3.right * (_dashInput * 500));
    }

    void ProcessInput()
    {
        _forwardInput = Input.GetAxisRaw("Vertical");
        _spinInput = Input.GetAxisRaw("Horizontal");
        _leftClickInput = Input.GetKey(KeyCode.Mouse0);
        _boostInput = Input.GetKey(KeyCode.LeftShift);
        _dashInput = 0;
        if (Input.GetKeyDown(KeyCode.A))
            _dashInput = -1;
        if (Input.GetKeyDown(KeyCode.D)) 
            _dashInput++;
        
        _timeUntilDashAvailable = _timeUntilDashAvailable - Time.deltaTime;
        _timeSinceLast = _timeSinceLast + Time.deltaTime;
        if (_timeUntilDashAvailable < 0)
            _timeUntilDashAvailable = 0;
        if (_dashInput != 0 && _timeUntilDashAvailable==0)
        {
            if (_dashInput != _lastDashInput || _timeSinceLast>deltaDashTime)
            {
                _lastDashInput = _dashInput;
                _dashInput = 0;
                _timeSinceLast = 0;
            }
            else
            {
                _timeUntilDashAvailable = timeBetweenDashes;
            }
        }
        if (_leftClickInput)
        {
            if (_clickRotateToggle)
            {
                _torqueDirection = (Input.mousePosition - _firstMousePosition).normalized;
                _torqueDirection = new Vector3(-_torqueDirection.y, _torqueDirection.x);
            }
            else
            {
                _firstMousePosition = Input.mousePosition;
                _clickRotateToggle = true;
            }
        }
        else
        {
            _torqueDirection=Vector3.zero;
            _clickRotateToggle = false;
        }
    }

    private void ComputeDrag()
    {
        Vector3 dragForce = Mathf.Pow(rigidbody.velocity.magnitude,2) * dragFactor * rigidbody.velocity.normalized;
        dragForce = -Vector3.Min(rigidbody.velocity, dragForce);
        rigidbody.AddForce(dragForce*Time.fixedDeltaTime,ForceMode.Impulse);

        Vector3 dragTorque = Mathf.Pow(rigidbody.angularVelocity.magnitude,2) * torqueFactor *
                             rigidbody.angularVelocity.normalized;
        dragTorque = -Vector3.Min(rigidbody.angularVelocity, dragTorque);
        rigidbody.AddTorque(dragTorque*Time.fixedDeltaTime,ForceMode.Impulse);
    }
}