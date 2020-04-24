using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Gotomenu : MonoBehaviour
{

    public AudioClip Clips;
    private AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void Menu()
    {
        audioSource.PlayOneShot(Clips);
        SceneManager.LoadScene("Start");
    }
}
