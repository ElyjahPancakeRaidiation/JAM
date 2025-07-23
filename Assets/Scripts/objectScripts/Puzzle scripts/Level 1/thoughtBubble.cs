using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class thoughtBubble : MonoBehaviour
{
    public static event Action<bool> triggerThoughtBubble;
    private PlayerManager playerManager;
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
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        playerManager.PlayerAbility().GetAbilityEvent()?.AddListener(HasUsedAbility);
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
            yield return new WaitUntil(() => completed);
        }
        if (triggerThoughtBubble != null) { triggerThoughtBubble(false); }
    }

    private void HasUsedAbility()
    {
        completed = true;
        playerManager.PlayerAbility().GetAbilityEvent()?.RemoveListener(HasUsedAbility);
    }
    
    
}
