using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Physics : MonoBehaviour
{
    private float coefficientOfFriction;
    private float rainyFriction;
    public Rigidbody2D _rb;

    public Physics(Rigidbody2D rb){
        _rb = rb;
    }

    public void Friction()
    {
        // Air resistance opposes motion but in ball motion is reversed because rotation
        // Grabs the sign of velocity and multiplies it by -1 to get opposite
        int OppositedirectionMultipleX = -1 * Mathf.RoundToInt(_rb.velocity.x / Mathf.Abs(_rb.velocity.x));
        int OppositedirectionMultipleY = -1 * Mathf.RoundToInt(_rb.velocity.y / Mathf.Abs(_rb.velocity.y));
        // Multiplies the direction then coefficient of air resistence and the velocity squared
        _rb.AddForce(new Vector2(OppositedirectionMultipleX * coefficientOfFriction * Mathf.Abs(_rb.velocity.x * _rb.velocity.x),
        OppositedirectionMultipleY * coefficientOfFriction * Mathf.Abs(_rb.velocity.y * _rb.velocity.y)));
    }

    public void slipperyShitFunction(){//Just applies a downward force in the y direction 
        _rb.AddForce(new Vector2(0, -rainyFriction), ForceMode2D.Impulse);
    }

    public void setCoefficientOfFriction(float amount){ coefficientOfFriction = amount; }
    public void setRainyFriction(float amount){ rainyFriction = amount; }
}
