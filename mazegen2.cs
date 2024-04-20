using UnityEngine;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour
{
    public GameObject wallPrefab;
    public int width = 10;
    public int height = 10;
    public GameObject wallWithLightPrefab;

    public GameObject pitPrefab; // Asigna tu prefab del foso aquí en el Inspector
    private float pitProbability = 0.1f; // Probabilidad de que una celda contenga un foso

    private bool[,] visitedCells;
    private System.Random rng = new System.Random();

    private bool[,] pitLocations; // matriz de lacalizacion de fosos 

    public GameObject turretPrefab; // Asigna tu prefab de la torreta aquí en el Inspector


    bool[,] paredNorte;
    bool[,] paredSur;
    bool[,] paredEste;
    bool[,] paredOeste;


    List<GameObject> instancedTurrets = new List<GameObject>();


    public GameObject coinPrefab; // Asigna tu prefab de la moneda aquí en el Inspector
    private float coinPlacementProbability = 0.1f; // Probabilidad de colocar una moneda en una celda

    void Start()
    {
        InitializeMaze();
        GenerateMaze();
       
        // InstantiateMaze(); // Esta función ya no es necesaria si colocas las paredes en InitializeMaze
    }

    void InitializeMaze()
    {
        visitedCells = new bool[width, height];
        pitLocations = new bool[width, height]; // Inicializar la matriz de fosos
        paredNorte = new bool[width, height];
        paredSur = new bool[width, height];
        paredEste = new bool[width, height];
        paredOeste = new bool[width, height];
        // Colocar las paredes alrededor de cada celda
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                visitedCells[x, y] = false;
                paredNorte[x, y] = true;
                paredSur[x, y] = true;
                paredEste[x, y] = true;
                paredOeste[x, y] = true;
                CreateCellWalls(x, y);
            }
        }
    }
    bool ShouldWallHaveLight(int x, int y)
    {
        // Por ejemplo, puedes hacer que una de cada cinco paredes tenga luz.
        // Ajusta esta lógica según tus necesidades.
        return rng.Next(10) == 0;
    }
    void CreateCellWalls(int x, int y)
    {
        float wallLength = wallPrefab.transform.localScale.x;
        float wallHeight = wallPrefab.transform.localScale.y;

        // Decide si esta pared específica debería tener luz.
        bool shouldHaveLight = ShouldWallHaveLight(x, y);

        // Pared superior, excepto en la celda de salida
        if (!(x == width - 1 && y == height - 1))
        {
            GameObject prefabToInstantiate = shouldHaveLight ? wallWithLightPrefab : wallPrefab;
            Instantiate(prefabToInstantiate, new Vector3(x * wallLength, wallHeight / 2, (y + 0.5f) * wallLength), Quaternion.identity, transform);
        }

        // Pared derecha, excepto en la celda de salida
        if (!(x == width - 1 && y == height - 1))
        {
            GameObject prefabToInstantiate = shouldHaveLight ? wallWithLightPrefab : wallPrefab;
            Instantiate(prefabToInstantiate, new Vector3((x + 0.5f) * wallLength, wallHeight / 2, y * wallLength), Quaternion.Euler(0, 90, 0), transform);
        }

        // Añadir paredes en los límites exteriores del laberinto
        if (y == 0 && x != 0) // Excluir la pared de entrada
        {
            // Pared inferior
            Instantiate(wallPrefab, new Vector3(x * wallLength, wallHeight / 2, (y - 0.5f) * wallLength), Quaternion.identity, transform);
        }
        if (x == 0 && y != 0) // Excluir la pared de entrada
        {
            // Pared izquierda
            Instantiate(wallPrefab, new Vector3((x - 0.5f) * wallLength, wallHeight / 2, y * wallLength), Quaternion.Euler(0, 90, 0), transform);
        }

        if (!IsCornerCell(x, y) && !HasNearbyPit(x, y) && rng.NextDouble() < pitProbability && !(x == 0 && y == 0)) // Evita la celda de inicio
        {
            // Ajusta la posición y la rotación del foso
            Vector3 pitPosition = new Vector3(x, -0.91f, y); // Baja el foso 0.5 unidades
            Quaternion pitRotation = Quaternion.Euler(0, 90, 90); // Rota el foso 90 grados en el eje Y

            GameObject pit = Instantiate(pitPrefab, pitPosition, pitRotation, transform);
            pit.name = $"Pit_{x}_{y}"; // Asigna un nombre único
            pitLocations[x, y] = true; // Registrar la ubicación del foso
        }

    }

    void GenerateMaze()
    {
        Vector2Int currentCell = new Vector2Int(rng.Next(width), rng.Next(height));
        int visitedCellsCount = 1;
        int totalCells = width * height;

        visitedCells[currentCell.x, currentCell.y] = true;

        while (visitedCellsCount < totalCells)
        {
            List<Vector2Int> neighbors = GetNeighbours(currentCell);
            Vector2Int neighbor = neighbors[rng.Next(neighbors.Count)];

            // Si la celda vecina no ha sido visitada, elimina la pared
            if (!visitedCells[neighbor.x, neighbor.y])
            {
                RemoveWallBetweenCells(currentCell, neighbor);
                visitedCellsCount++;
            }

            // Mueve a la celda vecina
            currentCell = neighbor;
            visitedCells[currentCell.x, currentCell.y] = true;
        }
        PlaceCoins();
        Debug.Log("Generación de laberinto completada.");

        // Después de generar el laberinto, imprime el número de celdas visitadas y potenciales callejones sin salida para debugging
        int countDeadEnds = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (EsCallejonSinSalida(x, y))
                {
                    countDeadEnds++;
                   
                }
            }
        }

        // Luego llama a PlaceTurrets para colocar las torretas basándose en esta información
        PlaceTurrets();
    }

    List<Vector2Int> GetNeighbours(Vector2Int cell)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        if (cell.x > 0) neighbours.Add(new Vector2Int(cell.x - 1, cell.y));
        if (cell.y > 0) neighbours.Add(new Vector2Int(cell.x, cell.y - 1));
        if (cell.x < width - 1) neighbours.Add(new Vector2Int(cell.x + 1, cell.y));
        if (cell.y < height - 1) neighbours.Add(new Vector2Int(cell.x, cell.y + 1));

        return neighbours;
    }

    void RemoveWallBetweenCells(Vector2Int current, Vector2Int next)
    {
        // La posición de la pared a eliminar
        Vector3 wallPosition = GetWallPositionBetweenCells(current, next);

        // Tamaño de la OverlapBox - ajusta estos valores si tus paredes son más grandes o más pequeñas
        Vector3 boxSize = new Vector3(0.1f, 0.1f, 0.1f);

        // Encuentra la pared en esa posición y destrúyela
        Collider[] colliders = Physics.OverlapBox(wallPosition, boxSize);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.CompareTag("Wall")|| (collider.gameObject.CompareTag("Wall2")))
            {
                Destroy(collider.gameObject); // Elimina el GameObject de la pared
                break; // Rompe el ciclo si encontraste y eliminaste una pared
            }
        }
        // Aquí, necesitarías lógica para determinar la orientación de la pared a eliminar entre 'current' y 'next'
        // y luego actualizar las matrices de estado de paredes correspondientes.
        if (current.x == next.x)
        {
            if (current.y < next.y) // Norte
            {
                paredNorte[current.x, current.y] = false;
                paredSur[next.x, next.y] = false;
            }
            else // Sur
            {
                paredSur[current.x, current.y] = false;
                paredNorte[next.x, next.y] = false;
            }
        }
        else if (current.y == next.y)
        {
            if (current.x < next.x) // Este
            {
                paredEste[current.x, current.y] = false;
                paredOeste[next.x, next.y] = false;
            }
            else // Oeste
            {
                paredOeste[current.x, current.y] = false;
                paredEste[next.x, next.y] = false;
            }
        }
    }

    Vector3 GetWallPositionBetweenCells(Vector2Int current, Vector2Int next)
    {
        // Asumiendo que cada celda es de 1x1 unidad y las paredes están en el borde entre celdas
        Vector3 position = new Vector3((current.x + next.x) / 2.0f, 0, (current.y + next.y) / 2.0f);
         
        // Ajusta este vector según la posición real y el tamaño de tus paredes
        return position;
    }

    bool IsCornerCell(int x, int y)
    {
        return (x == 0 || x == width - 1) && (y == 0 || y == height - 1);
    }

    bool HasNearbyPit(int x, int y)
    {
        // Verifica las celdas vecinas directamente adyacentes para fosos
        int[] dx = { 1, -1, 0, 0 }; // Desplazamientos en x
        int[] dy = { 0, 0, 1, -1 }; // Desplazamientos en y

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];

            // Verifica los límites y si la celda vecina tiene un foso
            if (nx >= 0 && nx < width && ny >= 0 && ny < height && pitLocations[nx, ny])
            {
                return true; // Hay un foso cercano
            }
        }

        return false; // No hay fosos cercanos
    }

    void PlaceTurrets()
    {
        float probabilidadDeColocarTorreta = 0.125f; // Ajusta a tus necesidades.

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (EsCallejonSinSalida(x, y) && !IsCornerCell(x, y))
                {
                    if (rng.NextDouble() < probabilidadDeColocarTorreta)
                    {
                        Vector3 position = new Vector3(x, 0, y);
                        GameObject turret = Instantiate(turretPrefab, position, Quaternion.identity, transform);
                        instancedTurrets.Add(turret);
                    }
                }
            }
        }
        OrientTurrets(instancedTurrets);
    }

    bool EsCallejonSinSalida(int x, int y)
    {
        int cantidadParedes = 0;

        // Verifica las paredes alrededor de la celda
        if (paredNorte[x, y]) cantidadParedes++;
        if (paredSur[x, y]) cantidadParedes++;
        if (paredEste[x, y]) cantidadParedes++;
        if (paredOeste[x, y]) cantidadParedes++;

        // Un callejón sin salida tiene 3 paredes alrededor
        return cantidadParedes == 3;
    }

    void OrientTurrets(List<GameObject> turrets)
    {
    foreach (GameObject turret in turrets)
      {
            Debug.Log("Lanzada orientacion");
        // Asume que TurretScript tiene un método para iniciar la orientación.
        turret.GetComponent<TurretScript>().OrientTurret();
      }
    }
    void PlaceCoins()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Asegúrate de no colocar monedas en la celda de inicio o en los fosos
                if (!(x == 0 && y == 0) && !pitLocations[x, y])
                {
                    if (Random.value < coinPlacementProbability)
                    {
                        // Ajusta la posición de la moneda si es necesario, por ejemplo, elevándola un poco sobre el suelo
                        Vector3 coinPosition = new Vector3(x, 0.5f, y);
                        Instantiate(coinPrefab, coinPosition, Quaternion.identity, transform);
                    }
                }
            }
        }
    }
}
