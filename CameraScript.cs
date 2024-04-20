using UnityEngine;

public class TransparencyController : MonoBehaviour
{
    public Transform player;
    public LayerMask obstructionLayer; // Aseg�rate de configurar esta capa para incluir solo los objetos que pueden obstruir la vista

    private Renderer lastObstructingObjectRenderer; // Almacena el �ltimo objeto que obstruy� la vista
    private Color originalColor; // Almacena el color original del objeto

    void Update()
    {
        RaycastHit hit;

        // Direcci�n desde la c�mara al jugador
        Vector3 direction = player.position - transform.position;

        bool hitObstruction = Physics.Raycast(transform.position, direction, out hit, direction.magnitude, obstructionLayer);

        // Si hay un objeto obstruyendo la vista
        if (hitObstruction && hit.collider.GetComponent<Renderer>() != null)
        {
            Renderer currentRenderer = hit.collider.GetComponent<Renderer>();

            // Si el objeto actual es diferente al �ltimo que obstruy� la vista
            if (lastObstructingObjectRenderer != currentRenderer)
            {
                // Restablecer la opacidad del �ltimo objeto obstructor
                ResetObjectOpacity();

                // Almacenar el nuevo objeto obstructor y su color original
                lastObstructingObjectRenderer = currentRenderer;
                originalColor = currentRenderer.material.color;
            }

            // Hacer el objeto actual semi-transparente
            SetObjectOpacity(currentRenderer, 0.25f); // Ajusta el valor de transparencia seg�n sea necesario
        }
        else
        {
            // Si no hay obstrucci�n, restablecer la opacidad del �ltimo objeto obstructor
            ResetObjectOpacity();
        }
    }

    void SetObjectOpacity(Renderer renderer, float opacity)
    {
        Color color = renderer.material.color;
        color.a = opacity;
        renderer.material.color = color;
    }

    void ResetObjectOpacity()
    {
        if (lastObstructingObjectRenderer != null)
        {
            // Restablecer la opacidad al valor original
            SetObjectOpacity(lastObstructingObjectRenderer, originalColor.a);
            lastObstructingObjectRenderer = null; // Limpiar la referencia
        }
    }
}