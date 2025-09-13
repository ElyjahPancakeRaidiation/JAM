using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopSidedCollider : MonoBehaviour
{
    [SerializeField] private Collider2D _activeCollider;
    [SerializeField] private float rayLength;
    [SerializeField] private Vector2 raycastOffset;
    private PlayerManager playerManager;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        if (playerManager == null)
        {
            Debug.LogError("Missing playermanager in a TopSidedCollider script");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerAbove())
        {
            _activeCollider.isTrigger = false;
        }
        else
        {
            _activeCollider.isTrigger = true;
        }
    }

    bool IsPlayerAbove()
    {
        return playerManager.IsGrounded().isGroundedRay(raycastOffset, Vector2.down, rayLength, LayerMask.GetMask("TopCollider"));
    }

    private void OnDrawGizmosSelected()
    {
        if (playerManager != null)
        {
            playerManager.IsGrounded().DrawCustomRay(raycastOffset, Vector2.down, rayLength);
        }
    }
}
