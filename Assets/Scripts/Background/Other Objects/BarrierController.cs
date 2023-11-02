using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierController : MonoBehaviour
{
    public bool breadCollected = false;
    // Start is called before the first frame update
    void Start()
    {
        GameEvent.current.onDoorTriggerEnter += OnDoorwayOpen;
    }

    private void OnDoorwayOpen()
    {
        LeanTween.moveLocalY(gameObject, 1.6f, 4f).setEaseOutQuad();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Triggered");
            Collector collector = other.attachedRigidbody.gameObject.GetComponent<Collector>();
            if (collector.breadNum > 4)
            {
                GameEvent.current.DoorTriggerEnter();
            }
            
        }
    }
}
