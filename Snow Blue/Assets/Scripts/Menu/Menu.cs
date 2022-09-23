using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class Menu : MonoBehaviour
    {
        public void Quit()
        {
            Application.Quit();
        }

        public void ChangeScene(string scene)
        {
            SceneManager.LoadScene(scene);
        }

        public void OpenForm()
        {
            Application.OpenURL("https://forms.gle/gWnjEWLdzDsF2CARA");
        }
    }
}
