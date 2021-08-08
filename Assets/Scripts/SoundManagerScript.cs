using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip hitSound;
    static AudioSource audioSrc;

    private void Start()
    {
        hitSound = Resources.Load<AudioClip>("HitSFX");

        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Hit":
                audioSrc.PlayOneShot(hitSound);
                break;
        }

    }
}
