using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public FreeCam cam;
    public GameObject pausePanel;
    public GameObject infoPanel;
    public Toggle shadowsToggle;
    public Toggle presizeToggle;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("shadowsOn")) {
            shadowsToggle.isOn = (PlayerPrefs.GetInt("shadowsOn") == 1);
        }

        presizeToggle.isOn = (PlayerPrefs.GetInt("isPrecise") == 1);
        SetShadows();
        SetPrecise();
    }

    // Action
    [System.Obsolete]
    public void Pause()
    {
        pausePanel.SetActive(!pausePanel.activeSelf);
        cam.isPaused = pausePanel.active;
        gameObject.GetComponent<CubePlacer>().enabled = !gameObject.GetComponent<CubePlacer>().enabled;
    }

    public void SetShadows()
    {
        QualitySettings.shadows = (shadowsToggle.isOn) ? ShadowQuality.All : ShadowQuality.Disable;
        PlayerPrefs.SetInt("shadowsOn", (shadowsToggle.isOn) ? 1 : 0);
    }

    public void SetPrecise()
    {
        gameObject.GetComponent<CubePlacer>().isPrecise = presizeToggle.isOn;
        PlayerPrefs.SetInt("isPrecise",(presizeToggle.isOn) ? 1 : 0);
    }

    public void TutorialDidPressed()
    {
        Application.OpenURL("https://youtu.be/DGynCiq5f00");
    }

    public void InfoPressed()
    {
        infoPanel.SetActive(!infoPanel.activeSelf);
    }
}
