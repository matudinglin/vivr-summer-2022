using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Semantic : MonoBehaviour
{
    public string description;
	public float lightness;
	public float hue;
	public AudioClip backgroundAudio;
	public Material mainMaterial;
	// Start is called before the first frame update
	void Start()
    {
		CalcAverageColor();
	}

    // Update is called once per frame
    void Update()
    {
   
    }

    void CalcAverageColor()
    {
		var mainColor = mainMaterial.color; // get the color
		float h, s, v; 
		Color.RGBToHSV(mainColor, out h, out s, out v);
		lightness = v;
		hue = h;
		Debug.Log("Material color: " +mainColor+ h + s + v);
	}
}
