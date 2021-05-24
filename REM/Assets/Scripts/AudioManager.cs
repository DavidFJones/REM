using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer masterMixer;

    public void SetMasterVolume(float masterLvl) {
        masterMixer.SetFloat("masterVolume", masterLvl);
    }
    public void SetSFXVolume(float sfxLvl) {
        masterMixer.SetFloat("sfxVolume", sfxLvl);
    }
    public void SetMusicVolume(float musicLvl) {
        masterMixer.SetFloat("musicVolume", musicLvl);
    }
    public void SetdialogueVolume(float dialogueLvl) {
        masterMixer.SetFloat("dialogueVolume", dialogueLvl);
    }

    //Plays an audio clip at a given source
    public static void PlaySound(AudioSource source, AudioClip clip) {
        if (clip != null)
            source.clip = clip;

        source.Play();
    }
    //Stops audio at a given source
    public static void StopSound(AudioSource source) {
        source.Stop();
    }
    //Fades out an audio clip at a given source
    public static IEnumerator FadeOutSound(AudioSource source, AudioClip clip) {
        if (clip != null)
            source.clip = clip;

        float startVolume = source.volume;

        while (source.volume > 0) {
            source.volume -= startVolume * Time.deltaTime / 0.2f;

            yield return null;
        }

        source.Stop();
        source.volume = startVolume;
    }

}
