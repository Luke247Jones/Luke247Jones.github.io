using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChallengeSystem : MonoBehaviour
{
    // UI
    public List<Text> limitTexts;
    public GameObject limitsPanel;

    private Dictionary<string, int> blockLimits = new Dictionary<string, int>();
    private List<BlockDTO> blockDTOs;

    public void Setup()
    {
        blockDTOs = GlobalVariables.currentChallenge.blocks;
        SetLimits();
    }

    private void SetLimits(bool updateUI = true)
    {
        foreach (BlockDTO block in blockDTOs)
        {
            blockLimits[block.blockName.Trim()] = block.count;
        }

        if (updateUI) {
            UpdateTexts();
            limitsPanel.SetActive(true);
        }
    }

    private void UpdateTexts()
    {
        foreach (BlockDTO block in blockDTOs)
        {
            int limit = blockLimits[block.blockName.Trim()];
            limitTexts.FindLast(x => x.name == block.blockName.Trim().ToLower()).text = limit.ToString();
        }
    }

    // Public

    public void UpdateLimits(List<Transform> placedBlocks)
    {
        if (blockDTOs == null) { return; }

        SetLimits(false);
        foreach (BlockDTO block in blockDTOs)
        {
            int amount = placedBlocks.FindAll(x => x.gameObject.name.ToLower() == block.blockName.Trim().ToLower()).Count;
            blockLimits[block.blockName.Trim()] -= amount;
        }
        UpdateTexts();
    }

    public bool CheckForLimit(string blockName)
    {
        var block = blockDTOs.FindLast(x => x.blockName.Trim() == blockName);
        if (block != null)
        {
            int limit = blockLimits[block.blockName.Trim()];
            if (limit > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckPasteLimit(List<GameObject> copiedBlocks)
    {
        foreach (BlockDTO block in blockDTOs)
        {
            int amount = copiedBlocks.FindAll(x => x.name.ToLower() == block.blockName.Trim().ToLower()).Count;
            if (amount > blockLimits[block.blockName.Trim()]){ return false; }
        }
        return true;
    }
}
