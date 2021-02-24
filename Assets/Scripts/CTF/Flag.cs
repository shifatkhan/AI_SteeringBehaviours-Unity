using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public Vector3 originPos;
    public Team team;

    bool beingTaken = false;
    Transform takenBy;

    private void Awake()
    {
        originPos = transform.position;
    }

    private void Start()
    {
        InvokeRepeating("AlertTeamBeingTaken", 0f, 2f);
    }

    public void ResetPos()
    {
        beingTaken = false;
        takenBy = null;
        transform.parent = null;
        transform.position = originPos;
    }

    public void FlagIsBeingTakenBy(Transform target)
    {
        beingTaken = true;
        takenBy = target;
        AlertTeamBeingTaken();
    }

    public void AlertTeamBeingTaken()
    {
        if(takenBy != null)
            team.AssignPlayerToPursue(takenBy);
    }
}
