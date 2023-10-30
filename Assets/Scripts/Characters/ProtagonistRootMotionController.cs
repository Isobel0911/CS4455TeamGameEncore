using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtagonistRootMotionController : MonoBehaviour {

    System.Random random = new System.Random();
    private Animator animator;

    private Dictionary<KeyCode, float> keyDownTime = new Dictionary<KeyCode, float>();
    private float timeNextIdleMotion = -0.1f;
    public float DeltaDirectionX = 0.01f;


    // Start is called before the first frame update
    void Start() {
        animator = GetComponent<Animator>();
        animator.SetFloat("DirectionX", 0);
        animator.SetInteger("CastingIdx", 0);
        animator.SetInteger("AttackIdx", 0);
        animator.ResetTrigger("IdleSwitch01");
        animator.ResetTrigger("IdleSwitch02");
        animator.ResetTrigger("IdleSwitch03");
        animator.ResetTrigger("Turn");
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Casting");
        animator.ResetTrigger("Damaged");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("PowerUp");
        animator.SetBool("isIdle", true);
        animator.SetBool("isForward", false);
        animator.SetBool("isBackward", false);
        animator.SetBool("isRun", false);
        animator.SetBool("isCrouch", false);
        animator.SetBool("isStrafe", false);
        animator.SetBool("isBlocking", false);
        animator.SetBool("isGrounded", true);
        animator.SetBool("LockMotion", false);
        keyDownTime.Add(KeyCode.W, 0.0f);
        keyDownTime.Add(KeyCode.S, 0.0f);
        keyDownTime.Add(KeyCode.A, 0.0f);
        keyDownTime.Add(KeyCode.D, 0.0f);
    }

    // Update is called once per frame
    void Update() {
        RecordKeyPressedTime();
        animator.SetBool("isIdle", true);

        // Key: Forward/Backward
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S)) {
            bool check = keyDownTime[KeyCode.W] >= keyDownTime[KeyCode.W];
            pressWS(check);
        }
        else if (Input.GetKey(KeyCode.W)) pressWS(true);
        else if (Input.GetKey(KeyCode.S)) pressWS(false);
        else {
            animator.SetBool("isForward", false);
            animator.SetBool("isBackward", false);
        }

        // Key: Left/Right
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D)) {
            bool check = keyDownTime[KeyCode.A] >= keyDownTime[KeyCode.D];
            pressAD(check);
        }
        else if (Input.GetKey(KeyCode.A)) pressAD(true);
        else if (Input.GetKey(KeyCode.D)) pressAD(false);
        else animator.SetFloat("DirectionX", 0);

        animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));
        animator.SetBool("isRun", Input.GetKey(KeyCode.LeftShift));

        // Check status of special-motion locker
        if (!animator.GetBool("LockMotion")) {
            
            // Idle Motion Setting & Switching
            if ( animator.GetBool("isIdle")     && !animator.GetBool("isRun")      &&
                !animator.GetBool("isCrouch")   && !animator.GetBool("isStrafe")   &&
                !animator.GetBool("isBlocking") &&  animator.GetBool("isGrounded") &&
                !animator.GetBool("isForward")  && !animator.GetBool("isBackward") &&
                 animator.GetFloat("DirectionX") == 0)
                if (timeNextIdleMotion < Time.time)
                    timeNextIdleMotion = Time.time + 1000 * ((float) (4 + random.NextDouble() * (7 - 4))); // after 4-7 sec
                else if (timeNextIdleMotion == Time.time) {
                    animator.SetBool("LockMotion", true);
                    animator.SetTrigger("IdleSwitch0"+random.Next(1, 4).ToString());
                }
            else timeNextIdleMotion = Time.time - 0.1f;
            
            // Turn Back
            if (Input.GetKey(KeyCode.Tab)       && !animator.GetBool("isIdle")     &&
                !animator.GetBool("isCrouch")   && !animator.GetBool("isStrafe")   &&
                !animator.GetBool("isBlocking") &&  animator.GetBool("isGrounded") &&
                !animator.GetBool("LockMotion")) {
                animator.SetBool("LockMotion", true);
                animator.SetTrigger("Turn");
                timeNextIdleMotion = Time.time - 0.1f;
            }
        }
    }

    void RecordKeyPressedTime() {
        if (Input.GetKeyDown(KeyCode.W)) keyDownTime[KeyCode.W] = Time.time;
        if (Input.GetKeyDown(KeyCode.S)) keyDownTime[KeyCode.S] = Time.time;
        if (Input.GetKeyDown(KeyCode.A)) keyDownTime[KeyCode.A] = Time.time;
        if (Input.GetKeyDown(KeyCode.D)) keyDownTime[KeyCode.D] = Time.time;
    }

    void pressWS(bool pressed) {
        animator.SetBool("isForward", pressed);
        animator.SetBool("isBackward", !pressed);
        animator.SetBool("isIdle", false);
    }

    void pressAD(bool pressed) {
        updateDirectionX(pressed);
        animator.SetBool("isIdle", false);
    }

    void updateDirectionX(bool left) {
        if (left && animator.GetFloat("DirectionX") > -1) animator.SetFloat("DirectionX", animator.GetFloat("DirectionX") - DeltaDirectionX);
        if (!left && animator.GetFloat("DirectionX") < 1) animator.SetFloat("DirectionX", animator.GetFloat("DirectionX") + DeltaDirectionX);
    }

    void OnCollisionEnter(Collision collision) {
        animator.SetBool("isGrounded", collision.gameObject.layer == LayerMask.NameToLayer("Terrain"));
    }
}
