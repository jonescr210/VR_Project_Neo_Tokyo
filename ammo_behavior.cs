using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammo_behavior : MonoBehaviour
{
    public double bullets;
    public string ammoName;
    public GameObject ammoCasing;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void grabAmmo(GameObject temp)
    {
        this.transform.parent = temp.transform;
        this.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void releaseAmmo()
    {
        this.transform.parent = null;
        this.GetComponent<Rigidbody>().isKinematic = false;
    }
}
