using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioPlayer : MonoBehaviour
{
    private AudioManager audioManager;
    private WinLoseManager winLoseManager;
    private bool winLoseAudioPlayed;

    void Start()
    {
        winLoseAudioPlayed = false;
        audioManager = transform.GetComponent<AudioManager>();
        winLoseManager = GameObject.Find("SceneManager").GetComponent<WinLoseManager>();

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Main Menu Theme");
        }

        if(SceneManager.GetActiveScene().name == "Prologue")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Cutscene Theme");
        }

        if (SceneManager.GetActiveScene().name == "Level0" || SceneManager.GetActiveScene().name == "Level1")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Battle Theme - Gorandora");
        }

        if (SceneManager.GetActiveScene().name == "Credits")
        {
            audioManager.StopAllAudio();
            audioManager.Play("Credits Theme");
        }

        Debug.Log("am playing" + SceneManager.GetActiveScene().name);
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
