using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIChatPanel : MonoBehaviour
{
    [HideInInspector] public bool refreshing;

    private GameObject msgPrefab;
    private Queue<GameObject> msgObjs = new Queue<GameObject>();

    private Transform msgPanel;
    private InputField inputField;
    private Button btnSend;



    private void Awake()
    {
        msgPanel = this.transform.Find("PanelMessage");
        inputField = this.GetComponentInChildren<InputField>();
        btnSend = this.transform.Find("ButtonSend").GetComponent<Button>();
    }

    void Start()
    {
        refreshing = true;

        msgPrefab = this.msgPanel.GetChild(0).gameObject;



        btnSend.onClick.AddListener(() => { PlayerSendMsg(); });

        StartCoroutine(CoRefreshChat());
    }
    
    

    private IEnumerator CoRefreshChat()
    {
        while (refreshing)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 8f));

            if(Random.value < 0.5f)
            {
                UserSendInvite();
            }
            else
            {
                UserSendMsg();
            }
        }
    }

    private void RefreshChat(string nameSender, bool isPlayer, string msg, Sprite avatar = null, Sprite border = null, int callbackType = 0,UnityAction callback = null)
    {
        //---CLEAN---
        if(msgObjs.Count > 3)
        {
            GameObject objDel = msgObjs.Dequeue();
            Destroy(objDel);
        }

        

        //---MOVE---
        foreach(RectTransform rectt in msgObjs.Select(obj => obj.GetComponent<RectTransform>()))
        {
            rectt.anchoredPosition += new Vector2(0, 120);
        }

        //---NEW---
        GameObject o = Instantiate(msgPrefab, msgPanel);
        o.SetActive(true);
        o.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 75);

        //---Sender---
        if (isPlayer)
        {
            o.transform.Find("ImageAvatar").GetComponent<RectTransform>().anchoredPosition = new Vector2(225, 0);
            o.transform.Find("ImageContentBottom0").gameObject.SetActive(true);
            o.transform.Find("ImageContentBottom1").gameObject.SetActive(false);
        }
        else
        {
            o.transform.Find("ImageAvatar").GetComponent<RectTransform>().anchoredPosition = new Vector2(-250, 0);
            o.transform.Find("ImageContentBottom0").gameObject.SetActive(false);
            o.transform.Find("ImageContentBottom1").gameObject.SetActive(true);
        }

        //avatar
        o.transform.Find("ImageAvatar").GetComponent<Image>().sprite = avatar;

        //border
        o.transform.Find("ImageAvatar").Find("ImageBorder").GetComponent<Image>().sprite = border;



        //text
        o.transform.Find("TextSender").GetComponent<Text>().text = nameSender;
        o.transform.Find("TextContent").GetComponent<Text>().text = msg;

        //call back
        if(callback != null)
        {
            if(callbackType == 0)
            {
                o.transform.Find("ButtonEnter0").gameObject.SetActive(true);
                o.transform.Find("ButtonEnter1").gameObject.SetActive(false);

                o.transform.Find("ButtonEnter0").GetComponent<Button>().onClick.RemoveAllListeners();
                o.transform.Find("ButtonEnter0").GetComponent<Button>().onClick.AddListener(callback);
            }
            else
            {
                o.transform.Find("ButtonEnter0").gameObject.SetActive(false);
                o.transform.Find("ButtonEnter1").gameObject.SetActive(true);
                
                o.transform.Find("ButtonEnter1").GetComponent<Button>().onClick.RemoveAllListeners();
                o.transform.Find("ButtonEnter1").GetComponent<Button>().onClick.AddListener(callback);
            }
        }
        else
        {
            o.transform.Find("ButtonEnter0").gameObject.SetActive(false);
            o.transform.Find("ButtonEnter1").gameObject.SetActive(false);
        }

        //---Enqueue---
        msgObjs.Enqueue(o);
    }

    private void UserSendInvite()
    {
        List<string> rdmNames = Infomanager.GetInstance().nickNames;

        string senderName = rdmNames[Random.Range(0, rdmNames.Count)];
        int senderAvatarId = Random.Range(0, Infomanager.countAvatars);
        int senderBorderId = Random.Range(0, Infomanager.countBorders);
        int senderRank = Random.Range(0, 3);

        Sprite spriteAvatar = Resources.Load<Sprite>("Sprites/Avatars/" + Random.Range(0, Infomanager.countAvatars));
        Sprite spriteBorder = Resources.Load<Sprite>("Sprites/Borders/" + Random.Range(0, Infomanager.countBorders));


        int type = Random.Range(0, 2);

        if(type == 0)
        {
            RefreshChat(senderName, false, "来玩躲猫猫模式" + System.DateTime.Now.ToLongTimeString(), spriteAvatar, spriteBorder, 0, () => {

                MatchUser knownUser = new MatchUser(senderName, senderAvatarId, senderBorderId, senderRank);
                FindObjectOfType<MainMenuCanvas>().CreateRoom(new List<MatchUser>() { knownUser });

                Infomanager.gameType = 0;
                Infomanager.playerTeam = Random.Range(0, 2);
                FindObjectOfType<MainMenuCanvas>().ShowHideWindowChat(false);
                FindObjectOfType<MainMenuCanvas>().ToggleWindowMatch();
            });
        }
        else
        {
            RefreshChat(senderName, false, "来玩吃鸡模式" + System.DateTime.Now.ToLongTimeString(), spriteAvatar, spriteBorder, 1, () => {


                MatchUser knownUser = new MatchUser(senderName, senderAvatarId, senderBorderId, senderRank);
                FindObjectOfType<MainMenuCanvas>().CreateRoom(new List<MatchUser>() { knownUser });

                Infomanager.gameType = 1;
                Infomanager.playerTeam = 1;
                FindObjectOfType<MainMenuCanvas>().ShowHideWindowChat(false);
                FindObjectOfType<MainMenuCanvas>().ToggleWindowMatch();
            });
        }
    }

    public void UserSendMsg()
    {
        List<string> rdmNames = Infomanager.GetInstance().nickNames;

        string senderName = rdmNames[Random.Range(0, rdmNames.Count)];
        int senderAvatarId = Random.Range(0, Infomanager.countAvatars);
        int senderBorderId = Random.Range(0, Infomanager.countBorders);
        int senderRank = Random.Range(0, 3);

        Sprite spriteAvatar = Resources.Load<Sprite>("Sprites/Avatars/" + Random.Range(0, Infomanager.countAvatars));
        Sprite spriteBorder = Resources.Load<Sprite>("Sprites/Borders/" + Random.Range(0, Infomanager.countBorders));


        //content
        string content = Infomanager.GetInstance().chatContents[Random.Range(0, Infomanager.GetInstance().chatContents.Count)];

        //send
        RefreshChat(senderName, false, content, spriteAvatar, spriteBorder);
    }

    private void PlayerSendMsg()
    {
        Sprite spriteAvatar = Resources.Load<Sprite>("Sprites/Avatars/" + Infomanager.GetInstance().userData.avatarId);
        Sprite spriteBorder = Resources.Load<Sprite>("Sprites/Borders/" + Infomanager.GetInstance().userData.borderId);

        RefreshChat(Infomanager.GetInstance().userData.name, true, inputField.text, spriteAvatar, spriteBorder, 0, null);
    }
}
