using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndandStartPoint : MonoBehaviour
{
    [SerializeField]private GameObject endPoint;
    [SerializeField]private Transform spawnPoint;
    


    public GameObject getEndPoint(){
        return endPoint;
    }
    public Transform getSpawnPoint(){
        return spawnPoint;
    }
}
