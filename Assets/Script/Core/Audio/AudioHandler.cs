using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace Script.Core.Audio
{
    public class AudioHandler : MonoBehaviour {

    [Header("Audio Mixer & Groups")]
    [SerializeField] private AudioMixer mainMixer;
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup potGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource potLayerSource; 
    [SerializeField] private AudioSource panLayerSource;
    [SerializeField] private AudioSource littlePotLayerSource;
    [SerializeField] private AudioSource sfxSource;
    
    [Header("Audio Clips")]
    [SerializeField] private AudioClip magicBGMClip;

    private float _defaultFadeDuration = 0.5f;
    private Dictionary<AudioClip, AudioSource> _sourceDict;
    
    public void SetUp()
    {
        if (bgmSource != null) bgmSource.outputAudioMixerGroup = bgmGroup;
        if (sfxSource != null) sfxSource.outputAudioMixerGroup = sfxGroup;
        _sourceDict  = new Dictionary<AudioClip, AudioSource>();
        _sourceDict.Add(potLayerSource.clip, potLayerSource);
        _sourceDict.Add(panLayerSource.clip, panLayerSource);
        _sourceDict.Add(littlePotLayerSource.clip, littlePotLayerSource);
    }

    #region Basic Playback Functions (基本播放)

    /// <summary>
    /// 播放背景音樂
    /// </summary>
    public void PlayBGM(AudioClip clip, bool loop = true, float fadeDuration = 0.5f)
    {
        if (clip == null) return;

        if (bgmSource.isPlaying)
        {
            // 如果已有 BGM 正在播，做平滑切換 (Crossfade)
            bgmSource.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                bgmSource.clip = clip;
                bgmSource.loop = loop;
                bgmSource.Play();
                bgmSource.DOFade(1f, fadeDuration);
            });
        }
        else
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.volume = 0f;
            bgmSource.Play();
            bgmSource.DOFade(1f, fadeDuration);
        }
    }
    
    public void PlayMagicBGM()
    {
        PlayBGM(magicBGMClip, false);
        foreach (KeyValuePair<AudioClip, AudioSource> kvp in _sourceDict)
        {
            kvp.Value.DOFade(0f, 0.1f);
        }
    }

    /// <summary>
    /// 播放一次性音效
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    #endregion

    #region Pot Layer Audio Logic (鍋子動態伴奏核心)

    /// <summary>
    /// 🌟 加入/切換鍋子伴奏 (支援無縫同步與淡入淡出)
    /// </summary>
    /// <param name="potAccompaniment">伴奏音訊檔</param>
    /// <param name="fadeDuration">淡入時間</param>
    /// <param name="syncWithBGM">是否要強制對齊主 BGM 的播放時間點 (對拍)</param>
    public void AddPotAccompaniment(AudioClip potAccompaniment, float fadeDuration = 0.5f, bool syncWithBGM = true)
    {
        _sourceDict[potAccompaniment].DOFade(1f, fadeDuration);
        Debug.Log($"[AudioManager] 已加入鍋子伴奏: {potAccompaniment.name}");
    }

    /// <summary>
    /// 🌟 移除/淡出鍋子伴奏
    /// </summary>
    public void RemovePotAccompaniment(AudioClip potAccompaniment, float fadeDuration = 0.5f)
    {
        if (!_sourceDict.ContainsKey(potAccompaniment)) return;

        _sourceDict[potAccompaniment].DOKill();
        _sourceDict[potAccompaniment].DOFade(0f, fadeDuration).OnComplete(() =>
        {
            potLayerSource.Stop();
            Debug.Log("[AudioManager] 已移除鍋子伴奏");
        });
    }

    /// <summary>
    /// 微調伴奏軌音量 (例如根據當前滾沸程度/鍋子數量)
    /// </summary>
    public void SetPotAccompanimentVolume(float targetVolume, float fadeDuration = 0.2f)
    {
        if (!potLayerSource.isPlaying) return;
        potLayerSource.DOKill();
        potLayerSource.DOFade(Mathf.Clamp01(targetVolume), fadeDuration);
    }

    #endregion
    }
}