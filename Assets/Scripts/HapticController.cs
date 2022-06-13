using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HapticController : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject wallHapticObj;
	public GameObject poiHapticObj;
	HapticSource wallHaptic;
	HapticSource poiHaptic;
	AudioSource wallAudioSource;

	object mutex = new object();
	bool vibrating = false;
	void Start()
	{
		wallHaptic = wallHapticObj.GetComponent<HapticSource>();
		poiHaptic = poiHapticObj.GetComponent<HapticSource>();
		wallAudioSource = wallHapticObj.GetComponent<AudioSource>();
	}

	// Update is called once per frame
	void Update()
	{
	}
	IEnumerator Vibrate(float duration)
	{
		lock (mutex) { vibrating = true; }
		Handheld.Vibrate();
		yield return new WaitForSeconds(duration);
		lock (mutex) { vibrating = false; }
	}

	IEnumerator POIFeedback()
	{
		lock (mutex) { vibrating = true; }
		//Debug.Log("Wall haptic.");
		wallHaptic.Play();
		yield return new WaitForSeconds(1.5f);
		lock (mutex) { vibrating = false; }
	}
	public void PlayWallFeedback(bool playSound = false)
	{
		if (!vibrating) { StartCoroutine(Vibrate(.8f)); }
		if(playSound)
		{
			// playing the sound
			Debug.Log("Wall is playing the sound");
			wallAudioSource.Play();
		}
	}

	public void PlayPOIFeedback()
	{
		if (!vibrating) { StartCoroutine(POIFeedback()); }
	}
}

