using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float smoothing;
    [SerializeField] float lookAheadSmooth;
    [SerializeField] float lookAheadDistance;
    [SerializeField] float minDistanceToTarget;
    [SerializeField] Vector3 offset = new Vector3(0, 0, 0);

    Vector3 currentVelocity = Vector3.zero;
    Vector3 lookAhead;
    Vector3 lookAheadVelocity = Vector3.zero;
    Vector3 directionToTarget;


    void Start()
    {
        transform.position = target.position;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(
            transform.localPosition,
            target.position - target.forward * offset.x + target.up * offset.y,
            ref currentVelocity,
            smoothing);

        directionToTarget = target.position - transform.position;
        if (directionToTarget.magnitude < minDistanceToTarget)
            transform.position = target.position - directionToTarget.normalized * minDistanceToTarget;

        transform.LookAt(target.position);

        lookAhead = Vector3.SmoothDamp(
            lookAhead,
            target.forward * lookAheadDistance,
            ref lookAheadVelocity,
            lookAheadSmooth
            );

        transform.LookAt(target.position + lookAhead);

    }
}