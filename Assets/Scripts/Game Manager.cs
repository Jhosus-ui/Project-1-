using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public float tiempoInicial = 60f; 
    public float tiempoMinimoReloj = 5f; 
    public float tiempoMaximoReloj = 15f; 
    public TMP_Text textoReloj; 

    
    private float tiempoRestante; 
    private bool juegoActivo = true; 
    private bool temporizadorIniciado = false; 

    void Start()
    {
        tiempoRestante = tiempoInicial; 
        ActualizarTextoReloj(); 
    }

    void Update()
    {
        if (!temporizadorIniciado && (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0))
        {
            temporizadorIniciado = true; 
        }

        if (temporizadorIniciado && juegoActivo)
        {
            tiempoRestante -= Time.deltaTime; 
            if (tiempoRestante <= 0)
            {
                tiempoRestante = 0;
                juegoActivo = false;
                Debug.Log("¡Tiempo agotado!"); 
            }
            ActualizarTextoReloj(); 
        }
    }

    void ActualizarTextoReloj()
    {
        if (textoReloj != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoReloj.text = $"{minutos:00}:{segundos:00}"; 
        }
    }

    public void AumentarTiempo(float tiempoAñadido)
    {
        if (juegoActivo)
        {
            tiempoRestante += tiempoAñadido;
            Debug.Log($"Tiempo añadido: {tiempoAñadido} segundos. Tiempo restante: {tiempoRestante}");
        }
    }

    public float ObtenerTiempoRelojAleatorio() => Random.Range(tiempoMinimoReloj, tiempoMaximoReloj);
}