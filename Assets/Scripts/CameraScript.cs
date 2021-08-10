using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraScript : MonoBehaviour
{
    public Transform cameraPoint;

    public Transform bossPoint;

    public float orthographicSize = 10.0f;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position;

        if (Player.isInBossFight)
        {
            Camera.main.orthographicSize = orthographicSize;
            transform.DOMove(bossPoint.position, 1);
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
