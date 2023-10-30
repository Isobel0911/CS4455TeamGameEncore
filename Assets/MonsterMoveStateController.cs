using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveStateController : MonoBehaviour
{

    /*
    0 -> idle
    1 -> attack 1
    2 -> attack 2
    3 -> die
    */

    Animator animator;
    public GameObject Protagonist;
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Protagonist = GameObject.Find("Protagonist");
        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead == true){
            return;
        }
        if (Input.GetKey("3")){
            animator.SetInteger("MonsterMoveState", 3);
            dead = true;
            return;
        }
        Vector3 protagonistPosition = Protagonist.transform.position;
        Vector3 myPosition = transform.position;
        if ((myPosition - protagonistPosition).magnitude <= 4) {
            animator.SetInteger("MonsterMoveState", 2);
        }
        else if ((myPosition - protagonistPosition).magnitude <= 12) {
            animator.SetInteger("MonsterMoveState", 1);
        }
        else {
            animator.SetInteger("MonsterMoveState", 0);
        }
    }
}
