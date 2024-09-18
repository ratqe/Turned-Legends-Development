using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    private static MusicManager instance;
    private AudioClip originalSong; 
    private AudioSource audioSource;
    private bool hasOriginalSong = false;
    
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  
        }
        else if (instance != this)
        {
            Destroy(gameObject); 
        }


        audioSource = GetComponent<AudioSource>();
    }

    public float GetSongTime()
    {
        return audioSource.time;  // current time of the playing song
    }


    public void SetSongTime(float time)
    {
        audioSource.time = time;  //set the song time
    }

    public void ChangeSong(AudioClip newSong, bool shouldLoop = true)
    {

              
        if (!hasOriginalSong)
        {
            originalSong = audioSource.clip;
            hasOriginalSong = true;
        }

        if (audioSource.clip != newSong)
        {
            audioSource.clip = newSong;
            audioSource.loop = shouldLoop;
            audioSource.Play();
        }
    }

    public void RevertToOriginalSong()
    {
        if (originalSong != null && audioSource.clip != originalSong)
        {
            audioSource.clip = originalSong;
            audioSource.Play();
        }
    }


}
