//
// Fingers Gestures
// (c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DigitalRubyShared
{

    public class GestureController : MonoBehaviour
    {
        public Camera viewCamera;
        public GameObject avatar;
        private AvatarController avatarController;
        private LevelController levelController;
        private IntersectionController intersectionController;
        private AudioController audioController;
        private TTSController ttsController;
        private CameraController cameraController;
        private POIController poiController;

        private ScaleGestureRecognizer scaleGesture;
        private TapGestureRecognizer FPDoubleTapGesture;
        private LongPressGestureRecognizer FPlongPressGesture;
        
        private TapGestureRecognizer SRTapGesture;
        private TapGestureRecognizer SRDoubleTapGesture;

        private TapGestureRecognizer ATapGesture;
        private TapGestureRecognizer ADoubleTapGesture;
        private TapGestureRecognizer ATripleTapGesture;
        private SwipeGestureRecognizer ASwipeGesture;
        private LongPressGestureRecognizer ALongPressGesture;

        private WalkingDirections walkDirection;

        private static int repeatCOUNT = 5;

        private readonly List<Vector3> swipeLines = new List<Vector3>();

        bool zooming = false;

        Camera activeCamera
        {
            get { return cameraController.ActiveCamera(); }
        }


        private void BeginDrag(float screenX, float screenY)
        {
            Vector3 pos = new Vector3(screenX, screenY, 0.0f);
            pos = activeCamera.ScreenToWorldPoint(pos);
            RaycastHit2D hit = Physics2D.CircleCast(pos, 10.0f, Vector2.zero);
        }

        private void DragTo(float screenX, float screenY)
        {
            intersectionController.Exploring(new Vector2(screenX, screenY));
        }

        private void EndDrag(float velocityXScreen, float velocityYScreen)
        {
            Vector3 origin = activeCamera.ScreenToWorldPoint(Vector3.zero);
            Vector3 end = activeCamera.ScreenToWorldPoint(new Vector3(velocityXScreen, velocityYScreen, 0.0f));
            Vector3 velocity = (end - origin);
        }
        IEnumerator Zoom(float scale, GameObject room)
        {
            zooming = true;
            if (scale > 1f)
            {
                // Zoom in
                Debug.Log("Zoom in");
                levelController.ZoomIn(room, audioController);
            }
            else
            {
                // Zoom out
                Debug.Log("Zoom out");
                levelController.ZoomOut(room, audioController);
            }
            yield return new WaitForSeconds(.8f);
            zooming = false;
        }

        private void HandleSwipe(float endX, float endY)
        {
            Vector2 start = new Vector2(ASwipeGesture.StartFocusX, ASwipeGesture.StartFocusY);
            Vector3 startWorld = activeCamera.ScreenToWorldPoint(start);
            Vector3 endWorld = activeCamera.ScreenToWorldPoint(new Vector2(endX, endY));
            float distance = Vector3.Distance(startWorld, endWorld);
            startWorld.z = endWorld.z = 0.0f;

            swipeLines.Add(startWorld);
            swipeLines.Add(endWorld);

            if (swipeLines.Count > 4)
            {
                swipeLines.RemoveRange(0, swipeLines.Count - 4);
            }

            RaycastHit2D[] collisions = Physics2D.CircleCastAll(startWorld, 10.0f, (endWorld - startWorld).normalized, distance);

            if (collisions.Length != 0)
            {
                Debug.Log("Raycast hits: " + collisions.Length + ", start: " + startWorld + ", end: " + endWorld + ", distance: " + distance);

                Vector3 origin = activeCamera.ScreenToWorldPoint(Vector3.zero);
                Vector3 end = activeCamera.ScreenToWorldPoint(new Vector3(ASwipeGesture.VelocityX, ASwipeGesture.VelocityY, activeCamera.nearClipPlane));
                Vector3 velocity = (end - origin);
                Vector2 force = velocity * 500.0f;

                foreach (RaycastHit2D h in collisions)
                {
                    h.rigidbody.AddForceAtPosition(force, h.point);
                }
            }
        }
        private void shootLaser()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("I am trying to shoot a laser!");
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo)) // if the ray is hitting anything
                {
                    var rig = hitInfo.collider.GetComponent<Rigidbody>();
                    if (rig != null)
                    {
                        Debug.Log("HIT: " + rig.name);

                    }
                }
            }
        }

        // ====================================================================================================================
        // Change Level
        // ====================================================================================================================
        
        private void ScaleGestureCallback(GestureRecognizer gesture)
        {
            float scale = scaleGesture.ScaleMultiplier;
            if (gesture.State == GestureRecognizerState.Executing
                && !zooming && scale != 1f)
            {
                StartCoroutine(Zoom(scale, intersectionController.focusRoom));
                SwitchLevelGesture(levelController.viewLevel);
            }
        }

        private void ScaleGestureCreate()
        {
            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.StateUpdated += ScaleGestureCallback;
            FingersScript.Instance.AddGesture(scaleGesture);
        }

        // ====================================================================================================================
        // Floor Plan Gestures
        // ====================================================================================================================
        private void FPDoubleTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                var focusRoom = intersectionController.focusRoom;
                if (focusRoom != null)
                {
                    var description = focusRoom.GetComponent<Semantic>().description;
                    if (description != null)
                    {
                        Debug.Log("description: " + description);
                        ttsController.Speak(description);
                    }
                }
            }
        }

        private void FPDoubleTapGestureCreate()
        {
            FPDoubleTapGesture = new TapGestureRecognizer();
            FPDoubleTapGesture.NumberOfTapsRequired = 2;
            FPDoubleTapGesture.StateUpdated += FPDoubleTapGestureCallback;
            FPDoubleTapGesture.RequireGestureRecognizerToFail = ATripleTapGesture;
            FingersScript.Instance.AddGesture(FPDoubleTapGesture);
        }

        private void FPLongPressGestureCallback(GestureRecognizer gesture)
        {

            Debug.Log("Captured long press");
            if (gesture.State == GestureRecognizerState.Began)
            {
                BeginDrag(gesture.FocusX, gesture.FocusY);
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                DragTo(gesture.FocusX, gesture.FocusY);
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                EndDrag(FPlongPressGesture.VelocityX, FPlongPressGesture.VelocityY);
            }
        }

        private void FPLongPressGestureCreate()
        {
            FPlongPressGesture = new LongPressGestureRecognizer();
            FPlongPressGesture.MaximumNumberOfTouchesToTrack = 1;
            FPlongPressGesture.StateUpdated += FPLongPressGestureCallback;
            FingersScript.Instance.AddGesture(FPlongPressGesture);
        }

        // ====================================================================================================================
        // SINGLE ROOM Gestures
        // ====================================================================================================================
        private void SRDoubleTapGestureCallback(GestureRecognizer gesture)
        {
            
            if (gesture.State == GestureRecognizerState.Ended)
            {
                //Debug.LogWarning("SRDouble called");
                Debug.LogWarning("Tapped at: " + gesture.FocusX.ToString() + ", " + gesture.FocusY.ToString());
                Vector3 screenPosition = new Vector3(gesture.FocusX, gesture.FocusY, 0);
                //Vector3 cameraPosition = viewCamera.ScreenToWorldPoint(screenPosition);
                avatarController.MoveInRoomMode(screenPosition);
            }
        }
        private void SRTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                Debug.Log("Tapped at: " + gesture.FocusX.ToString() + ", " + gesture.FocusY.ToString());
                // this does not work
                // FIXME
                Debug.Log("calling speak here");
                intersectionController.Exploring(new Vector2 (gesture.FocusX, gesture.FocusY));
                //ttsController.Speak(intersectionController.exploringPOI.description);
            }
        }

        private void SRTapGestureCreate()
        {
            SRTapGesture = new TapGestureRecognizer();
            SRTapGesture.StateUpdated += SRTapGestureCallback;
            SRTapGesture.RequireGestureRecognizerToFail = SRDoubleTapGesture;
            FingersScript.Instance.AddGesture(SRTapGesture);
        }
        private void SRDoubleTapGestureCreate()
        {
            SRDoubleTapGesture = new TapGestureRecognizer();
            SRDoubleTapGesture.NumberOfTapsRequired = 2;
            SRDoubleTapGesture.StateUpdated += SRDoubleTapGestureCallback;
            SRDoubleTapGesture.RequireGestureRecognizerToFail = ATripleTapGesture;
            FingersScript.Instance.AddGesture(SRDoubleTapGesture);
        } 

        // ====================================================================================================================
        // AVATAR Gestures
        // ====================================================================================================================
        private void ASwipeGestureCallback(GestureRecognizer gesture)
        {
            Debug.Log("Captured swipe");
            if (gesture.State == GestureRecognizerState.Ended)
            {
                var deltaX = gesture.StartFocusX - gesture.FocusX;
                var deltaY = gesture.StartFocusY - gesture.FocusY;
                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
                {
                    // Horizontal
                    if (deltaX > 0)
                    {
                        walkDirection = WalkingDirections.left;
                        //ttsController.Speak("Take one step left!");

                    }
                    else
                    {
                        walkDirection = WalkingDirections.right;
                        //ttsController.Speak("Take one step right!");
                    }
                }
                else
                {

                    // Vertical
                    if (deltaY > 0)
                    {
                        walkDirection = WalkingDirections.backward;
                        //ttsController.Speak("Take one step backward!");
                    }
                    else
                    {
                        walkDirection = WalkingDirections.forward;
                        //ttsController.Speak("Take one step forward!");
                    }
                }
                for(int i = 0; i < 40; i++)
                {
                    avatarController.MoveAvatar(walkDirection);
                }
                walkDirection = WalkingDirections.hold;
            }
        }

        private void ASwipeGestureCreate()
        {
            ASwipeGesture = new SwipeGestureRecognizer();
            ASwipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            ASwipeGesture.StateUpdated += ASwipeGestureCallback;
            ASwipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(ASwipeGesture);
        }


        private void ALongPressGestureCallback(GestureRecognizer gesture)
        {
            Debug.Log("Captured long press");
            if(gesture.State == GestureRecognizerState.Began)
            {
                ttsController.Speak("Start Walking.");
                walkDirection = WalkingDirections.hold;
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                var deltaX = gesture.StartFocusX - gesture.FocusX;
                var deltaY = gesture.StartFocusY - gesture.FocusY;
                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
                {
                    // Horizontal
                    if (deltaX > 0)
                    {
                        walkDirection = WalkingDirections.left;
                        //ttsController.Speak("Walking left!");

                    }
                    else
                    {
                        walkDirection = WalkingDirections.right;
                        //ttsController.Speak("Walking right!");
                    }
                }
                else
                {

                    // Vertical
                    if (deltaY > 0)
                    {
                        walkDirection = WalkingDirections.backward;
                        //ttsController.Speak("Walking backward!");
                    }
                    else
                    {
                        walkDirection = WalkingDirections.forward;
                        //ttsController.Speak("Walking forward!");
                    }
                }
                avatarController.MoveAvatar(walkDirection);
            }
            walkDirection = WalkingDirections.hold;
        }

        private void ALongPressGestureCreate()
        {
            ALongPressGesture = new LongPressGestureRecognizer();
            ALongPressGesture.MaximumNumberOfTouchesToTrack = 1;
            ALongPressGesture.StateUpdated += ALongPressGestureCallback;
            FingersScript.Instance.AddGesture(ALongPressGesture);
        }

        private void ATripleTapGestureCallBack(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                Debug.Log("Try to call PlayPOIHint()");
                poiController.PlayPOIHint();
            }
        }

        private void ATripleTapGestureCreate()
        {
            ATripleTapGesture = new TapGestureRecognizer();
            ATripleTapGesture.StateUpdated += ATripleTapGestureCallBack;
            ATripleTapGesture.NumberOfTapsRequired = 3;
            // ATripleTapGesture.PlatformSpecificView = bottomLabel.gameObject;
            FingersScript.Instance.AddGesture(ATripleTapGesture);
        }

        private void ADoubleTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                POI latestObj = intersectionController.latestPOI;

                ttsController.Speak("Latest Seen Object is " + latestObj.name);
                // latestObj.PlayAuditoryIcon();
            }
        }

        private void ADoubleTapGestureCreate()
        {
            ADoubleTapGesture = new TapGestureRecognizer();
            ADoubleTapGesture.NumberOfTapsRequired = 2;
            ADoubleTapGesture.StateUpdated += ADoubleTapGestureCallback;
            ADoubleTapGesture.RequireGestureRecognizerToFail = ATripleTapGesture;
            FingersScript.Instance.AddGesture(ADoubleTapGesture);
        }

        private void ATapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                
            }
        }
        private void ATapGestureCreate()
        {
            ATapGesture = new TapGestureRecognizer();
            ATapGesture.StateUpdated += ATapGestureCallback;
            ATapGesture.RequireGestureRecognizerToFail = FPDoubleTapGesture;
            FingersScript.Instance.AddGesture(ATapGesture);
        }




        // ====================================================================================================================
        // Gesture System Control
        // ====================================================================================================================


        private static bool? CaptureGestureHandler(GameObject obj)
        {
            // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
            if (obj.name.StartsWith("PassThrough"))
            {
                // allow the pass through for any element named "PassThrough*"
                return false;
            }
            else if (obj.name.StartsWith("NoPass"))
            {
                // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
                // the triple tap gesture can tap on it
                return true;
            }

            // fall-back to default behavior for anything else
            return null;
        }   

        private void SwitchLevelGesture(ViewLevel viewLevel)
        {
            FingersScript.Instance.ResetState(true);
            switch (viewLevel)
            {
                case ViewLevel.FLOOR_PLAN:
                    CreateFloorPlanGesture();
                    break;
                case ViewLevel.SINGLE_ROOM:
                    CreateSingleRoomGesture();
                    break;
                case ViewLevel.AVATAR:
                    CreateAvatarGesture();
                    break;
            }
        }


        private void CreateFloorPlanGesture()
        {
            ScaleGestureCreate();
            FPDoubleTapGestureCreate();
            FPLongPressGestureCreate();
        }

        private void CreateSingleRoomGesture()
        {
            ScaleGestureCreate();
            SRDoubleTapGestureCreate();
            SRTapGestureCreate();
        }

        private void CreateAvatarGesture()
        {
            ScaleGestureCreate();
            ATripleTapGestureCreate();
            ADoubleTapGestureCreate();
            ATapGestureCreate();
            ASwipeGestureCreate();
            ALongPressGestureCreate();
        }

        private void Start()
        {
            // Get Components
            ttsController = GetComponent<TTSController>();
            cameraController = GetComponent<CameraController>();
            levelController = GetComponent<LevelController>();
            intersectionController = GetComponent<IntersectionController>();
            poiController = GetComponent<POIController>();
            avatarController = avatar.GetComponent<AvatarController>();

            if (avatarController == null)
            {
                Debug.Log("In start: avatar is null");
            }
            audioController = GetComponent<AudioController>();
            if (audioController == null)
            {
                Debug.Log("In start: audio is null");
            }

            // Create Gesture
            SwitchLevelGesture(levelController.viewLevel);

            //// don't reorder the creation of these :)
            //CreatePlatformSpecificViewTripleTapGesture();
            //CreateDoubleTapGesture();
            //CreateTapGesture();
            //CreateSwipeGesture();
            //CreatePanGesture();
            //CreateScaleGesture();
            //CreateRotateGesture();
            //CreateLongPressGesture();

            // pan, scale and rotate can all happen simultaneously
            //panGesture.AllowSimultaneousExecution(scaleGesture);
            //panGesture.AllowSimultaneousExecution(rotateGesture);
            //scaleGesture.AllowSimultaneousExecution(rotateGesture);
            // prevent the one special no-pass button from passing through,
            // even though the parent scroll view allows pass through (see FingerScript.PassThroughObjects)

            walkDirection = WalkingDirections.forward;
            FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
        }

        private void Update()
        {

        }

        private void LateUpdate()
        {
            int touchCount = Input.touchCount;
            if (FingersScript.Instance.TreatMousePointerAsFinger && Input.mousePresent)
            {
                touchCount += (Input.GetMouseButton(0) ? 1 : 0);
                touchCount += (Input.GetMouseButton(1) ? 1 : 0);
                touchCount += (Input.GetMouseButton(2) ? 1 : 0);
            }
            string touchIds = string.Empty;
            int gestureTouchCount = 0;
            foreach (GestureRecognizer g in FingersScript.Instance.Gestures)
            {
                gestureTouchCount += g.CurrentTrackedTouches.Count;
            }
            foreach (GestureTouch t in FingersScript.Instance.CurrentTouches)
            {
                touchIds += ":" + t.Id + ":";
            }
        }
    }
}


//Fingers Gestures
//(c) 2015 Digital Ruby, LLC
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.



