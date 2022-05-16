using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head_Behavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject lEye, rEye, cEye;

    public float visionDistance, enemyTargetTime, gunTargetTime;
    GameObject enemyThrowTarget, gunTarget;

    private bool testTargeting;

    float lastTargetCounter;

    RaycastHit hit, hitL, hitR, hitC;
    bool hitLTrue, hitRTrue, hitCTrue;
    void Start()
    {
        enemyThrowTarget = null;
        lastTargetCounter = 0;
        testTargeting = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Debug.Log("Last target = " + enemyThrowTarget + " " + lastTargetCounter);
        if (enemyThrowTarget != null && !enemyThrowTarget.GetComponent<Enemy_Behavior>().isAlive)
        {
            enemyThrowTarget.GetComponent<Enemy_Behavior>().unselectedTarget();
            enemyThrowTarget = null;
            lastTargetCounter = 0;
        }
        

        if (lastTargetCounter <= 0 && (enemyThrowTarget != null))
        {
            enemyThrowTarget.GetComponent<Enemy_Behavior>().unselectedTarget();
            enemyThrowTarget = null;
        }

        if (lastTargetCounter < 0 && gunTarget != null)
        {
            gunTarget = null;
        }

        hitLTrue = (Physics.Raycast(lEye.transform.position, lEye.transform.forward, out hitL, visionDistance) && (hitL.collider.gameObject.CompareTag("Enemy_body") || hitL.collider.gameObject.CompareTag("Weapon")));
        hitRTrue = (Physics.Raycast(rEye.transform.position, rEye.transform.forward, out hitR, visionDistance) && (hitR.collider.gameObject.CompareTag("Enemy_body") || hitR.collider.gameObject.CompareTag("Weapon")));
        hitCTrue = (Physics.Raycast(cEye.transform.position, cEye.transform.forward, out hitC, visionDistance) && (hitC.collider.gameObject.CompareTag("Enemy_body") || hitC.collider.gameObject.CompareTag("Weapon")));

        bool eyeHit = (hitLTrue || hitRTrue || hitCTrue);

        if (eyeHit)
        {
            if (hitLTrue) hit = hitL;
            if (hitRTrue) hit = hitR;
            if (hitCTrue) hit = hitC;

            //Debug.Log(hit.collider.gameObject);
            string tag = hit.collider.gameObject.tag;

            switch (tag)
            {
                case "Enemy_body":
                    if (hit.collider.transform.root.GetComponent<Enemy_Behavior>().isAlive)
                    {
                        lastTargetCounter = enemyTargetTime;
                        if (enemyThrowTarget != null && enemyThrowTarget != hit.collider.gameObject)
                            enemyThrowTarget.GetComponent<Enemy_Behavior>().unselectedTarget();

                        enemyThrowTarget = hit.collider.transform.root.gameObject;
                        enemyThrowTarget.GetComponent<Enemy_Behavior>().selectedTarget();
                        //Debug.DrawRay(transform.position, transform.forward, Color.red, 10);
                        //Debug.LogError(hitLTrue + " " + hitCTrue + " " + hitRTrue);
                    }
                    break;

                case "Weapon":
                    if (hit.collider.gameObject.GetComponent<gun_behavior>().isFloating)
                    {
                        lastTargetCounter = gunTargetTime;
                        if (gunTarget != null)
                            gunTarget.GetComponent<gun_behavior>().floatHighlight.SetActive(false);


                        gunTarget = hit.collider.gameObject; ;
                        gunTarget.GetComponent<gun_behavior>().floatHighlight.SetActive(true);
                        //Debug.DrawRay(transform.position, transform.forward, Color.yellow, 10);
                    }
                    break;
            }
        }
        if (lastTargetCounter > 0 ) lastTargetCounter -= 1;
    }

    public GameObject getThrowTarget()
    {
        return enemyThrowTarget;
    }

    public GameObject getGunTarget()
    {
        return gunTarget;
    }

    public void OnTriggerExit(Collider other)
    {
        
    }
}
