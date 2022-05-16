using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class head_rot_clamp : MonoBehaviour
{
    // Start is called before the first frame update
    public float maxX, minX;
    public float maxY, minY;
    public float maxZ, minZ;
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float x = transform.rotation.eulerAngles.x;
        float y = transform.rotation.eulerAngles.y;
        float z = transform.rotation.eulerAngles.z;

        x = Mathf.Clamp(x, maxX, minX);
        y = Mathf.Clamp(y, maxY, minY);
        z = Mathf.Clamp(z, maxZ, minZ);

        //transform.rotation = Quaternion.Euler(x, y, z);
    }
}
