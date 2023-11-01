using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBreads : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        // inventory breads ++
        Collector collector = c.attachedRigidbody.gameObject.GetComponent<Collector>();
        collector.ReceiveBread();

        Destroy(gameObject);
    }
}
