using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;  
using UnityEngine.UI;

public class thoughtBubble : MonoBehaviour
{
    public Image thoughtBub;//Change
    private float timer;
    [SerializeField]private float maxTime;
    // Start is called before the first frame update
    void Start()
    {
        thoughtBub = GameObject.FindGameObjectWithTag("ThoughtBubble").GetComponent<Image>();
        thoughtBub.enabled = false;
    }

    private void OnTriggerStay2D(Collider2D collision){
        if(collision.tag == "Player"){
            timer += Time.deltaTime;
            if (timer >= maxTime)
            {
                thoughtBub.enabled = true;
            }else{
                thoughtBub.enabled = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            thoughtBub.enabled = false;
            timer = 0;
        }
    }
    
    
}
