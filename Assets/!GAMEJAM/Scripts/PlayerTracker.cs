
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class PlayerTracker : UdonSharpBehaviour
{
    VRCPlayerApi localPlayer;

    private void Start()
    {
        localPlayer = Networking.LocalPlayer;
        SendCustomEventDelayedSeconds(nameof(LateStart),1f);
    }

    private void LateStart()
    {
        if (localPlayer.IsUserInVR())
        {
            transform.position += new Vector3(0.5f, 0, 0);
        }
    }

    public override void PostLateUpdate()
    {
        if (!Utilities.IsValid(localPlayer)) return;

        transform.position = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).position;

        if (localPlayer.IsUserInVR())
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation, Time.deltaTime * 10f);
        } else
        {
            transform.rotation = localPlayer.GetTrackingData(VRCPlayerApi.TrackingDataType.Head).rotation;
        }
            
    }
}
