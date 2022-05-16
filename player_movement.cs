using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class player_movement : MonoBehaviour

{
    private CharacterController character;

    public double playerSpeed;

    void Start()
    {
        character = GetComponent<CharacterController>();   
    }

    // Update is called once per frame
    void Update()
    {

    }

}
