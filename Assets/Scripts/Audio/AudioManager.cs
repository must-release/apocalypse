
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace AD.Audio
{
    public class AudioManager : MonoBehaviour
    {
        /****** Public Members ******/
        public static AudioManager Instance { get; private set; }


        /****** Private Members ******/
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;

        private Dictionary<string, AudioClip> _bgmClips;
        private Dictionary<string, AudioClip> _sfxClips;

        private void Awake()
        {
            if (null == Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (null == _bgmSource)
            {
                _bgmSource = gameObject.AddComponent<AudioSource>();
                _bgmSource.loop = true;
            }

            if (null == _sfxSource)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
            }

            _bgmClips = new Dictionary<string, AudioClip>();
            _sfxClips = new Dictionary<string, AudioClip>();
        }

        private async void Start()
        {
            await LoadAudioAssets();
        }

        private async UniTask LoadAudioAssets()
        {
            var bgmAssetHandle = Addressables.LoadAssetAsync<BGMAsset>(AssetPath.BGM);
            var sfxAssetHandle = Addressables.LoadAssetAsync<SFXAsset>(AssetPath.SFX);

            await UniTask.WhenAll(bgmAssetHandle.ToUniTask(), sfxAssetHandle.ToUniTask());

            Debug.Assert(bgmAssetHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded,
                        "Failed to load BGM asset.");
            BGMAsset bgmAsset = bgmAssetHandle.Result;
            foreach (var clip in bgmAsset.BGMList)
            {
                _bgmClips[clip.name] = clip;
            }

            Debug.Assert(sfxAssetHandle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded,
                        "Failed to load SFX asset.");
            SFXAsset sfxAsset = sfxAssetHandle.Result;
            foreach (var clip in sfxAsset.SFXList)
            {
                _sfxClips[clip.name] = clip;
            }

            Addressables.Release(bgmAssetHandle);
            Addressables.Release(sfxAssetHandle);
        }

        public void PlayBGM(string clipName)
        {
            Debug.Assert(null != _bgmSource, "_bgmSource is not initialized.");
            Debug.Assert(_bgmClips.TryGetValue(clipName, out AudioClip clip), $"BGM clip not found: {clipName}.");
            Debug.Assert(null != clip, "BGM clip cannot be null.");

            _bgmSource.clip = _bgmClips[clipName];
            _bgmSource.Play();
        }

        public void StopBGM()
        {
            Debug.Assert(null != _bgmSource, "_bgmSource is not initialized.");
            _bgmSource.Stop();
        }

        public void SetBGMVolume(float volume)
        {
            Debug.Assert(volume >= 0f && volume <= 1f, "Volume must be between 0 and 1.");
            Debug.Assert(null != _bgmSource, "_bgmSource is not initialized.");
            _bgmSource.volume = volume;
        }

        public void PlaySFX(string clipName)
        {
            Debug.Assert(null != _sfxSource, "_sfxSource is not initialized.");
            Debug.Assert(_sfxClips.TryGetValue(clipName, out AudioClip clip), $"SFX clip not found: {clipName}.");
            Debug.Assert(null != clip, "SFX clip cannot be null.");

            _sfxSource.PlayOneShot(_sfxClips[clipName]);
        }

        public void SetSFXVolume(float volume)
        {
            Debug.Assert(volume >= 0f && volume <= 1f, "Volume must be between 0 and 1.");
            Debug.Assert(null != _sfxSource, "_sfxSource is not initialized.");
            _sfxSource.volume = volume;
        }
    }
}
