using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This NPC class is directly connected to Question 2 > R2 > A and B. in the assignment 1.
/// It also indirectly solves C if you enable the "flee" boolean.
/// </summary>
public class NPC_AB : NPC
{
    protected override void KinematicUpdate()
    {
        // Condition (A) i. and ii.
        if (distance.magnitude < nearDistance)
        {
            KinematicMoveUpdate();
        }
        // Condition (B)
        else
        {
            // Condition i.
            if (IsTargetInView())
            {
                KinematicMoveUpdate();
                RotateUpdate();
            }
            // Condition ii.
            else
            {
                StopMovement();
                RotateUpdate();
            }
        }
    }

    protected override void SteeringUpdate()
    {
        // Condition (A)
        if (distance.magnitude < nearDistance)
        {
            // Condition i.
            if (distance.magnitude < smallDistance)
            {
                SteeringMoveUpdate();
            }
            // Condition ii.
            else
            {
                StopMovement();
                RotateUpdate();
            }
        }
        // Condition (B)
        else
        {
            // Condition i.
            if (IsTargetInView())
            {
                SteeringMoveUpdate();
                RotateUpdate();
            }
            // Condition ii.
            else
            {
                StopMovement();
                RotateUpdate();
            }
        }
    }
}
