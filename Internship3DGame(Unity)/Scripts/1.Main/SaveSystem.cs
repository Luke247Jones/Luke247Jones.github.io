using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.EventSystems;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SFB;

public static class SaveSystem
{
    public static bool isLoading = false;
    private static PlacedBrocksData data;
    //private static string documentsFolderPath;
    //private static string fileName;


    public static void SetData(PlacedBrocksData newData)
    {
        data = newData;
    }
    public static PlacedBrocksData GetData()
    {
        return data;
    }


    public static bool SaveData(Transform[] placedBricks)
    {
        // Get user selected file path
        string filePath = StandaloneFileBrowser.SaveFilePanel("Save", "", "CBP Creation", "cbp");
        if (filePath.Length == 0) return false;

        // Create folder for scene documents
        Directory.CreateDirectory(filePath.Replace(".cbp", " Documents"));
        string documentsFolderPath = filePath.Replace(".cbp", " Documents");
        string fileName = Path.GetFileName(filePath);

        // Create file stream
        BinaryFormatter formatter = new BinaryFormatter();
        string path = filePath;
        FileStream stream = new FileStream(path, FileMode.Create);

        // Get brocks data
        PlacedBrocksData data = new PlacedBrocksData(placedBricks);
        formatter.Serialize(stream, data);

        // Create documents files
        CreateTextFile(data, path, documentsFolderPath, fileName);
        CreateScreenshot(path, documentsFolderPath, fileName);
        stream.Close();
        return true;
    }

    private static void CreateTextFile(PlacedBrocksData data, string path, string folderPath, string fileName)
    {
        string filePath = folderPath + "/" + fileName.Replace(".cbp", "(Document).doc");

        string documentTitle = "CORKBRICK PLAY CREATION DOCUMENT \n\n\n";
        string brocksUsedTitle = "Brocks Used: \n\n";

        int baseCount = 0;
        int doubleCount = 0;
        int tCount = 0;
        int oneDCount = 0;
        int twoDCount = 0;
        int threeDCount = 0;
        int fourDCount = 0;
        for (int i = 0; i < data.count; i++)
        {
            switch (data.objectName[i])
            {
                case "Base":
                    baseCount++;
                    break;
                case "Double":
                    doubleCount++;
                    break;
                case "T":
                    tCount++;
                    break;
                case "1D":
                    oneDCount++;
                    break;
                case "2D":
                    twoDCount++;
                    break;
                case "3D":
                    threeDCount++;
                    break;
                case "4D":
                    fourDCount++;
                    break;
                default:
                    break;
            }
        }
        string brocksUsed = " Base: " + baseCount + "\n"
                            + " Double: " + doubleCount + "\n"
                            + " T: " + tCount + "\n"
                            + " 1D: " + oneDCount + "\n"
                            + " 2D: " + twoDCount + "\n"
                            + " 3D: " + threeDCount + "\n"
                            + " 4D: " + fourDCount + "\n\n\n";


        string brocksOrderTitle = "Brocks Order List: \n\n";
        string brocksOrder = "";
        for (int i = 0; i < data.count; i++)
        {
            int orderNum = i + 1;
            brocksOrder += " " + orderNum + ".  " + data.objectName[i] + "\n";
        }

        string content = documentTitle + brocksUsedTitle + brocksUsed + brocksOrderTitle + brocksOrder;

        if (File.Exists(path))
        {
            //Create file
            File.WriteAllText(filePath, content);
        }
        else
        {
            Debug.LogError("Text file not found in " + path);
        }
    }

    private static void CreateScreenshot(string path, string folderPath, string fileName)
    {
        string filePath = folderPath + "/" + fileName.Replace(".cbp", "(Screenshot).png");
        if (File.Exists(path))
        {
            //Create file
            ScreenCapture.CaptureScreenshot(filePath);
        }
        else
        {
            Debug.LogError("Png file not found in " + path);
        }

    }

    public static PlacedBrocksData LoadData()
    {
        string[] filePath = StandaloneFileBrowser.OpenFilePanel("Load", "", "cbp", false);
        //string file = filePath[0];

        if (filePath.Length == 0 || !filePath[0].EndsWith("cbp")) return null;

        string path = filePath[0];


        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            PlacedBrocksData data = formatter.Deserialize(stream) as PlacedBrocksData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }



}
