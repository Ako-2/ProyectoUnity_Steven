using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D instance;

    [Header("UI")]
    public TMP_Text diaTexto;
    public TMP_Text monedasTexto;

    [Header("Moneda")]
    public GameObject monedaPrefab;
    public Transform[] posicionesDias;

    [Header("Config")]
    public float tiempoMostrarDia = 2f;

    private int diaActual;
    private int monedas;
    private GameObject monedaInstanciada;

    void Awake()
    {
        // Singleton simple
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
        if (FindObjectsOfType<GameManager2D>().Length > 1)
{
    Destroy(gameObject);
    return;
}
DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        // Recuperar datos guardados
        diaActual = PlayerPrefs.GetInt("DiaActual", 1);
        monedas = PlayerPrefs.GetInt("Monedas", 0);

        // Mostrar día actual
        StartCoroutine(MostrarDia());

        // Crear la moneda según el día
        CrearMoneda();

        // Actualizar contador de monedas
        ActualizarMonedasUI();
    }

    IEnumerator MostrarDia()
    {
        if (diaTexto != null)
        {
            diaTexto.text = "DIA " + diaActual;
            diaTexto.gameObject.SetActive(true);
            yield return new WaitForSeconds(tiempoMostrarDia);
            diaTexto.gameObject.SetActive(false);
        }
    }

    void CrearMoneda()
{
    // 🔹 Si la moneda anterior existe, destrúyela de forma segura
    if (monedaInstanciada != null)
    {
        Destroy(monedaInstanciada);
        monedaInstanciada = null;
    }

    // 🔹 Verificar que el array tenga posiciones válidas
    int index = diaActual - 1;

    if (posicionesDias == null || posicionesDias.Length == 0)
    {
        Debug.LogWarning("⚠️ No hay posiciones de días configuradas en el GameManager2D.");
        return;
    }

    if (index >= 0 && index < posicionesDias.Length && posicionesDias[index] != null)
    {
        // 🔹 Crear nueva moneda solo si la posición existe
        monedaInstanciada = Instantiate(monedaPrefab, posicionesDias[index].position, Quaternion.identity);
    }
    else
    {
        Debug.LogWarning($"⚠️ No hay posición definida o fue destruida para el día {diaActual}");
    }
}


    public void RecogerMoneda()
    {
        monedas++;
        PlayerPrefs.SetInt("Monedas", monedas);
        PlayerPrefs.Save();
        ActualizarMonedasUI();
    }

    void ActualizarMonedasUI()
    {
        if (monedasTexto != null)
            monedasTexto.text = "Monedas: " + monedas;
    }

    // Llamar cuando GANAS
    public void AvanzarDia()
    {
        if (diaActual < 6)
            diaActual++;
        else
            diaActual = 1; // opcional: si quieres que vuelva al 1 tras completar 6

        PlayerPrefs.SetInt("DiaActual", diaActual);
        PlayerPrefs.Save();

        // actualizar UI y moneda para el nuevo día (si estás en MainGame)
        ActualizarMonedasUI();
        CrearMoneda(); // recrea la moneda en la nueva posición si hace falta
        // Si quieres mostrar el texto "DIA X" de nuevo:
        StartCoroutine(MostrarDia());
    }

    // Llamar cuando PIERDES
    public void ReiniciarMonedas()
    {
        monedas = 0;
        PlayerPrefs.SetInt("Monedas", 0);
        PlayerPrefs.Save();
        ActualizarMonedasUI();
    }

    public void ReiniciarDias()
    {
        diaActual = 1;
        PlayerPrefs.SetInt("DiaActual", 1);
        PlayerPrefs.Save();

        // actualizar estado
        StartCoroutine(MostrarDia());
        CrearMoneda();
    }

    // útiles por si llamas desde otras escenas inmediatamente
    private void OnEnable()
    {
        // Si cambias de escena y necesitas refrescar referencias UI asignadas por escena,
        // puedes hacerlo aquí (opcional). Lo dejamos vacío salvo que lo necesites.
    }
}
