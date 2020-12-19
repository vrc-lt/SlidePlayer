using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.SDKBase;
using VRC.Udon;

public class HandyVoiceController : UdonSharpBehaviour
{
    [UdonSynced]
    bool enabled;

    bool localEnabled;

    public override void OnPickup()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        enabled = true;
        Debug.Log("Loudspeaker activated");
    }

    public override void OnDrop()
    {
        enabled = false;
        Debug.Log("Loudspeaker deactivated");
    }
    public override void OnDeserialization()
    {
        VRCPlayerApi player = Networking.GetOwner(gameObject);
        Debug.Log("OnDeserialization");
        if (!Networking.IsOwner(gameObject))
        {
            if (localEnabled != enabled)
            {
                if (enabled)
                {
                    Debug.Log("Loudspeaker activated");
                    player.SetVoiceDistanceFar(400f);
                    player.SetAvatarAudioGain(7.5f);
                }
                else
                {
                    Debug.Log("Loudspeaker deactivated");
                    player.SetVoiceDistanceFar(25f);
                    player.SetAvatarAudioGain(10f);
                }
                localEnabled = enabled;
            }
        }
    }

}
