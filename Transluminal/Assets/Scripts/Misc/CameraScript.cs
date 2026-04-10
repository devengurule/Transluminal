using System;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float trackSpeed;
    [SerializeField] private float rotationSpeed;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, target.transform.position, trackSpeed);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);

        float targetZ = target.transform.eulerAngles.z;
        float thisZ = transform.eulerAngles.z;
        float lerpAngle = Mathf.Lerp(0, Mathf.DeltaAngle(thisZ, targetZ), rotationSpeed);

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + lerpAngle);
    }
}
