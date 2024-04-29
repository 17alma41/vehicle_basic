using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float forwardAcceleration = 15;
    public float backwardAcceleration = 5;
    public float maxSpeed = 50;
    public float groundFriction = .02f;
}
