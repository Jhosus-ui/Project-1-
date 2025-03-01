using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Configuración pública
    public float tiempoInicial = 60f;
    public float tiempoMinimoReloj = 5f, tiempoMaximoReloj = 15f;
    public float limiteSuperiorTiempo = 90f, limiteInferiorTiempo = 30f;
    public float intervaloMinimoGeneracion = 5f, intervaloMaximoGeneracion = 10f;
    public TMP_Text textoReloj;
    public TMP_Text textoOleada; // Texto permanente para mostrar "Round: X"
    public TMP_Text textoOleadaTemporal; // Texto temporal para mostrar "Ronda X"
    public GameObject relojPrefab;
    public Transform[] puntosSpawn;

    // Referencia al EnemySpawner
    public EnemySpawner enemySpawner;

    // Variables privadas
    private float tiempoRestante;
    private bool juegoActivo = true, temporizadorIniciado = false;
    private int relojesActivos = 0;
    private float tiempoUltimaGeneracion;
    private float siguienteIntervaloGeneracion;

    public static GameManager Instance;
    private bool isTimeUp = false;

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

    void Start()
    {
        tiempoRestante = tiempoInicial;
        ActualizarTextoReloj();
        siguienteIntervaloGeneracion = Random.Range(intervaloMinimoGeneracion, intervaloMaximoGeneracion);
    }

    void Update()
    {
        // Temporizador
        if (!temporizadorIniciado && JugadorSeMueve()) IniciarTemporizador();
        if (temporizadorIniciado && juegoActivo) ActualizarJuego();
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

    public void MostrarTextoOleadaTemporal(string mensaje)
    {
        textoOleadaTemporal.text = mensaje;
        Invoke("LimpiarTextoOleadaTemporal", 3f); // Limpiar el texto después de 3 segundos
    }

    void LimpiarTextoOleadaTemporal()
    {
        textoOleadaTemporal.text = "";
    }

    public void ActualizarTextoOleada(string mensaje)
    {
        textoOleada.text = mensaje;
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

    public float ObtenerTiempoRelojAleatorio()
    {
        return Random.Range(tiempoMinimoReloj, tiempoMaximoReloj);
    }
}