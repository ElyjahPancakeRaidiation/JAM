using System.Collections;
using UnityEngine;


[RequireComponent(typeof(BoxCollider2D))]
public class SpecialObjects : MonoBehaviour
{
    private bool activated;
    private SpriteRenderer _sprRender;


    //the object that will change when activated
    [SerializeField] private GameObject targetObject;
    [SerializeField] private AnimationCurve fadeCurve;

    [SerializeField] private enum ObjectIntendedState { FadeIn, FadeOut, }
    [SerializeField] private ObjectIntendedState objectIntendedState;
    [SerializeField] private bool collisionTrigger;

    private float time;

    // Start is called before the first frame update
    void Start()
    {
        if (targetObject == null) { targetObject = gameObject; }
        _sprRender = targetObject?.GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                if (_sprRender != null)
                {
                    switch (objectIntendedState)
                    {
                        case ObjectIntendedState.FadeIn:
                            StartCoroutine(FadeInObject());
                            break;
                        case ObjectIntendedState.FadeOut:
                            StartCoroutine(FadeOutObject());
                            break;
                    }

                    activated = true;
                }
            }
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


}
