using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Behavior : MonoBehaviour
{
    public GameObject weakSpot, head, headModel, headGib, visionCone, targetedPlayer, lastTargetedPlayer, targetIndicator, map;
    public bool isAlive;
    public float targetTime, lookAtThreshold, health;
    public AudioClip deathSound;

    private Rigidbody bodyPhysics, headPhysics;
    private vision_behavior enemyVision;
    private Animator anim;
    private Map_Behavior map_Behavior;
    private bool stunned, freshSpawn;
    private float stunTime, lastTargetTime, spawnProtection;

    private Quaternion headRotStart;
    Vector3 lookAtOld;

    void Start()
    {
        bodyPhysics = transform.GetComponent<Rigidbody>();
        headPhysics = weakSpot.GetComponent<Rigidbody>();
        anim = transform.GetComponent<Animator>();
        map_Behavior = map.GetComponent<Map_Behavior>();
        stunned = false;
        isAlive = false;
        stunTime = 3;
        enemyVision = visionCone.GetComponent<vision_behavior>();
        headRotStart = head.transform.rotation;
        lastTargetTime = 0;
        lookAtOld = Vector3.zero;
        spawnProtection = 0; freshSpawn = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (freshSpawn)
            spawnProtection += 1 * Time.fixedDeltaTime;
        //Debug.Log(spawnProtection);
        if (spawnProtection >= 1 && freshSpawn)
        {
            isAlive = true; freshSpawn = false;
        }
        //if (lastTargetTime > 0) lastTargetTime -= 1;
        if (stunned)
        {
            anim.Play("stun_anim");
            stunTime -= 1 * Time.fixedDeltaTime;
        }

        if (stunTime <= 0 && stunned)
        {
            stunned = false;
            anim.Play("Idle_Stance_01");
        }

        if (targetedPlayer == null & !stunned && lastTargetTime < 0)
        {
            anim.Play("Looking_Around_Idle_02");
            lastTargetedPlayer = null;
            head.transform.rotation = headRotStart;
            lastTargetTime = 0;
        }

        if (targetedPlayer != null)
        {
            lastTargetTime += 1;
            anim.Play("EnemyFound");
            Vector3 x = targetedPlayer.transform.position;
            //     x.x = Mathf.Round(x.x);
            //     x.y = Mathf.Round(x.y);
            //    x.z = Mathf.Round(x.z);
            //     Debug.Log(x);

            //      if ((xChange + yChange + zChange >= lookAtThreshold))
            //         head.transform.LookAt(x);
            lookAtOld = x;
            head.transform.LookAt(x);
            float rotX = Mathf.Clamp(head.transform.localEulerAngles.x, head.GetComponent<head_rot_clamp>().minX, head.GetComponent<head_rot_clamp>().maxX);
            float rotY = head.transform.localEulerAngles.y;
            float rotZ = Mathf.Clamp(head.transform.localEulerAngles.z, head.GetComponent<head_rot_clamp>().minZ, head.GetComponent<head_rot_clamp>().maxZ);

            if (rotY > 90 && rotY < 180) rotY = 90;
            if (rotY < 270 && rotY > 180) rotY = 270;

            Debug.Log("rotY" + rotY);
            head.transform.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
        }

        //targetedPlayer = enemyVision.targetedPlayer;
        lastTargetedPlayer = targetedPlayer;

        //Debug.Log(lastTargetedPlayer.transform);
    }

    public void selectedTarget()
    {
        //Debug.Log("Selected");
        targetIndicator.GetComponent<Light>().enabled = true;
    }

    public void unselectedTarget()
    {
        targetIndicator.GetComponent<Light>().enabled = false;

    }

    public void takeDamage(float damage, Vector3 force, Vector3 contactPoint)
    {
        health -= damage;
        if (health <= 0 && isAlive)
            killEnemy(force, true, null);
        Debug.Log(damage);
    }

    public void killEnemy(Vector3 force, bool gravity, GameObject spear)
    {
        isAlive = false;
        visionCone.SetActive(false);
        bodyPhysics.isKinematic = false;
        bodyPhysics.useGravity = gravity;
        bodyPhysics.AddForce(force, ForceMode.Impulse);
        anim.enabled = false;
        map_Behavior.removeEnemy(this.gameObject);
        //Debug.LogError("gib");
        Destroy(transform.gameObject, 10f);
        this.gameObject.GetComponent<AudioSource>().PlayOneShot(deathSound);
        //headGib.SetActive(true);
        //headGib.transform.GetComponent<SphereCollider>().enabled = false;
        Destroy(headModel);
        if (spear != null)
        {
            //headGib.transform.parent = spear.transform;
            //headGib.transform.localPosition = headGib.transform.GetComponent<gib_offset>().offset;
            //headGib.transform.localRotation.eulerAngles.Set(headGib.transform.GetComponent<gib_offset>().rotation.x, headGib.transform.GetComponent<gib_offset>().rotation.y, headGib.transform.GetComponent<gib_offset>().rotation.z);
           //headGib.transform.GetComponent<Rigidbody>().isKinematic = true;
            spear.transform.parent = null;
            spear.transform.GetComponent<CapsuleCollider>().enabled = false;

        }
        else {
            //headGib.transform.parent = null;
            //headGib.transform.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
         }
    }

    public void stunEnemy(float time)
    {
        stunned = true;
        stunTime = time;
    }

    public void ragdoll()
    {
        anim.enabled = false;
    }
}
