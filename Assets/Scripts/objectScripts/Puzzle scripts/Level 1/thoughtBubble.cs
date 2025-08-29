using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;
using Unity.VisualScripting;
using Unity.Properties;
using UnityEngine.Analytics;
using System.Net.NetworkInformation;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(TrackKeyOrder), typeof(BoxCollider2D))]
public class ThoughtBubble : MonoBehaviour
{
    [SerializeField] private GameObject thoughtBubble;
    public static event Action<bool> triggerThoughtBubble;
    private PlayerManager playerManager;

    [SerializeField] private float maxTime;
    [SerializeField] private Vector2 positionOffset;
    [SerializeField] private UnityEvent thoughtBubbleEvent;
    private Coroutine thoughtTrigger;
    private bool followPlayer;

    private TrackKeyOrder keyOrder;
    private bool inProgress;


    // Start is called before the first frame update
    void Start()
    {
        keyOrder = GetComponent<TrackKeyOrder>();
        if (thoughtBubble != null) { thoughtBubble.SetActive(false); }
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();

        if (thoughtBubbleEvent == null) { thoughtBubbleEvent = new UnityEvent(); }

    }

    private void Update()
    {
        if (followPlayer)
        {
            thoughtBubble.transform.position = playerManager.transform.position;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            if (!keyOrder.completed)
            {
                if (!inProgress)
                {
                    keyOrder.StartTrackingKeys();
                    inProgress = true;
                }
                TriggerThought(collision);
            }
        }
    }

    private void TriggerThought(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (thoughtTrigger == null && !keyOrder.completed) { thoughtTrigger = StartCoroutine(TriggerThoughtEnum()); }
        }
    }


    private IEnumerator TriggerThoughtEnum()
    {
        yield return new WaitForSecondsRealtime(maxTime);
        if (!keyOrder.completed)
        {
            followPlayer = true;
            if (thoughtBubble != null) { thoughtBubble.SetActive(true); }
            thoughtBubbleEvent?.Invoke();
            yield return new WaitUntil(() => keyOrder.completed);
        }
        followPlayer = false;
        if (thoughtBubble != null) { thoughtBubble.SetActive(false); }
    }

}
