using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AnimationControl : MonoBehaviour
{
    [Header("UI")]
    public Text animationButtonText;
    public Text baseText;
    public Text tText;
    public Text doubleText;
    public Text oneDText;
    public Text twoDText;
    public Text threeDText;
    public Text fourDText;

    [Header("Data")]
    public OrbitCamera orbitCamera;
    public GameObject[] allUIComponents;
    public GameObject[] allFurnitureModels;
    public AnimationClip[] allAnimationClips;

    // Private
    private int currentIndex = 0;
    private bool modelUnPacked = true;

    private ModelInfo[] modelsInformation = new ModelInfo[] {
        new ModelInfo(){ baseNumber = 18, doubleNumber = 0, fourDNumber = 0, oneDNumber = 0, twoDNumber = 4, threeDNumber = 0, tNumber = 0 },
        new ModelInfo(){ baseNumber = 36, doubleNumber = 0, fourDNumber = 0, oneDNumber = 16, twoDNumber = 0, threeDNumber = 0, tNumber = 0 },
        new ModelInfo(){ baseNumber = 76, doubleNumber = 142, fourDNumber = 0, oneDNumber = 62, twoDNumber = 0, threeDNumber = 0, tNumber = 72 }
    };

    private readonly float[] minZooms = new float[] { 2, 2.5f, 3.8f };
    private readonly string[] buyLinks = new string[] { "https://corkbrick.com/products/evora-coffee-table?_pos=1&_sid=ab70da01f&_ss=r",
        "https://corkbrick.com/products/coimbra-tv-unit?_pos=1&_sid=e55e1963e&_ss=r",
        "https://corkbrick.com/products/macau-staircase?_pos=1&_sid=59a211e1a&_ss=r"};
    private readonly string[] tutorialLinks = new string[] { "https://www.youtube.com/watch?v=G8_kGxzg4Ac",
        "https://www.youtube.com/watch?v=DXQaL0vC-6M",
        "https://www.youtube.com/watch?v=J_JBO-m3E9k" };

    private void Awake()
    {
        SetInformation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentIndex++;
            if (currentIndex == allFurnitureModels.Length)
            {
                currentIndex = 0;
            }
            SetInformation();
            SwapModels();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            currentIndex--;
            if (currentIndex == -1)
            {
                currentIndex = allFurnitureModels.Length - 1;
            }
            SetInformation();
            SwapModels();
        }
    }

    private void SetInformation() {
        ModelInfo modelInfo = modelsInformation[currentIndex];
        baseText.text = modelInfo.baseNumber.ToString();
        tText.text = modelInfo.tNumber.ToString();
        doubleText.text = modelInfo.doubleNumber.ToString();
        oneDText.text = modelInfo.oneDNumber.ToString();
        twoDText.text = modelInfo.twoDNumber.ToString();
        threeDText.text = modelInfo.threeDNumber.ToString();
        fourDText.text = modelInfo.fourDNumber.ToString();
    }

    private void SwapModels()
    {
        // Set camera
        orbitCamera.minDistance = minZooms[currentIndex];
        orbitCamera.ZoomOut();

        foreach(GameObject model in allFurnitureModels)
        {
            model.SetActive(false);
        }
        allFurnitureModels[currentIndex].SetActive(true);
    }

    // Public
    public void SetIndex(int newIndex) {
        currentIndex = newIndex;
        SetInformation();
        SwapModels();
    }

    public void PlayAnimation()
    {
        Animation animation = allFurnitureModels[currentIndex].GetComponent<Animation>();
        if (animation.isPlaying) { return; }

        AnimationClip clip = allAnimationClips[currentIndex];
        animation.clip = clip;
        animation[clip.name].time = (modelUnPacked) ? clip.length : 0;
        animation[clip.name].speed = (modelUnPacked) ? -1 : 1;
        modelUnPacked = !modelUnPacked;
        animation.Play();

        animationButtonText.text = (modelUnPacked) ? "PACK" : "UNPACK";
    }

    public void BuyDidTapped()
    {
        Application.OpenURL(buyLinks[currentIndex]);
    }

    public void TutorialDidTapped()
    {
        Application.OpenURL(tutorialLinks[currentIndex]);
    }

    public void BackDidTapped()
    {
        SceneManager.LoadScene("Menu");
    }

    public void HideDidTapped(Text buttonText)
    {
        buttonText.text = (buttonText.text == "HIDE") ? "SHOW" : "HIDE";
        foreach (GameObject component in allUIComponents)
        {
            component.SetActive(buttonText.text == "HIDE");
        }
    }
}
