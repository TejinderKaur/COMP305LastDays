using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;					//Drag a reference to the audio source which will play the sound effects.
    public AudioSource musicSource;					//Drag a reference to the audio source which will play the music.
    public static SoundManager instance = null;		//Allows other scripts to call functions from SoundManager.				
    public float lowPitchRange = .95f;				//The lowest a sound effect will be randomly pitched.
    public float highPitchRange = 1.05f;			//The highest a sound effect will be randomly pitched.
    
    
    public AudioClip scavengers_chop1;

    public AudioClip scavengers_die;
    public AudioClip scavengers_enemy1;
    public AudioClip    scavengers_enemy2;
    public AudioClip scavengers_footstep2;
    public AudioClip scavengers_fruit1;
    public AudioClip scavengers_fruit2;
    public AudioClip scavengers_soda1;
    public AudioClip scavengers_soda2;

    void Awake ()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy (gameObject);
        
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad (gameObject);
    }
    
    
    //Used to play single sound clips.
    public void PlaySingle(AudioClip clip)
    {
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        efxSource.clip = clip;
        
        //Play the clip.
        efxSource.Play ();
    }
    

    public void PlayChop() {
        PlaySingle(scavengers_chop1);
    }
    public void PlayDie() {
        PlaySingle(scavengers_die);
    }
    public void PlayEnemy1() {
        PlaySingle(scavengers_enemy1);        
    }
    public void PlayEnemy2() {
        PlaySingle(scavengers_enemy2); 
    }

    public void PlayWalk() {
        PlaySingle(scavengers_footstep2);
    }
    
    public void PlayGetFruit() {
        PlaySingle(scavengers_fruit1);
    }    

    public void PlayUseFruit() {
        PlaySingle(scavengers_fruit2);
    }    
    public void PlayGetSoda() {
            PlaySingle(scavengers_soda1);
    }
    public void PlayUseSoda() {
        PlaySingle(scavengers_soda2);
    }
    
    //RandomizeSfx chooses randomly between various audio clips and slightly changes their pitch.
    public void RandomizeSfx (params AudioClip[] clips)
    {
        //Generate a random number between 0 and the length of our array of clips passed in.
        int randomIndex = Random.Range(0, clips.Length);
        
        //Choose a random pitch to play back our clip at between our high and low pitch ranges.
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);
        
        //Set the pitch of the audio source to the randomly chosen pitch.
        efxSource.pitch = randomPitch;
        
        //Set the clip to the clip at our randomly chosen index.
        efxSource.clip = clips[randomIndex];
        
        //Play the clip.
        efxSource.Play();
    }
}
