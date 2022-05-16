using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class gun_behavior : MonoBehaviour
{
    public GameObject reloadPoint, barrel, handle, muzzlePrefab, body, bodyCols, shellEject, ammoType, floatShine, bulletHole, floatHighlight, impaledEnemy;
    public GameObject gunAttachment;


    public double ammoCount, ammoMax, fireRate;
    public float velocityMult, thrownWeaponSpeed, thrownThreshold, bounceThreshold, bulletPower;
    public string gunName;
    public bool isHeld, isThrown, throwAway, canHurt, isFloating, automatic, isDagger;

    public Vector3 gripOffset;
    public AudioClip shootAudio;

    private GameObject weaponHand, prevHolder;
    private Rigidbody bodyPhysics;
    private CapsuleCollider throwSphereCol;
    private float gravityTime, unheldTime, autoFireTime, posMag;

    private bool hammerState;
    private Animation shootAnim;

    GameObject thrownTarget;
    Vector3 spawnPos, thrownPos, prevPos;
    void Start()
    {
        bodyPhysics = body.GetComponent<Rigidbody>();
        throwSphereCol = transform.GetComponent<CapsuleCollider>();
        throwSphereCol.enabled = false;
        isHeld = false;
        isThrown = false;
        canHurt = false;
        isFloating = false;
        thrownTarget = null;
        spawnPos = transform.position;
        gravityTime = 0; unheldTime = 0;

        floatShine.SetActive(false);
        if (floatHighlight != null) floatHighlight.SetActive(false);

        bodyCols.SetActive(true);
        prevHolder = null; impaledEnemy = null;
        hammerState = true; autoFireTime = 0;

        shootAnim = floatShine.transform.GetChild(0).transform.GetComponent<Animation>();
    }

    public void Update()
    {
        if (gunAttachment != null) gunAttachment.SetActive(isHeld && ammoCount > 0);
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        posMag = Vector3.Magnitude(prevPos - transform.position);
        if (posMag > thrownThreshold)
        {
            canHurt = (posMag > thrownThreshold) && !isHeld;
            throwSphereCol.enabled = (posMag > thrownThreshold) && !isHeld;
        }
        if (gravityTime > 0) gravityTime -= 1 * Time.fixedDeltaTime;
        if (gravityTime <= 0 & isFloating)
        {
            floatWeapon(false, -10);
            if (floatHighlight != null) floatHighlight.SetActive(false);
            bodyCols.SetActive(true);
        }

        if ((throwAway && !isHeld) && !isFloating) unheldTime += 1 * Time.fixedDeltaTime;
        if (unheldTime > 10)
            Destroy(this.gameObject, 0f);

        if (isHeld)
        {

            if ((autoFireTime < fireRate) && automatic)
            {
                autoFireTime += Time.deltaTime;
            }
            if ((autoFireTime >= fireRate && automatic && !hammerState))
                resetHammer();
        }
        prevPos = transform.position;
        bodyCols.SetActive(!isHeld);
        floatShine.SetActive(isHeld);

    }

    public void respawnGun()
    {
        if (!throwAway)
        {
            GameObject temp = Instantiate(this.gameObject, null);
            temp.transform.position = spawnPos;
            Destroy(this.gameObject, 0f);
        }
    }

    public void resetHammer()
    {
        hammerState = true;
        //Debug.Log("Hammer reset");
    }

    public bool isReadyFire()
    {
        return (ammoCount > 0 && hammerState);
    }

    public void ShootGun()
    {
        if (ammoCount > 0 && hammerState)
        {
            ammoCount -= 1;
            hammerState = false;
            //Debug.Log("We're shooting!");
            this.gameObject.GetComponent<AudioSource>().PlayOneShot(shootAudio);
            if (shootAnim != null) shootAnim.Play();

            weaponHand.GetComponent<hand_behavior>().Pulse(0.2f, 75, 1, weaponHand.GetComponent<hand_behavior>().handType);

            GameObject casing = Instantiate(ammoType.GetComponent<ammo_behavior>().ammoCasing, null);
            casing.transform.position = shellEject.transform.position;
            casing.SetActive(true);
            casing.GetComponent<Rigidbody>().AddForce(shellEject.transform.forward * 2 + Vector3.up, ForceMode.Impulse);
            Destroy(casing, 3f);

            //Debug.LogError("playing anim");
            if (muzzlePrefab)
            {
                var flash = Instantiate(muzzlePrefab, barrel.transform);
                Destroy(flash, 0.15f);
            }
            //We're shooting - start pos, start direction, and how far
            //Returns all objects hit by raycast
            RaycastHit[] hits = Physics.RaycastAll(barrel.transform.position, barrel.transform.forward, 100.0f);
            ArrayList enemyHits = new ArrayList();
            ArrayList killedEnemies = new ArrayList();
            float wallHitDistance = 1000;

            float currentBulletPower = bulletPower;

            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                drawBulletHole(hit);

                if (hit.collider.gameObject.tag == "Wall_thick")
                {
                    if (wallHitDistance > hit.distance) wallHitDistance = hit.distance;
                    //Debug.LogError("Wall hit");
                }


                if (hit.collider.gameObject.tag == "Enemy" || hit.collider.gameObject.tag == "Enemy_head")
                {
                    enemyHits.Add(hit);
                    Debug.DrawLine(barrel.transform.position, barrel.transform.forward * 100, Color.red);
                }

                foreach (RaycastHit x in enemyHits)
                {
                    Debug.LogWarning("Hit wall at _ " + wallHitDistance + " " + x.distance);
                    if (x.distance < wallHitDistance)
                    {
                        if (!killedEnemies.Contains(hit.collider.gameObject))
                        {
                            float headShot = 1;
                            killedEnemies.Add(hit.collider.gameObject);
                            Debug.LogWarning("Hit enemy _ " + x.distance);
                            //Destroy(x.collider.gameObject, 1f);
                            if (hit.collider.gameObject.tag == "Enemy_head") headShot = 5;
                            hit.collider.transform.root.GetComponent<Enemy_Behavior>().takeDamage(currentBulletPower * headShot, Vector3.forward * (bulletPower * -1), hit.point);
                            currentBulletPower = currentBulletPower / 2;
                        }
                    }

                }


            }
            //else Debug.LogError("Out of ammo!");
            if (automatic) autoFireTime = 0;
        }
    }

        public void releaseWeapon(GameObject target, Vector3 force, bool thrown)
        {
            bodyPhysics.isKinematic = false; isThrown = thrown; thrownTarget = target;
            prevHolder = weaponHand;
            //Debug.LogError(prevHolder.transform.gameObject);
            if (thrown && (force.magnitude > thrownThreshold) && target != null)
            {
                Debug.Log("weapon thrown");
                bodyPhysics.transform.LookAt(target.transform.GetComponent<Enemy_Behavior>().weakSpot.transform);
                bodyPhysics.velocity = Vector3.zero;
                bodyPhysics.AddForce((bodyPhysics.transform.forward * thrownWeaponSpeed), ForceMode.Impulse);
                thrownPos = transform.position;
                //Debug.LogWarning(bodyPhysics.transform.forward * thrownWeaponSpeed);
            }
            else
            {
                bodyPhysics.AddForce(force * velocityMult, ForceMode.Impulse);
                throwSphereCol.enabled = true;
            }

            //throwSphereCol.enabled = true;
            //canHurt = true;

            weaponHand = null;
            body.transform.parent = null;
            isHeld = false;
        }

        public void grabWeapon(GameObject hand)
        {
            bodyPhysics.isKinematic = true;
            throwSphereCol.enabled = false;
            floatWeapon(false, 0f);
            weaponHand = hand;

            body.transform.parent = weaponHand.transform;
            body.transform.position = weaponHand.transform.position;
            body.transform.localEulerAngles = new Vector3(-45, 180, 0);

            body.transform.localPosition = gripOffset;

            //Debug.Log("weapon grabbed " + weaponHand.gameObject.name);

            isHeld = true;
            isThrown = false;
            canHurt = false;
            isFloating = false;

        }

        public void takeReload(GameObject ammo)
        {
            //Debug.Log("RELOAD");
            this.ammoCount += ammo.GetComponent<ammo_behavior>().bullets;
            if (this.ammoCount > this.ammoMax)
                this.ammoCount = this.ammoMax;
            Destroy(ammo);
        }

        public void drawBulletHole(RaycastHit hit)
        {
            GameObject x = GameObject.Instantiate(bulletHole, null, true);
            x.SetActive(true);
            x.transform.position = hit.point;
            Destroy(x, 6f);
        }

        private void OnTriggerEnter(Collider other)
        {
            bool goodEnemy = false;
            if (!isDagger && (other.gameObject.tag == ammoType.tag) && (ammoCount < ammoMax))
                takeReload(other.gameObject);
            if (other.gameObject.tag == "Enemy_body")
                if (other.transform.root.GetComponent<Enemy_Behavior>().isAlive)
                    goodEnemy = true;

            if (goodEnemy && canHurt && isThrown && transform.gameObject.tag == "Weapon")
            {
                canHurt = false;
                isThrown = false;
                
                
                if (other != null) other.transform.root.GetComponent<Enemy_Behavior>().takeDamage(8, bodyPhysics.velocity, other.ClosestPoint(other.transform.position));
                if (other != null) other.transform.root.GetComponent<Enemy_Behavior>().stunEnemy(5f);

                //Force weapon back to player via physics
                //Vector3 returnForce = ((bodyPhysics.transform.forward * -1) * (thrownWeaponSpeed / bounceThreshold)) + new Vector3(0, 5, 0);

                //Or loft in air for player?

                if (ammoCount > 0)
                {

                    Debug.Log("weapon coming back");
                    float distance = Vector3.Distance(thrownPos, other.transform.root.position);
                    float floatTime = Mathf.Clamp(distance * 10, 3, 5);
                    Debug.LogWarning("distance " + distance);
                    floatWeapon(true, floatTime);
                    bodyCols.SetActive(false);
                    bodyPhysics.velocity = Vector3.zero;
                    //Vector3 returnForce = ((bodyPhysics.transform.forward * -0.05f)) + new Vector3(0, 1, 0);
                    bodyPhysics.transform.LookAt(prevHolder.transform);
                    //bodyPhysics.AddForce(returnForce, ForceMode.Impulse);
                    bodyPhysics.AddForce((bodyPhysics.transform.forward * bounceThreshold) + Vector3.up / 3, ForceMode.Impulse);
                }
                else if (ammoCount <= 0)
                {
                    Destroy(transform.gameObject, 5f);
                    grabWeapon(other.transform.GetComponent<Enemy_Behavior>().weakSpot);
                }
            }

            if (goodEnemy && canHurt && transform.gameObject.tag == "knife")
            {
                Destroy(other.gameObject, 10f);
                other.gameObject.GetComponent<Enemy_Behavior>().killEnemy(Vector3.zero, true, transform.gameObject);

                Debug.Log("knife hit");
                canHurt = false;
                throwSphereCol.enabled = false;
                floatWeapon(false, 0f);
                grabWeapon(other.gameObject.GetComponent<Enemy_Behavior>().weakSpot);
                this.gameObject.GetComponent<AudioSource>().Play();
            }

            if (goodEnemy && canHurt && transform.gameObject.tag == "chain_end")
            {
                other.transform.root.GetComponent<Enemy_Behavior>().stunEnemy(5f);
                impaledEnemy = other.gameObject;
                gameObject.tag = "dead_weapon";

                Debug.Log("chain_end hit");
                canHurt = false;
                throwSphereCol.enabled = false;
                floatWeapon(false, 0f);
                grabWeapon(other.gameObject.GetComponent<Enemy_Behavior>().weakSpot);
                this.gameObject.GetComponent<AudioSource>().Play();
            }

            if (other.gameObject.tag == "Wall_thick" || other.gameObject.tag == "Wall_thin" || other.gameObject.tag == "floor")
                floatWeapon(false, 0);
        }

        public void pullWeapon(GameObject caller)
        {
            if (!isDagger && isFloating)
            {
                transform.LookAt(caller.transform);
                float dist = Vector3.Distance(bodyPhysics.transform.position, caller.gameObject.transform.position);
                bodyPhysics.AddForce((transform.forward * dist) + (Vector3.up / dist), ForceMode.Impulse);
            }
        }

        private void floatWeapon(bool x, float t)
        {
            bodyPhysics.useGravity = !x;
            bodyCols.SetActive(!x);
            floatShine.SetActive(x);
            if (floatHighlight != null) floatHighlight.SetActive(x);
            gravityTime = t;
            isFloating = x;
        }

        private void OnTriggerExit(Collider other)
        {

        }
    }
