using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats stats;
    [SerializeField] ParticleSystem accelerationParticles;
    bool particlesActive = false;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            
            if (!particlesActive) //Activar particulas si no estan activas 
            { 
                accelerationParticles.Play();
                particlesActive = true;
            } 

            movement.z += 1;
        }

        if (Input.GetKey(KeyCode.S))
            movement.z -= 1;

        if (movement != Vector3.zero)
        {
            //Aceleración
            rb.velocity += transform.forward * stats.forwardAcceleration * Time.deltaTime;
        }
        else
        {
            if(particlesActive) //Desactivar particulas si no estan activas 
            {
                accelerationParticles.Stop();
                particlesActive = false;
            }

            //Fricción en el suelo
            rb.velocity = new Vector3(
                rb.velocity.x / (1 + stats.groundFriction * Time.deltaTime),
                rb.velocity.y, 
                rb.velocity.z / (1 + stats.groundFriction * Time.deltaTime));
        }

        //Máxima velocidad del personaje en el suelo
        if (rb.velocity.magnitude > stats.maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * stats.maxSpeed;
        }

    }
}
