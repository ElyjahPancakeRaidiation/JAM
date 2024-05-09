using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handScript : MonoBehaviour
{
    GameObject player;
    Rigidbody2D rb;
    PlayerController controller;
    Abilities ability;
    HingeJoint2D hj;
    bool vineInRange;
    public bool isLeftHand;
    public int leftRightID = 0;
    GameObject connectedVine;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        controller = player.GetComponent<PlayerController>();
        ability = player.GetComponent<Abilities>();

        
        //print(ability.shootVector);
        //rb.AddForce(ability.shootVector);
        if (!isLeftHand)
        {
            leftRightID = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {    
        if (vineInRange && Input.GetKeyDown(ability.handKeys[leftRightID]))
        {
            if (isLeftHand)
            {
                ability.usingLeftArm = true;
            }
            else
            {
                ability.usingLeftArm = false;
            }
            ability.connectedVine = connectedVine;
            print("connecting arms is being called");
            StartCoroutine(ability.connectingArms());
        }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "vine")
        {
            vineInRange = true;
            connectedVine = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == "vine")
        {
            vineInRange = false;
        }
    }

}
