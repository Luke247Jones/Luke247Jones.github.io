using System.Collections;
using Assets.Scripts.TableView;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChallengeTableViewCell : TableViewCell
{
    public Text text;
    public Image image;
    public Image cover;

    public Sprite defaultImage;

    public override string ReuseIdentifier
    {
        get { return "ChallengeTableViewCell"; }
    }

    public override void SetHighlighted()
    {
        print("CellSetHighlighted : " + RowNumber);
    }

    public override void SetSelected()
    {
        print("CellSetSelected : " + RowNumber);
    }

    public override void Display()
    {
        string displayText = "";
        switch (GlobalVariables.tableViewDataType)
        {
            case 1:
                break;
            default:
                ChallengeDTO challenge = GlobalVariables.challengeResponse.challenges[RowNumber];
                cover.gameObject.SetActive(true);
                displayText = challenge.name;
                break;
        }
        text.text = displayText;
    }

    // Image
    public void SetDefaultImage()
    {
        image.sprite = defaultImage;
    }

    public IEnumerator LoadImage(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
        }
    }
}
