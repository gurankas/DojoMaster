using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public static AudioClip hitSound;
    public static AudioClip backGroundSound;
    public static AudioClip dashSound;
    public static AudioClip swordThrowSound;
    // public static AudioClip footStepSound;


    static AudioSource audioSrc;

    private void Start()
    {
        hitSound = Resources.Load<AudioClip>("HitSFX");
        backGroundSound = Resources.Load<AudioClip>("BackGroundMusic");
        swordThrowSound = Resources.Load<AudioClip>("SwordThrow");
        dashSound = Resources.Load<AudioClip>("DashSound");


        audioSrc = GetComponent<AudioSource>();
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Hit":
                audioSrc.PlayOneShot(hitSound);
                break;
            case "BackGroundMusic":
                audioSrc.PlayOneShot(backGroundSound);
                break;
            case "DashSound":
                audioSrc.PlayOneShot(dashSound);
                break;
            case "SwordThrow":
                audioSrc.PlayOneShot(swordThrowSound);
                break;
                // case "FootStepSound":
                //     audioSrc.PlayOneShot(footStepSound);
                //     break;
        }

    }

    // public static void StopSound(string clip)
    // {
    //     switch (clip)
    //     {
    //         case "BackGroundMusic":
    //             audioSrc.Stop();
    //             break;
    //             // case "FootStepSound":
    //             //     audioSrc.PlayOneShot(footStepSound);
    //             //     break;
    //     }
    // }
}
