using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour {
    
    public float currentAttack = 0f;
    private BoxCollider boxCollider;

    void Start() {
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.enabled = false;
    }

    public void enableAttack(float attack) {
        currentAttack = attack;
        boxCollider.enabled = true;
    }

    public void disableAttack() {
        boxCollider.enabled = false;
    }
}
