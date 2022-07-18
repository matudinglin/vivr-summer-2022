using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Start is called before the first frame update
    public enum RoomViewMode
    {
        cameraStatic,
        cameraFollow
    };

    public Camera viewCamera;
    public Camera followCamera;
    public Camera mapCamera;
    public RoomViewMode mode;
    public Vector3 floorPlanViewPoint;
    public GameObject defaultRoom;
    public bool hideOut = false;
    GameObject avatarRoom = null;
    public GameObject avatar;
    //public GameObject systemcontrollers;

    GameObject currentRoom = null;
    

    public float floorplanViewSize;
    AvatarController avatarController
    {
        get
        {
            if (avatar == null) { return null; }
            else { return avatar.GetComponent<AvatarController>(); }
        }
    }
    Camera avatarCamera
    {
        get
        {
            if (avatarController == null) { return null; }
            else { return avatarController.fpCamera; }
        }
    }
    LevelController levelController;
    HapticController hapticController;
    private Quaternion baseRotation = Quaternion.Euler(90, 0, 0);

    float singleRoomViewSize = 6f;
    float smallSingleRoomViewSize = 12f;
    void Start()
    {
        avatarCamera.enabled = false;
        followCamera.enabled = false;

        levelController = GetComponent<LevelController>();
        hapticController = GetComponent<HapticController>();
        if (hideOut)
        {
            viewCamera.cullingMask = 0;
            avatarCamera.cullingMask = 0;
        }
    }

    private static Quaternion Convert2LeftHandCoordinate(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
    void Update()
    {
        if (currentRoom != null && levelController.viewLevel == ViewLevel.SINGLE_ROOM)
        {
            //var roomCenter = currentRoom.transform.position;
            //float prevY = viewCamera.transform.position.y;
            //viewCamera.transform.position = new Vector3(roomCenter.x, prevY, roomCenter.z);
        }
        else if(levelController.viewLevel == ViewLevel.FLOOR_PLAN)
        {
            viewCamera.transform.position = floorPlanViewPoint;
        }
        // FIXME
        //if(avatarController != null)
        //{
        //    avatarCamera = avatarController.fpCamera;
        //}
        
    }

    // here, 1f is the default value
    //public void Move(float direction = 1f)
    //{
    //    if (IsImmersiveMode())
    //    {
    //        // this should be fpcamera
    //        //Vector3 forward = avatarCamera.transform.forward;
    //        //Vector3 forward = avatarController.fpCamera.transform.forward;
    //        Vector3 forward = avatarController.fpForword;
    //        //Debug.Log(forward.ToString());

    //        Debug.Log("forward x: " + forward.x);
    //        Debug.Log("forward z: " + forward.z);
    //        Vector3 delta = new Vector3(forward.x, 0, forward.z).normalized;
    //        //if (direction == -1f) {
    //        //    delta = new Vector3(forward.x, 0, forward.z).normalized;
    //        //}
    //       // Vector3 delta = new Vector3(forward.x, 0, forward.z).normalized;
    //        // avatar.transform.Translate(delta * sensitivity);
    //        //Debug.Log("delta: " + delta.ToString());
    //        //Debug.Log("delta * direction: " + (delta * direction).ToString());

    //        // call the move method, if it is not colliding, then it will move
    //        if (!avatarController.Move(delta * direction))
    //        {
    //            hapticController.PlayWallFeedback(true);
    //        }
    //    }
    //}

    bool VisibleToAvatar(GameObject gameObject)
    {
        // not used in current version but might be useful if you'd like to check the visability
        Vector3 viewPos = avatarCamera.WorldToViewportPoint(gameObject.transform.position);
        return 0f <= viewPos.x && viewPos.x <= 1f && 0f <= viewPos.y && viewPos.y <= 1f && viewPos.z > 0f;
    }

    bool LocatesInCircle(GameObject gameObject)
    {
        // not used in current version but might be useful if you'd like to check if something is around the avatar
        var dx = gameObject.transform.position.x - avatar.transform.position.x;
        var dz = gameObject.transform.position.z - avatar.transform.position.z;
        var disThreshold = 16f;
        return dx * dx + dz * dz < disThreshold;
    }

    bool IsPOI(GameObject gameObject)
    {
        var layerMask = LayerMask.GetMask("POI");
        return layerMask == (layerMask | 1 << gameObject.layer);
    }

    bool IsImmersiveMode()
    {
        return levelController.IsImmersive();
    }

    public Camera ActiveCamera()
    {
        if (IsImmersiveMode()) { return avatarCamera; }
        else { return viewCamera; }
    }

    public bool Switch2Avatar(GameObject room)
    {
        avatarRoom = room;
        if (room != null)
        {
            var origY = avatar.transform.position.y;
            var roomPos = room.transform.position;
            //avatar.transform.position = new Vector3(roomPos.x, origY, roomPos.z);// put avatar into the room
            viewCamera.enabled = false;
            viewCamera = avatarCamera;
            viewCamera.enabled = true;
            return true;
        }
        else
        {
            return false;
        }
    }

    //!! Here
    public bool Switch2View(GameObject room)
    {
        // commented for testing
        Debug.Log("Switch to room view: " + room.name);

        // "area of interest mode"
        if (room != null)
        {
            var roomCenter = room.transform.position;
            //Debug.Log("Room center:" + roomCenter);

            float prevY = viewCamera.transform.position.y;
            viewCamera.enabled = false;
            if (mode == RoomViewMode.cameraStatic)
            {
                viewCamera = mapCamera;
                viewCamera.gameObject.transform.position = new Vector3(roomCenter.x, prevY, roomCenter.z);
                //avatar.transform.position = new Vector3(roomCenter.x, avatar.transform.position.y, roomCenter.z);
                viewCamera.orthographicSize = singleRoomViewSize;


            }
            else if(mode == RoomViewMode.cameraFollow)
            {
                viewCamera = followCamera;
            }
            viewCamera.enabled = true;
            avatarController.roomCamera = viewCamera;
            currentRoom = room;


            return true;
        }
        else
        {
            return false;
        }
    }

    public void Switch2FloorPlan()
    {
        currentRoom = null;
        viewCamera.enabled = false;
        viewCamera = mapCamera;
        viewCamera.enabled = true;
        viewCamera.transform.position = floorPlanViewPoint;
        viewCamera.orthographicSize = floorplanViewSize;
    }
}
