using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
public class DynamicLowPass : MonoBehaviour {
    public AudioSource audioSource;
    public AudioLowPassFilter lowPassFilter;
    public Transform listener;
    private float maxCutoff = 16000; // Cutoff Frequency when near
    private float minCutoff = 8000;  // Cutoff Frequency when far

    void Start() {
        audioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
    }

    void Update() {
        updateLowPassFilter();
    }

    void FixedUpdate() {
        updateLowPassFilter();
    }

    void updateLowPassFilter() {
        // Get distance from current object to player's listener
        float distance = Vector3.Distance(listener.position, transform.position);

        // Calculate the Cutoff Frequency based on the distance and the minDistance and maxDistance of the AudioSource
        float normalizedDistance = (distance - audioSource.minDistance) / (audioSource.maxDistance - audioSource.minDistance);
        normalizedDistance = Mathf.Clamp01(normalizedDistance); // make sure it is between 0 and 1

        // Lerp implements interpolation from minCutoff to maxCutoff
        float cutoff = Mathf.Lerp(maxCutoff, minCutoff, normalizedDistance);
        lowPassFilter.cutoffFrequency = cutoff;
    }
}
