using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBreads : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // inventory count ++

            // disappear
            Destroy(this.gameObject);
        }
    }
}
