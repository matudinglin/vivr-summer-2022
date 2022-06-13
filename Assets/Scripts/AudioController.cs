using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	// Start is called before the first frame update
	public AudioClip baseAudio;
	public AudioSource audioSource;

	public int note = -1;

	void Start()
	{
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = false;
		audioSource.clip = baseAudio;
	}

	// Update is called once per frame
	void Update()
	{
	}

	public AudioSource switchHintAudio;

	public void SwitchViewHint()
	{
		switchHintAudio.Play();
	}

	private int HSV2Note(float hue, float lightness)
	{
		int lightShift = (int)(lightness / 0.5f) * 12;
		return (int)(hue / 0.1f) + lightShift;
	}

	public void PlayBackground(GameObject room)
	{
		if (room != null && !audioSource.isPlaying)
		{
			audioSource.Stop();
			var semantic = room.GetComponent<Semantic>();
			var lightness = semantic.lightness;
			var hue = semantic.hue;
			note = HSV2Note(hue, lightness);
			if (semantic.backgroundAudio)
			{
				audioSource.clip = semantic.backgroundAudio;
			}
			else
			{
				audioSource.clip = baseAudio;
			}
			// shift the pitch
			var transpose = -4;
			audioSource.pitch = Mathf.Pow(2, (note + transpose) / 12.0f);
			audioSource.Play();
		}
	}

	public void PauseBackground(GameObject room)
	{
		if (room != null)
		{
			note = -1;
			if (audioSource.isPlaying)
			{
				audioSource.Pause();
			}
		}
	}
}