using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStats stats;
    [SerializeField] ParticleSystem accelerationParticles;
    [SerializeField] ParticleSystem rightParticles;
    [SerializeField] ParticleSystem leftParticles;
    bool particlesActive = false;

    Vector2 inputMovement = Vector2.zero;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); 
    }

    void Update()
    {
        InputMovement();
    }


    private void FixedUpdate()
    {
        Physical();
    }

    void InputMovement()
    {
        inputMovement = Vector2.zero;

        if (Input.GetKey(KeyCode.W))
        {
            if (!particlesActive) //Activar particulas si no estan activas 
            {
                accelerationParticles.Play();
                particlesActive = true;
            }
           
            inputMovement.x += 1;
        }

        if (Input.GetKey(KeyCode.S))
            inputMovement.x -= 1;

        if (Input.GetKey(KeyCode.A))
            inputMovement.y -= 1;

        if (Input.GetKey(KeyCode.D))
            inputMovement.y += 1;

        if (Input.GetKeyDown(KeyCode.A))
            leftParticles.Play();
        if (Input.GetKeyDown(KeyCode.D))
            rightParticles.Play();
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            rightParticles.Stop();
            leftParticles.Stop();
        }

    }

    void Physical()
    {
        if (inputMovement.x > 0)
        {
            //Aceleración
            rb.velocity += transform.forward * stats.forwardAcceleration * Time.fixedDeltaTime;
        }
        else
        {
            if (particlesActive) //Desactivar particulas si no estan activas 
            {
                accelerationParticles.Stop();
                particlesActive = false;
            }

            //Fricción en el suelo
            rb.velocity = new Vector3(
                rb.velocity.x / (1 + stats.groundFriction * Time.fixedDeltaTime),
                rb.velocity.y,
                rb.velocity.z / (1 + stats.groundFriction * Time.fixedDeltaTime));
        }

        if(inputMovement.x < 0)
        {
            //Marcha atrás
            if(transform.InverseTransformDirection(rb.velocity).z < 0)
            {
                rb.velocity -= transform.forward * stats.forwardAcceleration * Time.fixedDeltaTime;
            }
            else
            {
                //Frenada en el suelo
                rb.velocity = new Vector3(
                   rb.velocity.x / (1 + stats.powerBreak * Time.fixedDeltaTime),
                   rb.velocity.y,
                   rb.velocity.z / (1 + stats.powerBreak * Time.fixedDeltaTime)
                   );
            }

        }


        if (inputMovement.y != 0)
        {
            rb.angularVelocity += new Vector3(
              0,
              stats.rotationSpeed * inputMovement.y,
              0
          );
        }


        //Máxima velocidad del personaje en el suelo
        if (rb.velocity.magnitude > stats.maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * stats.maxSpeed;
        }
    }
}
