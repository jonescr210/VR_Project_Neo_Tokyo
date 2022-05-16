using System.Collections;
using System.Collections.Generic;
using Valve.VR;
using UnityEngine;
using System;

public class hand_behavior : MonoBehaviour
{
    public SteamVR_Action_Boolean grab, use, armChain;
    public SteamVR_Action_Vector2 move;
    public SteamVR_Action_Vector3 move3;
    public SteamVR_Action_Vibration haptic;
    public SteamVR_Input_Sources handType;

    public bool debug;

    private GameObject item, heldObject, activeHolster, chain, chainStart;
    public GameObject playerBody, playerHead, foreArmGear;
    private new Collider collider;

    private Vector3 prevHandPos, handForce, move3Direction;
    private Vector2 moveDirection, rotationDirection;

    public float playerSpeed, rotateSpeed, forceCount;
    bool throwWeapon, inHolder, fireWeapon, gripTrue, chainButton;

    void Start()
    {
        grab.AddOnStateDownListener(GripTrue, handType);
        grab.AddOnStateUpListener(GripFalse, handType);
        use.AddOnStateDownListener(useItem, handType);
        use.AddOnStateUpListener(stopUseItem, handType);
        armChain.AddOnStateDownListener(readyChain, handType);
        armChain.AddOnStateUpListener(unReadyChain, handType);

        move.AddOnAxisListener(movePlayer, handType);
        //move3.AddOnAxisListener(move3PLayer, handType);
        moveDirection = new Vector2(0, 0);
        rotationDirection = new Vector2(0, 0);

        //playerSpeed = 0.5f;
        forceCount = 0;
       
        collider = this.gameObject.GetComponent<BoxCollider>();
        //Debug.LogError("Active hand is: " + handType);

        throwWeapon = false; inHolder = false; fireWeapon = false;
        gripTrue = false;
    }

    private void move3PLayer(SteamVR_Action_Vector3 fromAction, SteamVR_Input_Sources fromSource, Vector3 axis, Vector3 delta)
    {
        throw new NotImplementedException();
    }

    private void movePlayer(SteamVR_Action_Vector2 fromAction, SteamVR_Input_Sources fromSource, Vector2 axis, Vector2 delta)
    {
        if (fromSource == SteamVR_Input_Sources.RightHand)
        {
            //rotationDirection = axis;
            //Debug.Log("rotating player " + rotationDirection);
            //if (playerBody)
            //{

            //}
        }

        if (fromSource == SteamVR_Input_Sources.LeftHand)
        {
            moveDirection = axis;
            Quaternion headYaw = Quaternion.Euler(0, playerHead.transform.eulerAngles.y, 0);
            Vector3 move = headYaw * new Vector3(axis.x, 0, axis.y);

            //Debug.Log("Moving Player " + move);
            //Debug.Log("Player facing " + playerHead.transform.forward);
            if (playerBody)
            {
                playerBody.transform.position += move * playerSpeed * Time.fixedDeltaTime;
            }
        }
    }

    private void useItem(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (heldObject)
        {
            string tag = heldObject.gameObject.tag;
            switch (tag)
            {
                case "Weapon":
                    //Debug.Log("Firing Weapon");
                    fireWeapon = true;
                    // heldObject.GetComponent<gun_behavior>().ShootGun();
                    break;
                default:
                    //Debug.Log("Unknown item");
                    break;
            }
        }

        if (!heldObject && playerHead.GetComponent<Head_Behavior>().getGunTarget() != null)
        {
            //heldObject = playerHead.GetComponent<Head_Behavior>().getGunTarget();
            //playerBody.GetComponent<player_behavior>().grabWeapon(fromSource == SteamVR_Input_Sources.LeftHand, heldObject);
            //heldObject.GetComponent<gun_behavior>().grabWeapon(this.gameObject);


                GameObject temp = playerHead.GetComponent<Head_Behavior>().getGunTarget();
                temp.GetComponent<gun_behavior>().pullWeapon(this.gameObject);
        }
    }

    private void stopUseItem(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (heldObject)
        {
            string tag = heldObject.gameObject.tag;
            switch (tag)
            {
                case "Weapon":
                    fireWeapon = false;
                    heldObject.gameObject.GetComponent<gun_behavior>().resetHammer();
                    // heldObject.GetComponent<gun_behavior>().ShootGun();
                    break;
                case "chain":
                    heldObject = null;
                    break;

                default:
                    //Debug.Log("Unknown item");
                    break;
            }
        }

        if (!heldObject && foreArmGear.gameObject != null & chain != null)
        {
            Destroy(chain, 0f);

        }
    }

