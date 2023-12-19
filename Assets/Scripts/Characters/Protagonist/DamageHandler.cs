using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DamageHandler : MonoBehaviour {
    public GameObject person;
    private ProtagonistRootMotionController controller;
    public bool front = false;
    private Animator animator;

    void Start() {
        controller = person.GetComponent<ProtagonistRootMotionController>();
        front = this.gameObject.CompareTag("PlayerFront");
        animator = person.GetComponent<Animator>();
    }

    void OnCollisionStay(Collision  collision) {
        // ensure enough cooldown time for damaged
        Debug.Log($"Time difference: {Time.time - controller.lastDamagedTime}");
        if (Time.time - controller.lastDamagedTime >= controller.damagedCooldown) {
            // get component from damaged
            AttackController attackController = collision.gameObject.GetComponent<AttackController>();
            
            // ensure the object have AttackController component
            if (attackController != null) {
                // handle collision of daamaged
                float attackPower = attackController.currentAttack;

                Debug.Log($"Damaged: {attackPower}\nCollision Name: {this.gameObject.name}\nisFront: {front}");
                animator.SetBool("Front", front);
                
                if (controller.checkStateBlockingStates()) { // block damage
                    Debug.Log($"Blocked");
                    animator.SetTrigger("Damaged");
                } else {
                    controller.health = Math.Max(0, controller.health - Math.Max(0, attackPower - controller.defense));
                    controller.lerpTimerHealth = 0f;
                    if (controller.health == 0) {
                        Debug.Log($"Health_Reduce: {Math.Max(0, attackPower - controller.defense)}, CurrHealth: {controller.health}, Death");
                        animator.SetTrigger("Death"); // died if no health
                    } else {
                        Debug.Log($"Health_Reduce: {Math.Max(0, attackPower - controller.defense)}, CurrHealth: {controller.health}, Damaged");
                        animator.SetTrigger("Damaged"); // Damaging
                    }
                }

                // update last damaged time
                controller.lastDamagedTime = Time.time;
            }
        }
    }

    void OnTriggerStay(Collider other) {
        // ensure enough cooldown time for damaged
        if (Time.time - controller.lastDamagedTime >= controller.damagedCooldown) {
            // get component from damaged
            AttackController attackController = other.gameObject.GetComponent<AttackController>();
            
            // ensure the object have AttackController component
            if (attackController != null) {
                // handle collider of daamaged
                float attackPower = attackController.currentAttack;

                Debug.Log($"Damaged: {attackPower}\nCollider Name: {this.gameObject.name}\nisFront: {front}");
                animator.SetBool("Front", front);
                
                if (controller.checkStateBlockingStates()) { // block damage
                    Debug.Log($"Blocked");
                    animator.SetTrigger("Damaged");
                } else {
                    controller.health = Math.Max(0, controller.health - Math.Max(0, attackPower - controller.defense));
                    controller.lerpTimerHealth = 0f;
                    if (controller.health == 0) {
                        Debug.Log($"Health_Reduce: {Math.Max(0, attackPower - controller.defense)}, CurrHealth: {controller.health}, Death");
                        animator.SetTrigger("Death"); // died if no health
                    } else {
                        Debug.Log($"Health_Reduce: {Math.Max(0, attackPower - controller.defense)}, CurrHealth: {controller.health}, Damaged");
                        animator.SetTrigger("Damaged"); // Damaging
                    }
                }

                // update last damaged time
                controller.lastDamagedTime = Time.time;
            }
        }
    }
}
