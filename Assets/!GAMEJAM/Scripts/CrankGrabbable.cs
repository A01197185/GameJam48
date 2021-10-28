
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class CrankGrabbable : UdonSharpBehaviour
{
    [SerializeField]
    FishingRod fishingRod;

    [SerializeField] Transform knob;

    public override void OnPickup()
    {

    }

    public override void OnDrop()
    {
        transform.SetPositionAndRotation(knob.position, knob.rotation);
    }
}
