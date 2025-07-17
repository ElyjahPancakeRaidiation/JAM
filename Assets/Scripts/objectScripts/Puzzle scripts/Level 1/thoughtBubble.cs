using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class thoughtBubble : MonoBehaviour
{
    public static event Action<bool> triggerThoughtBubble;
    private PlayerAbilities playerAbilities;
    private bool completed = false;

    [SerializeField] private float maxTime;
    [SerializeField] private Vector2 positionOffset;

    private Coroutine thoughtTrigger;

    // Start is called before the first frame update
    void Start()
    {
        ///
        /// When player manager is added make sure to switch this out with the event instead, decouple this code.
        /// 
        playerAbilities = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAbilities>();
    }

    void Update()
    {
        //Ensures that the bubble wont appear if the player has already pressed dash before.
        if (playerAbilities.getDashAmount() < 1)
        {
            if (!completed)
            {
                completed = true;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (thoughtTrigger == null && !completed) { thoughtTrigger = StartCoroutine(TriggerThought()); }
        }
    }

    private IEnumerator TriggerThought()
    {
        yield return new WaitForSecondsRealtime(maxTime);
        if (!completed)
        {
            if (triggerThoughtBubble != null) { triggerThoughtBubble(true); }
            yield return new WaitUntil(() => playerAbilities.getDashAmount() < 1);
        }
        if (triggerThoughtBubble!=null){triggerThoughtBubble(false);}
        completed = true;
    }
    
    
}
