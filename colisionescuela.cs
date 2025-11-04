using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena2D : MonoBehaviour
{
    // Detecta colisiones 2D normales
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "jugador")
        {
            Debug.Log("Jugador detectado en 2D, cambiando escena...");
            SceneManager.LoadScene("MainGame");
        }
    }

    // Si quieres usar trigger 2D
    /*
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "jugador")
        {
            Debug.Log("Jugador detectado en 2D (trigger), cambiando escena...");
            SceneManager.LoadScene("MainGame");
        }
    }
    */
}
