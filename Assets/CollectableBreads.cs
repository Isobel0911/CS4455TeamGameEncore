using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBreads : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        // inventory breads ++


        Destroy(gameObject);
    }
}
