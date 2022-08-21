using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] clips;


    [Range(-1, 0)]
    public float alterationsToPitchLow;

    [Range(0,1)]
    public float alterationsToPitchHigh;



    [Range(-1,0)]
    public float alterationsToVolumeLow;

    [Range(0, 1)]
    public float alterationsToVolumeHigh;


    [SerializeField]
    private AudioSource audioSource;

    float originalPitch;
    float originalVolume;


    void Awake()
    {
       // audioSource = GetComponent<AudioSource>();
        originalPitch = audioSource.pitch;
        originalVolume = audioSource.volume;
    }

    // Update is called once per frame
    private void Step()
    {
        ResetAudioSource();

        AudioClip clip = GetRandomClip();
        audioSource.pitch += Random.Range(alterationsToPitchLow, alterationsToPitchHigh);
        audioSource.volume += Random.Range(alterationsToVolumeLow, alterationsToVolumeHigh);
        audioSource.clip = clip;
        audioSource.Play();


        
    }

    void ResetAudioSource()
    {
        audioSource.pitch = originalPitch;
        audioSource.volume = originalVolume;   
    }

    private AudioClip GetRandomClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }

}
