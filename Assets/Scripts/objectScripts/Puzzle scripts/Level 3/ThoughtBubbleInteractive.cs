using System;
using System.Collections;
using UnityEngine;

public class ThoughtBubbleInteractive : MonoBehaviour
{
    [SerializeField] private string startAnimName = "ThoughtBubbleMGStart", endAnimName = "ThoughtBubbleMGEnd";
    [SerializeField] private Animation _anim;
    [SerializeField] private int correctID;
    [SerializeField] private GameObject answerLocation;
    [SerializeField] private GameObject thoughtBubbleHolder;
    [SerializeField] private GameObject[] activeObjects;//Objects that will be turned off when the MG starts
    private GameObject controlledObject;//The object that is currently being controlled
    private int curControlledIdx;
    private int curID = 0;

    [Serializable]
    private class ThoughtBubbleAnswers
    {
        [SerializeField] private int ID;
        [SerializeField] private GameObject obj;
        private Vector2 defualtPosition;

        public void SetDefualtPosition()
        {
            defualtPosition = obj.transform.position;
        }

        public GameObject GetCurInstance() { return obj; }
        public int GetID() { return ID; }
        public Vector2 GetDefualtPosition() { return defualtPosition; }
    }
    [SerializeField] private ThoughtBubbleAnswers[] thoughtBubbleAnswers;
    [SerializeField] private float movingSpeedMultiplier;
    [SerializeField] private AnimationCurve speedCurve;
    // [SerializeField] private float answerLocRadius;
    private Coroutine CheckAnswerCoro;
    private Coroutine moved;

    private Collider2D col;
    [SerializeField] private float radius;
    [SerializeField] private LayerMask mask;
    public bool isPlayingMG { get; set; } = false;//is playing minigame
    public bool isCompleted { get; set; } = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (answerLocation == null) Debug.LogError("AnswerLocation is null for " + gameObject.name);
        if (_anim == null) Debug.LogError("anim is null for " + gameObject.name);
        thoughtBubbleHolder.SetActive(false);

        // StartCoroutine(StartMinigame());
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayingMG)
        {
            BlahBlahCantThinkOfName();
        }
    }

    void FixedUpdate()
    {
        col = Physics2D.OverlapCircle(answerLocation.transform.position, radius, mask);
    }

    private void BlahBlahCantThinkOfName()
    {
        if (!IsControllingObject() && controlledObject != null)
        {
            controlledObject = null;
            if (col && curID == 0)
            {
                curID = thoughtBubbleAnswers[curControlledIdx].GetID();
                CheckAnswer(curControlledIdx);
            }
            else
            {
                MoveAnswersToPosition(curControlledIdx, thoughtBubbleAnswers[curControlledIdx].GetDefualtPosition(), movingSpeedMultiplier);
            }
        }
    }

    public IEnumerator StartMinigame()
    {
        Debug.Log(isPlayingMG);
        thoughtBubbleHolder.SetActive(true);
        _anim.Play(startAnimName);
        foreach (GameObject item in activeObjects)
        {
            item.SetActive(false);
        }
        yield return new WaitForSeconds(_anim.clip.length);
        foreach (ThoughtBubbleAnswers answer in thoughtBubbleAnswers)
        {
            answer.SetDefualtPosition();
        }
        PlayerManager.playerManager.canControl = false;
        isPlayingMG = true;
    }

    private IEnumerator EndMinigame()
    {
        _anim.Play(endAnimName);
        yield return new WaitForSeconds(_anim.clip.length);
        foreach (GameObject item in activeObjects)
        {
            item.SetActive(true);
        }
        PlayerManager.playerManager.canControl = true;
        isCompleted = true;
        thoughtBubbleHolder.SetActive(false);
        Debug.Log("Finished");
    }

    private void CheckAnswer(int idx)
    {
        if (CheckAnswerCoro == null)
        {
            CheckAnswerCoro = StartCoroutine(CheckAnswerEnum(idx));
        }
    }

    private IEnumerator CheckAnswerEnum(int idx)
    {
        thoughtBubbleAnswers[idx].GetCurInstance().GetComponent<MoveUIAnswers>().canClick = false;
        yield return StartCoroutine(MoveAnswersToPositionEnum(thoughtBubbleAnswers[idx].GetCurInstance(), answerLocation.transform.position, movingSpeedMultiplier));
        yield return new WaitForSeconds(1f);
        if (curID == correctID)
        {
            StartCoroutine(EndMinigame());
            isPlayingMG = false;
        }
        else
        {
            StartCoroutine(MoveAnswersToPositionEnum(thoughtBubbleAnswers[idx].GetCurInstance(), thoughtBubbleAnswers[idx].GetDefualtPosition(), movingSpeedMultiplier));
            thoughtBubbleAnswers[idx].GetCurInstance().GetComponent<MoveUIAnswers>().canClick = true;

            curID = 0;
        }
        CheckAnswerCoro = null;
    }

    private bool IsControllingObject()
    {

        for (int i = 0; i < thoughtBubbleAnswers.Length; i++)
        {
            if (thoughtBubbleAnswers[i].GetCurInstance().GetComponent<MoveUIAnswers>().isClicked)
            {
                controlledObject = thoughtBubbleAnswers[i].GetCurInstance();
                curControlledIdx = i;
                return true;
            }
        }
        return false;
    }

    private void MoveAnswersToPosition(int idx, Vector2 position, float speedMultiplier)
    {
        StartCoroutine(MoveAnswersToPositionEnum(thoughtBubbleAnswers[idx].GetCurInstance(), position, speedMultiplier));
    }

    private IEnumerator MoveAnswersToPositionEnum(GameObject obj, Vector3 position, float speedMultiplier)
    {
        var conObject = obj.transform;
        float time = 0;
        while (conObject.position != position && !obj.GetComponent<MoveUIAnswers>().isClicked)
        {
            Debug.Log(conObject);
            time += Time.deltaTime;
            conObject.position = Vector2.MoveTowards(conObject.position, position, speedCurve.Evaluate(time) * speedMultiplier);

            Debug.Log(conObject.position + ": " + position);
            Debug.Log(obj.GetComponent<MoveUIAnswers>().isClicked);
            if (conObject.position == (Vector3)position)//For saftey
            {
                Debug.Log("HELLO?!!");
                // if (moved != null)
                // {
                //     StopCoroutine(moved);
                //     moved = null;
                // }
                break;
            }
            yield return null;
        }
        // if (moved != null)
        // {
        //     StopCoroutine(moved);
        //     moved = null;
        // }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(answerLocation.transform.position, radius);
    }


}
