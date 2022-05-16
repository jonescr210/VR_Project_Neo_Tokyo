using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rand_target : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject enemy;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.LogError(gameObject.tag);
        if (other.gameObject.tag == "playerHand") {
            GameObject target = Instantiate(enemy, enemy.transform);
            target.SetActive(true);
            Debug.LogError("Reset Target!");
        }
    }
}
