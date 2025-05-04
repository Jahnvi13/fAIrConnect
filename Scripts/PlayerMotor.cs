using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Handles player movement and jumping using CharacterController.
public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    [Header("Movement Settings")]
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 3f;

    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = controller.isGrounded;
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);
        controller.Move(transform.TransformDirection(move) * speed * Time.deltaTime);

        ApplyGravity();
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        else
            playerVelocity.y += gravity * Time.deltaTime;
    }

    public void Jump()
    {
        if (isGrounded)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
    }
}