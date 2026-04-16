using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnStartClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OnExitClick()
    {
#if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
#endif 
        Application.Quit();
    }
}
