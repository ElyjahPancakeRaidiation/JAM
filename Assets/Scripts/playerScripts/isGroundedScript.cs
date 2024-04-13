using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    GameObject player;
    [SerializeField] LayerMask groundLayer;
    public List<float> rayScales;
    public int timer;
    public Vector2[] vecScales;
    private float angle;
    

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        

    }
    
    private void FixedUpdate() 
    {
        transform.position = player.transform.position + new Vector3(0, -1 * (vecScales[(int) PlayerController.playerForm].y + .2f), 0);
        Debug.Log("is it grounded " + isGrounded());
        
    }

    public bool isGrounded()
    {
        
        
        if (Physics2D.Raycast(player.transform.position, Vector2.down, rayScales[(int) PlayerController.playerForm], groundLayer))
        {
            Debug.DrawRay(player.transform.position, Vector2.down * rayScales[(int) PlayerController.playerForm], Color.red);
            return true;
        }
        else
        {
            Debug.DrawRay(player.transform.position, Vector2.down * rayScales[(int) PlayerController.playerForm], Color.red);
            return false;
        }

        
        
        //return Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);
    }

    void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position, vecScales[(int) PlayerController.playerForm]);

}
