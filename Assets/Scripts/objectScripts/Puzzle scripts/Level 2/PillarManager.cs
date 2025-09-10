using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PillarManager : MonoBehaviour
{
    public static PillarManager current;
    public event Action startTrigger;

    [HideInInspector] public bool startPuzzle;


    void Awake() => current = this;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!startPuzzle && other.gameObject.CompareTag("Player"))
        {
            startPuzzle = true;
            startTrigger();
        }
    }
}
