using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    public static SoundManager Instance { get; private set; }
    
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioSource audioSource;

    public void PlayCorrectSound()
    {
        audioSource.PlayOneShot(correctSound);
    }

    public void PlayIncorrectSound()
    {
        audioSource.PlayOneShot(incorrectSound);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }

        audioSource = GetComponent<AudioSource>();
    }
}
