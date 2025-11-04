using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainGameManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public TMP_Text horaTexto;
    public TMP_Text vidasTexto;
    public GameObject panelVictoria;
    public GameObject panelDerrota;

    [Header("Config Juego")]
    public int vidasMax = 3;
    public float segundosPorHora = 5f; // cuanto dura una hora en tiempo real
    private int vidasActual;
    private int horaActual = 15;
    private int horaFinal = 22;
    private float timerHora;
    private bool juegoActivo = true;

    void Start()
    {
        vidasActual = vidasMax;
        ActualizarUI();
        if (panelVictoria) panelVictoria.SetActive(false);
        if (panelDerrota) panelDerrota.SetActive(false);
    }

    void Update()
    {
        if (!juegoActivo) return;

        timerHora += Time.deltaTime;
        if (timerHora >= segundosPorHora)
        {
            timerHora = 0f;
            horaActual++;
            if (horaActual >= horaFinal)
            {
                // Ganó el día
                Victoria();
                return;
            }
            ActualizarUI();
        }
    }

    void ActualizarUI()
    {
        if (horaTexto) horaTexto.text = $"{horaActual:00}:00";
        if (vidasTexto) vidasTexto.text = $"Numero de CCC: {vidasActual}/{vidasMax}";
    }

    // llamado desde ZonaVision cuando te pillan con el móvil
    public void QuitarVida()
    {
        if (!juegoActivo) return;

        vidasActual--;
        ActualizarUI();

        if (vidasActual <= 0)
        {
            Derrota();
        }
    }

    void Victoria()
    {
        juegoActivo = false;
        if (panelVictoria) panelVictoria.SetActive(true);

        // Guardar progreso del día
GameManager2D gm = Object.FindFirstObjectByType<GameManager2D>();
        if (gm != null)
        {
            if (GameManager2D.instance != null) GameManager2D.instance.AvanzarDia();
        }

        // Cambiar a MiniJuego1 después de unos segundos
        StartCoroutine(IrAMiniJuego());
    }

    public void Derrota()
    {
        juegoActivo = false;
        if (panelDerrota) panelDerrota.SetActive(true);

GameManager2D gm = Object.FindFirstObjectByType<GameManager2D>();
        if (GameManager2D.instance != null)
        {
            GameManager2D.instance.ReiniciarMonedas();
            GameManager2D.instance.ReiniciarDias();
        }

        Debug.Log("💀 Has perdido → se reinician monedas y días");
    }

    IEnumerator IrAMiniJuego()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("MiniJuego1");
    }

    // botón del panel derrota
    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
