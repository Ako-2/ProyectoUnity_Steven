using UnityEngine;
using System.Collections;

public class Puerta : MonoBehaviour
{
    [Header("Teletransporte")]
    [SerializeField] private Transform PuntoDestino;
    [SerializeField] private float delayTeletransporte = 0.5f;
    
    [Header("Efectos (Opcional)")]
    [SerializeField] private AudioClip sonidoPuerta;
    
    private bool puedeUsar = true;
    private AudioSource audioSource;
    
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && puedeUsar)
        {
            StartCoroutine(Teletransportar(collision.transform));
        }
    }
    
    private IEnumerator Teletransportar(Transform jugador)
    {
        puedeUsar = false;
        
        // Reproducir sonido
        if (audioSource != null && sonidoPuerta != null)
        {
            audioSource.PlayOneShot(sonidoPuerta);
        }
        
        Debug.Log("Entrando a la puerta...");
        
        // Esperar
        yield return new WaitForSeconds(delayTeletransporte);
        
        // Teletransportar
        jugador.position = PuntoDestino.position;
        
        Debug.Log("¡Teletransportado!");
        
        // Esperar antes de poder usar de nuevo
        yield return new WaitForSeconds(1f);
        puedeUsar = true;
    }
}