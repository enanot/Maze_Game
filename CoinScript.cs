using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public float rotationSpeed = 100f; // Velocidad de rotaci�n en grados por segundo

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Rota la moneda sobre el eje Y en cada frame
        transform.Rotate( 0,0, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verifica si el collider que entr� en contacto es el jugador
        if (other.CompareTag("Player"))
        {
            // Aqu� puedes incrementar un contador de monedas o realizar alguna acci�n
            // Por ejemplo: FindObjectOfType<GameManager>().AddCoins(1);

            // Destruye la moneda
            Destroy(gameObject);
        }
    }
}