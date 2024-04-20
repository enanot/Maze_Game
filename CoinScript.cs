using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public float rotationSpeed = 100f; // Velocidad de rotación en grados por segundo

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
        // Verifica si el collider que entró en contacto es el jugador
        if (other.CompareTag("Player"))
        {
            // Aquí puedes incrementar un contador de monedas o realizar alguna acción
            // Por ejemplo: FindObjectOfType<GameManager>().AddCoins(1);

            // Destruye la moneda
            Destroy(gameObject);
        }
    }
}