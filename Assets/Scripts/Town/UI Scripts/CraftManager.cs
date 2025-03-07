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

    // Start is called before the first frame update
    void Start()
    {
        uiCraft = CanvasManager.Instance.uiCraft;
    }

    public void Resume()
    {
        if(isCrafting) uiCraft.successText.text = craftStr;
    }

    public void ProcessCraft(int recipeId, int count)
    {
        int id = GameManager.Instance.recipeContainer.data.Find(d => d.recipe_id == recipeId).craft_item_id;
        List<MaterialItemData> data = ItemDataLoader.MaterialItemsList;
        targetItem = data.Find(d => d.ItemId == id);

        targetCount = count;
        currentCount = 0;

        if (uiCraft.gameObject.activeSelf)
        {
            uiCraft.progressTitle.text = $"{targetItem.ItemName}";
            uiCraft.successText.text = $"{currentCount}/{targetCount}개 제작 완료";
            uiCraft.disableMask.SetActive(true); // 제작하는동안 다른 아이템 제작 클릭못하게 막는용도
        }

        for (int i = 0; i < targetCount; i++)
        {
            craftQueue.Enqueue(recipeId);
        }

        isCrafting = true;

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

            GameManager.Instance.SManager.UiChat.PushMessage(
                    "System",
                    $"[제작 실패]{pkt.Msg}",
                    "System",
                    true
                );
            return;
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
