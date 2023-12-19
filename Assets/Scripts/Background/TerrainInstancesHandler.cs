using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.AI;

public class TerrainInstancesHandler : MonoBehaviour {
    // Player Transform
    public Transform playerTransform;

    // Terrains
    public Terrain[] terrains;

    // Terrain Trees
    public string[] treePrefabNames;
    public Mesh[] treeTrunkMeshes;
    public PhysicMaterial treeMaterial;
    // Tree Audio Settings
    public AudioClip[] treeAudioClips;
    public AudioMixerGroup treeAudioMixerGroup;

    // Terrain Rocks
    public string[] rockPrefabNames;
    public Mesh[] rockMeshes;
    public PhysicMaterial rockMaterial;

    private System.Random random;

    void Start() {
        random = new System.Random();
        foreach (Terrain terrain in terrains) {

            // Update the terrain's tree instances
            treeInstanceUpdate(terrain);
        }
        UnityEditor.AI.NavMeshBuilder.BuildNavMesh();
    }

    void treeInstanceUpdate(Terrain terrain) {

        // Get the current tree instances 
        TreeInstance[] treeInstances = terrain.terrainData.treeInstances;

        foreach (TreeInstance instance in treeInstances) {
            // Get the prefab for this tree instance
            TreePrototype prototype = terrain.terrainData.treePrototypes[instance.prototypeIndex];
            bool isChecked = false;

            // Check if the prefab name matches the name of a tree
            for (int i = 0; i < treePrefabNames.Length; i++) {
                if (prototype.prefab.name == treePrefabNames[i]) {
                    // Instantiate empty GameObject for tree collider and set position of tree collider game object
                    GameObject treeGameObject = new GameObject("TreeInstance_" + i + "_Collider");
                    treeGameObject.transform.position = Vector3.Scale(instance.position, terrain.terrainData.size) + terrain.transform.position;
                    Vector3 treeScale = new Vector3(instance.widthScale, instance.heightScale, instance.widthScale);
                    treeGameObject.transform.localScale = treeScale;

                    // Add an AudioSource component to treeGameObject
                    AudioSource treeAudioSource = treeGameObject.AddComponent<AudioSource>();
                    treeAudioSource.clip = treeAudioClips[random.Next(0, treeAudioClips.Length)];
                    treeAudioSource.outputAudioMixerGroup = treeAudioMixerGroup;
                    treeAudioSource.mute = false;
                    treeAudioSource.bypassEffects = false;
                    treeAudioSource.bypassListenerEffects = false;
                    treeAudioSource.bypassReverbZones = false;
                    treeAudioSource.loop = true;
                    treeAudioSource.volume = 0.3f;
                    treeAudioSource.panStereo = 0;
                    treeAudioSource.spatialBlend = 1;
                    treeAudioSource.reverbZoneMix = 0.3f;
                    // 3D Sound Settings
                    treeAudioSource.dopplerLevel = 1;
                    treeAudioSource.spread = 45;
                    treeAudioSource.rolloffMode = AudioRolloffMode.Linear;
                    treeAudioSource.minDistance = 0;
                    treeAudioSource.maxDistance = 75;
                    // Finally, add script to control playing of trees
                    treeAudioSource.enabled = false;
                    TreeSoundHandler treeSoundHandler = treeGameObject.AddComponent<TreeSoundHandler>();
                    // Assign the AudioSource and player Transform
                    treeSoundHandler.activationDistance = treeAudioSource.maxDistance;
                    treeSoundHandler.player = playerTransform;

                    // Add an AudioLowPassFilter component to treeGameObject
                    AudioLowPassFilter lowPassFilter = treeGameObject.AddComponent<AudioLowPassFilter>();
                    // Optionally configure the AudioLowPassFilter properties
                    lowPassFilter.cutoffFrequency = 8000;
                    lowPassFilter.lowpassResonanceQ = 1;

                    // Add mesh collider and set properties
                    MeshCollider treeTrunkMeshCollider = treeGameObject.AddComponent<MeshCollider>();
                    treeTrunkMeshCollider.convex = true;
                    // isTrigger property is false by default, so the following line is optional
                    // treeTrunkMeshCollider.isTrigger = false;
                    treeTrunkMeshCollider.sharedMaterial = treeMaterial;
                    treeTrunkMeshCollider.sharedMesh = treeTrunkMeshes[i];

                    isChecked = true;
                    break;
                }
            }

                
            // Check if the prefab name matches the name of a rock
            for (int i = 0; !isChecked && i < rockPrefabNames.Length; i++) {
                if (prototype.prefab.name == rockPrefabNames[i]){
                    // Instantiate rock GameObject
                    GameObject rockGameObject = new GameObject("RockInstance_" + i + "_Collider");
                    rockGameObject.transform.position = Vector3.Scale(instance.position, terrain.terrainData.size) + terrain.transform.position;
                    Vector3 rockScale = new Vector3(instance.widthScale, instance.heightScale, instance.widthScale);
                    rockGameObject.transform.localScale = rockScale;

                    // Add mesh collider and set properties
                    MeshCollider rockMeshCollider = rockGameObject.AddComponent<MeshCollider>();
                    rockMeshCollider.sharedMaterial = rockMaterial;
                    rockMeshCollider.sharedMesh = rockMeshes[i];
                    rockMeshCollider.convex = true;

                    // Exit the loop since we found the matching rock
                    isChecked = true;
                    break;
                }
            }
        }
    }
}
