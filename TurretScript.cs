using UnityEngine;
using System.Collections;

public class TurretScript : MonoBehaviour
{
    public float range = 1f; // Rango del raycast
    public LayerMask targetMask; // Máscara para filtrar a qué objetos afectará el raycast
    private bool isPlayerHit = false; // Controla si el jugador ha sido golpeado por el rayo

    // Este método puede ser llamado desde MazeGenerator.
    public void OrientTurret()
    {   

        StartCoroutine(CheckAndOrient());
    }

   


    IEnumerator CheckAndOrient()
    {
        // Espera un breve periodo para asegurarte de que todas las torretas están ya instanciadas y el laberinto está completo.
        yield return new WaitForSeconds(0.1f);

        int intentos = 0; // Para evitar un bucle infinito
        while (intentos < 3) // Limita a 4 intentos, un giro  de 270 grados
        {
            Vector3 direction = transform.forward;
            RaycastHit hit;

            // Realiza un raycast en la dirección en que la torreta está mirando
            if (Physics.Raycast(transform.position, direction, out hit, range, targetMask))
            {
                Debug.Log(hit.collider.tag);
                if (hit.collider.CompareTag("Wall"))
                {
                    Debug.Log("muro contacto");
                    // Si golpea un muro, gira 90 grados y prueba de nuevo
                    transform.Rotate(0, 90, 0);
                    intentos++;
                    yield return new WaitForSeconds(0.1f); // Pequeña espera para evitar cambios demasiado rápidos
                }
                else
                {
                    // Si no golpea un muro, finaliza la corutina
                    break;
                }
            }
            else
            {
                // Si el raycast no golpea nada, también finaliza la corutina
                break;
            }
        }
    }
    void Update()
    {
        EmitRaycast();
    }

    void EmitRaycast()
    {
        float alturaRayoDesdeTorreta = 0.5f;
        Vector3 puntoInicioRayo = transform.position + transform.up * alturaRayoDesdeTorreta;
        Vector3 direction = transform.forward;
        RaycastHit hit;

        Debug.DrawRay(puntoInicioRayo, direction * range, Color.red);

        if (Physics.Raycast(puntoInicioRayo, direction, out hit, range, targetMask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (!isPlayerHit)
                {
                    isPlayerHit = true;
                    StartCoroutine(HandlePlayerHit(hit.collider.gameObject));
                }
            }
        }
        else
        {
            isPlayerHit = false; // El jugador ya no está siendo golpeado por el rayo
        }
    }

    IEnumerator HandlePlayerHit(GameObject player)
    {
        yield return new WaitForSeconds(1); // Espera un segundo

        // Si el jugador todavía está siendo golpeado después de un segundo, lo envía de vuelta
        if (isPlayerHit)
        {
            player.transform.position = new Vector3(-1, 0, -1); // Envía al jugador a la posición inicial
        }
        isPlayerHit = false; // Reinicia el estado
    }
}

