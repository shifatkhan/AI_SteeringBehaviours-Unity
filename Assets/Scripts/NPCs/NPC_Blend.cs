using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Blend : NPC
{
    public float evadeRadius;
    public float evadeWeight = 1f;
    public float arriveWeight = 1f;
    
    protected override void SteeringMoveUpdate()
    {
        SteeringOutput steering;

        // ARRIVE
        if (flee)
            steering = arrive[behaviorIndex].GetSteering(transform.position, target.position, velocity);
        else
            steering = arrive[behaviorIndex].GetSteering(target.position, transform.position, velocity);

        steering.linear *= arriveWeight;

        // EVADE
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, evadeRadius);
        int numColliders = hitColliders.Length;
        SteeringOutput evade = new SteeringOutput();

        for (int i = 0; i < numColliders; i++)
        {
            if (hitColliders[i].CompareTag("Blue"))
            {
                steering.linear += evadeWeight * arrive[behaviorIndex].GetSteering(transform.position, hitColliders[i].transform.position, velocity).linear;
            }
        }

        // Restrict y translation.
        steering.linear = new Vector3(steering.linear.x, 0f, steering.linear.z);

        // Update the position.
        transform.position += velocity * Time.deltaTime;

        // Update velocity.
        velocity += steering.linear * Time.deltaTime;

        // Limit velocity.
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // DRAW NEAR RADIUS
        Gizmos.color = Color.white;
        float theta = 0;
        float x = evadeRadius * Mathf.Cos(theta);
        float y = evadeRadius * Mathf.Sin(theta);
        Vector3 pos = transform.position + new Vector3(x, 0, y);
        Vector3 newPos = pos;
        Vector3 lastPos = pos;
        for (theta = 0.1f; theta < Mathf.PI * 2; theta += 0.1f)
        {
            x = evadeRadius * Mathf.Cos(theta);
            y = evadeRadius * Mathf.Sin(theta);
            newPos = transform.position + new Vector3(x, 0, y);
            Gizmos.DrawLine(pos, newPos);
            pos = newPos;
        }
        Gizmos.DrawLine(pos, lastPos);
    }
}
