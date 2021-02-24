using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// NPC class for the game Capture the Flag.
/// We use an Enum to keep track of the state the NPC is currently.
/// </summary>
public enum CTF_STATE
{
    WANDER,
    PURSUE,
    FROZEN,
    HELPING,
    GETTING_FLAG
}
public class NPC_CTF : NPC
{
    [SerializeField]
    private GameObject frozenSignal;
    [SerializeField]
    private float askForHelpTime = 3.2f; // every x seconds, ask for help if frozen.

    // Current state.
    public CTF_STATE currState;

    // Pursue target
    private NPC_CTF pursueTarget;

    // TODO: Replace with S.O. Events instead.
    // The current team this NPC is part of.
    private Team team;
    private GameObject enemyHomeZone;
    private Transform homeFlagSpot;

    // Whether or not NPC is in enemy home base.
    public bool inEnemyHome;

    public bool hasFlag { get; protected set; }
    private Transform enemyFlag;

    public string enemyTag;

    public float evadeRadius = 5f;
    public float evadeWeight = 1.5f;
    public float arriveWeight = 1f;

    private void Start()
    {
        // Initialize current state.
        currState = CTF_STATE.WANDER;
        inEnemyHome = true;
        hasFlag = false;

        // Set to steering behaviors.
        behaviorIndex = 1;

        InvokeRepeating("AskForHelp", 5f, askForHelpTime);
    }

    protected override void Update()
    {
        base.Update();
        frozenSignal.SetActive(currState == CTF_STATE.FROZEN);
    }

    protected override void FixedUpdate()
    {
        if (currState == CTF_STATE.WANDER)
        {
            WanderUpdate();
        }
        else if (currState == CTF_STATE.PURSUE)
        {
            PursueUpdate();
            if (target != null)
                RotateUpdate();
        }
        else if (currState == CTF_STATE.FROZEN)
        {
            StopMovement();
        }
        else if (currState == CTF_STATE.GETTING_FLAG)
        {
            switch (behaviorIndex)
            {
                case 0:
                    KinematicUpdate();
                    break;
                case 1:
                    SteeringUpdate();
                    break;
                default:
                    Debug.LogError("Invalid behaviorIndex: Should be either 0 or 1.");
                    break;
            }
        }
        else if (currState == CTF_STATE.HELPING)
        {
            if (target.GetComponent<NPC_CTF>().currState != CTF_STATE.FROZEN)
            {
                SetState(CTF_STATE.WANDER);
                target = null;
                return;
            }

            switch (behaviorIndex)
            {
                case 0:
                    KinematicUpdate();
                    break;
                case 1:
                    SteeringUpdate();
                    break;
                default:
                    Debug.LogError("Invalid behaviorIndex: Should be either 0 or 1.");
                    break;
            }
        }
    }

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
        if (hasFlag)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, evadeRadius);
            int numColliders = hitColliders.Length;
            SteeringOutput evade = new SteeringOutput();

            for (int i = 0; i < numColliders; i++)
            {
                if (hitColliders[i].CompareTag(enemyTag) && hitColliders[i].GetComponent<NPC_CTF>().currState != CTF_STATE.FROZEN)
                {
                    steering.linear += evadeWeight * arrive[behaviorIndex].GetSteering(transform.position, hitColliders[i].transform.position, velocity).linear;
                }
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

    // ====================== ACTIONS ====================== //

    private void WanderUpdate()
    {
        SteeringOutput steering = wander.GetSteering(target, transform, velocity);

        velocity = steering.linear;
        // Update the position and random orientation.
        transform.position += steering.linear * Time.deltaTime;
    }

    private void PursueUpdate()
    {
        if (pursueTarget.hasFlag || pursueTarget.inEnemyHome)
        {
            SteeringOutput steering;

            steering = pursue.GetSteering(target, transform, velocity);

            // Restrict y translation.
            steering.linear = new Vector3(steering.linear.x, 0f, steering.linear.z);

            velocity = steering.linear;

            // Update the position.
            transform.position += steering.linear * Time.deltaTime;
        }
        // Stop pursuing if enemy is not in our home.
        else
        {
            SetState(CTF_STATE.WANDER);
            target = null;
            return;
        }
    }

    public void Pursue(Transform newTarget)
    {
        target = newTarget;
        pursueTarget = target.GetComponent<NPC_CTF>();
        SetState(CTF_STATE.PURSUE);
    }

    public void AskForHelp()
    {
        if (currState == CTF_STATE.FROZEN)
        {
            team.AssignPlayerToSaveTeammate(transform);
        }
    }

    // ====================== COLLISION ====================== //

    private void OnTriggerEnter(Collider other)
    {
        if (pursueTarget != null && other.gameObject == pursueTarget.gameObject)
        {
            // Pursue enemy to freeze them.
            pursueTarget.SetState(CTF_STATE.FROZEN);
            SetState(CTF_STATE.WANDER);
            target = null;
        }
        else if (other.CompareTag("Flag") && target != null && other.gameObject == target.gameObject)
        {
            hasFlag = true;

            // Got the flag
            target.position = transform.position;
            target.parent = transform;

            target.GetComponent<Flag>().FlagIsBeingTakenBy(transform);

            // Go to home spot
            target = homeFlagSpot;
        }
        else if (other.CompareTag(tag) && target != null && other.gameObject == target.gameObject)
        {
            // Save frozen teammate.
            NPC_CTF teammate = target.GetComponent<NPC_CTF>();
            if(teammate != null && teammate.currState == CTF_STATE.FROZEN)
            {
                teammate.SetState(CTF_STATE.WANDER);
                SetState(CTF_STATE.WANDER);
                target = null;
            }
        }
        else if(hasFlag && other.gameObject == homeFlagSpot.gameObject)
        {
            team.Win();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == enemyHomeZone)
        {
            inEnemyHome = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == enemyHomeZone)
        {
            inEnemyHome = false;
        }
    }

    // ====================== SETTERS ====================== //

    public void SetState(CTF_STATE newState)
    {
        if (currState == CTF_STATE.GETTING_FLAG && newState != CTF_STATE.GETTING_FLAG)
        {
            team.SetGettingFlag(false);

            // TODO: Double check.
            //target = null; 
        }
        if (newState == CTF_STATE.FROZEN)
        {
            if (hasFlag)
            {
                // Drop the flag.
                hasFlag = false;
                enemyFlag.GetComponent<Flag>().ResetPos();
            }
            AskForHelp();

            target = null;
        }

        currState = newState;
    }

    public void SetTeam(Team team)
    {
        this.team = team;
    }
    public void SetEnemyHomeBase(GameObject enemyHomeBase)
    {
        this.enemyHomeZone = enemyHomeBase;
    }

    public void SetEnemyFlag(Transform enemyFlag)
    {
        this.enemyFlag = enemyFlag;
    }

    public void SetHomeFlagSpot(Transform homeFlagSpot)
    {
        this.homeFlagSpot = homeFlagSpot;
    }
}
