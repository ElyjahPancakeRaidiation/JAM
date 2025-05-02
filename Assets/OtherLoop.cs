using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherLoop : LoopingBackGround1
{


    // Start is called before the first frame update
    void Start()
    {
        _currentCol = curImage.GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // CheckForCurrentImage();
        //currentCol = curImage.GetComponent<Collider2D>();
        Debug.Log(Camera.main.WorldToViewportPoint(test.transform.position));
        // Debug.Log(currentCol.bounds.max);
        // Debug.Log(currentCol.bounds.min);
        // Debug.Log(currentCol.bounds.extents);

        CheckForCurrentImage();
        
        CheckForPassedImage();

        float maxCoord = Camera.main.WorldToViewportPoint(_currentCol.bounds.max).y;
        float minCoord = Camera.main.WorldToViewportPoint(_currentCol.bounds.min).y;
        Debug.Log(maxCoord);
        Debug.Log(minCoord);

        //Vector3 rightPosition = Vector3.zero;
        Vector3 downPosition = new Vector3(_currentCol.bounds.max.x + offset.x, (_currentCol.bounds.min.y-_currentCol.bounds.extents.y)+offset.y);
        spawnObjectAfterAnother(maxCoord, minCoord, Vector3.zero, downPosition);

    }
}
