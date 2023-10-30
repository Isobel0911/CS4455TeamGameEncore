using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeColliderGenerator : MonoBehaviour {
    public Terrain[] terrains;
    public GameObject[] treeColliderPrefabs;

    void Start() {
        foreach (Terrain terrain in terrains) {
            TreeInstance[] trees = terrain.terrainData.treeInstances;
            foreach (var tree in trees) {
                int treeType = tree.prototypeIndex; // gain prototypeIndex of each tree
                Vector3 treeWorldPos = Vector3.Scale(tree.position, terrain.terrainData.size) + terrain.transform.position;

                if (treeType >= 0 && treeType < treeColliderPrefabs.Length)
                {
                    GameObject treeColliderPrefab = treeColliderPrefabs[treeType];
                    GameObject treeCollider = Instantiate(treeColliderPrefab, treeWorldPos, Quaternion.identity);
                    treeCollider.layer = LayerMask.NameToLayer("Trees");
                }
            }
        }
    }
}
