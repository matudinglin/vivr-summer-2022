using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum WalkingDirections
{
    left, right, forward, backward
}

public class AvatarController : MonoBehaviour
{
    public Camera fpCamera;  

    public float moveSpeed;
    public bool collide = false;
    public bool willCollide = false;
    public Vector3 fpForword;
    public Vector3 fpOrigin;

    public float wallDistance = 1.0f;
    private Vector3 previousDelta;
    HapticController hapticController;
    public Material hitMaterial;

    public AudioSource footStepAudioSource = default;
    public AudioClip[] footsteClips = default;
    private static float footStepTIME = 0.3f;
    private float footStepTimer = footStepTIME;

    void Start()
    {
        //the starting position is unknown
        hapticController = GetComponent<HapticController>();
        moveSpeed = 10.0f;
    }

    //public bool Move(Vector3 delta)
    //{
    //    // the condition will be updated in FixedUpdate
    //    if (!willCollide)
    //    {
    //        //Debug.Log("It is moving!!!");
    //        //Debug.Log("transform.position before: " + transform.position.ToString());
    //        Debug.Log("delta: " + delta.ToString());
    //        transform.position = Vector3.SmoothDamp(transform.position, transform.position + delta * moveSpeed, ref velocity, 0.1f);
    //        //Debug.Log("transform.position after: " + transform.position.ToString());
    //        return true;
    //    }
    //    else
    //    {
    //        // when it is collide
    //        // if it is going to the other direction, then it is able to leave
    //        // we would capture the previous Delta,
    //        // check the sign of previous x and previous z
    //        // if this time, the previous 

    //        return false; // true or false will tigger the audio feedback
    //    }

    //}

    public void MoveAvatar(WalkingDirections walkDirection)
    {
        Vector3 horizontal = new Vector3(fpForword.z, 0, fpForword.x).normalized;
        Vector3 vertical = new Vector3(fpForword.x, 0, fpForword.z).normalized;
        footStepTimer -= Time.deltaTime;
        if(footStepTimer < 0 )
        {
            footStepAudioSource.PlayOneShot(footsteClips[Random.Range(0, footsteClips.Length - 1)]);
            switch (walkDirection)
            {
                case WalkingDirections.backward:
                    transform.position = Vector3.Lerp(transform.position, transform.position + -vertical * moveSpeed, footStepTIME);
                    break;
                case WalkingDirections.forward:
                    transform.position = Vector3.Lerp(transform.position, transform.position + vertical * moveSpeed, footStepTIME);
                    break;
                case WalkingDirections.left:
                    transform.position = Vector3.Lerp(transform.position, transform.position + horizontal * moveSpeed, footStepTIME);
                    break;
                case WalkingDirections.right:
                    transform.position = Vector3.Lerp(transform.position, transform.position + -horizontal * moveSpeed, footStepTIME);
                    break;
                default:
                    break;
            }
            footStepTimer = footStepTIME;
        }
    }



    Vector3 getFpcameraForward()
    {
        return this.fpForword;
    }

    // Update is called once per frame
    void Update()
    {
        //changed x to 1.8f to see what will happen
        //height of the fpcamera
        fpCamera.transform.position = transform.position + new Vector3(0f, 1.8f, 0f); // transform is 0,0,0 but the fpcamera has to be higher like the eye of avatar
        //Debug.Log("fpCamerca position: " + fpCamera.transform.position);
        Vector3 origRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(origRotation.x, fpCamera.transform.rotation.eulerAngles.y - 90, origRotation.z);
        fpForword = fpCamera.transform.forward;
        fpOrigin = fpCamera.transform.position;
        //Debug.Log("fpCamera.transform.forward: " + fpCamera.transform.forward);
        Helper();
    }

    void Helper()
    {
        Vector3 o = fpCamera.transform.position;
        Vector3 d = fpCamera.transform.forward;
        
        //Debug.Log("fpCamerca position2-----: " + fpCamera.transform.position);
        Debug.DrawRay(o, d * wallDistance, Color.blue);
        Debug.DrawRay(o, d * wallDistance * -1.0f, Color.red);
        Ray ray = new Ray(o, d);
        Ray ray1 = new Ray(o, d * -1.0f);
        
        if (Physics.Raycast(ray, out RaycastHit raycastHit, wallDistance))
        {
            // Debug.Log("Fixed update____----here");
            var hitObj = raycastHit.collider.gameObject;
            Debug.Log("Collide the wall!");
            if (hitObj.tag == "Wall")
            {
                willCollide = true;
            }
            else
            {
                willCollide = false;
            }
        }
        else
        {
            willCollide = false;
        }

        // detect the back
        //if (Physics.Raycast(ray1, out RaycastHit raycastHit1, wallDistance))
        //{
        //    // Debug.Log("Fixed update____----here");
        //    var hitObj = raycastHit1.collider.gameObject;
        //    Debug.Log("Collide the back wall!");
        //    if (hitObj.tag == "Wall")
        //    {
        //        willCollide = true;
        //    }
        //    else
        //    {
        //        willCollide = false;
        //    }
        //}
        //else
        //{
        //    willCollide = false;
        //}
        //Debug.Log("willCollide: " + collide);
        // FIXME
    }


    /**
     * It will shoot a laser 
     * 
     * **/
    public void shootLaser() {
        //
        Debug.Log("I am in avatar and I am trying to shoot a laser!");
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

    private void OnCollisionEnter(Collision other)
    {

        Debug.Log("Checking if Enter");
        if (other.collider.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Enter");
            collide = true;
        }
    }

    //private void OnCollisionStay(Collision other)
    //{
    //    Debug.Log("Checking if Stay");
    //    if (other.gameObject.tag == "Wall")
    //    {
    //        Debug.Log("Stay");
    //        collide = true;
    //    }
    //}

    private void OnCollisionExit(Collision other)
    {
        Debug.Log("Checking if Exit");

        if (other.collider.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Exit");
            collide = false;

        }
    }

    void FixedUpdate()
    {
        
//        Vector3 o = fpCamera.transform.position;
//        Vector3 d = fpCamera.transform.forward;
//        Debug.Log("fpCamerca position2-----: " + fpCamera.transform.position);
//        Debug.DrawRay(o, d * wallDistance, Color.blue);
//        Ray ray = new Ray(o, d);
//        if (Physics.Raycast(ray, out RaycastHit raycastHit, wallDistance))
//        {
//// Debug.Log("Fixed update____----here");
//            var hitObj = raycastHit.collider.gameObject;
//            Debug.Log("Collide the wall!");
//            if (hitObj.tag == "Wall")
//            {
//                willCollide = true;
//            }
//            else
//            {
//                willCollide = false;
//            }
//        }
//        else
//        {
//            willCollide = false;
//        }
//        //Debug.Log("willCollide: " + collide);
//        // FIXME
    }
}
