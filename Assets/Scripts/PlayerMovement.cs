using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [SerializeField] Canvas uiGameOver;


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
        uiGameOver.enabled = false;

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

        //if (Input.GetKey(KeyCode.S)) //Normal break
        //    inputMovement.x -= 1;

        inputMovement.y = controls.Movement.Turn.ReadValue<float>();

        if (inputMovement.y < 0) //Girando a la izquierda
        {
            if (!leftParticles.isPlaying)
            {
                leftParticles.Play();
            }
        }
        else if (inputMovement.y > 0) //Girando a la derecha
        {
            if (!rightParticles.isPlaying)
            {
                rightParticles.Play();
            }
        }
        else //No girando
        {
            if (leftParticles.isPlaying)
            {
                leftParticles.Stop();
            }
            if (rightParticles.isPlaying)
            {
                rightParticles.Stop();
            }
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

            if (!particlesActive) //Activar particulas si no estan activas 
            {
                accelerationParticles.Play();
                particlesActive = true;
            }

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finish"))
        {
            uiGameOver.enabled = true;
            Invoke(nameof(LoadNewScene), 5f);
        }
    }

    void LoadNewScene()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
