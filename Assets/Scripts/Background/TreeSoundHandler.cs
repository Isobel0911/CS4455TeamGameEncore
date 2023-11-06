using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSoundHandler : MonoBehaviour {
    public Transform player;
    public float activationDistance;
    private AudioSource treeAudioSource;
    private AudioLowPassFilter lowPassFilter;
    private float maxCutoff = 16000f; // Maximum cutoff frequency of LowPassFilter
    private float minCutoff = 8000f;  // Minimum cutoff frequency of LowPassFilter
    private System.Random random;

    void Start() {
        random = new System.Random();
        treeAudioSource = GetComponent<AudioSource>();
        lowPassFilter = GetComponent<AudioLowPassFilter>();
        detectDistance();
    }

    // Update is called once per frame
    void Update() {
        detectDistance();
    }

    void detectDistance() {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= activationDistance) {
            // Interpolate the cutoff frequency between 8k and 16k based on proximity
            // Calculate and apply interpolation factor (0 at max distance, 1 at zero distance)
            lowPassFilter.cutoffFrequency = Mathf.Lerp(minCutoff, maxCutoff, 1 - (distance / activationDistance));
            if (!treeAudioSource.isPlaying){
                // Random start time
                float clipLength = treeAudioSource.clip.length;
                float randomStartPercentage = (float)random.NextDouble();
                float randomStartTime = clipLength * randomStartPercentage;
                treeAudioSource.time = randomStartTime;
                treeAudioSource.enabled = true;
                treeAudioSource.Play();
            }
        } else if (distance > activationDistance && treeAudioSource.isPlaying) {
            treeAudioSource.Stop();
            treeAudioSource.enabled = false;
        }
    }
}
