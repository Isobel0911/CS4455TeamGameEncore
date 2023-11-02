using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFootstep : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip runSound;
    public AudioClip powerUpSound;
    public AudioClip blockSound;
    public AudioClip bladeWieldingSound;
    public AudioClip yawnSound;
    public AudioClip damagedSound;
    public AudioClip deathSound;
    public Animator animator;

    public void walk() {audioSource.PlayOneShot(walkSound);}
    public void run() {audioSource.PlayOneShot(runSound);}
    public void powerUp() {audioSource.PlayOneShot(powerUpSound);}
    public void block() {audioSource.PlayOneShot(blockSound);}
    public void bladeWielding() {audioSource.PlayOneShot(bladeWieldingSound);}
    public void yawn() {audioSource.PlayOneShot(yawnSound);}
    public void damaged() {audioSource.PlayOneShot(damagedSound);}
    public void death() {audioSource.PlayOneShot(deathSound);}
}
