using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicAttack : MonoBehaviour {
    private AttackController attackController;
    public float timeGap = 0f;
    private float lastAttackTime = 0;
    public float middleDisableRatio = 0f;
    // Start is called before the first frame update
    void Start() {
        attackController = GetComponent<AttackController>();
    }

    // Update is called once per frame
    void Update() {
        attackController.enableAttack(15);
    }
}
