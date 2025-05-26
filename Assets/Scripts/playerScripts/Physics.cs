using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics
{
    private float coefficientOfFriction;
    private float rainyFrictionDown, rainyFrictionUp;
    public Rigidbody2D _rb;

    public Physics(Rigidbody2D rb){
        _rb = rb;
    }

    public void Friction()
    {
        
        // Grabs the sign of velocity and multiplies it by -1 to get opposite
        int OppositedirectionMultipleX = -1 * (int)Mathf.Sign(_rb.velocity.x);
        int OppositedirectionMultipleY = -1 * (int)Mathf.Sign(_rb.velocity.y);
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        _rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfFriction * Mathf.Abs(_rb.velocity.x * _rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfFriction * Mathf.Abs(_rb.velocity.y * _rb.velocity.y/4.3f)));
    }

    public void slipperyShitFunction(){//Just applies a downward force in the y direction 
        if(_rb.velocity.y < 0){//Going down on the y axis
            _rb.AddForce(new Vector2(0, -rainyFrictionDown), ForceMode2D.Impulse);
        }else if(_rb.velocity.y > 0){//Going up on the y axis
            _rb.AddForce(new Vector2(0, -rainyFrictionUp), ForceMode2D.Impulse);
        }
    }

    public void setCoefficientOfFriction(float amount){ coefficientOfFriction = amount; }
    public void setRainyFrictionDown(float amount){ rainyFrictionDown = amount; }
    public void setRainyFrictionUp(float amount){ rainyFrictionUp = amount; }
}
