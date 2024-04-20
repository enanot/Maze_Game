using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody rb;
    public float movVel;
    private Transform tr;
    public Animator animator; // Asegúrate de asignar esto en el inspector
    public Transform CameraPivot;
    public float JumpForce;
    private bool isGrounded = true; // Verifica si el jugador está en el suelo
    private int jumpCount = 0; // Contador de saltos
    public int maxJump = 2; // Número máximo de saltos (1 para salto normal, 2 para doble salto)

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        // Inicializa las variables para controlar el movimiento lateral y hacia adelante/atrás


        // Convertir la posición del cursor del ratón de coordenadas de pantalla a coordenadas del mundo
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        MovePlayer();
        Jump();

        // Rotar el personaje para que siempre mire hacia el puntero del ratón
        //RotateCharacterTowardsMouseCursor();

       
        if (CameraPivot != null)
        {
            CameraPivot.transform.position = transform.position;
            // Sigue al jugador

            // No rotamos el pivote, así que la cámara mantiene su rotación original
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("respawn"))
        {
            tr.position = new Vector3(0, 1, 0);
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        if (other.gameObject.CompareTag("Pit"))
        {
            // Suponiendo que tienes un solo collider para el suelo
            Collider groundCollider = GameObject.FindGameObjectWithTag("Ground").GetComponent<Collider>();
            Collider playerCollider = GetComponent<Collider>();
            Physics.IgnoreCollision(playerCollider, groundCollider, true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Pit"))
        {
            // Cuando el jugador sale del área del foso, reactivar las colisiones con el suelo
            Collider groundCollider = GameObject.FindGameObjectWithTag("Ground").GetComponent<Collider>();
            Collider playerCollider = GetComponent<Collider>();
            Physics.IgnoreCollision(playerCollider, groundCollider, false);
        }
    }

    public void FootR()
    {
        // Implementa la lógica deseada aquí, como reproducir un sonido de paso
        Debug.Log("Pie derecho tocó el suelo.");
    }
    public void FootL()
    {
        // Implementa la lógica deseada aquí, como reproducir un sonido de paso
        Debug.Log("Pie izquierdo tocó el suelo.");
    }
    public void Land()
    {
        Debug.Log("landed");
    }


    void MovePlayer()
    {

        float moveVerticalF = 0f;

        Quaternion targetRotation = transform.rotation; // Inicializa con la rotación actual para mantenerla si no hay entrada.

        // Manejo de la entrada del jugador para movimiento
        if (Input.GetKey(KeyCode.A))
        {
            moveVerticalF = 1f; // Mover hacia adelante
            rb.AddForce(new Vector3(-movVel, 0, 0));
            transform.rotation = Quaternion.Euler(0, 270, 0);

        }
        if (Input.GetKey(KeyCode.D))
        {
            moveVerticalF = 1f; // Mover hacia adelante
            rb.AddForce(new Vector3(movVel, 0, 0));
            transform.rotation = Quaternion.Euler(0, 90, 0);

        }
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(new Vector3(0, 0, movVel));
            transform.rotation = Quaternion.Euler(0, 0, 0);
            moveVerticalF = 1f; // Mover hacia adelante
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(new Vector3(0, 0, -movVel));
            moveVerticalF = 1f; // Mover hacia adelante
            transform.rotation = Quaternion.Euler(0, 180, 0);

        }
        // Interpolación suave hacia la rotación objetivo
        transform.rotation = Quaternion.Slerp(targetRotation, transform.rotation, 0.02f);


        // Actualizar Animator basado en la entrada del jugador
        animator.SetFloat("ForwardSpeed", moveVerticalF);

    }
    public void Jump()
    {   

        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < maxJump))
        {  
            animator.SetTrigger("jump"); // Activa el trigger de salto
            rb.velocity = new Vector3(rb.velocity.x, JumpForce, rb.velocity.z);
            isGrounded = false; // El jugador deja el suelo
            jumpCount++; // Incrementa el contador de saltos
            animator.ResetTrigger("land"); // Resetea el trigger de aterrizaje por si acaso
           
        }
    }
    // Detecta colisiones con el suelo
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // Asume que el suelo tiene el Tag "Ground"
        {   animator.SetTrigger("land"); // Activa el trigger de aterrizaje
            isGrounded = true; // El jugador toca el suelo
            jumpCount = 0; // Restablece el contador de saltos
            animator.ResetTrigger("jump"); // Resetea el trigger de salto para evitar conflictos
           
        }
    }

}