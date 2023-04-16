using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private bool isMoving = false;
    private int currentLane = 1; // The starting lane
    public float laneDistance = 2.0f;
    private float defaultZ = 1.0f;
    public float minY = -2.0f;
    public float maxY = 2.0f;

    private float inputDelay = 0.2f;
    private bool canInput = true;

    private bool shouldStartCoroutine = false;
    private bool isCoroutineRunning = false;

    private void Update()
    {
        // Check for vertical input
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Move the player character if the key has been pressed and the player is not already moving
        if (verticalInput != 0 && !isMoving && canInput)
        {
            // Check if another coroutine is currently running
            if (isCoroutineRunning)
            {
                // If a coroutine is running, set the flag to start the coroutine when it finishes
                shouldStartCoroutine = true;
                return;
            }

            isMoving = true;
            canInput = false;
            StartCoroutine(DelayInput());

            // Calculate the new lane that the player will move to
            int newLane = currentLane + (int)verticalInput;

            // Clamp the new lane value between -1 and 1 to stay within the valid lanes
            newLane = Mathf.Clamp(newLane, -1, 1);

            // Calculate the target position based on the new lane
            Vector3 targetPosition = transform.position;
            targetPosition.y = newLane * laneDistance;
            targetPosition.z = defaultZ + newLane;

            // Move the player character to the target position over time
            StartCoroutine(MoveToTargetPosition(targetPosition));

            // Update the current lane
            currentLane = newLane;
        }
    }

    IEnumerator MoveToTargetPosition(Vector3 targetPosition)
    {
        // Set the flag to indicate that a coroutine is running
        isCoroutineRunning = true;

        // Calculate the distance and direction to move
        float distance = Vector3.Distance(transform.position, targetPosition);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Move the player character while there is still distance to cover
        while (distance > 0.01f)
        {
            // Move the player character based on the speed and direction
            transform.position += direction * speed * Time.deltaTime;

            // Update the distance remaining
            distance = Vector3.Distance(transform.position, targetPosition);

            // Check if a new coroutine should be started
            if (shouldStartCoroutine)
            {
                // If a new coroutine should be started, clear the flag and exit the coroutine
                shouldStartCoroutine = false;
                isMoving = false;
                isCoroutineRunning = false;
                yield break;
            }

            // Wait for the next frame
            yield return null;
        }

        // Snap the player character to the target position
        transform.position = targetPosition;

        // Set isMoving to false and the flag to indicate that the coroutine has finished
        isMoving = false;
        isCoroutineRunning = false;
    }

    IEnumerator DelayInput()
    {
        yield return new WaitForSeconds(inputDelay);
        canInput = true;
    }
}