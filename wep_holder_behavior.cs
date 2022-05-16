using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wep_holder_behavior : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject heldWeapon;
    void Start()
    {
        heldWeapon = null;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void holdWeapon(GameObject weapon)
    {
        heldWeapon = weapon;
        heldWeapon.transform.parent = transform;
        heldWeapon.GetComponent<Rigidbody>().isKinematic = true;
        Debug.LogError("Got weapon - " + weapon);
    }

    public void releaseWeapon(GameObject hand)
    {

    }
}
