using System.Collections;
using UnityEngine;

public class SpecialObjects : MonoBehaviour
{
    private bool activated;
    private SpriteRenderer _sprRender;


    //the object that will change when activated
    [SerializeField] private GameObject targetObject;
    [SerializeField] private Vector2 customColliderSize;
    [SerializeField] private Vector2 customColliderSizeOffset;
    private Collider2D customCol;
    private float colAngle;


    [SerializeField] private enum ObjectIntendedState { FadeIn, FadeOut, MoveToPosition}
    [SerializeField] private ObjectIntendedState objectIntendedState;

    [Header("Fading options")]
    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private bool collisionTrigger;
    private float time;

    [Header("Moving position variables")]
    [SerializeField] private GameObject finalPosition;
    [SerializeField] private float movingSpeed;


    // Start is called before the first frame update
    void Start()
    {
        if (targetObject == null) { targetObject = gameObject; }
        _sprRender = targetObject?.GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (!activated)
        {
            customCol = Physics2D.OverlapBox(transform.position + (Vector3)customColliderSizeOffset, customColliderSize, colAngle, LayerMask.GetMask("Player"));
            if (customCol != null)
            {
                if (_sprRender != null)
                {
                    ObjectStateMethod(objectIntendedState);
                    activated = true;
                }
            }
        }
    }

    private void ObjectStateMethod(ObjectIntendedState objectIntendedState)
    {
        switch (objectIntendedState)
        {
            case ObjectIntendedState.FadeIn:
                StartCoroutine(FadeInObject());
                break;
            case ObjectIntendedState.FadeOut:
                StartCoroutine(FadeOutObject());
                break;
            case ObjectIntendedState.MoveToPosition:
                StartCoroutine(MoveObjectToPosition());
                break;
        }
    }

    private IEnumerator FadeOutObject()
    {
        while (_sprRender.color.a > 0)
        {
            // Debug.Log(new Color(0.3f, 0.2f, 0.3f).linear);
            var color = _sprRender.color;
            color.a = fadeCurve.Evaluate(time);
            time += Time.deltaTime;
            _sprRender.color = color;
            yield return null;
        }
        if (targetObject.GetComponent<Collider2D>())
        {
            targetObject.GetComponent<Collider2D>().enabled = collisionTrigger;
        }
    }
    private IEnumerator FadeInObject()
    {
        while (_sprRender.color.a < fadeCurve[fadeCurve.length - 1].value)
        {
            var color = _sprRender.color;
            color.a = fadeCurve.Evaluate(time);
            time += Time.deltaTime;
            _sprRender.color = color;
            yield return null;
        }
        if (targetObject.GetComponent<Collider2D>())
        {
            targetObject.GetComponent<Collider2D>().enabled = collisionTrigger;
        }
    }

    private IEnumerator MoveObjectToPosition()
    {
        while ((Vector2)targetObject.transform.position != (Vector2)finalPosition.transform.position)
        {
            targetObject.transform.position = Vector2.MoveTowards(targetObject.transform.position, (Vector2)finalPosition.transform.position, movingSpeed * Time.deltaTime);
            yield return null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position + (Vector3)customColliderSizeOffset, customColliderSize);  
    }


}
