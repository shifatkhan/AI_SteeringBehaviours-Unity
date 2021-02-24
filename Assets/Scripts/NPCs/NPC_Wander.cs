using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This NPC class is basically the same as the base NPC class, except
/// that it calls "wander" for getting the Steering.
/// </summary>
public class NPC_Wander : NPC
{
    protected override void KinematicUpdate()
    {
        KinematicMoveUpdate();
    }

    protected override void SteeringUpdate()
    {
        KinematicUpdate();
    }

    protected override void KinematicMoveUpdate()
    {
        SteeringOutput steering = wander.GetSteering(target, transform, velocity);

        velocity = steering.linear;
        // Update the position and random orientation.
        transform.position += steering.linear * Time.deltaTime;
    }

    protected override void OnDrawGizmos()
    {
        // DRAW velocity
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, velocity+transform.position);
    }
}
