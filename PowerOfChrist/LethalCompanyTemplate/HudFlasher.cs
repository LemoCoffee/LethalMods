using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace PowerOfChrist
{
    class HudFlasher : MonoBehaviour
    {
        public UnityEngine.UI.RawImage image;
        public Texture2D[] textures;
        public bool isFlashing = false;
        public float flashTime = 2f;
        public GameNetcodeStuff.PlayerControllerB player;
        public AudioSource audioSource;

        void Start()
        {
            image = gameObject.GetComponent<UnityEngine.UI.RawImage>();
            image.color = new Color(1, 1, 1, 0);

            RectTransform transform = gameObject.GetComponent<RectTransform>();
            transform.anchoredPosition = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        void Update()
        {
            if (image.color.a > 0)
            {
                image.color = new Color(1, 1, 1, Mathf.MoveTowards(image.color.a, 0, (1 / flashTime) * Time.deltaTime));

                if (image.color.a == 0)
                {
                    image.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1); // Bandaid fix to stop pause menu bug
                }
            }
        }

        public void Flash()
        {
            if (textures == null)
            {
                return;
            }

            /*if (!player.IsClient) // This might be breaking the mod for host
            {
                Plugin.Logger.LogInfo("Other client, skipping hud flash");
                return;
            }*/

            this.SetImageToRandTexture(ref image, textures);

            image.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);

            RectTransform transform = gameObject.GetComponent<RectTransform>();
            transform.anchoredPosition = Vector3.zero;
            image.color = new Color(1, 1, 1, 1);
        }

        void SetImageToRandTexture(ref UnityEngine.UI.RawImage image, Texture2D[] textures)
        {
            image.texture = textures[UnityEngine.Random.Range(0, textures.Length)];
        }

        public bool BeginFlash()
        {
            
            if (player == null)
            {
                player = GameNetworkManager.Instance.localPlayerController;
                Plugin.Logger.LogInfo("Player set");
            }

            if (audioSource == null)
            {
                audioSource = GetOrAddAudioSource();
                Plugin.Logger.LogInfo("Audio Source Set");
            }

            if (!isFlashing)
            {
                RepeatFlash();
                return true;
            }

            return false;
        }

        public void StopFlash()
        {
            this.isFlashing = false;
            image.color = new Color(1, 1, 1, 0); // Make flasher transparent
            image.GetComponent<RectTransform>().sizeDelta = new Vector2(1, 1); // Bandaid fix to stop pause menu bug

            if (this.audioSource != null)
            {
                audioSource.Stop(); // Stop the sound to account for longer audioclips
            }
            
        }

        void RepeatFlash()
        {
            if (player == null)
            {
                Plugin.Logger.LogWarning("Player object is null, stopping");
                this.isFlashing = false;
                return;
            }

            if (player.health <= 20 && player.health > 0 && !player.isPlayerDead)
            {
                audioSource.PlayOneShot(Plugin.dangerSound);
                Flash();

                Invoke(nameof(RepeatFlash), 5);

                this.isFlashing = true;
            }
            else
            {
                this.isFlashing = false;
            }
        }

        AudioSource GetOrAddAudioSource()
        {
            Transform audiosObj = player.transform.Find("Audios");
            Transform pocAudio = audiosObj.Find("PowerOfChristAudio");

            if (pocAudio == null)
            {

                // Create audio source
                GameObject audioGameObj = new GameObject("PowerOfChristAudio", typeof(AudioSource));
                AudioSource audioSource = audioGameObj.GetComponent<AudioSource>();

                // Set the audio clip
                audioSource.clip = Plugin.dangerSound;
                audioSource.loop = false;
                audioSource.playOnAwake = false;
                audioSource.Stop();

                // Attach to audiosObj
                audioGameObj.transform.SetParent(audiosObj);

                // Set audioSource reference
                Plugin.audioSource = audioSource;

                return audioSource;
            }
            else
            {
                Plugin.Logger.LogInfo("Audio source already exists");
                return pocAudio.GetComponent<AudioSource>();
            }
        }
    }
}
