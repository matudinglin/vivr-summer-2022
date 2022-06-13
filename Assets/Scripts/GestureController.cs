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

        private TapGestureRecognizer tapGesture;
        private TapGestureRecognizer doubleTapGesture;
        private TapGestureRecognizer tripleTapGesture;
        private SwipeGestureRecognizer swipeGesture;
        private PanGestureRecognizer panGesture;
        private ScaleGestureRecognizer scaleGesture;
        private RotateGestureRecognizer rotateGesture;
        private LongPressGestureRecognizer longPressGesture;
        private readonly List<Vector3> swipeLines = new List<Vector3>();

        LevelController levelController;
        AvatarController avatarController;
        IntersectionController intersectionController;
        AudioController audioController;
        TTSController ttsController;
        CameraController cameraController;
        POIController poiController;
        bool zooming = false;

        Camera activeCamera
        {
            get { return cameraController.ActiveCamera(); }
        }


        private void DebugText(string text, params object[] format)
        {
            //bottomLabel.text = string.Format(text, format);
            //Debug.Log(string.Format(text, format));
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

        private void HandleSwipe(float endX, float endY)
        {
            Vector2 start = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);
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
                Vector3 end = activeCamera.ScreenToWorldPoint(new Vector3(swipeGesture.VelocityX, swipeGesture.VelocityY, activeCamera.nearClipPlane));
                Vector3 velocity = (end - origin);
                Vector2 force = velocity * 500.0f;

                foreach (RaycastHit2D h in collisions)
                {
                    h.rigidbody.AddForceAtPosition(force, h.point);
                }
            }
        }

        private void TapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                DebugText("Tapped at {0}, {1}", gesture.FocusX, gesture.FocusY);
                Debug.Log("Tapped at: " + gesture.FocusX.ToString() + ", " + gesture.FocusY.ToString());
                ViewLevel viewLevel = levelController.viewLevel;
                if (viewLevel == ViewLevel.AVATAR)
                {
                    // it should shoot a laser to that direction and return the object name
                    PlayHint();
                }
                else if (viewLevel == ViewLevel.SINGLE_ROOM)
                {
                    // this does not work
                    // FIXME
                    Debug.Log("calling speak here");
                    ttsController.Speak(intersectionController.exploringPOI.description);
                }
            }
        }

        private void CreateTapGesture()
        {
            tapGesture = new TapGestureRecognizer();
            tapGesture.StateUpdated += TapGestureCallback;
            tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
            FingersScript.Instance.AddGesture(tapGesture);
        }

        /**
 * It will shoot a laser 
 * 
 * **/
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

        private void DoubleTapGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                var viewLevel = levelController.viewLevel;
                if (viewLevel == ViewLevel.FLOOR_PLAN)
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
                // FIXME
                // if it is in the avatar view then, double tap would be the object identification
                else if (viewLevel == ViewLevel.AVATAR)
                {
                    // let avatar shoot a laser to identify an object
                    //
                    Debug.Log("Captured double tap in avatar mode");
                    shootLaser();
      
                    //if (levelController == null)
                    //{
                    //    Debug.Log("levelController is null");
                    //}
                    //else
                    //{
                    //    Debug.Log("levelController is not null");
                    //}
                    

                    //avatarController.shootLaser();
                    
                }

                // FIXME
            }
        }

        private void CreateDoubleTapGesture()
        {
            doubleTapGesture = new TapGestureRecognizer();
            doubleTapGesture.NumberOfTapsRequired = 2;
            doubleTapGesture.StateUpdated += DoubleTapGestureCallback;
            doubleTapGesture.RequireGestureRecognizerToFail = tripleTapGesture;
            FingersScript.Instance.AddGesture(doubleTapGesture);
        }

        private void SwipeGestureCallback(GestureRecognizer gesture)
        {
           
            if (gesture.State == GestureRecognizerState.Ended)
            {
                HandleSwipe(gesture.FocusX, gesture.FocusY);
                // DebugText("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}", gesture.StartFocusX, gesture.StartFocusY, gesture.FocusX, gesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY);
                var deltaX = gesture.StartFocusX - gesture.FocusX;
                var deltaY = gesture.StartFocusY - gesture.FocusY;
                DebugText("Swiped. Delta: ({0}, {1})", deltaX, deltaY);
                if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
                {
                    //// Horizontal
                    //// FIXME
                    //if (deltaY > 0)
                    //{
                    //    // Swipe down
                    //    // Move forward
                    //    cameraController.Move(1f);
                    //}
                    //else
                    //{
                    //    // Swipe up
                    //    cameraController.Move(2f);
                    //}
                    //// FIXME

                }
                else
                {
                    
                    // Vertical
                    if (deltaY > 0)
                    {
                        // Swipe down
                        // Move forward
                        cameraController.Move(-1f);
                    }
                    else
                    {
                        // Swipe up
                        cameraController.Move(1f);
                    }
                }
            }
        }

        private void CreateSwipeGesture()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(swipeGesture);
        }

        private void PanGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
              //  Debug.Log("herrrr");
                DebugText("Panned, Location: {0}, {1}, Delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
                float deltaX = panGesture.DeltaX / 25.0f;
                float deltaY = panGesture.DeltaY / 25.0f;
            }
        }

        private void CreatePanGesture()
        {
            panGesture = new PanGestureRecognizer();
            panGesture.MinimumNumberOfTouchesToTrack = 2;
            panGesture.StateUpdated += PanGestureCallback;
            FingersScript.Instance.AddGesture(panGesture);
        }

        private void ScaleGestureCallback(GestureRecognizer gesture)
        {
            DebugText("Scaled: {0}, Focus: {1}, {2}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY);
            float scale = scaleGesture.ScaleMultiplier;
            if (gesture.State == GestureRecognizerState.Executing
                && !zooming && scale != 1f)
            {
                StartCoroutine(Zoom(scale, intersectionController.focusRoom));
            }
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

        private void CreateScaleGesture()
        {
            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.StateUpdated += ScaleGestureCallback;
            FingersScript.Instance.AddGesture(scaleGesture);
        }

        private void RotateGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
            }
        }

        private void CreateRotateGesture()
        {
            rotateGesture = new RotateGestureRecognizer();
            rotateGesture.StateUpdated += RotateGestureCallback;
            FingersScript.Instance.AddGesture(rotateGesture);
        }

        private void LongPressGestureCallback(GestureRecognizer gesture)
        {

            Debug.Log("Captured long press");
            if (gesture.State == GestureRecognizerState.Began)
            {
                DebugText("Long press began: {0}, {1}", gesture.FocusX, gesture.FocusY);
                BeginDrag(gesture.FocusX, gesture.FocusY);
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                DebugText("Long press moved: {0}, {1}", gesture.FocusX, gesture.FocusY);
                DragTo(gesture.FocusX, gesture.FocusY);
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                DebugText("Long press end: {0}, {1}, delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
                EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);
            }
        }

        private void PlayHint()
        {
            //
            Debug.Log("Try to call PlayPOIHint()");
            poiController.PlayPOIHint();
        }


        private void CreateLongPressGesture()
        {
            longPressGesture = new LongPressGestureRecognizer();
            longPressGesture.MaximumNumberOfTouchesToTrack = 1;
            longPressGesture.StateUpdated += LongPressGestureCallback;
            FingersScript.Instance.AddGesture(longPressGesture);
        }

        private void PlatformSpecificViewTapUpdated(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                Debug.Log("You triple tapped the platform specific label!");
            }
        }

        private void CreatePlatformSpecificViewTripleTapGesture()
        {
            tripleTapGesture = new TapGestureRecognizer();
            tripleTapGesture.StateUpdated += PlatformSpecificViewTapUpdated;
            tripleTapGesture.NumberOfTapsRequired = 3;
            // tripleTapGesture.PlatformSpecificView = bottomLabel.gameObject;
            FingersScript.Instance.AddGesture(tripleTapGesture);
        }

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

        private void Start()
        {
            // don't reorder the creation of these :)
            CreatePlatformSpecificViewTripleTapGesture();
            CreateDoubleTapGesture();
            CreateTapGesture();
            CreateSwipeGesture();
            CreatePanGesture();
            CreateScaleGesture();
            CreateRotateGesture();
            CreateLongPressGesture();

           
            avatarController = GetComponent<AvatarController>();

            if(avatarController == null)
            {
                Debug.Log("In start: avatar is null");
            }
            audioController = GetComponent<AudioController>();
            if (audioController == null)
            {
                Debug.Log("In start: audio is null");
            }
            ttsController = GetComponent<TTSController>();
            cameraController = GetComponent<CameraController>();
            levelController = GetComponent<LevelController>();
            intersectionController = GetComponent<IntersectionController>();
            poiController = GetComponent<POIController>();

            // pan, scale and rotate can all happen simultaneously
            panGesture.AllowSimultaneousExecution(scaleGesture);
            panGesture.AllowSimultaneousExecution(rotateGesture);
            scaleGesture.AllowSimultaneousExecution(rotateGesture);

            // prevent the one special no-pass button from passing through,
            //  even though the parent scroll view allows pass through (see FingerScript.PassThroughObjects)
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