    private void GripFalse(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        //Debug.LogError("Letting go..." + heldObject.name);
        gripTrue = false;
        if (heldObject)
        {
            if (heldObject.tag == "Weapon" || heldObject.tag == "knife" || heldObject.tag == "chain_end" || heldObject.tag == "chain")
            {
                //Debug.Log("Release weapon");
                playerBody.GetComponent<player_behavior>().releaseWeapon(fromSource == SteamVR_Input_Sources.LeftHand);


                if (heldObject.tag == "chain_end")
                {
                    chainStart = foreArmGear.GetComponent<chain_behavior_script>().createChain();
                }

                if (heldObject.tag == "chain")
                {
                    foreArmGear.GetComponent<chain_behavior_script>().releaseChain();
                    Destroy(chainStart, 0f);
                }



                if (throwWeapon)
                    heldObject.GetComponent<gun_behavior>().releaseWeapon(playerHead.GetComponent<Head_Behavior>().getThrowTarget(), handForce, true);
                else
                {
                    if (heldObject.tag != "chain") heldObject.GetComponent<gun_behavior>().releaseWeapon(null, handForce, false);
                    if (inHolder && activeHolster != null)
                        activeHolster.GetComponent<wep_holder_behavior>().holdWeapon(heldObject);
                }
            }
        else if (heldObject.tag == "ammo")
            {
                heldObject.GetComponent<ammo_behavior>().releaseAmmo();
            }
        }
        heldObject = null;
        item = null;
    }

    private void GripTrue(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        gripTrue = true;
        bool leftHand = (fromSource == SteamVR_Input_Sources.LeftHand);
        if (item)
        {
            //Debug.LogWarning("Trying to grab..." + item.gameObject.tag);
            string tag = item.gameObject.tag;
            switch (tag)
            {
                case "Weapon":
                    //Debug.Log("Weapon held " + item.gameObject);
                    if (!item.gameObject.GetComponent<gun_behavior>().isHeld && !chainButton)
                    {
                        heldObject = item.gameObject;
                        playerBody.GetComponent<player_behavior>().grabWeapon(fromSource == SteamVR_Input_Sources.LeftHand, heldObject);

                        heldObject.GetComponent<gun_behavior>().grabWeapon(transform.gameObject);

                        //Test Method for test Map
                        //GameObject.FindGameObjectWithTag("Map").GetComponent<test_range>().spawnTargets();
                    }
                        break;
                case "knife":
                    if (!item.gameObject.GetComponent<gun_behavior>().isHeld && !chainButton)
                    {
                        heldObject = item.gameObject;
                        playerBody.GetComponent<player_behavior>().grabWeapon(fromSource == SteamVR_Input_Sources.LeftHand, heldObject);

                        heldObject.GetComponent<gun_behavior>().grabWeapon(transform.gameObject);

                        //Test Method for test Map
                        //GameObject.FindGameObjectWithTag("Map").GetComponent<test_range>().spawnTargets();
                    }
                    break;
                case "chain_end":
                    if (!item.gameObject.GetComponent<gun_behavior>().isHeld)
                    {
                        heldObject = item.gameObject;
                        playerBody.GetComponent<player_behavior>().grabWeapon(fromSource == SteamVR_Input_Sources.LeftHand, heldObject);

                        heldObject.GetComponent<gun_behavior>().grabWeapon(transform.gameObject);

                        //Test Method for test Map
                        //GameObject.FindGameObjectWithTag("Map").GetComponent<test_range>().spawnTargets();
                    }
                    break;
                case "chain":
                    {
                        heldObject = item.gameObject;
                        playerBody.GetComponent<player_behavior>().grabWeapon(fromSource == SteamVR_Input_Sources.LeftHand, heldObject);

                        foreArmGear.GetComponent<chain_behavior_script>().grabChain();
                    }
                    break;
                case "ammoPouch":
                    //Debug.LogWarning("Trying pouch...");

                    if (!chainButton)
                    {
                        GameObject currentObject;
                        if (leftHand)
                            currentObject = playerBody.GetComponent<player_behavior>().rightHandObject.GetComponent<gun_behavior>().ammoType;
                        else if (!leftHand)
                            currentObject = playerBody.GetComponent<player_behavior>().leftHandObject.GetComponent<gun_behavior>().ammoType;
                        else currentObject = null;

                        if (!(currentObject == null))
                        {
                            //Debug.LogWarning("Grabbing ammo from pouch");
                            GameObject ammo = Instantiate(currentObject, transform);
                            ammo.SetActive(true);
                            ammo.transform.localPosition = Vector3.zero;
                            heldObject = ammo;
                            ammo.GetComponent<ammo_behavior>().grabAmmo(gameObject);
                        }
                    }
                    break;
                case "ammo":
                    //Debug.LogWarning("Grabbing Ammo item");
                    if (!chainButton)
                    {
                        heldObject = item.gameObject;
                        heldObject.GetComponent<ammo_behavior>().grabAmmo(gameObject);
                    }
                    break;
            }
        }
    }

