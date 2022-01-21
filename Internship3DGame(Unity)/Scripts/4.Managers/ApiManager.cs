using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ApiManager : MonoBehaviour
{
    private readonly string url = "https://web.corkbrick.com/api/Challenges";

    public IEnumerator GetChallenges(System.Action<ChallengeResponse> callback)
    {
        ChallengeResponse challenges = new ChallengeResponse
        {
            challenges = new ChallengeDTO[0]
        };
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();

        /*if (webRequest.isNetworkError)
        {
            
        }*/
        if (webRequest.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(webRequest.error);
            Debug.Log("[ERROR] Failed to get challenges.");
        }
        else
        {
            var json = "{ \"challenges\": " + webRequest.downloadHandler.text + "}";
            Debug.Log("[INFO] Response: " + json);
            challenges = JsonUtility.FromJson<ChallengeResponse>(json);
        }
        callback(challenges);
    }
}
