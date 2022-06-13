using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeaconController : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject beaconPrefab;

	GameObject[] beaconSources = null;
	LevelController levelController;
	GameObject activeRoom;

	void Start()
	{
		levelController = GetComponent<LevelController>();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateBeacons();
		if (levelController.IsImmersive())
		{
			foreach (var beaconSource in beaconSources)
			{
				if (beaconSource.activeInHierarchy)
				{
					var audioSource = beaconSource.GetComponent<AudioSource>();
					if (!audioSource.isPlaying)
					{
						audioSource.Play();
					}
				}
			}
		}
	}


	void UpdateBeacons()
	{
		if (activeRoom != levelController.currentRoom)
		{
			activeRoom = levelController.currentRoom;
			Beacon[] beacons = activeRoom.GetComponentsInChildren<Beacon>();
			beaconSources = new GameObject[beacons.Length];
			for (int i = 0; i < beacons.Length; i++)
			{
				GameObject beaconObj = beacons[i].gameObject;
				beaconSources[i] = Instantiate(
					beaconPrefab,
                    // Adjust the human's height
					beaconObj.transform.position + new Vector3(0f, 1.6f, 0f),
					beaconObj.transform.rotation
				);
				beaconSources[i].GetComponent<AudioSource>().clip = beacons[i].beaconClip;
				beaconSources[i].transform.parent = beaconObj.transform;
			}
		}
	}
}
