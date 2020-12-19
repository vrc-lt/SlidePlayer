
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
    public class SlidePlayer : UdonSharpBehaviour
    {
        [UdonSynced]
        VRCUrl _syncedURL;
        [UdonSynced]
        int _page = 0;

        string localURL = "";

        public VRCUrlInputField inputField;
        public Text urlText;
        public Text statusText;

        public VRCUnityVideoPlayer unityVideoPlayer;
        int localPage = 0;

        public float timeSpan = 2f;
        private float timeOffset = 1f;


        public void OnURLChanged()
        {
            VRCUrl url = inputField.GetUrl();
            if (url != null)
            {
                Debug.Log("OnURLChanged url: " + url.ToString());
            }
            if (!Networking.IsOwner(gameObject))
            {
                Debug.Log("Take ownership");
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
                statusText.text = "Loading...";
                _page = 0;
                localPage = _page;
            }
            _syncedURL = url;
            unityVideoPlayer.LoadURL(url);
        }

        public void OnNextSlideButtonClick()
        {
            if (Networking.IsOwner(gameObject))
            {
                Debug.Log("OnNextSlideButtonClick as owner");
                _page++;
                localPage = _page;
                ChangeVideoPosition(localPage);
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
                if (_page > 0)
                {
                    _page--;
                    localPage = _page;
                }
                ChangeVideoPosition(localPage);
            }
        }

        public void OnResetButtonClick()
        {
            if (Networking.IsOwner(gameObject))
            {
                _page = 0;
                localPage = _page;
                ChangeVideoPosition(localPage);
            }
        }


        private void ChangeVideoPosition(int pageNumber)
        {
            Debug.Log("ChangeVideoPosition: " + pageNumber);
            unityVideoPlayer.SetTime(((float)pageNumber * timeSpan) + timeOffset);
        }

        public override void OnDeserialization()
        {
            Debug.Log("OnDeserialization");
            if (!Networking.IsOwner(gameObject))
            {
                if (_page != localPage)
                {
                    Debug.Log("OnDeserialization: Page Changed");
                    localPage = _page;
                    ChangeVideoPosition(localPage);
                }
                if (_syncedURL != null)
                {
                    if (_syncedURL.ToString() != localURL)
                    {
                        Debug.Log("OnDeserialization: URL Changed");
                        Debug.Log("Local url: " + localURL);
                        Debug.Log("Synced url: " + _syncedURL.ToString());
                        localURL = _syncedURL.ToString();
                        unityVideoPlayer.LoadURL(_syncedURL);
                    }
                }
            }
        }

        public override void OnVideoReady()
        {
            Debug.Log("OnVideoReady");
            ChangeVideoPosition(localPage);
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
