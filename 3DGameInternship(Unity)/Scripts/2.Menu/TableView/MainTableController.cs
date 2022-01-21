using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Collections;

using Assets.Scripts.TableView;

public class MainTableController : MonoBehaviour, ITableViewDataSource, ITableViewDelegate
{
    [Header("Main")]
    public Menu menu;
    public TableView tableView;
    public GameObject cellPrefarb;

    [Header("Scenes")]
    public GameObject noScene;

    [Header("Scene Info")]
    public GameObject sceneInfoPanel;
    public Text sceneDescription;
    public Text challenge;

    [Header("Challenge Info")]
    public GameObject challengeInfoPanel;
    public Text challengeDescription;
    public Image challengeImage;

    // Private
    private ApiManager apiManager;
    private ChallengeDTO challengeInformation;

    private void Start()
    {
        apiManager = gameObject.AddComponent<ApiManager>();

        // Set table view
        tableView.Delegate = this;
        tableView.DataSource = this;
        tableView.RegisterPrefabForCellReuseIdentifier(cellPrefarb, "ChallengeTableViewCell");
        SetData(type: 0);
    }

    /// <summary>
    /// 0 - Challenges, 1 - Inspiration
    /// </summary>
    /// <param name="type"></param>
    public void SetData(int type)
    {
        menu.ShowLoading();
        switch (type)
        {
            case 1:
                break;
            default:
                /*StartCoroutine(apiManager.GetChallenges((response) =>
                {
                    menu.HideLoading();
                    if (response.challenges.Length > 0)
                    {
                        GlobalVariables.challengeResponse = response;
                        GlobalVariables.tableViewDataType = type;
                        tableView.ReloadData();
                    }
                    else
                    {
                        SetData(type: type);
                    }
                }));*/
                break;
        }
    }

    // Action

    // Challenge Info
    public void HideChallengeInformation()
    {
        challengeInfoPanel.SetActive(false);
    }

    public void LoadChallenge()
    {
        menu.ShowLoading();
        GlobalVariables.currentChallenge = challengeInformation;
        PlayerPrefs.SetFloat("length", 1);
        PlayerPrefs.SetFloat("width", 1);
        PlayerPrefs.SetInt("challengeType", 3);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Main");
    }

    // Scene Info

    public void HideSceneInformation()
    {
        sceneInfoPanel.SetActive(false);
    }

    public void LoadScene()
    {
        PlayerPrefs.SetInt("challengeType", 0);
        SceneManager.LoadScene("Main");
    }

    // Inspiration
    public void LoadInspiration()
    {
        SceneManager.LoadScene("Inspiration");
    }

    // Private voids

    private void ShowChallengeInformation()
    {
        menu.ShowLoading();
        challengeDescription.text = challengeInformation.description;
        StartCoroutine(LoadImage(url: challengeInformation.imageUrl));
    }

    private IEnumerator LoadImage(string url)
    {
        challengeImage.sprite = null;
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            challengeImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
        challengeInfoPanel.SetActive(true);
        menu.HideLoading();
    }

    // Table View Delegates/Data Source
    public int NumberOfRowsInTableView(TableView tableView)
    {
        switch (GlobalVariables.tableViewDataType)
        {
            case 0:
                return GlobalVariables.challengeResponse.challenges.Length;
            case 1:
                break;
            default:
                return 0;
        }
        return 0;
    }

    public float SizeForRowInTableView(TableView tableView, int row)
    {
        return 600f;
    }

    public TableViewCell CellForRowInTableView(TableView tableView, int row)
    {
        TableViewCell cell = tableView.ReusableCellForRow("ChallengeTableViewCell", row);
        cell.name = "Cell " + row;

        ChallengeTableViewCell challengeTableViewCell = cell.GetComponent<ChallengeTableViewCell>();
        switch (GlobalVariables.tableViewDataType)
        {
            case 0:
                challengeTableViewCell.SetDefaultImage();
                ChallengeDTO challenge = GlobalVariables.challengeResponse.challenges[row];
                StartCoroutine(challengeTableViewCell.LoadImage(challenge.imageUrl));
                break;
            case 1:
                break;
            default:
                break;
        }
        return cell;
    }

    public void TableViewDidHighlightCellForRow(TableView tableView, int row)
    {
        // NOT USED
    }

    public void TableViewDidSelectCellForRow(TableView tableView, int row)
    {
        switch (GlobalVariables.tableViewDataType)
        {
            case 0:
                challengeInformation = GlobalVariables.challengeResponse.challenges[row];
                ShowChallengeInformation();
                break;
            case 1:
                break;
            default:
                break;
        }
    }
}
