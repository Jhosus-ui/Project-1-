using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float tiempoInicial = 60f;
    public float tiempoMinimoReloj = 5f, tiempoMaximoReloj = 15f;
    public float limiteSuperiorTiempo = 90f, limiteInferiorTiempo = 30f;
    public float intervaloMinimoGeneracion = 5f, intervaloMaximoGeneracion = 10f;
    public TMP_Text textoReloj;
    public TMP_Text textoOleada; 
    public TMP_Text textoOleadaTemporal; 
    public GameObject relojPrefab;
    public Transform[] puntosSpawn;

    public AudioClip timeAddedSound; 
    private AudioSource audioSource;
    public EnemySpawner enemySpawner;

    private float tiempoRestante;
    private bool juegoActivo = true, temporizadorIniciado = false;
    private int relojesActivos = 0;
    private float tiempoUltimaGeneracion;
    private float siguienteIntervaloGeneracion;

    public static GameManager Instance;
    private bool isTimeUp = false;

    void Awake()
    {
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
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
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

    bool DebeGenerarReloj() =>  //Experiemtando con Youtube 
        relojesActivos < 2 &&
        tiempoRestante > limiteInferiorTiempo &&
        tiempoRestante < limiteSuperiorTiempo &&
        Time.time - tiempoUltimaGeneracion >= siguienteIntervaloGeneracion;

    void GenerarReloj()
    {
        if (relojPrefab != null && puntosSpawn.Length > 0)
        {
            Transform puntoSpawn = puntosSpawn[Random.Range(0, puntosSpawn.Length)]; 
            if (!IsPositionOccupied(puntoSpawn.position)) // Hay que saber si está ocupada antes de generar el reloj
            {
                Instantiate(relojPrefab, puntoSpawn.position, Quaternion.identity).GetComponent<Reloj>().gameManager = this;
                relojesActivos++;
                tiempoUltimaGeneracion = Time.time;
                siguienteIntervaloGeneracion = Random.Range(intervaloMinimoGeneracion, intervaloMaximoGeneracion);
            }
        }
    }

    void ActualizarTextoReloj() =>
        textoReloj.text = $"{Mathf.FloorToInt(tiempoRestante / 60):00}:{Mathf.FloorToInt(tiempoRestante % 60):00}";

    public void AumentarTiempo(float tiempoAñadido)
    {
        if (juegoActivo)
        {
            tiempoRestante += tiempoAñadido;  //Debug.Log($"Tiempo añadido: {tiempoAñadido} segundos. Tiempo restante: {tiempoRestante}");
            relojesActivos--;
            if (timeAddedSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(timeAddedSound);
            }
        }
    }

    public void MostrarTextoOleadaTemporal(string mensaje)
    {
        textoOleadaTemporal.text = mensaje;
        Invoke("LimpiarTextoOleadaTemporal", 3f); // Desaparecer Texto
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
        SceneManager.LoadScene("Game Over");
    }

    public void PlayerDied()
    {
        Debug.Log("El jugador ha muerto.");
        isTimeUp = false;
        SceneManager.LoadScene("Game Over");
    }

    public bool IsTimeUp()
    {
        return isTimeUp;
    }

    public float ObtenerTiempoRelojAleatorio()
    {
        return Random.Range(tiempoMinimoReloj, tiempoMaximoReloj);
    }

    private bool IsPositionOccupied(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, 5f); // Radio de  unidades
        return colliders.Length > 0; // Si hay colisiones, la posición está ocupada
    }
}