using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    [Header("Referencias")]
    public Camera mainCamera;             
    public MobileController mobile;       
    public BoredomManager boredomManager; 

    [Header("Control de 'cabeza' (rotación de cámara)")]
    public float maxYawDegrees = 45f;     
    public float smoothTime = 0.12f;      
    public float lookBackYaw = 180f;      
    [Range(0.9f, 1f)] public float lookBackThresholdNormalized = 0.98f; 

    [Header("Zonas")]
    [Range(0f, 0.5f)] public float bottomZone = 0.15f; 

    // estado interno
    private Quaternion initialCameraLocalRot;
    private Quaternion targetRotation;
    private bool isLookingBack = false;

    // --- nuevo para toggle del móvil ---
    private bool wasInBottomZone = false; // para detectar flanco
    private bool isMobileOut = false;     // estado actual del móvil

    void Start()
    {
        if (mainCamera == null)
        {
            if (Camera.main != null) mainCamera = Camera.main;
            else Debug.LogWarning("[CameraMouseController] mainCamera no asignada y Camera.main es null.");
        }

        if (mainCamera != null)
            initialCameraLocalRot = mainCamera.transform.localRotation;
        else
            initialCameraLocalRot = Quaternion.identity;

        targetRotation = initialCameraLocalRot;
    }

    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        float screenW = Screen.width;
        float screenH = Screen.height;

        float nx = (mouse.x / screenW - 0.5f) * 2f;
        nx = Mathf.Clamp(nx, -1f, 1f);

        // ---------- LOOK BACK (extremo derecho) ----------
        bool wantLookBack = (mouse.x / screenW) >= lookBackThresholdNormalized;

        if (wantLookBack)
        {
            float extraYaw = nx * (maxYawDegrees * 0.2f);
            targetRotation = initialCameraLocalRot * Quaternion.Euler(0f, lookBackYaw + extraYaw, 0f);

            if (mobile != null && mobile.IsVisible)
            {
                mobile.HideMobile();
                boredomManager?.OnMobileHidden();
                isMobileOut = false;
            }

            isLookingBack = true;
        }
        else
        {
            float yaw = nx * maxYawDegrees;
            targetRotation = initialCameraLocalRot * Quaternion.Euler(0f, yaw, 0f);
            isLookingBack = false;
        }

        // Suavizado exponencial
        float t = 1f - Mathf.Exp(-20f * Time.deltaTime / Mathf.Max(0.0001f, smoothTime));
        if (mainCamera != null)
            mainCamera.transform.localRotation = Quaternion.Slerp(mainCamera.transform.localRotation, targetRotation, t);

        // ---------- TOGGLE del móvil al entrar en bottomZone ----------
        bool inBottomZone = mouse.y < screenH * bottomZone;

        // detectamos cuando el ratón ENTRA en la zona inferior (flanco descendente)
        if (inBottomZone && !wasInBottomZone && !isLookingBack)
        {
            // toggle móvil
            if (isMobileOut)
            {
                mobile?.HideMobile();
                boredomManager?.OnMobileHidden();
                isMobileOut = false;
                
            }
            else
            {
                mobile?.ShowMobile();
                boredomManager?.OnMobileShown();
                isMobileOut = true;
            }
        }

        // actualizamos estado previo para siguiente frame
        wasInBottomZone = inBottomZone;
    }
}
