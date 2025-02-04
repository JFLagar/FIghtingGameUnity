using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public AudioClip music;
    public AudioClip[] sounds;
    public AudioSource musicSource;
    public AudioSource[] soundsSources;
    // Start is called before the first frame update
    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayAnimationEffect(AudioClip clip, int playerSoundSourceId = 0)
    {
        soundsSources[playerSoundSourceId].clip = clip;
        soundsSources[playerSoundSourceId].Play();
    }
    public void PlaySoundEffect(int id)
    {
        soundsSources[0].clip = sounds[id];
        soundsSources[0].Play();
    }
}
