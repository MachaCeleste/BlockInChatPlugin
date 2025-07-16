using AssetBundleTools;
using BlockInChatPlugin;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch]
public class ChatGuildPatch
{
    private static BlockListManager bm;

    [HarmonyPatch(typeof(ChatGuild), "Awake")]
    class AwakePatch
    {
        static void Postfix(ChatGuild __instance)
        {
            DataUtils.LoadBlocklist();
            var content = __instance.transform.Find("Dialog/Container/Viewport/Content");
            var origButtonObj = content.Find("JoinChannel");
            var buttonObj = UnityEngine.Object.Instantiate(origButtonObj, content);
            buttonObj.transform.SetAsFirstSibling();
            buttonObj.name = "ManageBlocks";
            var rect = buttonObj.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(146.6459f, -18.7999f);
            var imageObj = buttonObj.Find("Image");
            var oldImage = imageObj.GetComponent<Image>();
            var imgColor = oldImage.color;
            UnityEngine.Object.DestroyImmediate(oldImage);
            var newImg = imageObj.gameObject.AddComponent<Image>();
            newImg.color = imgColor;
            newImg.sprite = BundleTool.GetSprite("Assets/BlockInChatPlugin/Block.png");
            var toolTip = buttonObj.gameObject.GetComponent<Tooltip>();
            toolTip.texto = "Manage Blocklist";
            var oldButton = buttonObj.GetComponent<Button>();
            var colors = oldButton.colors;
            UnityEngine.Object.DestroyImmediate(oldButton);
            var button = buttonObj.gameObject.AddComponent<Button>();
            button.colors = colors;
            button.targetGraphic = buttonObj.GetComponent<Image>();
            button.onClick.AddListener(() =>
            {
                OnManageClick();
            });
            var bmPrefab = BundleTool.GetPrefab("Assets/BlockInChatPlugin/PanelBlock.prefab");
            if (bmPrefab == null)
            {
                return;
            }
            var bmPanel = UnityEngine.Object.Instantiate(bmPrefab, content);
            bmPanel.name = "PanelBlock";
            bm = bmPanel.AddComponent<BlockListManager>();
            bmPanel.SetActive(false);
        }

        private static void OnManageClick()
        {
            bm.gameObject.SetActive(true);
            bm.LoadBlocklist();
        }
    }

    [HarmonyPatch(typeof(ChatGuild), "RecibeMensajePrivado")]
    class RecibeMensajePrivadoPatch
    {
        static bool Prefix(ChatGuild __instance, ref PlayerUtilsChat.ChatMessage message, ref string otherNickname)
        {
            if (DataUtils.blockedPlayers.Contains(otherNickname)) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(ChatGuild), "RecibeMensaje")]
    class RecibeMensajePatch
    {
        static bool Prefix(ChatGuild __instance, ref PlayerUtilsChat.ChatMessage message)
        {
            if (DataUtils.blockedPlayers.Contains(message.nickName))
            {
                message.message = "<color=#6a7485><i> --- Message Blocked ---<i></color>";
            }
            return true;
        }
    }
}