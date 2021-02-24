using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicAlign : Align
{
    public float rotationSpeedRads = 1f;

    public Transform target;

    public KinematicAlign()
    {
        this.rotationSpeedRads = 1f;
    }

    public KinematicAlign(float rotationSpeedRads)
    {
        this.rotationSpeedRads = rotationSpeedRads;
    }

    void Update()
    {
        // transform.rotation = GetRotation((target.transform.position - transform.position).normalized);
    }

    public override Quaternion GetRotation(Transform target, Transform origin, bool flee)
    {
        Vector3 goalFacing;
        if (flee)
            goalFacing = (origin.transform.position - target.position).normalized;
        else
            goalFacing = (target.transform.position - origin.position).normalized;

        // Figure out where you want to face.
        Quaternion faceTowards = Quaternion.LookRotation(goalFacing, Vector3.up);

        return Quaternion.RotateTowards(origin.rotation, faceTowards, rotationSpeedRads);
    }
}
