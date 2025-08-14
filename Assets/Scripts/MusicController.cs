using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField] private List<AudioClip> audioClips;
    private AudioSource _audioSource;
    private int _songIndex;
    
    private static MusicController _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        PlaySong();
    }
    
    private void NextSong()
    {
        _songIndex++;
        
        if (_songIndex >= audioClips.Count)
            _songIndex = 0;
        
        PlaySong();
    }

    private void PlaySong()
    {
        _audioSource.clip = audioClips[_songIndex];
        _audioSource.Play();
        StartCoroutine(QueueNextSong());
    }

    private IEnumerator QueueNextSong()
    {
        yield return new WaitForSeconds(_audioSource.clip.length);
        NextSong();
    }
}
