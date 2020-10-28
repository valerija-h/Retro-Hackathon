using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public float volume;
    public AudioClip themeSong;
    public AudioClip jump;
    private AudioSource audioSource;

    public void PlaySoundEffect(string soundEffect) {
        AudioClip soundEffectClip = null;
        switch (soundEffect)
        {
            case "jump":
                soundEffectClip = jump;
                break;
            default:
                break;
        }

        if (soundEffectClip != null)
        {
            audioSource.PlayOneShot(soundEffectClip, volume);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


}
