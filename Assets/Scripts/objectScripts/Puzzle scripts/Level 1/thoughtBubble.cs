using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

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

    [SerializeField] private Animation anim;
    private Collider2D col;
    [SerializeField] private Vector2 colSize, colOffset;
    private float angle;


    // Start is called before the first frame update
    void Start()
    {
        colSize = GetComponent<BoxCollider2D>().size;
        colOffset = GetComponent<BoxCollider2D>().offset;
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

    void FixedUpdate()
    {
        col = Physics2D.OverlapBox(transform.position + (Vector3)colOffset, colSize, angle, LayerMask.GetMask("Player"));
        if (col)
        {
            if (!keyOrder.completed)
            {
                if (!inProgress)
                {
                    keyOrder.StartTrackingKeys();
                    inProgress = true;
                }
                TriggerThought(col);
            }
        }
        else
        {
            if (keyOrder.GetStopWhenOutofBounds())
            {
                keyOrder.StopTrackingKeys(TurnOffThoughtBubble);
                if (thoughtTrigger != null)
                {
                    StopCoroutine(thoughtTrigger);
                }
                thoughtTrigger = null;
                inProgress = false;
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
            thoughtBubble.transform.position = playerManager.transform.position;
            if (thoughtBubble != null) { thoughtBubble.SetActive(true); }
            anim.Play("FadeInObj");
            thoughtBubbleEvent?.Invoke();
            yield return new WaitUntil(() => keyOrder.completed);
            anim.Play("FadeOutObj");
            yield return new WaitForSeconds(anim.clip.length);
            anim.gameObject.SetActive(false);
        }

    }

    private void TurnOffThoughtBubble()
    {
        followPlayer = false;
        if (thoughtBubble != null)
        {
            thoughtBubble.SetActive(false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)colOffset, colSize);
    }
}
