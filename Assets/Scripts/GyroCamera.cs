using UnityEngine;
using System.Collections;

public class GyroCamera : MonoBehaviour
{
    private Quaternion correctionQuaternion;

    private void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            correctionQuaternion = Quaternion.Euler(90f, 0f, 0f);
        }
    }

    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Quaternion gyroQuaternion = GyroToUnity(Input.gyro.attitude);
            Quaternion calculatedRotation = correctionQuaternion * gyroQuaternion;
            transform.rotation = calculatedRotation;

        }
    }

    private Quaternion GyroToUnity(Quaternion q)
    {

        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}
