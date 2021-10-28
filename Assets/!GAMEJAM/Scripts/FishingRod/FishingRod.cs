﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingRod : UdonSharpBehaviour
{
    [Range(0, 1f)]
    public float bend = 0f;

    [Header("Bobber")]
    [SerializeField] Bobber bobber;
    [SerializeField] Transform bobberTransform;

    [Header("Line")]
    [SerializeField] int lineQuality;
    [SerializeField] LineRenderer lineRenderer;

    [SerializeField] Transform startPoint;
    Vector3 midPoint;
    [SerializeField] Transform endPoint;

    [Header("References")]
    [SerializeField] VRC_Pickup pickup;
    [SerializeField] Rigidbody rb;

    [Header("Crank")]
    [SerializeField] Transform knob;
    [SerializeField] VRC_Pickup knobPickup;
    [SerializeField] Transform knobPickupTransform;
    [SerializeField] Transform handle;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip reelInAudio;

    Vector3 previousPosition;
    Vector3 knobVelocity;

    Vector3 bobberReset;

    bool isUserInVR;

    bool reeledIn = true;

    void Start()
    {
        lineRenderer.positionCount = lineQuality;
        bobberReset = bobberTransform.localPosition;
        audioSource.clip = reelInAudio;
        audioSource.loop = true;
    }

    public override void PostLateUpdate()
    {
        DrawRope();
    }

    private void Update()
    {
        if (!Networking.IsOwner(gameObject)) return;

        float crankPower = 0f;
        float newIntensity = 0f;

        if (pickup.IsHeld && pickup.currentPlayer.isLocal)
        {
            if (isUserInVR)
            {
                Vector3 lookPos = knobPickupTransform.localPosition - handle.localPosition;
                lookPos.y = 0f;
                handle.localRotation = Quaternion.LookRotation(lookPos);
            } else
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    handle.Rotate(Vector3.up * 20f, Space.Self);
                    knobPickupTransform.SetPositionAndRotation(knob.position, knob.rotation);
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    handle.Rotate(Vector3.up * 20f, Space.Self);
                    knobPickupTransform.SetPositionAndRotation(knob.position, knob.rotation);
                }
            }

            knobVelocity = (knobPickupTransform.localPosition - previousPosition) / Time.deltaTime;
            previousPosition = knobPickupTransform.localPosition;

            crankPower = Mathf.Clamp01(knobVelocity.magnitude / 1.5f);

            if (!isUserInVR) crankPower *= 5;
        } else
        {
            crankPower = 0f;
        }

        if (audioSource.isPlaying && crankPower <= 0.15f)
        {
            audioSource.Stop();
        }
        else if (!audioSource.isPlaying && crankPower > 0.15f)
        {
            audioSource.Play();
        }

    }

    public override void OnPickupUseDown()
    {
        reeledIn = !reeledIn;

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, reeledIn ? nameof(ReelInLine) : nameof(CastLine));
    }

    public void CastLine()
    {
        bobberTransform.parent = null;
        bobber.rb.isKinematic = false;
        bobber.reeledIn = false;

        bobber.rb.velocity = transform.forward * 10;
    }

    public void ReelInLine()
    {

        bobber.rb.isKinematic = true;
        bobber.reeledIn = true;
        bobberTransform.parent = transform;

        bobberTransform.localPosition = bobberReset;
    }

    public override void OnPickup()
    {
        rb.isKinematic = false;

        knobPickup.pickupable = true;
        isUserInVR = Networking.LocalPlayer.IsUserInVR();
        Networking.SetOwner(Networking.LocalPlayer, bobberTransform.gameObject);

        if (!isUserInVR)
        {
            pickup.orientation = VRC_Pickup.PickupOrientation.Gun;
        }
    }

    public override void OnDrop()
    {
        knobPickup.Drop();
        knobPickup.pickupable = false;
    }

    private void DrawRope()
    {
        lineRenderer.SetPosition(0, startPoint.position);
        midPoint = new Vector3(startPoint.position.x,endPoint.position.y/2,startPoint.position.z);

        int i = 0;
        for(float ratio = 0; ratio <= 1f; ratio += 1.0f / lineQuality, i++)
        {
            var tangentLineVertex1 = Vector3.Lerp(startPoint.position, midPoint, ratio);
            var tangentLineVertex2 = Vector3.Lerp(midPoint, endPoint.position, ratio);
            var bezierPoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
            
            if(i<lineQuality)
                lineRenderer.SetPosition(i, bezierPoint);
        }

        lineRenderer.SetPosition(lineQuality - 1, endPoint.position);
    }
}
