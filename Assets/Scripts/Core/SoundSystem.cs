using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class SoundSystem : MonoSingleton<SoundSystem>
{
    AudioSource _music;
    List<AudioSource> _sources = new();
    Dictionary<string, AudioClip> _cachedClips = new();

    protected override void Initialize()
    {
        DontDestroyOnLoad(this);

        _music = GetComponent<AudioSource>();
        NewSourceEntry();
    }

    public void NewScene(int sceneIndex)
    {
        _sources.ForEach(s => Destroy(s));
        _sources = new List<AudioSource>();
        _cachedClips = new ();
        
        _music.clip = sceneIndex switch
        {
            0 => LoadSong("Alien"),
            // MAIN MENU
            1 => LoadSong("Beach"),
            // lvl 1
            2 => LoadSong("Ocean"),
            // lvl 2
            3 => LoadSong("Swamp"),
            // lvl 3
            _ => LoadSong("Z"),
            // None
        };
        _music.Play();
        // Loads music based on what scene is being used
        _music.loop = true;
    }

    public void PlaySound(string clip)
    {
        var availableSource = _sources.FirstOrDefault(source => !source.isPlaying) ?? NewSourceEntry();

        if (!_cachedClips.TryGetValue(clip, out AudioClip sfx))
            sfx = NewClip(clip);
        // If it can't find the sound in the dict it makes a new entry

        availableSource.clip = sfx;
        availableSource.Play();
        // Plays the source after being fitted to the proper clip
    }
    internal void SetSpeed(float curSpeed)
    {
        _music.pitch = curSpeed;
        _sources.ForEach(audio => audio.pitch = curSpeed);
        // For each sound effect it matches pitch+speed to timescale
    }
    AudioSource NewSourceEntry()
    {
        var newSource = gameObject.AddComponent<AudioSource>();
        newSource.playOnAwake = false;
        _sources.Add(newSource);
        // Caches a reference to this new audio source in the sfx list for later use
        return newSource;
    }
    AudioClip NewClip(string name)
    {
        var clip = Resources.Load<AudioClip>(string.Format("SFX/{0}", name));
        _cachedClips.Add(name, clip);
        // Loads audio clip by name at /Resources/SFX/ & adds it to cache
        return clip;
    }

    public void PauseAudio()
    {
        _music.Pause();
        _sources.ForEach(source => source.Pause());
    }

    public void UnpauseAudio()
    {
        _music.UnPause();
        _sources.ForEach(source => source.UnPause());
    }

    AudioClip LoadSong(string song)
        => Resources.Load<AudioClip>(string.Format("Music/{0}", song));
    // We load from the song name from the music folder
}