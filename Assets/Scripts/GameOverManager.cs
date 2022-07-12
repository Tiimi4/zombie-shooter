
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{

    public Button quitBtn;

    public Button restartBtn;
    // Start is called before the first frame update
    void Start()
    {
        restartBtn.onClick.AddListener(OnRestartClick);
        quitBtn.onClick.AddListener(OnQuitClick);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void OnRestartClick()
    {
        SceneManager.LoadScene("CombineFeaturesScene");
    }
    public void OnQuitClick()
    {
        Application.Quit();
    }
}
