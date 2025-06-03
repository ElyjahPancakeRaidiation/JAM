using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class isGroundedScript : MonoBehaviour
{
    private GameObject player;
    [SerializeField]private List<AbilitySettingScriptable> playersForms;
    [SerializeField] LayerMask groundLayer;
    public List<float> rayScales;
    public int timer;
    public Vector2[] vecScales;
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
        transform.position = player.transform.position + new Vector3(0, -1 * (player.GetComponent<PlayerMovement>().getCurForm().groundChecker.y + .2f), 0);
        curForm = player.GetComponent<PlayerMovement>().getFormInt();
        //groundCol = Physics2D.OverlapBox(transform.position, vecScales[(int) PlayerController.playerForm], angle, groundLayer);
        
    }

    public bool isGrounded()
    {
        return Physics2D.OverlapBox(transform.position + (Vector3)player.GetComponent<PlayerMovement>().getCurForm().positionOffsetforPlayersothattheledgecandetecttheplayersotherearenotsoftlocks, player.GetComponent<PlayerMovement>().getCurForm().groundChecker, angle, groundLayer);
    //     return Physics2D.OverlapBox(transform.position + offset, groundChecker, angle, groundLayer);
     }

    // void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position + (Vector3)vecScales[], player.GetComponent<PlayerMovement>().getCurForm().groundChecker);
    private void OnDrawGizmos() => Gizmos.DrawWireCube(transform.position + (Vector3)playersForms[curForm].positionOffsetforPlayersothattheledgecandetecttheplayersotherearenotsoftlocks, playersForms[curForm].groundChecker);

}