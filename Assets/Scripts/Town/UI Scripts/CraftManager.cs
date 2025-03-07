using System.Collections;
using System.Collections.Generic;
using Google.Protobuf.Protocol;
using TMPro;
using UnityEngine;

public class CraftManager : MonoBehaviour
{
    public UICraft uiCraft;
    private Queue<int> craftQueue = new Queue<int>();
    private bool isCrafting = false;
    private int targetCount;
    private int currentCount;
    private string craftStr;
    private MaterialItemData targetItem;
    private int craftingRecipeId;

    // Start is called before the first frame update
    void Start()
    {
        uiCraft = CanvasManager.Instance.uiCraft;
    }

    public void Resume()
    {
        if(isCrafting) uiCraft.successText.text = craftStr;
        else 
        {
            uiCraft.progressBar.value = uiCraft.progressBar.maxValue;
            uiCraft.successText.text = craftStr;
            uiCraft.confirmButton.gameObject.SetActive(true);
            uiCraft.disableMask.SetActive(false);
        }
    }

    public void ProcessCraft(int recipeId, int count)
    {
        craftingRecipeId = recipeId;
        targetCount = count;
        currentCount = 0;

        for (int i = 0; i < targetCount; i++)
        {
            craftQueue.Enqueue(recipeId);
        }

        if (craftQueue.Count > 0)
        {
            var startPkt = new C2SCraftStart
            {
                RecipeId = craftQueue.Dequeue(),
            };
            GameManager.Network.Send(startPkt);
        }
    }

    public void OnStart(S2CCraftStart pkt)
    {
        if (pkt.IsSuccess == false)
        {
            Debug.LogError("제작 시작 실패:" + pkt.Msg);

            if (uiCraft.gameObject.activeSelf)
            {
                uiCraft.successText.text = craftStr;
                uiCraft.confirmButton.gameObject.SetActive(true);
                uiCraft.disableMask.SetActive(false);
            }

            isCrafting = false;

            GameManager.Instance.SManager.UiChat.PushMessage(
                    "System",
                    $"{targetItem.ItemName} {currentCount}개 제작에 성공하였습니다.",
                    "System",
                    true
                );

            GameManager.Instance.SManager.UiChat.PushMessage(
                    "System",
                    $"[추가 제작 실패]{pkt.Msg}",
                    "System",
                    true
                );
            return;
        }
        
        Recipe targetRecipe = GameManager.Instance.recipeContainer.data.Find(d => d.recipe_id == craftingRecipeId);
        int targetId = targetRecipe.craft_item_id;
        List<MaterialItemData> itemData = ItemDataLoader.MaterialItemsList;
        targetItem = itemData.Find(d => d.ItemId == targetId);

        uiCraft.detailFrame.SetActive(false);
        uiCraft.confirmButton.gameObject.SetActive(false);
        uiCraft.progressFrame.SetActive(true);
        uiCraft.craftingImage.sprite = ItemDataLoader.GetSpriteByItemId(targetId);
        
        isCrafting = true;

        if (uiCraft.gameObject.activeSelf)
        {
            uiCraft.progressTitle.text = $"{targetItem.ItemName}";
            uiCraft.successText.text = $"{currentCount}/{targetCount}개 제작 완료";
            uiCraft.disableMask.SetActive(true); // 제작하는동안 다른 아이템 제작 클릭못하게 막는용도
        }

        StartCoroutine(UpdateProgressBar(3f, () =>
            {
                var endPkt = new C2SCraftEnd
                {
                    RecipeId = pkt.RecipeId,
                };
                GameManager.Network.Send(endPkt);

                currentCount++;
                craftStr = $"{currentCount}/{targetCount}개 제작 완료";
                if (uiCraft.gameObject.activeSelf)
                {
                    uiCraft.successText.text = craftStr;
                }
            }));
    }

    public void OnEnd(S2CCraftEnd pkt)
    {
        if (pkt.IsSuccess == false)
        {
            Debug.LogError("제작 완료 실패:" + pkt.Msg);
            return;
        }

        if (craftQueue.Count > 0)
        {
            StartCoroutine(NextCraft());
        }
        else
        {
            // 모두 제작 완료
            if (uiCraft.gameObject.activeSelf)
            {
                uiCraft.confirmButton.gameObject.SetActive(true);
                uiCraft.disableMask.SetActive(false);
            }

            isCrafting = false;

            GameManager.Instance.SManager.UiChat.PushMessage(
                    "System",
                    $"{targetItem.ItemName} {targetCount}개 제작에 성공하였습니다.",
                    "System",
                    true
                );
        }
    }

    private IEnumerator NextCraft()
    {
        yield return new WaitForSeconds(0.2f);

        int recipeId = craftQueue.Dequeue();

        var startPkt = new C2SCraftStart
        {
            RecipeId = recipeId,
        };
        GameManager.Network.Send(startPkt);
    }

    private IEnumerator UpdateProgressBar(float duration, System.Action onProgressComplete)
    {
        uiCraft.progressBar.maxValue = duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            if (uiCraft.gameObject.activeSelf)
            {
                uiCraft.progressBar.value = elapsed;
            }
            yield return null;
        }

        onProgressComplete?.Invoke();
    }
}
