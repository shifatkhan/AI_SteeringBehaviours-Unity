using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is used for ddebugging purposes. It allows the developer to control a "target"
/// with keyboard controls to see if NPC's follow it correctly.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f; // Speed at which the player moves.
    [SerializeField] private float turnSpeed = 50f; // Speed at which the player turns.

    [SerializeField] private float idleTime = 2f;
    private float lastMoveTime;

    void Update()
    {
        // Obtain input information (See "Horizontal" and "Vertical" in the Input Manager)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Check for inputs
        if (!Mathf.Approximately(vertical, 0.0f) || !Mathf.Approximately(horizontal, 0.0f))
        {
            transform.localScale = Vector3.one; // Reset scale in case animation was stopped at scale > 1.

            Vector3 direction = new Vector3(horizontal, 0.0f, vertical);
            direction = Vector3.ClampMagnitude(direction, 1.0f);

            // Translate the game object in world space
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Rotate the game object
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(direction), turnSpeed * Time.deltaTime);

            // Reset idle timer to zero
            lastMoveTime = Time.time;
        }
    }
}
