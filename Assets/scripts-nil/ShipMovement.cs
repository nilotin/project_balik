using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float turnSpeed = 80f;
    public float fixedY = 0f;

    void Update()
    {
        // New Input System
        float moveInput = 0f;
        float turnInput = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput += 1f;
            if (Keyboard.current.sKey.isPressed) moveInput -= 1f;
            if (Keyboard.current.aKey.isPressed) turnInput -= 1f;
            if (Keyboard.current.dKey.isPressed) turnInput += 1f;
        }

        // Hareket
        Vector3 move = transform.forward * moveInput * moveSpeed * Time.deltaTime;
        transform.position += move;

        // Dönüş (yaw)
        float turn = turnInput * turnSpeed * Time.deltaTime;
        transform.Rotate(0f, turn, 0f);

        // Y eksenini kilitle
        transform.position = new Vector3(
            transform.position.x,
            fixedY,
            transform.position.z
        );
    }
}