using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ViewLevel
{
    FLOOR_PLAN, SINGLE_ROOM, AVATAR
}

public class LevelController : MonoBehaviour
{

    // Start is called before the first frame update
    //??? Here!!!!   public ViewLevel viewLevel;??
    public GameObject avatar;
    public ViewLevel viewLevel = ViewLevel.SINGLE_ROOM;
    public GameObject defaultRoom;
    CameraController cameraController;
    public GameObject currentRoom;
    GameObject[] rooms;
    GameObject[] allObjects; // It means the stuffs in the rooms, like table and chair, here.
    TTSController tTSController;
    void Start()
    {
        cameraController = GetComponent<CameraController>();
        tTSController = GetComponent<TTSController>();
        rooms = GameObject.FindGameObjectsWithTag("Room");
        allObjects = GameObject.FindGameObjectsWithTag("Object");
        if (viewLevel == ViewLevel.AVATAR)
        {
            AvatarView(defaultRoom, null);
        }
        else if (viewLevel == ViewLevel.SINGLE_ROOM)
        {
            SingleRoomView(defaultRoom, null);
        }
        else if (viewLevel == ViewLevel.FLOOR_PLAN)
        {
            FloorPlanView(null);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsImmersive()
    {
        return viewLevel == ViewLevel.AVATAR;
    }

    void ReadSwitchDescription(string modeName)
    {
        // FIXME
        Debug.Log("Modename: " + modeName);
        tTSController.SpeakInterruptively(modeName);
    }

    void FloorPlanView(AudioController audioController)
    {
        Debug.Log("FLoor plan view");
        if (audioController && viewLevel != ViewLevel.FLOOR_PLAN)
        {
            audioController.SwitchViewHint();
            ReadSwitchDescription("overview mode");
        }
        viewLevel = ViewLevel.FLOOR_PLAN;
        currentRoom = null;
        cameraController.Switch2FloorPlan();
        ShowAllRooms();
        HideObjects();
    }

    void SingleRoomView(GameObject room, AudioController audioController)
    {
        // Area of interets
        if (cameraController.Switch2View(room))
        {
            // commented for testing
            Debug.Log("Single room view");

            currentRoom = room;
            if (audioController) { audioController.SwitchViewHint(); }
            ReadSwitchDescription("area of interest mode");
            viewLevel = ViewLevel.SINGLE_ROOM;
            ShowObjects();
            HideOtherRooms(room);
            avatar.transform.position = room.transform.position + new Vector3(0,0.5f,0);
            avatar.GetComponent<AvatarController>().targetPosition = room.transform.position + new Vector3(0, 0.5f, 0);
        }
    }

    void AvatarView(GameObject room, AudioController audioController)
    {
        if (cameraController.Switch2Avatar(room))
        {
            currentRoom = room;
            if (audioController)
            {
                audioController.SwitchViewHint();
                audioController.PauseBackground(room);
            }
            
            ReadSwitchDescription("Immersive mode This is the " + room.name);
            viewLevel = ViewLevel.AVATAR;
            ShowAllRooms();
            ShowObjects();
            // HideOtherRooms(room);
        }
    }

    public void ZoomIn(GameObject room, AudioController audioController)
    {
        if (viewLevel == ViewLevel.FLOOR_PLAN)
        {
            SingleRoomView(room, audioController);
        }
        else if (viewLevel == ViewLevel.SINGLE_ROOM)
        {
            AvatarView(room, audioController);
        }
    }

    public void ZoomOut(GameObject room, AudioController audioController)
    {
        if (viewLevel == ViewLevel.AVATAR)
        {
            SingleRoomView(room, audioController);
        }
        else
        {
            FloorPlanView(audioController);
        }
    }

    void HideObjects()
    {// commented for testing
     //Debug.Log("Hide all objects");
        foreach (var obj in allObjects)
        {
            obj.SetActive(false);
        }
    }

    void ShowObjects()
    {// commented for testing
     //Debug.Log("Show all objects");
        foreach (var obj in allObjects)
        {
            obj.SetActive(true);
        }
    }

    void HideOtherRooms(GameObject currentRoom)
    {// commented for testing
        foreach (var room in rooms)
        {
            if (room != currentRoom)
            {
                room.SetActive(false);
            }
        }
    }

    void ShowAllRooms()
    {// commented for testing
     //Debug.Log("Show all rooms");
        foreach (var room in rooms)
        {
            room.SetActive(true);
        }
    }
}

