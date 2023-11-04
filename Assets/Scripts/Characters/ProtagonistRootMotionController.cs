using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class ProtagonistRootMotionController : MonoBehaviour {

    System.Random random = new System.Random();
    private Animator animator;

    private Dictionary<KeyCode, float> keyDownTime = new Dictionary<KeyCode, float>();
    private float timeNextIdleMotion = -0.1f;
    public float gravityScale; // Custom gravity scale
    public float DeltaDirectionX = 0.05f;
    public float DeltaDirectionY = 0.05f;

    // jumping
    public float jumpForce;
    private Rigidbody rb;
    private bool isJumping = false;
    public float maxDistance;
    public Transform footTransform; 

    public int heath;
    public int defence;
    public int attack;


    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.SetFloat("DirectionX", 0);
        animator.SetFloat("DirectionY", 0);
        animator.SetInteger("CastingIdx", 0);
        animator.SetInteger("AttackIdx", 0);
        animator.SetBool("isIdle", true);
        animator.SetBool("isForward", false);
        animator.SetBool("isBackward", false);
        animator.SetBool("isRun", false);
        animator.SetBool("isCrouch", false);
        animator.SetBool("isStrafe", false);
        animator.SetBool("isBlocking", false);
        animator.SetBool("isGrounded", true);
        animator.SetBool("LockMotion", false);
        resetAllTriggers();
        keyDownTime.Add(KeyCode.W, 0.0f);
        keyDownTime.Add(KeyCode.S, 0.0f);
        keyDownTime.Add(KeyCode.A, 0.0f);
        keyDownTime.Add(KeyCode.D, 0.0f);

        Vector3 gravity = gravityScale * Physics.gravity;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    void resetAllTriggers() {
        animator.ResetTrigger("IdleSwitch01");
        animator.ResetTrigger("IdleSwitch02");
        animator.ResetTrigger("IdleSwitch03");
        animator.ResetTrigger("Turn");
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Casting");
        animator.ResetTrigger("Damaged");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("PowerUp");
    }

    // Update is called once per frame
    void Update() {

        RecordKeyPressedTime();
        animator.SetBool("isIdle", true);
        resetAllTriggers();

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
            animator.SetFloat("DirectionY", 0);
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
        if (Input.GetKey(KeyCode.T)) animator.SetTrigger("Turn");


        // Idle Motion Setting & Switching
        if (!animator.GetBool("LockMotion")) {
            if ( animator.GetBool("isIdle")     && !animator.GetBool("isRun")      &&
                !animator.GetBool("isCrouch")   && !animator.GetBool("isStrafe")   &&
                !animator.GetBool("isBlocking") &&  animator.GetBool("isGrounded") &&
                !animator.GetBool("isForward")  && !animator.GetBool("isBackward") &&
                 animator.GetFloat("DirectionX") == 0) {
                if (timeNextIdleMotion < Time.time) {
                    animator.SetBool("LockMotion", true);
                    timeNextIdleMotion = Time.time + ((float) (10 + random.NextDouble() * (15 - 10))); // after 10-15 sec
                }
            } else {
                timeNextIdleMotion = Time.time - 0.1f;
                animator.SetBool("LockMotion", false);
            }
        } else {
            if (timeNextIdleMotion == Time.time || timeNextIdleMotion < Time.time + 0.3f) {
                animator.SetTrigger("IdleSwitch0"+random.Next(1, 4).ToString());
                timeNextIdleMotion = Time.time + 5f;
                animator.SetBool("LockMotion", false);
            }
        }
        
        // Turn Back
        if (Input.GetKeyDown(KeyCode.Tab)   && !animator.GetBool("isIdle")     &&
            !animator.GetBool("isCrouch")   && !animator.GetBool("isStrafe")   &&
            !animator.GetBool("isBlocking") &&  animator.GetBool("isGrounded")) {
            animator.SetTrigger("Turn");
            animator.SetBool("LockMotion", false);
            timeNextIdleMotion = Time.time - 0.1f;
        }

        // Jump
        if (animator.GetBool("isGrounded") && !isJumping && Input.GetKeyDown(KeyCode.K)) {
            animator.SetTrigger("Jump");
            animator.SetBool("LockMotion", false);
        }

        if (isJumping) {
             rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
             isJumping = false;
        }
    }

    void FixedUpdate() {
        heightRayCasting();
    }

    void RecordKeyPressedTime() {
        if (Input.GetKeyDown(KeyCode.W)) keyDownTime[KeyCode.W] = Time.time;
        if (Input.GetKeyDown(KeyCode.S)) keyDownTime[KeyCode.S] = Time.time;
        if (Input.GetKeyDown(KeyCode.A)) keyDownTime[KeyCode.A] = Time.time;
        if (Input.GetKeyDown(KeyCode.D)) keyDownTime[KeyCode.D] = Time.time;
    }

    void pressWS(bool pressed) {
        updateDirectionY(pressed);
        animator.SetBool("isForward", pressed);
        animator.SetBool("LockMotion", false);
        animator.SetBool("isBackward", !pressed);
        animator.SetBool("isIdle", false);
    }

    void pressAD(bool pressed) {
        updateDirectionX(pressed);
        animator.SetBool("LockMotion", false);
        animator.SetBool("isIdle", false);
    }

    void updateDirectionX(bool left) {
        if (left && animator.GetFloat("DirectionX") > -1) animator.SetFloat("DirectionX", Math.Max(animator.GetFloat("DirectionX") - DeltaDirectionX, -1));
        if (!left && animator.GetFloat("DirectionX") < 1) animator.SetFloat("DirectionX", Math.Min(animator.GetFloat("DirectionX") + DeltaDirectionX, 1));
    }

    void updateDirectionY(bool forward) {
        if (!forward && animator.GetFloat("DirectionY") > -1) animator.SetFloat("DirectionY", Math.Max(animator.GetFloat("DirectionY") - DeltaDirectionY, -1));
        if ( forward && animator.GetFloat("DirectionY") < 1) animator.SetFloat("DirectionY", Math.Min(animator.GetFloat("DirectionY") + DeltaDirectionY, 1));
    }

    void OnCollisionStay(Collision collision) {
        // Initialize isGrounded to false. It will become true if we find a terrain collision.
        bool isGrounded = false;

        // Go through all the contact points in the collision
        foreach (ContactPoint contact in collision.contacts) {
            // Check if the layer of the object we're colliding with is "Terrain"
            if (contact.otherCollider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
                // If it is terrain, set isGrounded to true and stop checking other contacts
                isGrounded = true;
                break; // No need to check further contacts since we found a terrain layer
            }
        }

        // Set the animator's isGrounded boolean to true if we're on terrain
        if (isGrounded) {
            animator.SetBool("isGrounded", true);
        } else {
            heightRayCasting();
        }
    }

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
            heightRayCasting();
        }
    }

    void jump() {
        isJumping = true;
    }

    void heightRayCasting() {
        RaycastHit hit;
        if (Physics.Raycast(footTransform.position, Vector3.down, out hit)) {
            float distanceToGround = hit.distance;
            animator.SetFloat("Test", distanceToGround);
            if (distanceToGround <= maxDistance) {
                animator.SetBool("isGrounded", true);
            } else {
                animator.SetBool("isGrounded", false);
            }
        } else {
            animator.SetFloat("Test", 999);
            animator.SetBool("isGrounded", true);
        }
    }
}
