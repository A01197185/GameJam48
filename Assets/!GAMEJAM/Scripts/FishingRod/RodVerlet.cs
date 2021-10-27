
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class RodVerlet : UdonSharpBehaviour
{
    float maxBendAngle = 20f;
    [SerializeField] FishingRod fishingRod;
    

    public void Verlet(float bend)
    {
        Vector3 newRotation = new Vector3(maxBendAngle * bend, 0, 0);
        transform.localRotation = Quaternion.Euler(newRotation);
    }

    public void Update()
    {
        Verlet(fishingRod.bend);
    }
}
