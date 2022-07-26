using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class StartScreenManager : MonoBehaviour
{
    public Button quitBtn;

    public Button startBtn;
    // Start is called before the first frame update
    void Start()
    {
        startBtn.onClick.AddListener(OnStartClick);
        quitBtn.onClick.AddListener(OnQuitClick);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void OnStartClick()
    {
        SceneManager.LoadScene("CombineFeaturesScene");
    }
    public void OnQuitClick()
    {
        Application.Quit();
    }
}
