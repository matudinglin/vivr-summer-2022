using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HintType { TTS, AuditoryIcon }
public class POIController : MonoBehaviour
{
	public enum PlayStrategy
	{
		Sequential, Simultaneous
	}


	public PlayStrategy strategy;
	public HintType hintType;
	
	// Start is called before the first frame update

	LevelController levelController;
	TTSController ttsController;

	GameObject currentRoom
	{
		get { return levelController.currentRoom; }
	}
	void Start()
	{
		levelController = GetComponent<LevelController>();
		ttsController = GetComponent<TTSController>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void PlayPOIHint()
	{
		//
		Debug.Log("This is PlayPOIHint() in POIController");
		Debug.Log("the strategy: " + strategy.ToString());
		if (strategy == PlayStrategy.Simultaneous)
		{
			HintSimultaneously();
		}
        else if(strategy == PlayStrategy.Sequential)
        {
			StartCoroutine(PlaySequentially());
        }
	}

	IEnumerator PlaySequentially()
	{
		//
		Debug.Log("This is PlaySequentially()");
		Debug.Log("currentRoom: " + currentRoom.ToString());

        // FIXME
        Debug.Log("Let's print the children in this room");
		//obj = currentRoom.GetComponentsInChildren<GameObject>();
		//foreach (GameObject ob in obj)
		//{
		//	Debug.Log(ob.name);
		//}
		
		// FIXME
		POI[] pois = currentRoom.GetComponentsInChildren<POI>();
		//GameObject[] obj = currentRoom.GetComponentInChildren<GameObject>();
		//GameObject[] obj = currentRoom.GetComponentsInChildren<GameObject>();
		//Debug.Log("obj.length: " + obj.Length);

		

		Debug.Log("pois: " + pois.ToString());
		Debug.Log("pois.length: " + pois.Length);

		// error: the pois length is always 0
		foreach (var poi in pois)
		{
			if (hintType == HintType.TTS)
			{
				if (poi.ttsPhrase == null)
				{
					poi.ttsPhrase = ttsController.GenerateTTSAudioClip(poi.poiName);
				}
				poi.PlayTTS();
				OutlineController outlineController = poi.GetComponent<OutlineController>();
				if(outlineController != null)
					outlineController.ShowOutline();
			}
			else if (hintType == HintType.AuditoryIcon)
			{
				poi.PlayAuditoryIcon();
			}
			var clipLength = poi.ClipLength(hintType);
			yield return new WaitForSeconds(clipLength);
		}
		yield return null;
	}

	void HintSimultaneously()
	{
		POI[] pois = currentRoom.GetComponentsInChildren<POI>();
		foreach (var poi in pois)
		{
			if (hintType == HintType.TTS)
			{
				if (poi.ttsPhrase == null)
				{
					poi.ttsPhrase = ttsController.GenerateTTSAudioClip(poi.poiName);
				}
				poi.PlayTTS();
			}
			else if (hintType == HintType.AuditoryIcon)
			{
				poi.PlayAuditoryIcon();
			}
		}
	}
}
