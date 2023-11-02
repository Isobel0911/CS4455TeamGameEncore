using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Terrains {
    Stone,
    Grass,
    Sand
}

[System.Serializable]
public class GroundType {
    public Terrains terrainType;
    public TerrainLayer terrainLayer;
}

[System.Serializable]
public class TerrainLayerData {
    public Terrain terrain;
    public GroundType[] groundTypes;
}

public class GroundDetector : MonoBehaviour {
    public TerrainLayerData[] terrainLayerDataArray;
    private Animator animator;
    public Transform waterPlane;
    private bool outWater = true;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (animator.GetInteger("groundType") == 4) return;
        Vector3 playerPosition = transform.position;
        foreach (var terrainLayerData in terrainLayerDataArray) {
            TerrainData terrainData = terrainLayerData.terrain.terrainData;
            Vector3 terrainPosition = terrainLayerData.terrain.transform.position;

            int mapX = (int)(((playerPosition.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
            int mapZ = (int)(((playerPosition.z - terrainPosition.z) / terrainData.size.z) * terrainData.alphamapHeight);

            float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);

            bool broker=false;
            for (int i = 0; i < terrainLayerData.groundTypes.Length; i++) {
                var groundType = terrainLayerData.groundTypes[i];
                if (splatmapData[0, 0, i] > 0.5f) {
                    animator.SetInteger("groundType", (int)groundType.terrainType);
                    broker=true;
                    break;
                }
            }
            if (broker) break;
        }

        GameObject[] feet = GameObject.FindGameObjectsWithTag("PlayerFeet");
        foreach(GameObject foot in feet) {
            if (foot.transform.position.y < waterPlane.position.y) {
                animator.SetInteger("groundType", 3);
            }
        }

        bool check = true;
        foreach(GameObject foot in feet) {
            if (!outWater && foot.transform.position.y > waterPlane.position.y || 
                 outWater && foot.transform.position.y < waterPlane.position.y) {
                animator.SetInteger("groundType", 3);
            }
            check = check && (foot.transform.position.y > waterPlane.position.y);
        }
        outWater = check;
    }

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("Bridges")) animator.SetInteger("groundType", 4);
    }
    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Bridges")) animator.SetInteger("groundType", 0);
    }
}