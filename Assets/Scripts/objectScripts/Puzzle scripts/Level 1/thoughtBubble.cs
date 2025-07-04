using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;  
using UnityEngine.UI;

public class thoughtBubble : MonoBehaviour
{
    public GameObject thoughtBub;//Change
    private PlayerAbilities playerAbilities;
    private bool completed = false;

    [SerializeField] private float maxTime;
    [SerializeField] private Vector2 positionOffset;

    private Coroutine thoughtTrigger;

    // Start is called before the first frame update
    void Start()
    {
        thoughtBub = GameObject.FindGameObjectWithTag("ThoughtBubble");
        ///
        /// When player manager is added make sure to switch this out with the event instead, decouple this code.
        /// 
        playerAbilities = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAbilities>();
        thoughtBub.gameObject.SetActive(false);
    }

    void Update()
    {
        thoughtBub.transform.position = playerAbilities.transform.position + (Vector3)positionOffset;
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
            thoughtBub.gameObject.SetActive(true);
            yield return new WaitUntil(() => playerAbilities.getDashAmount() < 1);
        }
        thoughtBub.gameObject.SetActive(false);
        completed = true;
    }
    
    
}
