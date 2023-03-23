using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleForwardScript : MonoBehaviour
{
    public GameObject cameraObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = cameraObject.transform.position;
        transform.eulerAngles = new Vector3(0, cameraObject.transform.eulerAngles.y, cameraObject.transform.eulerAngles.z);
    }
}
