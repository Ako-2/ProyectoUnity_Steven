using UnityEngine;
using System.Collections;

public class MobileController : MonoBehaviour
{
    public CamJugador3D camJugador3D;
    private BoredomManager boredomManager; // referencia automática al manager

    [Header("Referencias")]
    public Transform mobileTransform; // asigna el Transform del prefab/modelo del móvil (hijo)

    [Header("Posiciones locales (si dejas vacío se calcularán al Start)")]
    public Vector3 hiddenLocalPos = Vector3.zero; // si es Vector3.zero y Start detecta que no lo cambias, lo sobreescribe
    public Vector3 shownLocalPos = Vector3.zero;
    public float moveSpeed = 9f;

    [HideInInspector] public bool IsVisible = false;

    private Coroutine moveRoutine;
    private bool positionsInitialized = false;

    void Start()
    {
         StartCoroutine(InitializeReferences());
        // Buscamos automáticamente el BoredomManager en la escena
        boredomManager = FindObjectOfType<BoredomManager>();

        // si no asignas, usamos el mismo objeto

        // Si no has puesto posiciones manuales (siguen en zero), las inferimos:
        if (shownLocalPos == Vector3.zero && hiddenLocalPos == Vector3.zero)
        {
            // Guardamos la posición actual como shown, y hidden la ponemos abajo fuera de vista
            shownLocalPos = mobileTransform.localPosition;
            hiddenLocalPos = shownLocalPos + new Vector3(0f, -4f, 0f); // mover -4 unidades locales hacia abajo por defecto
        }
        else
        {
            // Si sólo una está en zero, respetamos la otra y no tocamos
            if (shownLocalPos == Vector3.zero)
                shownLocalPos = mobileTransform.localPosition;
            if (hiddenLocalPos == Vector3.zero)
                hiddenLocalPos = shownLocalPos + new Vector3(0f, -4f, 0f);
        }

        // Inicializamos el móvil en hidden (por defecto)
        mobileTransform.localPosition = hiddenLocalPos;
        IsVisible = false;
        positionsInitialized = true;
    }
    
    private IEnumerator InitializeReferences()
{
    yield return null; // espera un frame para asegurar que todo esté cargado

    if (camJugador3D == null)
    {
        camJugador3D = FindObjectOfType<CamJugador3D>();
        if (camJugador3D == null)
            Debug.LogWarning("❌ camJugador3D sigue sin encontrarse. Asegúrate de que CamJugador3D está en la Main Camera.");
    }

    if (boredomManager == null)
        boredomManager = FindObjectOfType<BoredomManager>();

    if (mobileTransform == null)
        mobileTransform = transform;

    if (shownLocalPos == Vector3.zero && hiddenLocalPos == Vector3.zero)
    {
        shownLocalPos = mobileTransform.localPosition;
        hiddenLocalPos = shownLocalPos + new Vector3(0f, -4f, 0f);
    }
    else
    {
        if (shownLocalPos == Vector3.zero)
            shownLocalPos = mobileTransform.localPosition;
        if (hiddenLocalPos == Vector3.zero)
            hiddenLocalPos = shownLocalPos + new Vector3(0f, -4f, 0f);
    }

    mobileTransform.localPosition = hiddenLocalPos;
    IsVisible = false;
    positionsInitialized = true;

    Debug.Log("✅ MobileController inicializado correctamente.");
}

    public void ShowMobile()
    {
        if (!positionsInitialized) Start(); // por seguridad
        // no mostrar si ya visible
        if (IsVisible) return;
        IsVisible = true;
        StartMove(shownLocalPos);
        if (boredomManager != null)
            boredomManager.OnMobileShown();
        if (camJugador3D != null)
    {
        camJugador3D.conMovil = true;
        Debug.Log("📱 conMovil = TRUE");
    }

    }

    public void HideMobile()
    {
        if (!positionsInitialized) Start();
        if (!IsVisible) return;
        IsVisible = false;
        StartMove(hiddenLocalPos);
        if (boredomManager != null)
            boredomManager.OnMobileHidden();
        if (camJugador3D != null)
    {
        camJugador3D.conMovil = false;
        Debug.Log("📵 conMovil = FALSE");
    }
    }

    private void StartMove(Vector3 target)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveToLocal(target));
    }

    private IEnumerator MoveToLocal(Vector3 target)
    {
        while (Vector3.Distance(mobileTransform.localPosition, target) > 0.04f)
        {
            mobileTransform.localPosition = Vector3.Lerp(mobileTransform.localPosition, target, Time.deltaTime * moveSpeed);
            yield return null;
        }
        mobileTransform.localPosition = target;
        moveRoutine = null;
    }
}
