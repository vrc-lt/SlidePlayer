
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace VRCLT
{
    public class PrevSlideButtonController : UdonSharpBehaviour
    {
        public SlidePlayer slidePlayer;

        public override void Interact()
        {
            slidePlayer.OnPrevSlideButtonClick();
        }
    }
}