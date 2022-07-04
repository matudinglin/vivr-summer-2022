using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchTeleport : MonoBehaviour
{
    public Camera cam;
    public float height;
    public HapticController hapticController;
    public LevelController levelController;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if (levelController.viewLevel == ViewLevel.SINGLE_ROOM)
        {
            oneFinger();
        }else if (levelController.viewLevel == ViewLevel.FLOOR_PLAN)
        {
            //oneFinger();
        }
    }
    private void oneFinger()
    {
        
        if (Input.touchCount == 1)
        {
            cam = cam.gameObject.GetComponent<Camera>();
            Vector3 midPoint = Input.GetTouch(0).position;
            //Debug.LogWarning("Screen Position: " + midPoint);
            //var ray = cam.ScreenPointToRay(midPoint);
            var ray = cam.ScreenPointToRay(midPoint);
            Debug.DrawLine(ray.origin, ray.origin - new Vector3(0, 5f, 0), Color.yellow);
            //Debug.LogWarning("oneFinger!" + ray.origin);


            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.CompareTag("Room"))
            {
                
                Debug.DrawLine(ray.origin, hitInfo.point, Color.red);
                ray.origin = hitInfo.point - new Vector3(0, 0.1f, 0);
                
            }
            if (Physics.Raycast(ray, out hitInfo) && !hitInfo.collider.CompareTag("Wall")) // if the ray is hitting anything
            {
                //Debug.Log(!hitInfo.collider.CompareTag("Room"));
                Debug.DrawLine(ray.origin, hitInfo.point, Color.cyan);
                Vector3 targetPosition = new Vector3(cam.ScreenToWorldPoint(midPoint).x, hitInfo.point.y, cam.ScreenToWorldPoint(midPoint).z) + new Vector3(0, height, 0);
                //Vector3 targetPosition = cam.ScreenToWorldPoint(midPoint) - offset + new Vector3(0,height,0);
                this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1);
            }
        }
    }
    private void twoFingers()
    {
        if (cam.isActiveAndEnabled)
        {
            if (Input.touchCount == 2)
            {
                // Midpoint Mode
                Vector3 midPoint = Input.GetTouch(0).position + (Input.GetTouch(1).position - Input.GetTouch(0).position) / 2;

                Vector3 midLine = cam.ScreenToWorldPoint(Input.GetTouch(1).position) - cam.ScreenToWorldPoint(Input.GetTouch(0).position);
                midLine = new Vector3(midLine.x, 0, midLine.z);
                Debug.DrawLine(cam.ScreenToWorldPoint(Input.GetTouch(1).position), cam.ScreenToWorldPoint(Input.GetTouch(0).position), Color.green);
                Debug.DrawLine(this.transform.position, this.transform.position + midLine, Color.green);
                Debug.DrawLine(this.transform.position, this.transform.position + new Vector3(0,10,0), Color.red);
                

                var ray = cam.ScreenPointToRay(midPoint);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.CompareTag("Room"))
                {
                    ray.origin = hitInfo.point - new Vector3(0, 0.1f, 0);
                }
                if (Physics.Raycast(ray, out hitInfo) && !hitInfo.collider.CompareTag("Wall")) // if the ray is hitting anything
                {
                    Debug.Log(!hitInfo.collider.CompareTag("Room"));

                    Vector3 targetPosition = new Vector3(cam.ScreenToWorldPoint(midPoint).x, hitInfo.point.y, cam.ScreenToWorldPoint(midPoint).z) + new Vector3(0, height, 0);
                    //Vector3 targetPosition = cam.ScreenToWorldPoint(midPoint) - offset + new Vector3(0,height,0);
                    this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, 1);

                    // Rotation
                    Vector3 direction = Vector3.Cross(new Vector3(0, 1, 0), midLine);
                    Debug.DrawLine(this.transform.position, this.transform.position + direction, Color.yellow);
                    this.transform.rotation = Quaternion.LookRotation(direction, this.transform.up);
                }
            }
            else
            {
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            hapticController.PlayWallFeedback();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //if (collision.collider.CompareTag("Wall"))
        //{
        //    hapticController.
        //}

    }
}
