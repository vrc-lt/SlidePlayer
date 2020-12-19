
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class VoiceVolumeController : UdonSharpBehaviour
{
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        player.SetVoiceDistanceFar(400f);
        Debug.Log("Loudspeaker activated");
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        player.SetVoiceDistanceFar(25f);
        Debug.Log("Loudspeaker deactivated");
    }

}
