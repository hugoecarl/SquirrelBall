using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosao : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator animator;
    public bool Atingiu;
    public AudioClip exp;
    private AudioSource audioSource;

    void Start()
    {
        animator = GetComponent<Animator>();
        Atingiu = false;
        audioSource = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Atingiu", Atingiu);
        if (Atingiu)
        {
            audioSource.PlayOneShot(exp);
            Atingiu = false;
        }
        }
}
