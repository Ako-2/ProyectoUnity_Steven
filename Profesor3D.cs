using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public enum TipoProfesor { Delantero, Trasero }

public class Profesores3D : MonoBehaviour
{
    [Header("General")]
    public TipoProfesor tipo;
    public Transform camJugador;
    public Image panelNegro;
    public float distanciaMaxOscuro = 10f;
    public float alphaMax = 0.5f;
    private bool inmune = false;
public float tiempoInmunidad = 4f; // segundos de inmunidad tras recibir una CCC

    [Header("Delantero (pizarra)")]
    public Transform objetivoPizarra;
    public float tiempoGiroMin = 10f;     // tiempo mínimo entre giros
    public float tiempoGiroMax = 20f;     // tiempo máximo entre giros
    public float duracionGiro = 2.5f;     // duración del giro (más lento)
    public float anguloGiro = 100f;       // cuánto gira hacia la derecha
    private float timerGiro;
    private bool estaGirando = false;
    private Quaternion rotacionPizarra;

    [Header("Trasero (NavMesh)")]
    public float tiempoRandomMin = 3f;
    public float tiempoRandomMax = 8f;
    public float visionTrasero = 2f;
    private float timerRandom;
    private NavMeshAgent agent;
    private Vector3 posicionOriginal;
    private bool persiguiendo = false;
    private bool regresando = false;

    void Start()
    {
        timerGiro = Random.Range(tiempoGiroMin, tiempoGiroMax);
        timerRandom = Random.Range(tiempoRandomMin, tiempoRandomMax);

        if (tipo == TipoProfesor.Trasero)
        {
            agent = GetComponent<NavMeshAgent>();
            if (agent)
            {
                agent.isStopped = true;
                posicionOriginal = transform.position;
            }
        }

        if (tipo == TipoProfesor.Delantero && objetivoPizarra != null)
        {
            Vector3 dir = objetivoPizarra.position - transform.position;
            dir.y = 0;
            rotacionPizarra = Quaternion.LookRotation(dir);
            transform.rotation = rotacionPizarra;
        }
    }

    void Update()
    {
        if (tipo == TipoProfesor.Delantero)
            ActualizarDelantero();
        else
        {
            ActualizarTrasero();
            Oscurecer();
        }
    }

    // -----------------------------------------------------------
    // PROFESOR DELANTERO
    // -----------------------------------------------------------
    void ActualizarDelantero()
    {
        timerGiro -= Time.deltaTime;
        if (timerGiro <= 0 && !estaGirando)
        {
            StartCoroutine(GiroAleatorio());
            timerGiro = Random.Range(tiempoGiroMin, tiempoGiroMax);
        }
    }

    IEnumerator GiroAleatorio()
{
    estaGirando = true;

    // Referencia a la cámara del jugador (para ver si tiene el móvil)
    CamJugador3D cam = camJugador != null ? camJugador.GetComponent<CamJugador3D>() : null;

    // Girar más a la derecha (donde está el jugador)
    Quaternion rotInicial = transform.rotation;
    Quaternion rotFinal = Quaternion.Euler(0, rotInicial.eulerAngles.y + anguloGiro, 0);

    float t = 0;
    while (t < duracionGiro)
    {
        t += Time.deltaTime;
        transform.rotation = Quaternion.Slerp(rotInicial, rotFinal, t / duracionGiro);
        yield return null;
    }

    // 🔹 En este punto está mirando al jugador
    // Comprobamos si el jugador tiene el móvil fuera


    // Se queda mirando un momento (simula observar el aula)
    yield return new WaitForSeconds(Random.Range(2f, 4f));

    // Vuelve lentamente a la pizarra
    t = 0;
    while (t < duracionGiro)
    {
        t += Time.deltaTime;
        transform.rotation = Quaternion.Slerp(rotFinal, rotacionPizarra, t / duracionGiro);
        yield return null;
    }

    estaGirando = false;
}

    // -----------------------------------------------------------
    // PROFESOR TRASERO
    // -----------------------------------------------------------
    void ActualizarTrasero()
    {
        if (!agent || !agent.isOnNavMesh) return;

        CamJugador3D cam = camJugador.GetComponent<CamJugador3D>();
        if (cam == null) return;

        if (MiradoPorJugador())
        {
            regresando = true;
            persiguiendo = false;
        }

        if (regresando)
        {
            agent.isStopped = false;
            agent.SetDestination(posicionOriginal);
            if (Vector3.Distance(transform.position, posicionOriginal) < 0.3f)
            {
                agent.isStopped = true;
                regresando = false;
                timerRandom = Random.Range(tiempoRandomMin, tiempoRandomMax);
            }
            return;
        }

        if (!persiguiendo)
{
    timerRandom -= Time.deltaTime;
    if (timerRandom <= 0)
    {
        persiguiendo = true;
        agent.isStopped = false;
        agent.SetDestination(camJugador.position);
    }
}
else
{
    agent.SetDestination(camJugador.position);
    float dist = Vector3.Distance(transform.position, camJugador.position);

    if (dist <= visionTrasero)
    {
        if (cam.conMovil && !inmune)
        {
            Debug.Log($"📱 CCC detectado por profesor {tipo} → restar vida");

            MainGameManager main = FindObjectOfType<MainGameManager>();
            if (main != null)
                main.QuitarVida();

            // Activar inmunidad temporal
            StartCoroutine(ActivarInmunidad());
        }
        else if (!cam.conMovil)
        {
            regresando = true;
            persiguiendo = false;
        }
    }
}
    }

    bool MiradoPorJugador()
    {
        Vector3 dir = (transform.position - camJugador.position).normalized;
        float ang = Vector3.Angle(camJugador.forward, dir);
        return ang < 45f;
    }
void Oscurecer()
{
    if (!panelNegro) return;

    Color c = panelNegro.color;

    if (persiguiendo)
    {
        // Calcula la distancia al jugador
        float dist = Vector3.Distance(transform.position, camJugador.position);

        // Cuanto más cerca, más se oscurece (distancia corta = alpha alto)
        // Si dist = 0 -> alpha = alphaMax
        // Si dist >= distanciaMaxOscuro -> alpha = 0
        float alpha = Mathf.Clamp01((distanciaMaxOscuro - dist) / distanciaMaxOscuro) * alphaMax;

        // Transición suave hacia el nuevo alpha
        c.a = Mathf.Lerp(c.a, alpha, Time.deltaTime * 5f);
    }
    else
    {
        // Si no persigue, desvanece el panel poco a poco
        c.a = Mathf.Lerp(c.a, 0f, Time.deltaTime * 5f);
    }

    panelNegro.color = c;
}



    private IEnumerator ActivarInmunidad()
{
    inmune = true;
    yield return new WaitForSeconds(tiempoInmunidad);
    inmune = false;
}
    // -----------------------------------------------------------
// DETECCIÓN POR COLISIÓN (para el profesor delantero)
// -----------------------------------------------------------


}
