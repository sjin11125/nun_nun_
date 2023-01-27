using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    public ParticleSystem particleObj;

    AudioSource audioSource;
    public AudioSource audioSource2;
    public AudioSource audioSource3;

    private void Start()
    {
        audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    public void particlePlay() //animation add event�� ���
    {
        particleObj.Play();
        this.audioSource.Play();
    }

    public void audioPlay() //animation add event�� ���
    {
        audioSource2.Play();
    }

    public void audioPlay2() //animation add event�� ���
    {
        audioSource3.Play();
    }
}
