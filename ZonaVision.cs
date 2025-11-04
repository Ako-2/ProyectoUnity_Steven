using UnityEngine;

public class ZonaVision : MonoBehaviour
{
    public TipoProfesor tipo;
    public Transform camJugador; // arrastra la Main Camera (jugador)
    public float tiempoEntreDetecciones = 2f; // segundos entre restas de vida

    private float ultimoTiempoDeteccion = -999f;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("MainCamera")) return;

        if (camJugador == null) camJugador = other.transform;
        CamJugador3D cam = camJugador.GetComponent<CamJugador3D>();
        if (cam == null) return;

        // Solo detecta si lleva el móvil
        if (cam.conMovil)
        {
            // Verifica si ha pasado el tiempo de cooldown
            if (Time.time - ultimoTiempoDeteccion >= tiempoEntreDetecciones)
            {
                ultimoTiempoDeteccion = Time.time;

                Debug.Log($"📱 CCC detectado por profesor {tipo} → restar 1 vida");

                MainGameManager main = FindObjectOfType<MainGameManager>();
                if (main != null)
                {
                    main.QuitarVida();
                }
            }
        }
    }
}
