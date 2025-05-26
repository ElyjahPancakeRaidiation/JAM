using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    private GameObject player;
    private List<AbilitySettingScriptable> playersForms; 
    [SerializeField] LayerMask groundLayer;
    public List<float> rayScales;
    public int timer;
    public Vector2[] vecScales;
    //public Collider2D groundCol;
    private float angle;


    //Called only when the inspector is changed.
    private void OnValidate()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playersForms = player.GetComponent<PlayerMovement>().getAllForms();
        //transform.position = player.transform.position + new Vector3(0, -1 * (vecScales[(int) PlayerController.playerForm].y + .2f), 0);

    }   
    
    private void FixedUpdate() 
    {
        transform.position = player.transform.position + new Vector3(0, -1 * (player.GetComponent<PlayerMovement>().getCurForm().groundChecker.y + .2f), 0);
        //groundCol = Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);
        
    }

    public bool isGrounded()
    {
        
        return Physics2D.OverlapBox(transform.position + (Vector3)player.GetComponent<PlayerMovement>().getCurForm().positionOffsetforPlayersothattheledgecandetecttheplayersotherearenotsoftlocks, player.GetComponent<PlayerMovement>().getCurForm().groundChecker, angle, groundLayer);
    }

    // void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position + (Vector3)vecScales[], player.GetComponent<PlayerMovement>().getCurForm().groundChecker);

}
