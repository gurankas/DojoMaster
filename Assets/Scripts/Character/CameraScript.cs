using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraScript : MonoBehaviour
{
    public Transform cameraPoint;

    public Transform bossPoint;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;

        if (Player.isInBossFight)
        {
            Camera.main.orthographicSize = 9;
            transform.DOMove(bossPoint.position, 1);
            // newPosition.x = bossPoint.position.x;
            // newPosition.y = bossPoint.position.y;
        }
        else
        {
            newPosition.x = cameraPoint.position.x;
            newPosition.y = cameraPoint.position.y;

            transform.position = newPosition;
        }

        transform.position = newPosition;
    }
}
