using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBreads : MonoBehaviour
{
    private void OnCollision(Collision c)
    {
        // inventory breads ++
        
        Destroy(c.gameObject);
    }
}
