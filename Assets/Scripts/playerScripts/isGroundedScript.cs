using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private List<AbilitySettingScriptable> playersForms;
    [SerializeField] LayerMask groundLayer;
    public List<float> rayScales;
    public int timer;
    public Vector2[] vecScale;
    public int curForm;
    //public Collider2D groundCol;
    private float angle;


    //Called only when the inspector is changed.
    private void OnValidate()
    {

        // Debug.Log(player.GetComponent<PlayerMovement>().getAllForms());
        // playersForms = player.GetComponent<PlayerMovement>().forms;
        // curForm = player.GetComponent<PlayerMovement>().getFormInt();
    }

    // Start is called before the first frame update
    void Start()
    {
        
        player = GameObject.FindGameObjectWithTag("Player");
        curForm = player.GetComponent<PlayerMovement>().getFormInt();
        
        //transform.position = player.transform.position + new Vector3(0, -1 * (vecScales[(int) PlayerController.playerForm].y + .2f), 0);

    }

    private void FixedUpdate()
    {


        //only applying when the form is ball, otherwise when changing the groundchecker collider size, in pogo, it changes for both
        //the jump and hopping. With this if statement, the two colliders are independent of each other.
        if (playersForms[curForm] == playersForms[0])
        {   //this locks the hitbox collider and prevents it from rolling with the ball
            transform.position = player.transform.position + new Vector3(0, -1 * (player.GetComponent<PlayerMovement>().getCurForm().groundChecker.y + .2f), 0);
        }
        curForm = player.GetComponent<PlayerMovement>().getFormInt();
        //groundCol = Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);

    }
    //isgrounded for ball and pogo jumping
    public bool isGrounded()
    {

        return Physics2D.OverlapBox(transform.position + (Vector3)player.GetComponent<PlayerMovement>().getCurForm().startPositionOffset, player.GetComponent<PlayerMovement>().getCurForm().groundChecker, angle, groundLayer);
        //     return Physics2D.OverlapBox(transform.position + offset, groundChecker, angle, groundLayer);
    } 
    //specifically for hopping
    public bool isGroundedForHopping()
    {
        
        return Physics2D.OverlapBox(transform.position + (Vector3)player.GetComponent<PlayerMovement>().getCurForm().hoppingStartPositionOffset, player.GetComponent<PlayerMovement>().getCurForm().hoppingGroundChecker, angle, groundLayer);
    }

    public bool isArmsColliding()
    {
        return Physics2D.OverlapBox(transform.position + (Vector3)player.GetComponent<PlayerMovement>().getCurForm().armStartPositionOffset, player.GetComponent<PlayerMovement>().getCurForm().armSizeChecker, angle, groundLayer);
    }
    // void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position + (Vector3)vecScales[], player.GetComponent<PlayerMovement>().getCurForm().groundChecker);
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)playersForms[curForm].startPositionOffset, playersForms[curForm].groundChecker);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + (Vector3)playersForms[curForm].hoppingStartPositionOffset, playersForms[curForm].hoppingGroundChecker);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position + (Vector3)playersForms[curForm].armStartPositionOffset, playersForms[curForm].armSizeChecker);


    }   

    

  
    
       
       
    
}