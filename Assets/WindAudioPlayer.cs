using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ManageWind))]
public class WindAudioPlayer : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] AudioSource source;

    [SerializeField] bool isPlayerWithinZone;

    ManageWind manageWind;
    void Awake()
    {
        manageWind = GameObject.FindGameObjectWithTag("Wind").GetComponent<ManageWind>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayAudio(); 
    }

    void PlayAudio()
    {
        if (manageWind.IsPlayerWithinZone())
        {
            source.Play();
        }
    }
}
