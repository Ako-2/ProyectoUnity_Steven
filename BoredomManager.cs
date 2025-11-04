using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoredomManager : MonoBehaviour
{
    private MainGameManager gameManager;

    [Header("UI")]
    public Slider boredomSlider;      // Asigna tu Slider aquí
    public TMP_Text textPercent;      // Texto de porcentaje (TextMeshPro)
    public TMP_Text titleText;        // Texto “Diversión”

    [Header("Parámetros de velocidad")]
    [Tooltip("Velocidad a la que baja la diversión (sin móvil)")]
    public float decreaseRate = 2f;
    [Tooltip("Velocidad a la que sube la diversión (con móvil)")]
    public float increaseRate = 6f;
    [Tooltip("Valor máximo de diversión")]
    public float maxValue = 100f;

    [Header("Opciones visuales")]
    public bool colorChange = true; // cambia color según valor

    private float current;
    private bool mobileVisible = false;
    private Image fillImage; // para cambiar color dinámicamente

    void Start()
    {
        // Asignar valores iniciales
        current = maxValue;

        if (boredomSlider == null)
        {
            Debug.LogError("❌ [BoredomManager] Falta asignar el Slider en el Inspector.");
            return;
        }

        boredomSlider.minValue = 0f;
        boredomSlider.maxValue = maxValue;
        boredomSlider.value = current;

        // Guardar referencia al Fill del Slider
        if (boredomSlider.fillRect != null)
            fillImage = boredomSlider.fillRect.GetComponent<Image>();

        if (titleText != null)
            titleText.text = "Diversión";

        UpdateUI();
        gameManager = FindObjectOfType<MainGameManager>();

    }

    void Update()
    {
        float delta = Time.deltaTime;

        // Si el móvil está visible, sube; si no, baja
        if (mobileVisible)
            current += increaseRate * delta;
        else
            current -= decreaseRate * delta;

        current = Mathf.Clamp(current, 0f, maxValue);

        // Si llega a 0% de diversión → perder partida
if (current <= 0f && gameManager != null)
{
    gameManager.Derrota();
    return; // para no seguir actualizando
}


        boredomSlider.value = current;

        UpdateUI();
    }

    // ===========================
    // 📱 LLAMADOS DESDE CameraMouseController
    // ===========================

    public void OnMobileShown()
    {
        mobileVisible = true;
        Debug.Log("📱 Móvil mostrado → diversión subiendo");
    }

    public void OnMobileHidden()
    {
        mobileVisible = false;
        Debug.Log("📱 Móvil oculto → diversión bajando");
    }

    // ===========================
    // 🎨 ACTUALIZAR INTERFAZ
    // ===========================
    private void UpdateUI()
    {
        // Porcentaje numérico
        if (textPercent != null)
            textPercent.text = Mathf.RoundToInt((current / maxValue) * 100f) + "%";

        // Color dinámico (opcional)
        if (colorChange && fillImage != null)
        {
            Color full = Color.green;
            Color mid = new Color(1f, 0.8f, 0f);
            Color low = Color.red;

            float ratio = current / maxValue;
            if (ratio > 0.5f)
                fillImage.color = Color.Lerp(mid, full, (ratio - 0.5f) * 2f);
            else
                fillImage.color = Color.Lerp(low, mid, ratio * 2f);
        }
    }
}
