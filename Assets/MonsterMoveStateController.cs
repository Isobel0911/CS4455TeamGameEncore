using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveStateController : MonoBehaviour
{
    Animator animator;
    public GameObject Protagonist;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Protagonist = GameObject.Find("Protagonist");
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
        Vector3 protagonistPosition = Protagonist.transform.position;
        Vector3 myPosition = transform.position;
        if ((myPosition - protagonistPosition).magnitude <= 5) {
            animator.SetInteger("MonsterMoveState", 1);
        }
        else {
            animator.SetInteger("MonsterMoveState", 0);
        }
    }
}
