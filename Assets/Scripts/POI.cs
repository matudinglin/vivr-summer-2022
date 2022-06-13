using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POI : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject spatialAudioPrefab;
	public AudioClip auditoryIcon;
	public AudioClip ttsPhrase = null;
	public AudioClip clip;

	public string poiName;
	public string description;
	AudioSource audioSource;

	object mutex = new object();
	bool isPlaying = false;
	void Start()
	{
		GameObject spatialAudioObj = Instantiate(spatialAudioPrefab, transform.position, transform.rotation);
		audioSource = spatialAudioObj.GetComponent<AudioSource>();
		spatialAudioObj.transform.parent = transform;
	}

	// Update is called once per frame
	void Update()
	{

	}

	// play the clip
	public void PlayClip()
    {
		audioSource.clip = clip;
		audioSource.Play();
    }

	public void PlayTTS()
	{
		Debug.Log("Play TTS: ");
		audioSource.clip = ttsPhrase;
		audioSource.Play();
	}

	public float ClipLength(HintType hintType)
	{
		AudioClip clip = hintType == HintType.TTS ? ttsPhrase : auditoryIcon;
		if (clip)
		{
			return clip.length;
		}
		else 
		{
			return 0f;
		}
	}

	public void PlayAuditoryIcon()
	{
		Debug.Log("bjahfaksakcabscba play tts" + poiName);
		audioSource.clip = auditoryIcon;
		audioSource.Play();
	}

}
