using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeTransparent : MonoBehaviour
{
    private Image image;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    public void changeTransparency(float value)
    {
        var temColor = image.color;
        temColor.a = value;
        image.color = temColor;
    }
}
