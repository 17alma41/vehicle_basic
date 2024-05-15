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
    [SerializeField] TrailRenderer skidMarkLeft;
    [SerializeField] TrailRenderer skidMarkRight;
    bool particlesActive = false;
    bool handbreakActive = false;
    [SerializeField] float bouncingForce;
    bool isBouncing = false;


    Vector2 inputMovement = Vector2.zero;

    Rigidbody rb;
    Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Enable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        skidMarkLeft.emitting = false;
        skidMarkRight.emitting = false;
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

        inputMovement.x = controls.Movement.AccelerationOrBreak.ReadValue<float>();

        if (Input.GetKey(KeyCode.W))
        {
            if (!particlesActive) //Activar particulas si no estan activas 
            {
                accelerationParticles.Play();
                particlesActive = true;
            }
           
           //inputMovement.x += 1;
        }

        //if (Input.GetKey(KeyCode.S)) //Normal break
        //    inputMovement.x -= 1;

        inputMovement.y = controls.Movement.Turn.ReadValue<float>();

        //if (Input.GetKey(KeyCode.A))
        //    inputMovement.y -= 1;
        //if (Input.GetKey(KeyCode.D))
        //    inputMovement.y += 1;

        if (Input.GetKeyDown(KeyCode.A))
            leftParticles.Play();
        if (Input.GetKeyDown(KeyCode.D))
            rightParticles.Play();
        else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.A))
        {
            rightParticles.Stop();
            leftParticles.Stop();
        }

        if (controls.Movement.Handbreak.IsPressed())
            handbreakActive = true;
    }

    void Physical()
    {
        if (inputMovement.x > 0)
        {
            //Aceleración
            rb.velocity += transform.forward * stats.forwardAcceleration * Time.fixedDeltaTime;

            //Máxima velocidad del personaje en el suelo
            if (rb.velocity.magnitude > stats.maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * stats.maxSpeed;
            }
        }
        else
        {
            if (particlesActive) //Desactivar particulas si no estan activas 
            {
                accelerationParticles.Stop();
                particlesActive = false;
            }

            //Fricción en el suelo
            ReduceVelocity(stats.groundFriction);
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
                ReduceVelocity(stats.breakStrenght);
            }

        }

        //Handbreak
        if (handbreakActive)
        {

            //Frenada en el suelo
            ReduceVelocity(stats.handbreakStrenght);

            if (inputMovement.y != 0)
                rb.angularVelocity += new Vector3(0, stats.handbreakAdditionalTurnSpeed * inputMovement.y,0);

            skidMarkLeft.emitting = true;
            skidMarkRight.emitting = true;
        }
        else
        {
            skidMarkLeft.emitting = false;
            skidMarkRight.emitting = false;
        }
            
        handbreakActive = false;

        if (inputMovement.y != 0)
        {
            rb.angularVelocity += new Vector3(
              0,
              stats.turnSpeed * inputMovement.y,
              0
          );
        }


    }

    void ReduceVelocity(float strenght)
    {
        rb.velocity = new Vector3(
              rb.velocity.x / (1 + strenght * Time.fixedDeltaTime),
              rb.velocity.y,
              rb.velocity.z / (1 + strenght * Time.fixedDeltaTime)
              );
    }

    void Bounce()
    {
        rb.velocity = -rb.velocity.normalized * bouncingForce;
        isBouncing = true;

        Invoke("UnPause", 0.05f);
    }

    void UnPause()
    {
        isBouncing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacule"))
        {
            Bounce();
        }

    }
}
