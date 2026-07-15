using Managers;
using Unity.Cinemachine; // CM3 (Unity 6). For Cinemachine 2.x use: using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI
{
    /// <summary>
    /// Lives on the rival-cam RawImage. Owns the RenderTexture, feeds it from the
    /// rival Camera, and throttles rendering to every Nth frame for mobile.
    ///
    /// Wire-up:
    ///   1. Add to the RawImage inside the masked PiP panel.
    ///   2. Assign the rival Camera (the one with the second CinemachineBrain).
    ///   3. From your spawn binding (Spawned() on the non-authority car, or your
    ///      signal/event handler) call Bind(rivalVcam target already set on the vcam)
    ///      and Unbind() on despawn.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public sealed class RivalCamView : MonoBehaviour
    {
        [Header("Rendering")]
        [SerializeField] private Camera rivalCamera;
        [SerializeField] private CinemachineBrain rivalBrain; // optional; auto-found from camera
        [SerializeField] private Vector2Int resolution = new(128, 72);
        [SerializeField, Range(1, 6)] private int renderEveryNthFrame = 3;

        [Header("UI")]
        [Tooltip("Shown while no rival is connected (e.g. 'WAITING…' panel). Optional.")]
        [SerializeField] private GameObject offlinePlaceholder;

        [Inject] private GameManager _gameManager;
        
        private RawImage _image;
        private RenderTexture _rt;
        private bool _bound;

        private void Awake()
        {
            _image = GetComponent<RawImage>();
            _image.raycastTarget = false;

            _rt = new RenderTexture(resolution.x, resolution.y, 16)
            {
                name = "RivalCamRT",
                antiAliasing = 1
            };

            if (rivalCamera != null)
            {
                rivalCamera.targetTexture = _rt;
                if (rivalBrain == null)
                    rivalBrain = rivalCamera.GetComponent<CinemachineBrain>();
            }

            _image.texture = _rt;
            SetFeedActive(false);

            _gameManager.OnRivalSpawned += Bind;
        }

        private void OnDestroy()
        {
            _gameManager.OnRivalSpawned -= Bind;
            
            if (rivalCamera != null && rivalCamera.targetTexture == _rt)
                rivalCamera.targetTexture = null;

            if (_rt != null)
            {
                _rt.Release();
                Destroy(_rt);
            }
        }

        /// <summary>Call when the rival car exists and its vcam target is assigned.</summary>
        public void Bind()
        {
            _bound = true;
            SetFeedActive(true);
        }

        /// <summary>Call on rival despawn / disconnect.</summary>
        public void Unbind()
        {
            _bound = false;
            SetFeedActive(false);
        }

        private void LateUpdate()
        {
            if (!_bound || rivalCamera == null) return;

            // Render only every Nth frame; RT keeps its last contents in between.
            bool renderThisFrame = Time.frameCount % renderEveryNthFrame == 0;
            rivalCamera.enabled = renderThisFrame;
            /*if (rivalBrain != null)
                rivalBrain.enabled = renderThisFrame;*/
        }

        private void SetFeedActive(bool active)
        {
            if (rivalCamera != null) rivalCamera.enabled = active;
            /*if (rivalBrain != null) rivalBrain.enabled = active;

            _image.enabled = active;*/
            if (offlinePlaceholder != null)
                offlinePlaceholder.SetActive(!active);
        }

        /// <summary>
        /// Optional: call from your quality settings (e.g. the in-race Graphics tab)
        /// to retune the PiP cost per device tier. Pass nthFrame = 0 to disable the feed.
        /// </summary>
        public void ApplyQuality(int nthFrame)
        {
            if (nthFrame <= 0)
            {
                Unbind();
                return;
            }
            renderEveryNthFrame = Mathf.Clamp(nthFrame, 1, 6);
            if (_bound) SetFeedActive(true);
        }
    }
}