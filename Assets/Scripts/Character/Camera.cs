using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{

    public Transform cameraPoint;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;

        newPosition.x = cameraPoint.position.x;
        newPosition.y = cameraPoint.position.y;

        transform.position = newPosition;
    }
}
