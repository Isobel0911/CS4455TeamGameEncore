using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySounds : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip[] walkGrassSound, walkStoneSound, walkSandSound, walkWaterSound;
    public AudioClip[] runGrassSound,  runStoneSound,  runSandSound,  runWaterSound;
    public AudioClip[] jumpSound, jumpWaterSound;
    public AudioClip[] powerUpSound;
    public AudioClip[] blockSound;
    public AudioClip[] swordWieldingSound;
    public AudioClip[] bigSwordWieldingSound;
    public AudioClip[] yawnSound;
    public AudioClip[] kickSound;
    public AudioClip[] damagedSound;
    public AudioClip[] deathSound;
    private Animator animator;

    private System.Random rand = new System.Random();
    private int[] soundIdx=new int[]{0, 0, 0, 0, 0, 0, 0, 0, 0};

    void Start() {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    public void walk() {
        switch(animator.GetInteger("groundType")) {
            case 0: // stone road
            case 4: // stone road
                audioSource.PlayOneShot(walkStoneSound[(soundIdx[0]++)%walkStoneSound.Length]);
                break;
            case 1: // grass land
                audioSource.PlayOneShot(walkGrassSound[(soundIdx[1]++)%walkGrassSound.Length]);
                break;
            case 2: // sand land
                audioSource.PlayOneShot(walkSandSound[(soundIdx[2]++)%walkSandSound.Length]);
                break;
            case 3: // underwater
                audioSource.PlayOneShot(walkWaterSound[(soundIdx[3]++)%walkWaterSound.Length]);
                break;
        }
    }
    public void run() {
        switch(animator.GetInteger("groundType")) {
            case 0: // stone road
            case 4: // stone road
                audioSource.PlayOneShot(runStoneSound[(soundIdx[4]++)%runStoneSound.Length]);
                break;
            case 1: // grass land
                audioSource.PlayOneShot(runGrassSound[(soundIdx[5]++)%runGrassSound.Length]);
                break;
            case 2: // sand land
                audioSource.PlayOneShot(runSandSound[(soundIdx[6]++)%runSandSound.Length]);
                break;
            case 3: // underwater
                audioSource.PlayOneShot(runWaterSound[(soundIdx[7]++)%runWaterSound.Length]);
                break;
        }
    }
    public void jump() {
        switch(animator.GetInteger("groundType")) {
            case 0: // stone road
            case 1: // grass land
            case 2: // sand land
            case 4: // stone road
                audioSource.PlayOneShot(jumpSound[(soundIdx[8]++)%jumpSound.Length]);
                break;
            case 3: // underwater
                audioSource.PlayOneShot(jumpWaterSound[(soundIdx[9]++)%jumpWaterSound.Length]);
                break;
        }
    }

    public void powerUp() {audioSource.PlayOneShot(powerUpSound[rand.Next(0, powerUpSound.Length)]);}
    public void block() {audioSource.PlayOneShot(blockSound[rand.Next(0, blockSound.Length)]);}
    public void swordWielding() {audioSource.PlayOneShot(swordWieldingSound[rand.Next(0, swordWieldingSound.Length)]);}
    public void bigSwordWielding() {audioSource.PlayOneShot(bigSwordWieldingSound[rand.Next(0, bigSwordWieldingSound.Length)]);}
    public void yawn() {audioSource.PlayOneShot(yawnSound[rand.Next(0, yawnSound.Length)]);}
    public void kick() {audioSource.PlayOneShot(kickSound[rand.Next(0, kickSound.Length)]);}
    public void damaged() {audioSource.PlayOneShot(damagedSound[rand.Next(0, damagedSound.Length)]);}
    public void death() {audioSource.PlayOneShot(deathSound[rand.Next(0, deathSound.Length)]);}

    public void enableSword() {animator.SetBool("enableSwordCollider", true);}
    public void disableSword() {animator.SetBool("enableSwordCollider", false);}
}
