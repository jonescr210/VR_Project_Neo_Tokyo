using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class laser_behavior : MonoBehaviour
{
    private LineRenderer line;
    private Vector3[] points;

    public float lineWidth;

    // Start is called before the first frame update
    void Start()
    {
        points = new Vector3[2];
        line = transform.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        points[0] = transform.GetChild(0).transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            points[1] = hit.point;
        }
        else points[1] = transform.forward * 3000;

        line.SetPositions(points);
    }
}
