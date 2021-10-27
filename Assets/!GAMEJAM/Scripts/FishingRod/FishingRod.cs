
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class FishingRod : UdonSharpBehaviour
{
    [Range(0, 1f)]
    public float bend = 0f;

    void Start()
    {
        
    }
}
