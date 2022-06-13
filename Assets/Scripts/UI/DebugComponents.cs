using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugComponents : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject systemControllers;

	public Button zoomOutButton;
	public Button zoomInButton;
	public GameObject defaultRoom;
	AudioController audioController;
	CameraController cameraController;
	LevelController levelController;

	void Start()
	{
        audioController = systemControllers.GetComponent<AudioController>();
		cameraController = systemControllers.GetComponent<CameraController>();
        levelController = systemControllers.GetComponent<LevelController>();

		zoomInButton.onClick.AddListener(delegate
		{
			levelController.ZoomIn(defaultRoom, audioController);
		});
		zoomOutButton.onClick.AddListener(delegate
		{
			levelController.ZoomOut(defaultRoom, audioController);
		});

	}


	// Update is called once per frame
	void Update()
	{

	}
}
