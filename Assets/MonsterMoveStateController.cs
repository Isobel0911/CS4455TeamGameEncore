using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveStateController : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey("1")){
            animator.SetInteger("MonsterMoveState", 1);
        }

        if (Input.GetKey("0")){
            animator.SetInteger("MonsterMoveState", 0);
        }
    }
}
