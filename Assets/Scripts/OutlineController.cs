using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Outline))]
public class OutlineController : MonoBehaviour
{
    private Outline outline;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineColor = new Color(255, 165, 0);
        outline.OutlineWidth = 20;
    }

    public bool isEnable()
    {
        return outline.enabled;
    }
    private IEnumerator Blink(int blinkCounter)
    {
        for (int i = 0; i < blinkCounter; i++)
        {
            outline.enabled = !outline.enabled;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void EnableOutline()
    {
        if(!outline.enabled)
        {
            StartCoroutine(Blink(5));
        }
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        if(outline.enabled)
        {
            outline.enabled = false;
        }
    }

    public void ShowOutline()
    {
        if (!outline.enabled)
        {
            StartCoroutine(Blink(5));
        }
        outline.enabled = false;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
