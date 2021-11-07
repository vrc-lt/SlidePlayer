
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;
using VRC.SDK3.Components;
using VRC.SDK3.Video.Components;
using VRC.SDK3.Video.Components.AVPro;
using VRC.SDK3.Video.Components.Base;
using VRC.SDK3.Components.Video;

namespace VRCLT
{
    [AddComponentMenu("SlidePlayer")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class SlidePlayer : UdonSharpBehaviour
    {
        [UdonSynced] [FieldChangeCallback(nameof(URL))]
        VRCUrl _syncedURL;

        [UdonSynced] [FieldChangeCallback(nameof(Page))]
        private int _syncedPage;

        private VRCUrl URL
        {
            get => _syncedURL;
            set
            {
                _syncedURL = value;
                Debug.Log("ChangeVideoURL: " + value);
                unityVideoPlayer.LoadURL(value);
            }
        }

        private int Page
        {
            get => _syncedPage;
            set
            {
                _syncedPage = value;
                ChangeVideoPosition();
            }
        }

        public VRCUrlInputField inputField;
        public Text urlText;
        public Text statusText;

        public VRCUnityVideoPlayer unityVideoPlayer;

        public float timeSpan = 2f;
        private float timeOffset = 1f;

        public void OnTakeOwnershipClicked()
        {
            Debug.Log("Take ownership");
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            inputField.gameObject.SetActive(true);
        }

        public void OnURLChanged()
        {
            VRCUrl url = inputField.GetUrl();
            if (Networking.IsOwner(gameObject) && url != null)
            {
                Debug.Log("OnURLChanged url: " + url.ToString());
                statusText.text = "Loading...";
                Page = 0;
                URL = url;
                RequestSerialization();
            }
            else
            {
                statusText.text = "You must be the owner to set the URL ";
            }
        }

        public void OnNextSlideButtonClick()
        {
            if (Networking.IsOwner(gameObject))
            {
                Debug.Log("OnNextSlideButtonClick as owner");
                Page++;
                RequestSerialization();
            }
            else
            {
                statusText.text = "Owner: " + Networking.GetOwner(gameObject).displayName;
            }
        }

        public void OnPrevSlideButtonClick()
        {
            if (Networking.IsOwner(gameObject))
            {
                Debug.Log("OnPrevSlideButtonClick as owner");
                if (Page > 0)
                {
                    Page--;
                    RequestSerialization();
                }
            }
        }

        public void OnResetButtonClick()
        {
            if (Networking.IsOwner(gameObject))
            {
                Debug.Log("OnResetButtonClick as owner");
                Page = 0;
                RequestSerialization();
            }
        }


        private void ChangeVideoPosition()
        {
            Debug.Log("ChangeVideoPosition: " + Page);
            unityVideoPlayer.SetTime(Page * timeSpan + timeOffset);
        }

        public override void OnOwnershipTransferred()
        {
            if (!Networking.IsOwner(gameObject))
            {
                inputField.gameObject.SetActive(false);
            }
        }

        public override void OnVideoReady()
        {
            Debug.Log("OnVideoReady");
            ChangeVideoPosition();
            if (!Networking.IsOwner(gameObject))
            {
                statusText.text = "Video ready. Owner: " + Networking.GetOwner(gameObject).displayName;
            }
            else
            {
                statusText.text = "Video ready. click \"next\" on control panel to start presentation.";
            }
        }
        public override void OnVideoError(VideoError videoError)
        {

            switch (videoError)
            {
                case VideoError.RateLimited:
                    statusText.text = "Rate limited, try again in a few seconds";
                    break;
                case VideoError.PlayerError:
                    statusText.text = "Video player error";
                    break;
                case VideoError.InvalidURL:
                    statusText.text = "Invalid URL";
                    break;
                case VideoError.AccessDenied:
                    statusText.text = "Video blocked, enable untrusted URLs";
                    break;
                default:
                    statusText.text = "Failed to load video";
                    break;
            }
        }
    }
}
