using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Configuración pública (Libre a Modificar pero no las variables de raiz)
    public float tiempoInicial = 60f;
    public float tiempoMinimoReloj = 5f, tiempoMaximoReloj = 15f;
    public float limiteSuperiorTiempo = 90f, limiteInferiorTiempo = 30f;
    public float intervaloMinimoGeneracion = 5f, intervaloMaximoGeneracion = 10f;
    public TMP_Text textoReloj;
    public GameObject relojPrefab;
    public Transform[] puntosSpawn;

    // Variables privadas (Ten Cuidado de Tocarlas)
    private float tiempoRestante;
    private bool juegoActivo = true, temporizadorIniciado = false;
    private int relojesActivos = 0;
    private float tiempoUltimaGeneracion;
    private float siguienteIntervaloGeneracion;



    // Variables para la Regeneracion del Enemy 
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int minEnemies = 3, maxEnemies = 7;
    public float minSpawnInterval = 2f, maxSpawnInterval = 5f;
    public float startDelay = 10f;

    // Variables privadas
    private int currentEnemies = 0;
    private float nextSpawnTime;
    private bool juegoIniciado = false;

    public static GameManager Instance;
    private bool isTimeUp = false;
    // Variables para la vida del enemigo
    public int vidaMinimaEnemigo = 1;
    public int vidaMaximaEnemigo = 3;

    // Métodos para obtener la vida mínima y máxima del enemigo
    public int GetVidaMinimaEnemigo() => vidaMinimaEnemigo;
    public int GetVidaMaximaEnemigo() => vidaMaximaEnemigo;

    void Start()
    {
        tiempoRestante = tiempoInicial;
        ActualizarTextoReloj();
        siguienteIntervaloGeneracion = Random.Range(intervaloMinimoGeneracion, intervaloMaximoGeneracion);
        nextSpawnTime = Time.time + startDelay;
    }

    void Update()
    {
        //Temporizador 
        if (!temporizadorIniciado && JugadorSeMueve()) IniciarTemporizador();
        if (temporizadorIniciado && juegoActivo) ActualizarJuego();


        //Enemy 
        if (!juegoIniciado && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            juegoIniciado = true;
            nextSpawnTime = Time.time + startDelay;
        }

        // Generar enemigos si el juego ha comenzado
        if (juegoIniciado && Time.time >= nextSpawnTime)
        {
            GenerarOleadaEnemigos();
            nextSpawnTime = Time.time + Random.Range(minSpawnInterval, maxSpawnInterval);
        }

    }

    bool JugadorSeMueve() => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;

    void IniciarTemporizador()
    {
        temporizadorIniciado = true;
        tiempoUltimaGeneracion = Time.time;
        GenerarReloj();
    }

    void ActualizarJuego()
    {
        tiempoRestante -= Time.deltaTime;
        if (tiempoRestante <= 0) TerminarJuego();
        ActualizarTextoReloj();
        if (DebeGenerarReloj()) GenerarReloj();
    }

    bool DebeGenerarReloj() =>
        relojesActivos < 2 &&
        tiempoRestante > limiteInferiorTiempo &&
        tiempoRestante < limiteSuperiorTiempo &&
        Time.time - tiempoUltimaGeneracion >= siguienteIntervaloGeneracion;

    void GenerarReloj()
    {
        if (relojPrefab != null && puntosSpawn.Length > 0)
        {
            Transform puntoSpawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)];
            Instantiate(relojPrefab, puntoSpawn.position, Quaternion.identity).GetComponent<Reloj>().gameManager = this;
            relojesActivos++;
            tiempoUltimaGeneracion = Time.time;
            siguienteIntervaloGeneracion = Random.Range(intervaloMinimoGeneracion, intervaloMaximoGeneracion);
        }
    }
    void ActualizarTextoReloj() =>
        textoReloj.text = $"{Mathf.FloorToInt(tiempoRestante / 60):00}:{Mathf.FloorToInt(tiempoRestante % 60):00}";
    public void AumentarTiempo(float tiempoAñadido)
    {
        if (juegoActivo)
        {
            tiempoRestante += tiempoAñadido;
            Debug.Log($"Tiempo añadido: {tiempoAñadido} segundos. Tiempo restante: {tiempoRestante}");
            relojesActivos--;
        }
    }

    void Awake()
    {
        // Configurar el Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public float ObtenerTiempoRelojAleatorio() => Random.Range(tiempoMinimoReloj, tiempoMaximoReloj);
    void GenerarOleadaEnemigos()
    {
        int cantidadEnemigos = Random.Range(minEnemies, maxEnemies + 1);
        for (int i = 0; i < cantidadEnemigos; i++)
        {
            if (currentEnemies < maxEnemies)
            {
                GenerarEnemigo();
            }
        }
    }
    void GenerarEnemigo()
    {
        if (enemyPrefab != null && spawnPoints.Length > 0)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            currentEnemies++;
        }
    }
    public void EnemigoDerrotado()
    {
        currentEnemies--;
        ScoreManager.Instance.AddScore(13);
    }
    void TerminarJuego()
    {
        tiempoRestante = 0;
        juegoActivo = false;
        isTimeUp = true;
        Debug.Log("¡Tiempo agotado!");
        SceneManager.LoadScene("GameOver");
    }


    public void PlayerDied()
    {
        Debug.Log("El jugador ha muerto.");
        isTimeUp = false;
        SceneManager.LoadScene("GameOver");
    }
    public bool IsTimeUp()
    {
        return isTimeUp;

    }
}