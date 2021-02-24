using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This NPC class is basically the same as the base NPC class, except
/// that it calls "pursue" for getting the Steering.
/// </summary>
public class NPC_Pursue : NPC
{
    protected override void KinematicMoveUpdate()
    {
        SteeringOutput steering;

        steering = pursue.GetSteering(target, transform, velocity);

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
