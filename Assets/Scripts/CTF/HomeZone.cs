using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeZone : MonoBehaviour
{
    // The current team this Zone is part of.
    private Team team;

    // RED or BLUE
    [SerializeField]
    private string enemyColor;

    public void SetTeam(Team team)
    {
        this.team = team;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(enemyColor))
        {
            team.AssignPlayerToPursue(other.transform);
        }
    }
}
