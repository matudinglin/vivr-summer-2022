using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RenderStreaming;
using Unity.RenderStreaming.Samples;


public class StreamingCamera : MonoBehaviour
{
	// Start is called before the first frame update
	[SerializeField] RenderStreaming renderStreaming;
	[SerializeField] Camera cam;
	void Start()
	{
		if (!renderStreaming.runOnAwake)
		{
			renderStreaming.Run(
				hardwareEncoder: RenderStreamingSettings.EnableHWCodec,
				signaling: RenderStreamingSettings.Signaling);
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
