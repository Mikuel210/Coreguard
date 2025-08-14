using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    
    [Space, SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    
    void Awake()
    {
        float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", -10f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0f);
        
        masterSlider.value = Map(masterVolume, -80f, 20f, 0f, 1f);
        musicSlider.value = Map(musicVolume, -80f, -10f, 0f, 1f);
        sfxSlider.value = Map(sfxVolume, -80f, 20f, 0f, 1f);
        
        UpdateVolume();
    }

    void Start()
    {
        masterSlider.onValueChanged.AddListener(_ => UpdateVolume());
        musicSlider.onValueChanged.AddListener(_ => UpdateVolume());
        sfxSlider.onValueChanged.AddListener(_ => UpdateVolume());
    }

    void UpdateVolume()
    {
        float masterVolume = Map(masterSlider.value, 0f, 1f, -80f, 20f);
        float musicVolume = Map(musicSlider.value, 0f, 1f, -80f, -10f);
        float sfxVolume = Map(sfxSlider.value, 0f, 1f, -80f, 20f);
        
        audioMixer.SetFloat("Master", masterVolume);
        audioMixer.SetFloat("Music", musicVolume);
        audioMixer.SetFloat("SFX", sfxVolume);
        
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    
    float Map(float x, float inMin, float inMax, float outMin, float outMax) {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
