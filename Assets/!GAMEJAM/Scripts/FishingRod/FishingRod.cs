
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingRod : UdonSharpBehaviour
{
    [HideInInspector]
    public float bend = 0f;

    [Header("Fishing")]
    [SerializeField] float minCatchTime = 5f;
    [SerializeField] float maxCatchTime = 15f;

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
    [SerializeField] AudioSource effectAudioSource;
    [SerializeField] AudioSource bobberAudioSource;
    [SerializeField] AudioClip castLineSound;
    [SerializeField] AudioClip reelInAudio;
    [SerializeField] AudioClip fishBitSound;
    [SerializeField] AudioClip caughtFish;
    [SerializeField] AudioClip looseFish;

    [Header("MiniGame")]
    public MiniGame miniGame;

    Vector3 previousPosition;
    Vector3 knobVelocity;

    Vector3 bobberReset;

    bool isUserInVR;

    bool reeledIn = true;
    bool fishBiting = false;

    float crankPower = 0f;

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

        float newCrankPower = 0f;

        if (pickup.IsHeld && pickup.currentPlayer.isLocal)
        {
            if (isUserInVR)
            {
                Vector3 lookPos = knobPickupTransform.localPosition - handle.localPosition;
                lookPos.y = 0f;
                handle.localRotation = Quaternion.LookRotation(lookPos);
            } else
            {
                if (Input.GetKey(KeyCode.E))
                {
                    handle.Rotate(Vector3.up * 20f, Space.Self);
                    knobPickupTransform.SetPositionAndRotation(knob.position, knob.rotation);
                }
                /*else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    handle.Rotate(Vector3.up * -20f, Space.Self);
                    knobPickupTransform.SetPositionAndRotation(knob.position, knob.rotation);
                }*/
            }

            knobVelocity = (knobPickupTransform.localPosition - previousPosition) / Time.deltaTime;
            previousPosition = knobPickupTransform.localPosition;

            newCrankPower = Mathf.Clamp01(knobVelocity.magnitude / 1.5f);

            //if (!isUserInVR) crankPower *= 5;
            if (!isUserInVR) newCrankPower *= 1;

        } else
        {
            newCrankPower = 0f;
        }

        /*
        crankPower = Mathf.Lerp(crankPower, newCrankPower, Time.deltaTime);

        crankPower = Mathf.Clamp(crankPower, 0, 0.16f);
        
        Debug.Log(crankPower);*/

        if (audioSource.isPlaying && newCrankPower <= 0.15f)
        {
            audioSource.Stop();
            miniGame.press = false;
        }
        else if (!audioSource.isPlaying && newCrankPower > 0.15f)
        {
            audioSource.Play();
            miniGame.press = true;
        }




        /*
        if(!reeledIn && crankPower >= 0.9f)
        {
            if (fishBiting)
            {
                CatchFish();
            } else
            {
                SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ReelInLine));
            }
            
        }*/

    }

    public override void OnPickupUseDown()
    {
        if (reeledIn)
        {
            SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(CastLine));
        }
        
        //SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, reeledIn ? nameof(CastLine) : nameof(ReelInLine));
    }

    public void CastLine()
    {
        if (!reeledIn) return;

        reeledIn = false;
        bobberTransform.parent = null;
        bobber.rb.isKinematic = false;
        bobber.reeledIn = false;

        bobber.rb.velocity = transform.forward * 10;

        effectAudioSource.PlayOneShot(castLineSound);

        if(Networking.IsOwner(gameObject))
            SendCustomEventDelayedSeconds(nameof(FishBit), Random.Range(minCatchTime, maxCatchTime));
    }

    public void FishBit()
    {
        if (reeledIn || fishBiting) return;

        miniGame._StartPlaying();

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(FishBitAll));
        fishBiting = true;
    }

    public void FishBitAll()
    {
        //TODO Bobber effect!
        bobberAudioSource.PlayOneShot(fishBitSound);
        bend = 1f;
    }

    public void CatchFish()
    {
        bend = 0f;
        fishBiting = false;
        
        effectAudioSource.PlayOneShot(caughtFish);

        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.All, nameof(ReelInLine));
    }

    public void ReelInLine()
    {
        if (fishBiting) effectAudioSource.PlayOneShot(looseFish);
        fishBiting = false;

        reeledIn = true;
        bobber.rb.isKinematic = true;
        bobber.reeledIn = true;
        bobberTransform.parent = transform;
        bend = 0f;

        bobberTransform.localPosition = bobberReset;
    }

    public override void OnPickup()
    {
        knobPickup.pickupable = true;
        isUserInVR = Networking.LocalPlayer.IsUserInVR();
        Networking.SetOwner(Networking.LocalPlayer, bobberTransform.gameObject);

        miniGame.localPlayer.fishingRod = this;

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
        midPoint = new Vector3(startPoint.position.x,endPoint.position.y,startPoint.position.z);

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
