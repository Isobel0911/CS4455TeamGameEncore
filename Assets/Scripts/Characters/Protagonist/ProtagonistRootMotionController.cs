using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProtagonistRootMotionController : MonoBehaviour {

    System.Random random = new System.Random();
    private Animator animator;

    private Dictionary<KeyCode, float> keyDownTime = new Dictionary<KeyCode, float>();
    private float timeNextIdleMotion = -0.1f;
    public float gravityScale; // Custom gravity scale
    public float DeltaDirectionX = 0.05f;
    public float DeltaDirectionY = 0.05f;

    // idle motion checker
    private bool idleMotionChecker = false;

    // jumping
    public float jumpForce;
    private Rigidbody rb;
    private bool isJumping = false;
    public float maxDistance;
    public Transform footTransform; 

    // attacking
    public GameObject sword;
    public GameObject shield;
    public GameObject mainCamera;
    public Image frontHealthBar, backHealthBar;
    public Image frontEnergyBar;
    private AttackController swordController;
    private ThirdPersonCamera cameraController;


    public int attackIdx = 0;

    // HealthBar Setting
    public float maxHealth = 100f;
    public float health = 100f;
    public float defense = 10f;
    public float attack = 5f;
    public float maxEnergy = 100f;
    public float energy = 100f;
    private float energyIncrease = 1f;
    private bool recoverRunState = false;
    public float lerpTimerHealth, lerpTimerEnergy;
    private float chipSpeed = 2f;
    // Damage from Colliders
    public float lastDamagedTime = 0f;
    public float damagedCooldown = 2f;


    private String[] attackableStates = {
        "Idle", "Idle_Motion_01", "Idle_Motion_02", "Idle_Motion_03",
        "Blend Tree - Run FWD", "Blend Tree - Run BWD",
        "Blend Tree - Walk FWD", "Blend Tree - Walk BWD",
        "Attack 01", "Attack 02",
        "Crouching",
        "Block_Hold", "Block_Hold (Crouch)"
    };
    private String[] resetAttackableStates = {
        "Idle", "Idle_Motion_01", "Idle_Motion_02", "Idle_Motion_03",
        "Blend Tree - Run FWD", "Blend Tree - Run BWD",
        "Blend Tree - Walk FWD", "Blend Tree - Walk BWD",
        "Crouching", "Block_Hold", "Block_Hold (Crouch)"
    };
    private String[] disableCameraRotateStates = {
        "Big Attack 01", "Big Attack 02"
    };
    private String[] runningStates = {
        "Blend Tree - Run FWD", "Blend Tree - Run BWD"
    };
    private String[] recoverableStates = {
        "Idle", "Idle_Motion_01", "Idle_Motion_02", "Idle_Motion_03",
        "Blend Tree - Walk FWD", "Blend Tree - Walk BWD", "Turn_Back (Walk)",
        "toCrouch", "Crouching", "endCrouchm",
        "Block_Raise", "Block_Hold", "Block_Black", "Block_Blocking",
        "Block_Raise (Crouch)", "Block_Hold (Crouch)", "Block_Black (Crouch)", "Block_Blocking (Crouch)"
    };
    private String[] blockingStates = {
        "Block_Blocking", "Block_Blocking (Crouch)"
    };
    private String[] damagingStates = {
        "Block_Blocking", "Block_Blocking (Crouch)", 
        "Damaged_Back", "Damaged_Front", "Damaged_Crouch"
    };
    private String[] biggestAttackAbleStates = {
        "Idle", "Idle_Motion_01", "Idle_Motion_02", "Idle_Motion_03",
        "Blend Tree - Run FWD", "Blend Tree - Run BWD", "Turn_Back (Run)",
        "Blend Tree - Walk FWD", "Blend Tree - Walk BWD", "Turn_Back (Walk)",
        "Crouching", "Block_Hold", "Block_Hold (Crouch)"
    };
    private String[] jumpableStates = {
        "Idle", "Idle_Motion_01", "Idle_Motion_02", "Idle_Motion_03",
        "Blend Tree - Run FWD",  "Blend Tree - Run BWD",  "Turn_Back (Run)",
        "Blend Tree - Walk FWD", "Blend Tree - Walk BWD", "Turn_Back (Walk)",
    };

    private String[] jumpingStates = {
        "Jump Up (Idle)", "Jump Up (Walk)", "Jump Up (Run)"
    };
    private String[] idleMotionStates = {
        "Idle_Motion_01", "Idle_Motion_02", "Idle_Motion_03"
    };

    /// --- Start/Update/FixedUpdate --- ///

    void Start() {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        animator.SetFloat("DirectionX", 0);
        animator.SetFloat("DirectionY", 0);
        animator.SetInteger("CastingIdx", 0);
        animator.SetBool("isIdle", true);
        animator.SetBool("isForward", false);
        animator.SetBool("isBackward", false);
        animator.SetBool("BigAttackIdx", false);
        animator.SetBool("isRun", false);
        animator.SetBool("isCrouch", false);
        animator.SetBool("isStrafe", false);
        animator.SetBool("isBlocking", false);
        animator.SetBool("isGrounded", true);
        animator.SetBool("LockJump", false);
        animator.SetBool("LockAttack", false);
        resetAllTriggers();
        keyDownTime.Add(KeyCode.W, 0.0f);
        keyDownTime.Add(KeyCode.S, 0.0f);
        keyDownTime.Add(KeyCode.A, 0.0f);
        keyDownTime.Add(KeyCode.D, 0.0f);

        Vector3 gravity = gravityScale * Physics.gravity;
        rb.AddForce(gravity, ForceMode.Acceleration);

        swordController = sword.GetComponent<AttackController>();
        cameraController = mainCamera.GetComponent<ThirdPersonCamera>();
        frontHealthBar.fillAmount = 1f; backHealthBar.fillAmount = 1f;
        frontEnergyBar.fillAmount = 1f;
    }

    void Update() {
        if (health > maxHealth) {health = maxHealth;}
        if (energy > maxEnergy) {energy = maxEnergy;}
        if (animator.IsInTransition(0)) {
            UpdateHealthUI();
            UpdateEnergyUI();
            return;
        }
        resetAllTriggers();
        energyIncrease = 0f;
        if (checkState(recoverableStates)) {
            energyIncrease = 0.2f;
            lerpTimerEnergy = 0f;
        }
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

        if (Input.GetKey(KeyCode.LeftShift)) {
            if (!recoverRunState) {
                if (energy <= 0.5f) {
                    recoverRunState = true;
                    animator.SetBool("isRun", false);
                } else animator.SetBool("isRun", true);
            } else {
                if (energy >= 10) {
                    recoverRunState=false;
                    animator.SetBool("isRun", true);
                } else {
                    animator.SetBool("isRun", false);
                }
            }
        } else {
            animator.SetBool("isRun", false);
            if (recoverRunState && energy >= 10) recoverRunState=false;
        }


        // Idle Motion Setting & Switching
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            if (Time.time > timeNextIdleMotion) {
                if (idleMotionChecker) {
                    animator.SetTrigger("IdleSwitch0"+random.Next(1, 4).ToString());
                } else {
                    timeNextIdleMotion = Time.time + ((float) (30 + random.NextDouble() * (60 - 30))); // after 30-60 sec
                    idleMotionChecker = true;
                }
            }
        } else {
            idleMotionChecker = false;
            animator.ResetTrigger("IdleSwitch01");
            animator.ResetTrigger("IdleSwitch02");
            animator.ResetTrigger("IdleSwitch03");
        }
        
        // Turn Back
        if (Input.GetKeyDown(KeyCode.Tab)   && !animator.GetBool("isIdle")     &&
            !animator.GetBool("isCrouch")   && !animator.GetBool("isStrafe")   &&
            !animator.GetBool("isBlocking") &&  animator.GetBool("isGrounded")) {
            animator.SetTrigger("Turn");
            timeNextIdleMotion = Time.time - 0.1f;
        }

        // calculate running energy
        if (animator.GetBool("isGrounded") && animator.GetBool("isRun") &&
            (animator.GetBool("isForward") || animator.GetBool("isBackward")) &&
            checkState(runningStates)) {
            energyIncrease = -0.5f;
            lerpTimerEnergy = 0f;
        }

        // Jump
        if (checkState(jumpableStates)) {
            if (!animator.GetBool("LockJump")) { // jump
                if (Input.GetKeyDown(KeyCode.Space) && energy >= 5f) {
                    animator.SetBool("LockJump", true);
                    animator.SetTrigger("Jump");
                    energyIncrease = -5f;
                    lerpTimerEnergy = 0f;
                }
            } else { // unlock attack trigger when jumpable
                animator.SetBool("LockJump", false);
            }
        }
        if (isJumping) {
             rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
             isJumping = false;
        }

        // Attack
        if (!animator.GetBool("LockAttack") && Input.GetMouseButtonDown(0) && checkState(attackableStates)) {
            animator.SetBool("LockAttack", true);
            animator.SetTrigger("Attack");
            if (random.Next(0, 2) == 1) animator.SetBool("BigAttackIdx", false);
            else animator.SetBool("BigAttackIdx", true);
            if (animator.GetBool("isGrounded") && animator.GetBool("isRun")) {
                energyIncrease = -5f;
                lerpTimerEnergy = 0f;
            }
        }
        if (checkState(disableCameraRotateStates)) { // camera rotate
            cameraController.disableCameraRotate();
        } else {
            cameraController.enableCameraRotate();
        }

        // Crouch
        if (Input.GetKey(KeyCode.LeftControl)) animator.SetBool("isCrouch", true);
        else animator.SetBool("isCrouch", false);

        // Block
        if (Input.GetMouseButton(1)) animator.SetBool("isBlocking", true);
        else animator.SetBool("isBlocking", false);

        // PowerUp
        if (Input.GetKey(KeyCode.R)) animator.SetTrigger("PowerUp");

        // Damaged
        if (Input.GetKey(KeyCode.E)) animator.SetTrigger("Damaged");

        if (Input.GetKeyDown(KeyCode.Q) && energy >= 30f && checkState(biggestAttackAbleStates)) { // biggest jump attack trigger
            animator.SetTrigger("BiggestAttack");
            energyIncrease = -30f;
            lerpTimerEnergy = 0f;
        }

        energy += energyIncrease; energy = Math.Max(0, energy);
        if (health > maxHealth) {health = maxHealth;} if (health == 0) {animator.SetTrigger("Death");}
        if (energy > maxEnergy) {energy = maxEnergy;}
        UpdateHealthUI();
        UpdateEnergyUI();
    }

    void FixedUpdate() {
        heightRayCasting();
    }

    /// --- local functions --- ///

    void resetAllTriggers() {
        if(checkState(idleMotionStates))
            animator.ResetTrigger("IdleSwitch01");
        animator.ResetTrigger("IdleSwitch02");
        animator.ResetTrigger("IdleSwitch03");
        animator.ResetTrigger("Turn");
        if (checkState(jumpingStates)) animator.ResetTrigger("Jump");
        animator.ResetTrigger("Casting");
        if (checkState(damagingStates)) animator.ResetTrigger("Damaged");
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Big Attack 03")) animator.ResetTrigger("BiggestAttack");
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("PowerUp")) animator.ResetTrigger("BiggestAttack");
        if (checkState(resetAttackableStates)) animator.SetBool("LockAttack", false);
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
        animator.SetBool("isBackward", !pressed);
        animator.SetBool("isIdle", false);
    }

    void pressAD(bool pressed) {
        updateDirectionX(pressed);
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
        // Emit a ray downward from the footTransform's position and only detect a specific LayerMask
        if (Physics.Raycast(footTransform.position, Vector3.down, out hit, Mathf.Infinity,  1 << 3)) {
            float distanceToGround = hit.distance;
            if (distanceToGround <= maxDistance) {
                animator.SetBool("isGrounded", true);
            } else {
                animator.SetBool("isGrounded", false);
            }
        } else {
            // If it does not collide with the Terrain, it is considered not to have touched the ground.
            animator.SetBool("isGrounded", true);
        }
    }

    private bool checkState(String[] checkList) {
        foreach (String currState in checkList) {
            if(animator.GetCurrentAnimatorStateInfo(0).IsName(currState)) return true;
        }
        return false;
    }
    public void lockAttack() {
        animator.SetBool("LockAttack", true);
    }

    /// --- global functions --- ///

    public void unlockAttack() {
        swordController.disableAttack();
        animator.SetBool("LockAttack", false);
    }

    public void Attack(int mode) {
        switch (mode) {
            case 1:  // attack 01
            case 2:  // attack 02
                swordController.enableAttack(attack);
                break;
            case 3:  // attack 03
                swordController.enableAttack(attack * 2f);
                break;
            case 4:  // big attack 01
            case 5:  // big attack 02
                swordController.enableAttack(attack * 1.5f);
                break;
            case 6:  // big attack 03
                swordController.enableAttack(attack * 2.5f);
                break;
            case 7:  // crouch attack
                swordController.enableAttack(attack * 0.8f);
                break;
        }
    }

    public void increaseHealth(float amount)  { maxHealth += amount; }
    public void increaseDefense(float amount) { defense += amount; }
    public void increaseAttack(float amount)  { attack += amount; }

    public void decreaseHealth(float amount)  { maxHealth = Math.Max(0, health - amount); }
    public void decreaseDefense(float amount) { defense = Math.Max(0, defense - amount); }
    public void decreaseAttack(float amount)  { attack = Math.Max(0, attack - amount); }

    public bool checkStateBlockingStates() {
        return checkState(blockingStates);
    }

    public void UpdateHealthUI() {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = health / maxHealth;
        if (fillB > hFraction) {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimerHealth += Time.deltaTime;
            float percentComplete = lerpTimerHealth / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction) {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimerHealth += Time.deltaTime;
            float percentComplete = lerpTimerHealth / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
    }

    public void UpdateEnergyUI() {
        frontEnergyBar.fillAmount = energy / maxEnergy;
    }
}
