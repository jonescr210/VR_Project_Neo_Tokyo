using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_behavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject rightHandObject, leftHandObject, ammoPouch, playerHead, feet, body;
    public GameObject leftHand, rightHand;

    GameObject enemyTarget;
    private Vector3 startPosition, lastPosition;
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        startPosition = transform.position;
    }

    public void releaseWeapon(bool hand)
    {
        if (hand) leftHandObject = null;
        else rightHandObject = null;
    }

    public void grabWeapon(bool hand, GameObject weapon)
    {
        if (hand) leftHandObject = weapon;
        else rightHandObject = weapon;
    }

    public void resetPosition()
    {
        transform.position = startPosition;
    }

    public void OnColliderEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall_thick")
            resetPosition();
    }
}
