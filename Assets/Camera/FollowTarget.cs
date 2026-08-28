using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform _followTarget;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _bottomClamp = -40f;
    [SerializeField] private float _topClamp = 70f;

    private float _cinemachineTargetPitch;
    private float _cinemachineTargetYaw;

    private void LateUpdate()
    {
        CameraLogic();
    }

    private void CameraLogic()
    {
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");

        _cinemachineTargetPitch = UpdateRotation(_cinemachineTargetPitch, mouseY, _bottomClamp, _topClamp, true);
        //Unrestricted horizontal rotation...
        _cinemachineTargetYaw = UpdateRotation(_cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotations(_cinemachineTargetPitch, _cinemachineTargetYaw);
    }

    private void ApplyRotations(float pitch, float yaw)
    {
        _followTarget.rotation = Quaternion.Euler(pitch, yaw, _followTarget.eulerAngles.z);
    }

    /// <summary>
    /// Updates the camera's rotation based on mouse input.
    /// </summary>
    /// <param name="currentRotation">Current angle of the camera.</param>
    /// <param name="input">The degree the mouse has moved.</param>
    /// <param name="min">Minimum limit for how far the camera can rotate.</param>
    /// <param name="max">Maximum limit for how far the camera can rotate.</param>
    /// <param name="isXAxis">To determine if the rotation is for the x or y axis.</param>
    /// <returns>Adjusted rotation clamped within min and max limits.</returns>
    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
    {
        //Adjusts current rotation based on input, inverts input if it's for the y axis
        currentRotation += isXAxis ? input : -input;
        //Clamping rotation within limits set
        return Mathf.Clamp(currentRotation, min, max);
    }

    private float GetMouseInput(string axis)
    {
        return Input.GetAxis(axis) * _rotationSpeed * Time.deltaTime;
    }
}
