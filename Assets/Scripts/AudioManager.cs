using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;  // Assign in inspector
    public AudioClip menuMusic;      // Assign in inspector
    public float volume = 1f;        // Default volume, adjustable in inspector

    void Start()
    {
        PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        musicSource.volume = volume;  // Set the volume
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void SetVolume(float vol)
    {
        volume = vol;
        musicSource.volume = volume;  // Update the volume
    }
}
