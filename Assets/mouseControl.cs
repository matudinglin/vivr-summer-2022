using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mouseControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        if (Input.touchCount == 1)
        {
            Input.simulateMouseWithTouches = true;
        }
        else
        {
            Input.simulateMouseWithTouches = false;
        }

    }
}
