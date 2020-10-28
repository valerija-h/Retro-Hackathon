using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // In game sound manager
    public float volume;
    public float backgroundVolume;
    public AudioClip themeSong;
    public AudioClip bigJump;
    public AudioClip smallJump;
    public AudioClip death;
    public AudioClip getHit;
    public AudioClip getPowerUp;
    public AudioClip appearPowerUp;
    public AudioClip enemyStomp;
    public AudioClip breakBrick;
    public AudioClip reachFlag;
    private AudioSource audioSource;

    public void PlaySoundEffect(string soundEffect) {
        AudioClip soundEffectClip = null;
        switch (soundEffect)
        {
            case "bigJump":
                soundEffectClip = bigJump;
                break;
            case "smallJump":
                soundEffectClip = smallJump;
                break;
            case "death":
                soundEffectClip = death;
                break;
            case "getPowerUp":
                soundEffectClip = getPowerUp;
                break;
            case "appearPowerUp":
                soundEffectClip = appearPowerUp;
                break;
            case "getHit":
                soundEffectClip = getHit;
                break;
            case "reachFlag":
                soundEffectClip = reachFlag;
                break;
            case "enemyStomp":
                soundEffectClip = enemyStomp;
                break;
            case "breakBrick":
                soundEffectClip = breakBrick;
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
