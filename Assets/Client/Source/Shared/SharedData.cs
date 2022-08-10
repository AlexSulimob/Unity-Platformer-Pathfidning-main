using System.Collections;
using System.Collections.Generic;
using Client;
using UnityEngine;
using Unity.Mathematics;

public class SharedData : MonoBehaviour
{
    public PlayerSettings playerSettings;
    [HideInInspector]
    public Graph graphLevel;
    // public string graphName;
    public TextAsset graphJson;
    private void Awake() {
        graphLevel = SaveAndLoadGraph.Load(graphJson);
        // foreach (var item in graphLevel.Nodes)
        // {
        //     Debug.DrawRay(item.Value.pos.ToVector2(), Vector3.up, Color.cyan, 100f);
        // }
        // var round = new float2(2.346f, -19.6332f);
        // Debug.Log((int2)round);
    }
    private void OnDestroy() {
        graphLevel.DisposeContainer();
    }
}
