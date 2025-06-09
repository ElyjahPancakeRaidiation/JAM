using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCamOp : MonoBehaviour
{

    [SerializeField] private float speed;
    private GameObject target;
    private float refFloatX, refFloatY;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void FixedUpdate()
    {
        moveToTarget(target);
    }

    public void setTarget(GameObject val) { target = val; }
    public void setSpeed(float val) { speed = val; }


    private void moveToTarget(GameObject targ)
    {
        float xSmooth = Mathf.SmoothDamp(transform.position.x, targ.transform.position.x, ref refFloatX, speed * Time.deltaTime);
        float ySmooth = Mathf.SmoothDamp(transform.position.y, targ.transform.position.y, ref refFloatY, speed * Time.deltaTime);

        transform.position = new Vector3(xSmooth, ySmooth, -10f);
    }
}
