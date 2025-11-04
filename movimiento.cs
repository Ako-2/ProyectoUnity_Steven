using UnityEngine;

public class movimiento : MonoBehaviour
{
    [SerializeField] private float velocidadMovimiento = 3f;
    [SerializeField] private Animator animator;

    private Vector2 direccion;
    private Vector2 ultimaDireccion = Vector2.down; // por defecto mira hacia abajo
    private Rigidbody2D rb2D;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // Leer input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        direccion = new Vector2(moveX, moveY);

        // Movimiento normalizado solo si hay input
        if (direccion.sqrMagnitude > 0f)
        {
            direccion.Normalize();
        }

        // Si hay movimiento real, guarda última dirección
        if (direccion != Vector2.zero)
        {
            ultimaDireccion = direccion;
        }

        // Calcular velocidad (solo 0 o 1)
        float velocidad = direccion.sqrMagnitude > 0f ? 1f : 0f;

        // Actualizar Animator
        animator.SetFloat("Speed", velocidad);

        // Siempre mantiene la dirección correcta
        animator.SetFloat("MoveX", (velocidad > 0f) ? direccion.x : ultimaDireccion.x);
        animator.SetFloat("MoveY", (velocidad > 0f) ? direccion.y : ultimaDireccion.y);
    }

    private void FixedUpdate()
    {
        if (direccion.sqrMagnitude > 0f)
        {
            rb2D.MovePosition(rb2D.position + direccion * velocidadMovimiento * Time.fixedDeltaTime);
        }
    }
}
