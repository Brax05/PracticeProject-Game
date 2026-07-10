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
        [SerializeField] private GameObject menuContentRoot;

        private GameObject[] fallbackMenuObjects;

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnOpenSettingsClicked()
        {
            SetMenuContentVisible(false);
            if (settingsPanelRoot != null)
                settingsPanelRoot.SetActive(true);
        }

        public void OnCloseSettingsClicked()
        {
            if (settingsPanelRoot != null)
                settingsPanelRoot.SetActive(false);
            SetMenuContentVisible(true);
        }

        private void SetMenuContentVisible(bool visible)
        {
            if (menuContentRoot == null)
                menuContentRoot = GameObject.Find("MenuContent");

            if (menuContentRoot != null)
            {
                menuContentRoot.SetActive(visible);
                return;
            }

            SetNamedObjectVisible("Titulo", visible);
            SetNamedObjectVisible("BotonJugar", visible);
            SetNamedObjectVisible("BotonConfiguracion", visible);
        }

        private void SetNamedObjectVisible(string objectName, bool visible)
        {
            GameObject go = GetFallbackMenuObject(objectName);
            if (go != null)
                go.SetActive(visible);
        }

        private GameObject GetFallbackMenuObject(string objectName)
        {
            if (fallbackMenuObjects == null)
            {
                fallbackMenuObjects = new[]
                {
                    GameObject.Find("Titulo"),
                    GameObject.Find("BotonJugar"),
                    GameObject.Find("BotonConfiguracion")
                };
            }

            foreach (GameObject go in fallbackMenuObjects)
            {
                if (go != null && go.name == objectName)
                    return go;
            }

            return null;
        }
    }
}