    public void readyChain(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        if (!heldObject && foreArmGear.gameObject != null)
        {
            chain = Instantiate(foreArmGear.GetComponent<chain_behavior_script>().chainEnd, transform);
            foreArmGear.GetComponent<chain_behavior_script>().activeChainEnd = chain;
            foreArmGear.GetComponent<chain_behavior_script>().resetChain();

            chain.SetActive(true);
        }
        chainButton = true;
    }

    public void unReadyChain(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {

        if (!heldObject && foreArmGear.gameObject != null)
        {
            chain.SetActive(false);
            Destroy(chain, 0f);
            foreArmGear.GetComponent<chain_behavior_script>().resetChain();
        }
        chainButton = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (forceCount < 0) prevHandPos = this.transform.position;

        forceCount += 1;

        if (forceCount > 5)
        {
            handForce = (this.transform.position - prevHandPos);
            if (debug) Debug.Log("handForce " + handForce);
            forceCount = -1;
        }

        if (fireWeapon && heldObject.tag == "Weapon")
            if (heldObject.GetComponent<gun_behavior>().isReadyFire())
                    heldObject.GetComponent<gun_behavior>().ShootGun();


        throwWeapon = (playerHead.transform.GetComponent<Head_Behavior>().getThrowTarget() != null);

        if (debug) Debug.Log(forceCount);
    }

    public void Pulse(float duration, float freq, float amplitude, SteamVR_Input_Sources source)
    {
        haptic.Execute(0, duration, freq, amplitude, source);
    }


    private void OnTriggerEnter(Collider other)
    {
        String x = other.gameObject.tag;
        switch (x)
        {
            case "Weapon":
                if (heldObject == null)
                {
                    item = other.gameObject;
                    Pulse(0, 50, 1, handType);
                }
                break;

            case "knife":
                if (heldObject == null)
                    item = other.gameObject;
                break;

            case "chain_end":
                if (heldObject == null)
                    item = other.gameObject;
                break;

            case "chain":
                if (heldObject == null)
                    chainStart = other.gameObject;
                break;

            case "ammoPouch":
                if (heldObject == null)
                    item = other.gameObject;
                break;

            case "ammo":
                if (heldObject == null)
                    item = other.gameObject;
                break;

            case "Weapon_holder":
                activeHolster = other.gameObject;
                inHolder = true;
                Pulse(0, 50, 1, handType);
                break;

            case "Weapon_spawn":
                if (heldObject == null && other.transform.childCount < 1)
                {
                    GameObject gun = GameObject.FindGameObjectWithTag("Map").GetComponent<Map_Behavior>().randomGun();
                    gun.transform.parent = other.gameObject.transform;
                    gun.transform.localPosition = Vector3.zero;
                    gun.transform.GetComponent<Rigidbody>().isKinematic = true;
                    gun.SetActive(true);
                    item = gun;
                    Pulse(0, 50, 1, handType);
                }
                break;

            default:
                break;
        }

        //if (other.gameObject.tag == "Weapon" & heldObject == null)
        //{
        //    item = other.gameObject;
        //    //Debug.LogError("Weapon found!! Item set to " + item.name);
        //}
        //else if ((other.gameObject.tag == "ammoPouch" || other.gameObject.tag == "ammo")
        //    && heldObject == null)
        //{
        //    item = other.gameObject;
        //    //Debug.LogError("Ammo found!! Item set to " + other.gameObject);
        //}
        ////Debug.Log(other.gameObject.name + ", " + other.gameObject.tag + " found");
    }

    private void OnTriggerExit(Collider other)
    {
        
        if (other.gameObject.CompareTag("Weapon_holder"))
        {
            inHolder = false;
            activeHolster = null;
        }
        if (chainStart != null)
        {
            item = chainStart;
        }

        if (!heldObject && chainStart == null)
            item = null;

        //Debug.LogError(other.gameObject.name);
    }
}
