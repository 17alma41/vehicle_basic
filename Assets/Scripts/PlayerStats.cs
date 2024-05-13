using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float forwardAcceleration = 15;
    public float backwardAcceleration = 5;
    public float turnSpeed = 1;
    public float handbreakAdditionalTurnSpeed = 1;
    public float maxSpeed = 50;
    public float breakStrenght = 2;
    public float handbreakStrenght = 2;
    public float groundFriction = .02f;
}
