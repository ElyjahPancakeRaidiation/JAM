using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flingPlayerScript : MonoBehaviour
{
    public Vector2 flingWhen;
    public Vector2 fling;
    public float flingStrength;
    public float flingStrengthStart;
    public float leftPower;
    public static GameObject player;
    public static Vector2 point;
    public LayerMask ground;
    public Vector2 freezeTest;

    // Start is called before the first frame update
    void Start()
    {
        //player = this.gameObject;
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Rigidbody2D>().AddForce(fling * flingStrengthStart, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(point);
        //Works??
        // GetComponent<Rigidbody2D>().rotation += -100 * Time.deltaTime;
        // GetComponent<Rigidbody2D>().rotation = Mathf.Clamp(GetComponent<Rigidbody2D>().rotation, -4000, 4000);
    }

    void FixedUpdate()
    {

        if(LoopingBackground.pushPlayer){
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            player.GetComponent<Rigidbody2D>().gravityScale = 2.5f;
            player.GetComponent<Rigidbody2D>().AddForce(flingWhen * flingStrength, ForceMode2D.Impulse);
            LoopingBackground.pushPlayer = false;
        }else if(LoopingBackground.isLooping && !LoopingBackground.pushPlayer){
            if(freezeTest == Vector2.zero){
                freezeTest = player.transform.position;
            }
            player.GetComponent<Rigidbody2D>().position = new Vector2(freezeTest.x, player.GetComponent<Rigidbody2D>().position.y);
        }

        //GetComponent<Rigidbody2D>().AddForce(Vector2.left * leftPower * Time.deltaTime, ForceMode2D.Impulse);
    }
}
