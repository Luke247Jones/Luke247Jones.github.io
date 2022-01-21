using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

// NEED TODO: Implement authorization manager

public class Menu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject infoPanel;
    public GameObject scenePanel;
    [Space(5)]
    public CanvasGroup loading;

    [Header("Other")]
    public MainTableController mainTableController;
    public GameObject eventSystem;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Application.Quit();
        }
    }

    // Loading
    public void ShowLoading()
    {
        loading.alpha = 1;
        //eventSystem.SetActive(false);
    }

    public void HideLoading()
    {
        loading.alpha = 0;
        //eventSystem.SetActive(true);
    }

    // Action

    // Other
    public void PolicyPressed()
    {
        Application.OpenURL("https://corkbrick.com/pages/corkbrick-play-privacy-policy");
    }

    public void TermsPressed()
    {
        Application.OpenURL("https://corkbrick.com/pages/corkbrick-play-terms-and-conditions");
    }

    public void WebsitePressed()
    {
        Application.OpenURL("https://corkbrick.com/");
    }

    public void MailPressed()
    {
        string email = "info@corkbrick.com";
        Application.OpenURL("mailto:" + email);
    }

    public void InfoPressed()
    {
        infoPanel.SetActive(!infoPanel.activeSelf);
    }

    public void TutorialDidPressed()
    {
        Application.OpenURL("https://youtu.be/DGynCiq5f00");
    }
}
