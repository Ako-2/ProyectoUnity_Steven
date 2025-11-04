using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BotonesMenu : MonoBehaviour
{
    [Header("Botones")]
    public Button botonVolverAJugar;
    public Button botonMainMenu;

    void Start()
    {
        // Asignar las funciones a los botones
        if (botonVolverAJugar != null)
            botonVolverAJugar.onClick.AddListener(VolverAJugar);

        if (botonMainMenu != null)
            botonMainMenu.onClick.AddListener(IrMainMenu);
    }

    // Función para volver a jugar
    public void VolverAJugar()
    {
        SceneManager.LoadScene("MiniJuego1"); // Ajusta el nombre de la escena
    }

    // Función para ir al menú principal
    public void IrMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Ajusta el nombre de la escena
    }
}
