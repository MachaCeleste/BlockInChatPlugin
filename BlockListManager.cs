using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace BlockInChatPlugin
{
    public class BlockListManager : MonoBehaviour
    {
        public List<BlockedUser> blocks = new List<BlockedUser>();
        private TMP_InputField addField;

        private void Awake()
        {
            addField = this.transform.Find("Panel/Top/AddNameInput").GetComponent<TMP_InputField>();
            var addButton = this.transform.Find("Panel/Top/AddButton").GetComponent<Button>();
            addButton.onClick.AddListener(() =>
            {
                OnAddBlock();
            });
            var saveButton = this.transform.Find("Panel/Bottom/SaveButton").GetComponent<Button>();
            saveButton.onClick.AddListener(() =>
            {
                OnSaveClick();
            });
            var exitButton = this.transform.Find("Panel/ExitButton").GetComponent<Button>();
            exitButton.onClick.AddListener(() =>
            {
                OnExitClick();
            });
            this.gameObject.SetActive(false);
        }

        public void LoadBlocklist()
        {
            foreach (string nickName in DataUtils.blockedPlayers)
            {
                blocks.Add(new BlockedUser(nickName));
            }
        }

        public void ClearBlocks()
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                UnityEngine.Object.Destroy(blocks[i]._blockedUser);
            }
            blocks.Clear();
        }

        private void OnAddBlock()
        {
            if (string.IsNullOrEmpty(addField.text))
            {
                OS.ShowError("Name input cannot be blank!");
                return;
            }
            if (blocks.FirstOrDefault(x => x._nickName == addField.text) != null)
            {
                OS.ShowError($"{addField.text} is already in list");
                return;
            }
            blocks.Add(new BlockedUser(addField.text));
            addField.text = "";
        }

        private void OnSaveClick()
        {
            List<string> newBlocks = new List<string>();
            foreach (var block in blocks)
            {
                newBlocks.Add(block._nickName);
            }
            DataUtils.blockedPlayers = newBlocks;
            DataUtils.SaveBlocklist();
            addField.text = "";
            this.ClearBlocks();
            this.gameObject.SetActive(false);
        }

        private void OnExitClick()
        {
            this.ClearBlocks();
            this.gameObject.SetActive(false);
        }

        public class BlockedUser
        {
            public string _nickName;
            public GameObject _blockedUser;

            public BlockedUser(string nickName)
            {
                _nickName = nickName;
                var parent = GameObject.Find("PanelBlock/Panel/BlockList/Viewport/Content").transform;
                var prefab = parent.Find("BlockedUserTemplate").gameObject;
                _blockedUser = UnityEngine.Object.Instantiate(prefab, parent);
                _blockedUser.SetActive(true);
                _blockedUser.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = _nickName;
                var button = _blockedUser.GetComponent<Button>();
                button.onClick.AddListener(() =>
                {
                    this.OnBlockedUserRemove();
                });
            }

            public void OnBlockedUserRemove()
            {
                GameObject.FindObjectOfType<BlockListManager>().blocks.Remove(this);
                UnityEngine.Object.Destroy(_blockedUser);
            }
        }
    }
}