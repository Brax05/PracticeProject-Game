using UnityEngine;
using UnityEngine.SceneManagement;

namespace TravesiaACasa.Menu
{
    /// <summary>
    /// Botones del menú principal (jugar.png / configuración.png del
    /// boceto). "Jugar" carga la escena de juego por nombre para no
    /// depender del orden en Build Settings.
    /// </summary>
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private GameObject settingsPanelRoot;

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnOpenSettingsClicked()
        {
            if (settingsPanelRoot != null)
                settingsPanelRoot.SetActive(true);
        }

        public void OnCloseSettingsClicked()
        {
            if (settingsPanelRoot != null)
                settingsPanelRoot.SetActive(false);
        }
    }
}
