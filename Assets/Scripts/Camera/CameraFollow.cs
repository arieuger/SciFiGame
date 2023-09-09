using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    
    [SerializeField] private float leftCamLimit = -10f;
    [SerializeField] private Transform playerTransform;

    private Transform followTransform;
    
    private CinemachineVirtualCamera vCam;
    private CinemachineBrain cinemachineBrain;
    private CinemachineFramingTransposer cinemachineFramingTransposer;
    
    public static CameraFollow Instance { get; private set; }
    private void Awake() 
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    
    void Start()
    {
        followTransform = playerTransform;
        vCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
        cinemachineFramingTransposer = vCam.GetComponentInChildren<CinemachineFramingTransposer>();
        cinemachineBrain = GameObject.Find("MainCamera").GetComponent<CinemachineBrain>();
    }
    
    void Update()
    {
        transform.position = followTransform.position;
    }

    void FixedUpdate()
    {
        CheckLimitsOnCam();
    }

    public void TurnCamera()
    {
        cinemachineFramingTransposer.m_TrackedObjectOffset.x *= -1f;
    }

    public void ChangeTarget(Transform target)
    {
        followTransform = target;
    }

    private void CheckLimitsOnCam() {
        
        if (transform.position.x < leftCamLimit && vCam.m_Follow != null) vCam.m_Follow = null;
        else if (transform.position.x >= leftCamLimit && vCam.m_Follow == null) vCam.m_Follow = followTransform;

        // Rigidbody2D rb = followTransform.GetComponent<Rigidbody2D>();
        // if (rb == null) return;
        //
        // if (rb.velocity.y < -0.1f) {
        //     if (cinemachineFramingTransposer.m_TrackedObjectOffset.y > 0f)
        //         cinemachineFramingTransposer.m_TrackedObjectOffset.y *= -1f;
        // } else {
        //     cinemachineFramingTransposer.m_TrackedObjectOffset.y = Mathf.Abs(cinemachineFramingTransposer.m_TrackedObjectOffset.y);
        // }
    }
}
