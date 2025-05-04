using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotates the player and camera according to mouse movement.
public class PlayerLook : MonoBehaviour
{
    [Header("Camera Settings")]
    public Camera playerCamera;
    public float mouseSensitivity = 100f;

    private float xRotation = 0f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x * mouseSensitivity * Time.deltaTime;
        float mouseY = input.y * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
