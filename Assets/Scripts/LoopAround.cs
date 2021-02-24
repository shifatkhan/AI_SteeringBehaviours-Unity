using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopAround : MonoBehaviour
{
    [SerializeField]
    private Transform teleportDestination;

    [SerializeField]
    private float offset = 1f;

    [SerializeField]
    private bool top;
    [SerializeField]
    private bool bot;
    [SerializeField]
    private bool left;
    [SerializeField]
    private bool right;

    void Start()
    {
        if (bot || left)
            offset *= -1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Red") || other.CompareTag("Blue"))
        {
            Vector3 otherPos = other.transform.position;

            if (left || right)
            {
                other.transform.position = new Vector3(teleportDestination.position.x + offset, otherPos.y, otherPos.z);
            }
            else
            {
                other.transform.position = new Vector3(otherPos.y, otherPos.y, teleportDestination.position.z + offset);
            }
        }
    }
}
