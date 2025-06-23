using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour
{
    public float Value;
    public GameObject AudioSource;
    public AudioSource DippingSoundTest;
    public AudioSource audio;
    // Start is called before the first frame update
    void Start()
    {

        audio= GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {

        //if (collision.gameObject("DippingSoundTest"))
        //{
        //    DippingSoundTest.volume = Mathf.Clamp(DippingSoundTest.volume + 0.01f, 0f, 1f);
        //}
        //else
        //{
        //    DippingSoundTest.volume = Mathf.Clamp(DippingSoundTest.volume - 0.01f, 0f, 1f);
        //}
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.CompareTag("Player"))
        {

            DippingSoundTest.volume = Mathf.Clamp(DippingSoundTest.volume + 0.1f, 0f, 1f);

        }
 
       
    }
    public void OnTriggerExit2D(UnityEngine.Collider2D collision)
    {

            DippingSoundTest.volume = Mathf.Clamp(DippingSoundTest.volume - 0.1f, 0f, 1f);
       }

}


