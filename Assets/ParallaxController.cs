using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [System.Serializable]
    public class ParallaxedObject
    {
        public GameObject spriteHolder;
        public float parallaxEffect; //0 follows player, 1 doesnt follow at all, choose between
        public float startX;

    }
    public List<ParallaxedObject> parallaxedObjects;
    public GameObject playerCamera;
    [SerializeField]private float offSet;
    void Start()
    {
        foreach (ParallaxedObject g in parallaxedObjects)
        {
            g.startX = g.spriteHolder.transform.position.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateParallaxedObjects();
    }

    private void updateParallaxedObjects()
    {
        foreach (ParallaxedObject g in parallaxedObjects)
        {
            float distance = (playerCamera.transform.position.x + offSet) * g.parallaxEffect;
            g.spriteHolder.transform.position = new Vector3(g.startX + distance, g.spriteHolder.transform.position.y, g.spriteHolder.transform.position.z);
        }
    }
}
