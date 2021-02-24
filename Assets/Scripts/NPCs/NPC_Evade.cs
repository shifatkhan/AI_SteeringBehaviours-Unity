using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Evade : NPC
{
    protected override void KinematicMoveUpdate()
    {
        SteeringOutput steering;

        steering = pursue.GetSteering(transform, target, velocity);

        // Restrict y translation.
        steering.linear = new Vector3(steering.linear.x, 0f, steering.linear.z);

        velocity = steering.linear;

        // Update the position.
        transform.position += steering.linear * Time.deltaTime;
    }

    protected override void SteeringMoveUpdate()
    {
        KinematicMoveUpdate();
    }
}
