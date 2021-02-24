using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    [SerializeField]
    private NPC_CTF[] players;
    private int playerLength;

    [SerializeField]
    private HomeZone homeZone;
    [SerializeField]
    private Transform homeFlagSpot;
    [SerializeField]
    private Flag homeFlag;

    [SerializeField]
    private GameObject enemyHomeZone;
    [SerializeField]
    private Transform enemyFlag;
    [SerializeField]
    private float AssignFlagTime = 2f;
    public string enemyTag;

    // Keeps track of whether someone is chasing flag or not.
    [SerializeField]
    private bool gettingFlag;

    public UI ui;

    void Start()
    {
        // Assign this team to all players.
        playerLength = players.Length;
        for (int i = 0; i < playerLength; i++)
        {
            players[i].SetTeam(this);
            players[i].SetEnemyHomeBase(enemyHomeZone);
            players[i].SetHomeFlagSpot(homeFlagSpot);
            players[i].SetEnemyFlag(enemyFlag);
            players[i].enemyTag = enemyTag;
        }

        homeZone.SetTeam(this);

        homeFlag.team = this;

        InvokeRepeating("AssignPlayerToGetFlag", 0f, AssignFlagTime);
    }

    void Update()
    {
    }

    public void SetGettingFlag(bool gettingFlag)
    {
        this.gettingFlag = gettingFlag;
    }

    public void AssignPlayerToGetFlag()
    {
        // Don't assign player if someone else is getting the flag.
        if (gettingFlag)
            return;

        // Assign a random player to get the flag.
        int randomPlayerIndex = Random.Range(0, playerLength);
        
        if(players[randomPlayerIndex].currState == CTF_STATE.WANDER)
        {
            players[randomPlayerIndex].SetBehaviorIndex(0);
            players[randomPlayerIndex].SetTarget(enemyFlag);
            players[randomPlayerIndex].SetState(CTF_STATE.GETTING_FLAG);

            gettingFlag = true;
        }
    }

    public void AssignPlayerToSaveTeammate(Transform teammate)
    {
        // Check if anyone can save teammate.
        for (int i = 0; i < playerLength; i++)
        {
            if (players[i].currState == CTF_STATE.WANDER)
            {
                players[i].SetBehaviorIndex(1);
                players[i].SetTarget(teammate);
                players[i].SetState(CTF_STATE.HELPING);
                break;
            }
        }
    }

    public void AssignPlayerToPursue(Transform target)
    {
        if (target.GetComponent<NPC_CTF>().currState == CTF_STATE.FROZEN)
            return;

        int closestToTarget = -1;
        float shortestDist = 9999;
        float newDist = 0f;

        for (int i = 0; i < playerLength; i++)
        {
            // Check if player is available to pursue.
            if (players[i].currState == CTF_STATE.WANDER)
            {
                // Check if player is closer than previously checked player.
                newDist = (target.position - players[i].transform.position).magnitude;
                if (newDist < shortestDist)
                {
                    closestToTarget = i;
                    shortestDist = newDist;
                }
            }
        }

        // Check if any players were available.
        if (closestToTarget != -1)
        {
            // Send player after target.
            players[closestToTarget].Pursue(target);
        }
    }

    public void Win()
    {
        ui.DisplayWinningTeam(name);
    }
}
