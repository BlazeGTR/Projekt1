using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Music_Start : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip Alarm;

    void Awake()
    {
        Invoke("StartAlarm", 5f);
    }

    void StartAlarm()
    {
        audioSource.PlayOneShot(Alarm, 0.5f);
    }

}
