using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vision_behavior : MonoBehaviour
{

    public GameObject targetedPlayer;
    public float targetTime;

    private MeshCollider visionCone;
    private float lastTarget;
    void Start()
    {
        visionCone = transform.GetComponent<MeshCollider>();
        lastTarget = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            targetedPlayer = other.gameObject;
            lastTarget = targetTime * 10;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
