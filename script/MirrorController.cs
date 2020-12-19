
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MirrorController : UdonSharpBehaviour
{
    public GameObject mirror;
    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.playerId == player.playerId)
        {
            mirror.SetActive(true);
        }
        Debug.Log("Mirror Enabled");
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
        if (Networking.LocalPlayer.playerId == player.playerId)
        {
            mirror.SetActive(false);
        }
        Debug.Log("Mirror Disabled");
    }

}
