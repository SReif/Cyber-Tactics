using System;
using UnityEngine.Audio;
using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Sound[] sounds;

    private List<Sound> pausedSounds;

    public static AudioManager Instance{
        get{
            return _instance;
        }
    }

    private static AudioManager _instance;
    
    void Awake()
    {
        if (_instance != null && _instance != this){
            Destroy(this.gameObject);
            return;
        }
        else {
            _instance = this;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
        pausedSounds = new List<Sound>();
    }

    public void SilenceAudioVolume(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarningFormat("Sound: {0} not found", name);
        }
        s.source.volume = 0f;
    }

    public void SetAudioVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarningFormat("Sound: {0} not found", name);
        }
        s.source.volume = volume;
    }

    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarningFormat("Sound: {0} not found", name);
        }
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.Play();
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarningFormat("Sound: {0} not found", name);
        }
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.PlayOneShot(s.source.clip);
    }

    public void Stop(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null){
            Debug.LogWarningFormat("Sound: {0} not found", name);
        }
        s.source.Stop();
    }

    public void StopAllAudio()
    {
        foreach (var sound in sounds)
        {
            sound.source.Stop();
        }
    }

    public void PauseAllAudio()
    {
        foreach (var sound in sounds)
        {
            if (sound.source.isPlaying)
            {
                pausedSounds.Add(sound);
                sound.source.Pause();
            }
        }
    }

    public void ResumeAllAudio()
    {
        foreach (var sound in pausedSounds)
        {
            sound.source.Play();
        }
        pausedSounds.Clear();

    }


}
