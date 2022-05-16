using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chain_behavior_script : MonoBehaviour
{
    public GameObject chainEnd, activeChainEnd, chainLink;

    GameObject chainHand, chainStart;
    LineRenderer chainLine;
    Vector3[] points;
    ArrayList chainPoints;

    private float chainTime;
    // Start is called before the first frame update
    void Start()
    {
        chainHand = transform.parent.gameObject;
        chainLine = GetComponent<LineRenderer>();
        chainLine.startWidth = 0.1f;
        chainLine.endWidth = 0.1f;
        points = new Vector3[2];
        activeChainEnd = null;
        chainPoints = new ArrayList();
        chainTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {

        if (!(activeChainEnd == null))
        {
            points[0] = chainHand.transform.position;
            points[1] = activeChainEnd.transform.position;
            chainLine.SetPositions(points);
            chainLine.enabled = true;
            //Debug.Log(points[1]);
        }
        else if (activeChainEnd == null)
        {
            resetPoints();
        }


        if (chainTime > 0)
        {
            chainTime -= 1 * Time.fixedDeltaTime;
            if (chainTime <= 0)
            {
                releaseChain();
                resetChain();
            }

        }
    }

    public GameObject createChain()
    {
        chainStart = Instantiate(chainLink, chainHand.transform);
        chainStart.transform.localPosition = Vector3.zero;
        chainPoints.Add(chainStart);
        chainStart.SetActive(true);
        chainStart.GetComponent<Rigidbody>().isKinematic = true;
        chainTime = 5f;
        return chainStart;

    }

    public GameObject createLink(Transform parent, Vector3 position)
    {
        GameObject temp = Instantiate(chainLink, position, parent.rotation, parent);
        temp.name = temp.name + "."+ Random.Range(0, 100);
        temp.SetActive(true);
        return temp;
    }

    public void grabChain()
    {
        GameObject endLink = createLink(chainStart.transform, activeChainEnd.transform.position);
        endLink.SetActive(true);
        chainPoints.Add(endLink);
        //Debug.LogError("Grabbing chain");
        activeChainEnd.GetComponent<ConfigurableJoint>().connectedBody = endLink.GetComponent<Rigidbody>();
        activeChainEnd.GetComponent<ConfigurableJoint>().massScale = 1f;
        activeChainEnd.GetComponent<Rigidbody>().freezeRotation = true;
        activeChainEnd.GetComponent<Rigidbody>().isKinematic = false;
        if (activeChainEnd.GetComponent<gun_behavior>().impaledEnemy != null)
            activeChainEnd.GetComponent<gun_behavior>().impaledEnemy.GetComponent<Enemy_Behavior>().killEnemy(Vector3.zero, false, activeChainEnd);

    }

    public void releaseChain()
    {
        Debug.Log("release chain");
        if (activeChainEnd != null)
        {
            activeChainEnd.GetComponent<ConfigurableJoint>().massScale = .001f;
            activeChainEnd.GetComponent<Rigidbody>().freezeRotation = false;
            activeChainEnd.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(activeChainEnd, 3f);
        }
        resetPoints();
        resetChain();
    }

    public void resetPoints()
    {
        points[0] = Vector3.zero;
        points[1] = Vector3.zero;
        chainLine.enabled = false;
    }

    public void resetChain()
    {
        Destroy(chainStart, 0f);
        chainTime = 0;
        //activeChainEnd = null;
        resetPoints();
    }
}
