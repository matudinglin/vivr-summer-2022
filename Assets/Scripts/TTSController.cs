using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

public class TTSController : MonoBehaviour
{
	private bool wait4speaking = false;
	private object threadLocker = new object();
	public AudioSource audioSource;
	private Dictionary<string, AudioClip> ttsCache = new Dictionary<string, AudioClip>();
	// Start is called before the first frame update

	// FIXME: Remember to deactivate it and use vault to manage it if we want to deploy it in production.
	private string azureSubscriptionKey = "4153265545034955baa5aae1ac760781";
	private string currentContent = null;
	public float repeatInterval = 2f;
	void Start()
	{
		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.playOnAwake = false;
		audioSource.loop = false;
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void Speak(string content)
	{
		//FIXME
		Debug.Log("Speak content: " + content);
		if (!wait4speaking)
		{
			// FIXME
			Debug.Log("I commented the speak here!");
            StartCoroutine(TTSCoroutine(content, repeatInterval));//!!!!!!! comment speak function and print things out
		}
	}

	public void SpeakInterruptively(string content)
	{
		//FIXME
		Debug.Log("SpeakInterruptively content: " + content);
		audioSource.Stop();
		wait4speaking = true;
		StartCoroutine(TTSCoroutine(content, repeatInterval));
		wait4speaking = false;
	}

	public AudioClip GenerateTTSAudioClip(string content)
	{
		
		if(ttsCache.ContainsKey(content)){
			return ttsCache[content];
		}
		// Thanks this blog: https://www.twblogs.net/a/5d48b0d7bd9eee5327fba4fd
		var config = SpeechConfig.FromSubscription(azureSubscriptionKey, "eastasia");
		using (var synthesizer = new SpeechSynthesizer(config, null))
		{
			Debug.Log("Call Azure API for" + content);
			var result = synthesizer.SpeakTextAsync(content).Result;
			if (result.Reason == ResultReason.SynthesizingAudioCompleted)
			{
				int sampleCount = result.AudioData.Length / 2;
				var audioData = new float[sampleCount];
				for (int i = 0; i < sampleCount; i++)
				{
					audioData[i] = (short)(result.AudioData[i * 2 + 1] << 8 | result.AudioData[i * 2]) / 32768.0F;
				}

				var audioClip = AudioClip.Create("SynthesizedAudio", sampleCount, 1, 16000, false);
				audioClip.SetData(audioData, 0);
				ttsCache[content] = audioClip;
				return audioClip;
			}
		}
		Debug.Log("Fail to call Azure API.");
		return null;
	}

	IEnumerator TTSCoroutine(string content, float waitSeconds)
	{
		//FIXME
		Debug.Log("HEREEEEEEEEE==========");
		Debug.Log("called: Ienumerator here");
		lock (threadLocker) { wait4speaking = true; }
		Debug.Log(content + " " + currentContent);
		if (content != currentContent)
		{
			var audioClip = GenerateTTSAudioClip(content); // it will make an audio here
			audioSource.clip = audioClip;
            currentContent = content;
		}
        audioSource.Play();
		yield return new WaitForSeconds(audioSource.clip.length + waitSeconds);
		lock (threadLocker) { wait4speaking = false; }
	}

}
