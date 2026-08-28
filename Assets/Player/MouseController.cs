using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(PlayerInput))]
public class MouseController : MonoBehaviour
{
    [SerializeField, Header("Movement")]
    private float _speed;
    [SerializeField]
    private float _jumpForce;

    [SerializeField, Header("Camera")]
    private Transform _camera;

    private float _topSpeed;
    private float _offset = 2f;
    private float _smoothTime = 0.05f;
    private float _currentVelocity;
    private Vector2 _locomotionInput;
    private Vector3 _direction;
    private bool _jumpInput;
    private bool _isGrounded;
    private bool _readyToJump = true;

    private Rigidbody _rigidBody;

    public float Speed 
    {
        get => _speed;
        set => _speed = value; 
    }

    // Start is called before the first frame update
    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _topSpeed = _speed;
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.OverlapSphere(transform.position + new Vector3(0f, -0.6f, 0f), 0.45f).Length > 1f;

        PlayerFacing();
    }

    private void FixedUpdate()
    {
        Vector3 force = _direction.normalized * (_speed * _offset) * Time.fixedDeltaTime;
        _rigidBody.AddForce(force, ForceMode.VelocityChange);
    }

    //Rotate player to face the direction of movement
    private void PlayerFacing()
    {
        //Guard that makeas the player continue facing previous direction
        if (_locomotionInput.sqrMagnitude == 0f)
            return;

        //Storing the target angles as radians converted to degrees
        float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg;
        //Smoothing player rotation
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentVelocity, _smoothTime);
        //Rotating player
        _rigidBody.MoveRotation(Quaternion.Euler(0f, angle, 0f));
    }

    //Makes player's movement relative to the camera
    private void CameraRelativeMovement()
    {
        //Storing camera vectors
        Vector3 cameraForward = _camera.forward;
        Vector3 cameraRight = _camera.right;

        //Setting y to 0 to prevent vertical movement
        cameraForward.y = 0f;
        cameraRight.y = 0f;

        //Normalizing camera vectors for diagonal movement
        cameraForward = _camera.forward.normalized;
        cameraRight = _camera.right.normalized;

        //Storing player input relative to camera orientation
        Vector3 forwardRelativeMovementVector = cameraForward * _locomotionInput.y;
        Vector3 rightRelativeMovementVector = cameraRight * _locomotionInput.x;

        Vector3 cameraRelativeMovement = forwardRelativeMovementVector + rightRelativeMovementVector;

        _direction = cameraRelativeMovement;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //Storing player input and direction when player moves
        _locomotionInput = context.action.ReadValue<Vector2>();
        _direction = new Vector3(_locomotionInput.x, 0f, _locomotionInput.y);

        CameraRelativeMovement();
        //_direction = transform.TransformDirection(_direction);
    }

    public void OnJump(InputAction.CallbackContext context)
    {

    }

    private IEnumerator Wait(Action callback, float delay)
    {
        yield return new WaitForSeconds(delay);
        callback();
    }
}
