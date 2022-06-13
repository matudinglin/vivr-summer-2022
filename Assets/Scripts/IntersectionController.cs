using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRubyShared;

public class IntersectionController : MonoBehaviour
{
	// Start is called before the first frame update
	public GameObject[] rooms;
	public GameObject focusRoom;
	public GameObject avatar;
	AudioController backgroundController;
	LevelController levelController;
	TTSController ttsController;
	HapticController hapticController;
	CameraController cameraController;
	GameObject selectObj;
	public POI exploringPOI;
	bool isExploring = false;
	bool die = true;
	float distance;
	Vector3 previouseD;


	Vector3 exploringPoint;
	AvatarController avatarController
	{
		get
		{
			if (avatar == null) { return null; }
			else { return avatar.GetComponent<AvatarController>(); }
		}
	}

	bool poiVibrating = false;
	bool touching = false;
	void Start()
	{
		backgroundController = GetComponent<AudioController>();
		ttsController = GetComponent<TTSController>();
		hapticController = GetComponent<HapticController>();
		levelController = GetComponent<LevelController>();
		cameraController = GetComponent<CameraController>();
		if(levelController.IsImmersive())
		{
			focusRoom = levelController.currentRoom;
		}
	}
	// Update is called once per frame
	void FixedUpdate()
	{
		if (levelController.IsImmersive())
		{
			// Ray cast detection in immersive mode.
			Camera cam = cameraController.ActiveCamera(); //might be here (move to update)
														  //Vector3 o = cam.transform.position;
														  //Vector3 d = cam.transform.forward;
			Vector3 o = avatarController.fpOrigin;
			Vector3 d = avatarController.fpForword;
			

			Debug.Log("This is forward: " + d.ToString());

			// shoot a list of arrays, and report the names of objects from bottom to top or from top to bottom

			Ray ray = new Ray(o, d);
			// Debug.Log("The ray: " + ray.ToString());

			if (Physics.Raycast(ray, out RaycastHit raycastHit, 10.0f))
			{
				Debug.DrawRay(o, d * 10.0f, Color.yellow);
				// TODO lines below should be deleted
				selectObj = raycastHit.collider.gameObject;
				//if (!allobjs.Exists(x => x.name == selectObj.name))
				//{
				//	allobjs.Add(selectObj);
				//}
				//else {
				//	Debug.Log(" here is repeated : " + selectObj.name);
				//}

				//POI poi = selectObj.GetComponent<POI>();
				//            if (poi)
				//            {
				//	Debug.Log("Found an object: " + poi.poiName);
				//}

				// TODO lines above should be deleted


				// FIXME, this should not be commented
				//public float x = Vector3.Distance(o, selectObj.transform.position);
                ReadPOIName(selectObj, (Mathf.Round(Vector3.Distance(o, selectObj.transform.position) * 10f) / 10f).ToString());

			}

			//List<GameObject> allobjs = new List<GameObject>();

			
			//for (int i = 0; i < 3; i = i + 1) {
			//	Vector3 delta = new Vector3(0f, i * 0.1f, 0f);

			//	Vector3 index = new Vector3(0f, 1f, 0f);
   //             for (int j = 0; j<2; j++)
   //             {
			//		Ray ray = new Ray(o, d + Vector3.Scale(index, delta));

			//		if (Physics.Raycast(ray, out RaycastHit raycastHit, 10.0f))
			//		{
			//			Debug.DrawRay(o, (d + Vector3.Scale(index, delta)) * 10.0f, Color.yellow);
			//			// TODO lines below should be deleted
			//			selectObj = raycastHit.collider.gameObject;
			//			if (!allobjs.Exists(x => x.name == selectObj.name))
			//			{
			//				allobjs.Add(selectObj);
			//			}
			//			//else {
			//			//	Debug.Log(" here is repeated : " + selectObj.name);
			//			//}

			//			//POI poi = selectObj.GetComponent<POI>();
			//			//            if (poi)
			//			//            {
			//			//	Debug.Log("Found an object: " + poi.poiName);
			//			//}

			//			// TODO lines above should be deleted


			//			// FIXME, this should not be commented
			//			// ReadPOIName(selectObj);

			//		}
			//		index = new Vector3(0f, -1f, 0f);
			//	}

			//}

			//if (previouseD != d) {
			//	die = true;
			//	previouseD = d;
			//}
			////
			//if (die) {
			//	Debug.Log("start the loop of objects");
			//	foreach (GameObject ob in allobjs)
			//	{
			//		ReadPOIName(ob);
			//		Debug.Log(" here is the objected e wanted : " + ob.name);
			//	}
			//	Debug.Log("end of the loop");
			//	die = false;
			//}
			

		}
	}

// this is where the object names got called 
	void ReadPOIName(GameObject obj, string distance)
	{
		POI poi = obj.GetComponent<POI>();
		if(poi)
		{
			Debug.Log("Mariupol");
			ttsController.Speak(poi.poiName + distance + " meters" ); //async
		}
	}

	public void Exploring(Vector2 screenPos)
	{
		Camera cam = cameraController.viewCamera;
		Vector3 screenPos3 = new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane);
		Vector3 worldPos = cam.ScreenToWorldPoint(screenPos3);
		Ray ray = cam.ScreenPointToRay(screenPos3);
		// commented for testing
		//Debug.Log("Mouse world pos: " + worldPos + "Ray: " + ray);
		RaycastHit roomHit;

		if (Physics.Raycast(ray, out roomHit, 100f))
		{
			// Room
			GameObject hitRoom = roomHit.collider.gameObject;
			// commented for testing
			//Debug.Log("Hit room:" + hitRoom.name);
			if (hitRoom != focusRoom)
			{
				backgroundController.PauseBackground(focusRoom);
			}
			backgroundController.PlayBackground(hitRoom);
			focusRoom = hitRoom;
			ray.origin = roomHit.point;
			// MoveAvatar(roomHit.point);
			RaycastHit poiHit;
			if (Physics.Raycast(ray, out poiHit, 100f))
			{
				GameObject hitPOI = poiHit.collider.gameObject;
				if(hitPOI.CompareTag("Wall"))
				{
					// commented for testing
					//Debug.Log("Hit wall:" + hitPOI.name);
					hapticController.PlayWallFeedback();
				}
				POI poi = hitPOI.GetComponent<POI>();
				if(poi)
				{
					hapticController.PlayPOIFeedback();
					ttsController.Speak(poi.poiName);
					exploringPOI = poi;
				}
			}
		}
		else
		{
			backgroundController.PauseBackground(focusRoom);
			focusRoom = null;
		}
	}

	void MoveAvatar(Vector3 pos)
	{
		// float origY = avatar.transform.position.y;
		// avatar.transform.position = new Vector3(pos.x, origY, pos.z);
		exploringPoint = pos;
	}
}
