using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance
    {
        get { return instance;  }
    }

    [SerializeField] public AudioClip waterSplash;
    [SerializeField] public AudioClip rakeHit;
    [SerializeField] public AudioClip soilSplash;
    [SerializeField] public AudioClip itemObtain;
    [SerializeField] public AudioClip interfaceSound;
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlaySound(AudioClip _clip, Vector3 pos, float _volume = 1.0f, bool is3D = true, float _pitch = 1.0f)
    {
        if (_clip == null)
        {
            Debug.LogWarning("clip we are using is null !");
            return;
        }
        
        GameObject emptyObject = new GameObject();
        emptyObject.transform.position = pos;
        emptyObject.name = "SoundPlayingObject";
        
        AudioSource emptyAudioSource = emptyObject.AddComponent<AudioSource>();

        if (emptyObject == null)
        {
            Debug.LogWarning("Audio clip we are trying to play is null !");
            return;
        }
        
        //destroy in 10 seconds
        Destroy(emptyObject, 10.0f);
        
        if (emptyAudioSource == null)
        {
            Debug.LogWarning("audio source we are using is null !");
            return;
        }

        emptyAudioSource.clip = _clip;
        emptyAudioSource.volume = _volume;
        emptyAudioSource.pitch = _pitch;
        //3d
        if(is3D)
            emptyAudioSource.spatialBlend = 1.0f;
        
        //play clip
        if (emptyAudioSource.clip != null)
        {
            emptyAudioSource.Play();
        }
        else
        {
            Debug.LogWarning("Audio clip we are trying to play is null !");
        }
    }
}
