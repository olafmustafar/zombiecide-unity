using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour{

    public Sound[] sounds;

    void Awake(){
        foreach( Sound s in sounds ){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.loop = s.loop;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
        }
    }

    void Start(){
        Play("Theme");
    }

    public void Play( string name ){
        Sound s = Array.Find( sounds, sound => sound.name == name );
        if( s == null ){
            Debug.LogWarning( $"Sound: {name} not found!");
        }
        s.source.Play();
    }

}
