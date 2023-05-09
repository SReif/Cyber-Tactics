using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayer : MonoBehaviour
{
    private AudioManager audioManager;
    private WinLoseManager winLoseManager;
    private bool winLoseAudioPlayed;

    void Awake()
    {
        winLoseAudioPlayed = false;
        audioManager = transform.GetComponent<AudioManager>();
        winLoseManager = GameObject.Find("SceneManager").GetComponent<WinLoseManager>();

        if (SceneManager.GetActiveScene().ToString() == "MainMenu")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Main Menu Theme");
        }

        if (SceneManager.GetActiveScene().ToString() == "Level0" || SceneManager.GetActiveScene().ToString() == "Level1")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Battle Theme - Gorandora");
        }

        if (SceneManager.GetActiveScene().ToString() == "Credits")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Credits Theme");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(winLoseManager.win && !winLoseAudioPlayed)
        {
            audioManager.StopAllAudio();
            audioManager.PlayOneShot("Win Theme");
            winLoseAudioPlayed = true;
        }

        if(winLoseManager.lose && !winLoseAudioPlayed)
        {
            audioManager.StopAllAudio();
            audioManager.PlayOneShot("Win Theme");
            winLoseAudioPlayed = true;
        }
    }
}
