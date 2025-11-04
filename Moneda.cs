using UnityEngine;

public class Moneda : MonoBehaviour
{
    private int diaActual;
    private Transform objetivoActual;
    private bool recogida = false;

    void Start()
    {
        // Recuperamos el día actual desde PlayerPrefs
        diaActual = PlayerPrefs.GetInt("DiaActual", 1);

        // Buscamos el GameObject vacío correspondiente (Dia1, Dia2, Dia3…)
        GameObject destino = GameObject.Find("Dia" + diaActual);

        if (destino != null)
        {
            objetivoActual = destino.transform;
            transform.position = objetivoActual.position;
            recogida = false;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró el objeto 'Dia{diaActual}' en la escena.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (recogida) return;

        if (other.CompareTag("Player"))
        {
            recogida = true;

            Debug.Log("💰 Moneda recogida en el Día " + diaActual);

            // Intentamos notificar al GameManager2D para que aumente el contador
            // Usamos las funciones modernas si están disponibles y fallback si no.
            GameManager2D gm = null;
#if UNITY_2023_1_OR_NEWER
            gm = Object.FindFirstObjectByType<GameManager2D>();
#else
            gm = FindObjectOfType<GameManager2D>();
#endif
            if (gm != null)
            {
                // Usa el método que tengas (RecogerMoneda existe en tu GameManager2D)
                gm.RecogerMoneda();
            }
            else
            {
                // Fallback: incrementar en PlayerPrefs por si no hay GameManager presente
                int monedas = PlayerPrefs.GetInt("Monedas", 0);
                monedas++;
                PlayerPrefs.SetInt("Monedas", monedas);
                PlayerPrefs.Save();
                Debug.Log("💾 GameManager2D no encontrado: incrementado Monedas en PlayerPrefs a " + monedas);
            }

            // Ocultamos la moneda hasta el siguiente día (teletransportar fuera de la vista)
            transform.position = new Vector3(9999f, 9999f, 0f);
        }
    }

    // Método público para forzar que la moneda se mueva a la posición del día actual
    // (si quieres llamarlo desde GameManager2D después de AvanzarDia)
    public void ActualizarPosicionSegunDia()
    {
        diaActual = PlayerPrefs.GetInt("DiaActual", 1);
        GameObject destino = GameObject.Find("Dia" + diaActual);
        if (destino != null)
        {
            transform.position = destino.transform.position;
            recogida = false;
        }
        else
        {
            Debug.LogWarning($"⚠️ No se encontró el objeto 'Dia{diaActual}' al actualizar posición.");
        }
    }

    // Método público para reiniciar (si se pierde)
    public void Reiniciar()
    {
        PlayerPrefs.SetInt("DiaActual", 1);
        PlayerPrefs.SetInt("Monedas", 0);
        PlayerPrefs.Save();
        ActualizarPosicionSegunDia();
    }
}
