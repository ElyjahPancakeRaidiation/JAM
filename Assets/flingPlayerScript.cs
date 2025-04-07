using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flingPlayerScript : MonoBehaviour
{
    public Vector2 fling;
    public float flingStrength;
    public float leftPower;
    public static GameObject player;
    public static Vector2 point;
    public LayerMask ground;

    // Start is called before the first frame update
    void Start()
    {
        player = this.gameObject;
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


        RaycastHit2D ray = Physics2D.Raycast(transform.position, Vector2.left, 5, ground);
        point = ray.point;

        if(LoopingBackground.pushPlayer){
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            GetComponent<Rigidbody2D>().AddForce(fling * flingStrength, ForceMode2D.Impulse);
            LoopingBackground.pushPlayer = false;
        }else if(LoopingBackground.isLooping && !LoopingBackground.pushPlayer){
            GetComponent<Rigidbody2D>().position = new Vector2(LoopingBackground.freezePosition.x, GetComponent<Rigidbody2D>().position.y);
        }

        //GetComponent<Rigidbody2D>().AddForce(Vector2.left * leftPower * Time.deltaTime, ForceMode2D.Impulse);
    }
}
