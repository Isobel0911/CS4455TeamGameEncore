using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEvent : MonoBehaviour
{
    public static GameEvent current;

    private void Awake()
    {
        current = this;
    }

    public event Action onDoorTriggerEnter;
    public void DoorTriggerEnter()
    {
        if (onDoorTriggerEnter != null)
        {
            onDoorTriggerEnter();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Triggered");
            GameEvent.current.DoorTriggerEnter();
        }
    }

}
