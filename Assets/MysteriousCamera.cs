using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MysteriousCamera : MonoBehaviour
{

    [SerializeField] private CameraManager cameraManager;
    private SpriteRenderer _sprRenderer;
    [SerializeField] private Color highlight;
    private Color defaultColor;
    private Collider2D col;
    private Collider2D highlightCol;
    [SerializeField] private Vector2 highlightColSize, highlightColOffset;
    private Vector2 colSize, colOffset;
    [SerializeField] private Vector2 innerRadiusSize, innerRadiusOffset;
    private float colAngle, highColAngle;
    private bool beingMysterious = false;


    // Start is called before the first frame update
    void Start()
    {
        if (cameraManager == null) { Debug.LogError("Missing Camera Manager in " + gameObject.name); }
        _sprRenderer = GetComponent<SpriteRenderer>();
        if (_sprRenderer == null) { Debug.LogError("Missing a sprite renderer in " + gameObject.name); }
        defaultColor = _sprRenderer.color;

        colSize = new Vector2(highlightColSize.x - innerRadiusSize.x, highlightColSize.y - innerRadiusSize.y);
        colOffset = new Vector2(highlightColOffset.x - innerRadiusOffset.x, highlightColOffset.y - innerRadiusOffset.y);
    }

    private void FixedUpdate()
    {
        highlightCol = Physics2D.OverlapBox(transform.position + (Vector3)highlightColOffset, highlightColSize, highColAngle, LayerMask.GetMask("Player"));
        if (highlightCol)
        {
            col = Physics2D.OverlapBox(transform.position + (Vector3)colOffset, colSize, colAngle, LayerMask.GetMask("Player"));
            _sprRenderer.color = highlight;
            if (col && !beingMysterious)
            {
                beingMysterious = true;
                cameraManager.SetMysteryBool(beingMysterious);

                StartCoroutine(cameraManager.PlayManagerAutomaticallyMystery());
            }
            else if (!col)
            {
                beingMysterious = false;
                cameraManager.SetMysteryBool(beingMysterious);
            }
        }
        else
        {
            _sprRenderer.color = defaultColor;
        }
    }

    void OnDrawGizmosSelected()
    {
        colSize = new Vector2(highlightColSize.x - innerRadiusSize.x, highlightColSize.y - innerRadiusSize.y);
        colOffset = new Vector2(highlightColOffset.x - innerRadiusOffset.x, highlightColOffset.y - innerRadiusOffset.y);
        Gizmos.DrawWireCube(transform.position + (Vector3)colOffset, colSize);
        Gizmos.DrawWireCube(transform.position + (Vector3)highlightColOffset, highlightColSize);
    }
}
