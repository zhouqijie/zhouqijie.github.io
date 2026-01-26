using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using DG.Tweening;


public interface IGame
{
     float GameTime { get; }
     int Status { get; }
     Vector2 StatusTime { get; }


    List<Bonus> Bonuses { get; }

    bool IsSafeArea(Vector3 pos);
    Collider SafeArea { get; }

    void Judge();

    void ForceEnd();

    void GetBonus(int multipiler);
    void PlayerDie();

    void ReturnMainMenu();
}

public class ClassicGame : MonoBehaviour , IGame
{
    private static ClassicGame instance;
    public static ClassicGame Instance { get { return instance; } }

    //private
    private UICanvas uicanvaas;


    //Time stuff
    private float gametime;
    public float GameTime { get { return instance.gametime; } }
    private float remainTime;
    public static float hideTime = 30f;

    //timer status
    private int status = 0;
    private float shrinkTime = 0f;// 50f ---- 0f
    private float safeTime = 0f;  // 0f --- 10f
    public Vector2 StatusTime
    {
        get
        {
            switch (status)
            {
                case 0://开局
                    return new Vector2(hideTime - gametime, 30f);
                case 1://等待缩圈
                    return new Vector2(safeTime, 50f);
                case 2://正在缩圈
                    return new Vector2(shrinkTime, 10f);
                case 3://终局
                    return new Vector2(0f, 1f);
                default:
                    return new Vector2();
            }
        }
    }
    public int Status { get { return this.status; } }

    //Safe Area
    private Collider safeArea;
    public Collider SafeArea { get { return this.safeArea; } }
    public bool IsSafeArea(Vector3 pos)
    {
        if (Physics.RaycastAll(new Ray(pos + new Vector3(0, -1000f, 0), Vector3.up), 2000f).Any(hit => hit.collider == safeArea))
        {
            return true;
        }
        return false;
    }
    public float SafeRadius { get { return (safeArea.transform.localScale.x / 2f); } }
    public Vector3 SafeCenter { get { return safeArea.transform.position; } }

    //Game Judge And End
    private bool isEnd = false;
    [HideInInspector] public int winTeam = -1;
    private int playerTeam = -1;



    //Bonus
    private List<Bonus> bonuses;
    public List<Bonus> Bonuses { get { return this.bonuses; } }

    //debug
    private bool aiDebugSet = false;


    

    private void Awake()
    {
        instance = this;

        safeArea = GameObject.FindGameObjectWithTag("SafeArea").GetComponent<Collider>();

        //---Instantiate camera/canvas---

        //Camera
        GameObject camSphere = Instantiate(Resources.Load<GameObject>("Prefabs/CameraSphere"), new Vector3(), new Quaternion(), null);
        if (Random.value < 0.4f)
        {
            camSphere.transform.Find("Effects").Find("Rain").gameObject.SetActive(true);
        }

        //Canvas
        Instantiate(Resources.Load<GameObject>("Prefabs/UICanvas"), new Vector3(), new Quaternion(), null);
        uicanvaas = FindObjectOfType<UICanvas>();
        if (Infomanager.playerTeam == 0)
        {
            uicanvaas.ShowHiderUI();
        }
        else
        {
            uicanvaas.ShowHunterUI();
        }
    }


    void Start()
    {
        gametime = 0f;
        remainTime = 300f;// = 300f;


        //Recorder
        Recorder.StartRecord(() => { }, (code, msg) => { }, 300);

        


        //出生点
        SpawnPoint[] spawnPoints = FindObjectsOfType<SpawnPoint>();

        //0-10
        List<int> numList = new List<int>();
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            numList.Add(i);
        }

        //搜寻者数量
        int searcherCount = 3;
        int[] searcherArr = new int[searcherCount];
        for(int i = 0; i < searcherArr.Length; i++)
        {
            int rdmInd = Random.Range(0, numList.Count);
            searcherArr[i] = numList[rdmInd];
            numList.RemoveAt(rdmInd);
        }

        //躲藏者数组
        int hiderCount = spawnPoints.Length - searcherCount;
        int[] hiderArr = new int[hiderCount];
        for(int i = 0; i < hiderCount; i++)
        {
            int rdmInd = Random.Range(0, numList.Count);
            hiderArr[i] = numList[rdmInd];
            numList.RemoveAt(rdmInd);
        }

        //出生点阵营数组
        int[] teamArr = new int[spawnPoints.Length];
        for(int i = 0; i < teamArr.Length; i++)
        {
            teamArr[i] = searcherArr.Contains(i) ? 1 : 0;
        }


        //玩家阵营
        this.playerTeam = Infomanager.playerTeam;

        //玩家出生点
        int playerSpawnIndex = playerTeam == 1 ? searcherArr[Random.Range(0, searcherArr.Length)] : hiderArr[Random.Range(0, hiderArr.Length)];


        //用户名列表
        Queue<string> names = new Queue<string>(Infomanager.currentRoomUsers.Select(u => u.name).Where(n => n != Infomanager.GetInstance().userData.name));

        //spawn
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if(i == playerSpawnIndex)
            {

                SkinInfo userSkin = Infomanager.GetInstance().userData.activeSkin;
                CharactorInstance userCharaStatus = Infomanager.GetInstance().userData.charactors.FirstOrDefault(c => c.info == userSkin.charaInfo);

                SpawnAndInitCharactor(Infomanager.GetInstance().userData.name, spawnPoints[i].transform, userSkin, userCharaStatus, true, teamArr[i]);
            }
            else
            {

                //AI -- 随机职业信息
                CharactorInstance charaStatus = new CharactorInstance();
                charaStatus.info = Infomanager.GetInstance().characterInfos[Random.Range(0, Infomanager.GetInstance().characterInfos.Length)];
                charaStatus.unlockedNodes = new int[0];
                //AI - 随机皮肤
                SkinInfo[] skinsThisChara = Infomanager.GetInstance().skinInfos.Where(s => s.charaInfo == charaStatus.info).ToArray();
                SkinInfo skinRdm = skinsThisChara[Random.Range(0, skinsThisChara.Length)];

                string userName = names.Dequeue();
                SpawnAndInitCharactor(userName, spawnPoints[i].transform, skinRdm, charaStatus, false, teamArr[i]);
                
            }
        }



        //Game Program
        Invoke("GameStartT0", 2f);//结束冻结
        Invoke("GameStartT1", hideTime);//结束冻结

        Invoke("SafeAreaShrink", 80f);
        Invoke("SafeAreaShrink", 140f);
        Invoke("SafeAreaShrink", 200f);
        Invoke("SafeAreaShrink", 260f);


        Invoke("GameFinalStatus", 260f);

        InvokeRepeating("GenerateAmmoSupply", 5f, 10f);
    }


    void Update()
    {
        this.gametime += Time.deltaTime;
        this.shrinkTime += Time.deltaTime;
        this.safeTime -= Time.deltaTime;
        this.remainTime -= Time.deltaTime;
        if (remainTime < 0f)
        {
            Judge();
        }
    }



    /// <summary>
    /// 生成并初始化角色
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="isPlayer"></param>
    /// <param name="team"></param>
    private void SpawnAndInitCharactor(string userName, Transform trans, SkinInfo skin, CharactorInstance chara, bool isPlayer, int team)
    {
        GameObject c = Instantiate(Resources.Load<GameObject>("Prefabs/Charactor"), trans.position, trans.rotation, null);
        CharactorController charactorCtroller = c.GetComponent<CharactorController>();

        //Charactor info instance
        charactorCtroller.charaInstance = chara;

        
        //player or AI
        if (isPlayer)
        {
            //name tag and layer
            c.name = "Player";
            c.tag = "Player";
            c.GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer("Player");

            //gameinput
            var gameInput = c.AddComponent<PlayerInput>(); charactorCtroller.gameInput = gameInput;
            gameInput.UserName = userName;
            gameInput.team = team;
            
        }
        else
        {
            //name tag and layer
            c.name = "AI " + Random.Range(0, 999).ToString();
            c.GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer("Charactor");


            //gameinput
            var gameInput = c.AddComponent<AIInput0>(); charactorCtroller.gameInput = gameInput;
            gameInput.UserName = userName;
            gameInput.team = team;

            if (!aiDebugSet)
            {
                aiDebugSet = true;
                c.GetComponent<AIInput0>().debug = true;
            }
            
        }



        //talent set(may conclude some abilities)
        List<TalentNodeInfo> talents = chara.info.talentNodes.Where(t => chara.unlockedNodes.Contains(t.id)).Where(t => t.team == team).ToList();//（已解锁&&对应阵营）的天赋设置
        
        charactorCtroller.SetTalents(talents);

        //abilities add
        //...(add through talent)

        //skin init
        foreach (SkinnedMeshRenderer smr in c.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if(smr.gameObject.name == skin.name)
            {
                smr.gameObject.SetActive(true);
                charactorCtroller.targetSmr = smr;
            }
            else
            {
                smr.gameObject.SetActive(false);
            }
        }

        //Skin Fade 
        if(team == 0)
        {
            charactorCtroller.SkinFade();
        }


        //weapon init
        string weaponName; // = Infomanager.GetInstance().userData.activeDecoration2 != null ? Infomanager.GetInstance().userData.activeDecoration2.name : "defaultWeapon";
        if (isPlayer)
        {
            weaponName = Infomanager.GetInstance().userData.activeDecoration2 != null ? Infomanager.GetInstance().userData.activeDecoration2.name : "defaultWeapon";
        }
        else
        {
            weaponName = "defaultWeapon";
        }
        Transform weaponHold = charactorCtroller.weaponHold;
        GameObject weapoon = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + weaponName), weaponHold);
        weapoon.transform.localPosition = new Vector3();
        weapoon.transform.localRotation = new Quaternion();
        
        if (team == 0)
        {
            weapoon.gameObject.SetActive(false);
        }

        //躲藏者不显示背饰头饰
        if(team != 0)
        {
            //back deco init
            string backDecoName = null;
            if (isPlayer && Infomanager.GetInstance().userData.activeDecoration1 != null)
            {
                backDecoName = Infomanager.GetInstance().userData.activeDecoration1.name;
            }
            if (backDecoName != null)
            {
                Transform backHold = charactorCtroller.backHold;
                GameObject backDeco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + Infomanager.GetInstance().userData.activeDecoration1.name), backHold);
                backDeco.transform.localPosition = new Vector3();
                backDeco.transform.localRotation = new Quaternion();
            }

            //HEAD deco init
            string headDecoName = null;
            if (isPlayer && Infomanager.GetInstance().userData.activeDecoration0 != null)
            {
                headDecoName = Infomanager.GetInstance().userData.activeDecoration0.name;
            }
            if (headDecoName != null)
            {
                Transform headHold = charactorCtroller.headHold;
                GameObject headDeco = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/" + Infomanager.GetInstance().userData.activeDecoration0.name), headHold);
                headDeco.transform.localPosition = new Vector3();
                headDeco.transform.localRotation = new Quaternion();
            }
        }


        //frz
        charactorCtroller.freezed = true;
    }


    public CharactorController SpawnAndIntFool(Vector3 pos, Quaternion rot)
    {
        Vector3 validPos = pos;

        RaycastHit hit;
        if(Physics.Raycast(new Ray(pos + new Vector3(0, 8f, 0), -Vector3.up), out hit, 16f))
        {
            validPos = hit.point + new Vector3(0f, 0.25f, 0f);
        }

        GameObject c = Instantiate(Resources.Load<GameObject>("Prefabs/Charactor"), validPos, rot, null);
        CharactorController charactorCtroller = c.GetComponent<CharactorController>();

        //Charactor info instance(NEW)
        charactorCtroller.charaInstance = new CharactorInstance();


        //name tag and layer
        c.name = "AI " + Random.Range(0, 999).ToString();
        c.GetComponentInChildren<Collider>().gameObject.layer = LayerMask.NameToLayer("Charactor");


        //gameinput
        var gameInput = c.AddComponent<AIInput0>(); charactorCtroller.gameInput = gameInput;
        gameInput.UserName = "AI";
        gameInput.team = 0;




        //talent set(NONE)
        charactorCtroller.SetTalents(new List<TalentNodeInfo>());

        //abilities add
        //...(add through talent)

        //skin init
        foreach (SkinnedMeshRenderer smr in c.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            if (smr.gameObject.name == "杰夫")
            {
                smr.gameObject.SetActive(true);
                charactorCtroller.targetSmr = smr;
            }
            else
            {
                smr.gameObject.SetActive(false);
            }
        }

        //Skin Fade 
        if (gameInput.Team == 0)
        {
            c.GetComponentInChildren<SkinnedMeshRenderer>(false).material.SetColor("_Color", new Color(1f, 1f, 1f, 0f));
            c.GetComponentInChildren<SkinnedMeshRenderer>(false).material.DOColor(new Color(1f, 1f, 1f, 0.1f), 1f);
        }

        Transform weaponHold = charactorCtroller.weaponHold;
        GameObject weapoon = Instantiate(Resources.Load<GameObject>("Prefabs/Decorations/defaultWeapon"), weaponHold);
        weapoon.transform.localPosition = new Vector3();
        weapoon.transform.localRotation = new Quaternion();

        if (gameInput.Team == 0)
        {
            weapoon.gameObject.SetActive(false);
        }


        //back deco init（NONE）
        //...

        //!!!!!!!!Un Freeze
        charactorCtroller.freezed = false;


        return charactorCtroller;
    }


    public void GameStartT0()
    {
        //解除冻结
        foreach(var c in FindObjectsOfType<CharactorController>())
        {
            c.freezed = false;
        }
        
    }

    public void GameStartT1()
    {
        //Timer
        status = 1;
        safeTime = 50f;

        //解除视觉
        foreach (var c in FindObjectsOfType<CharactorController>().Where(c => c.gameInput.Team == 0))
        {
            c.SkinFadeOut();
        }
    }

    public void GameFinalStatus()
    {
        StartCoroutine(CoFinalStatus());
    }
    private IEnumerator CoFinalStatus()
    {
        yield return new WaitUntil(() => status == 1);
        status = 3;
    }



    #region --------------游戏中---------------------

    /// <summary>
    /// 缩圈
    /// </summary>
    private void SafeAreaShrink()
    {
        NavLocation[] locationsInBound = FindObjectsOfType<NavLocation>().Where(loc => IsSafeArea(loc.transform.position)).ToArray();

        //Timer
        status = 2;
        shrinkTime = 0f;
        safeTime = 60f;

        //Position
        Vector3 pos = safeArea.transform.position;
        if (locationsInBound.Length > 0)
        {
            pos = locationsInBound[Random.Range(0, locationsInBound.Length)].transform.position;
        }

        //Scale
        float scale = safeArea.transform.localScale.x * 0.5f;


        //Tweener
        safeArea.transform.DOMove(new Vector3(pos.x, 0, pos.z), 10f);
        safeArea.transform.DOScale(new Vector3(scale, 100f, scale), 10f).OnComplete(() => { status = 1; });
    }

    private void GenerateAmmoSupply()
    {
        SupplyAmmo[] existAmmoItems = FindObjectsOfType<SupplyAmmo>();
        foreach(var item in existAmmoItems)
        {
            if (!IsSafeArea(item.transform.position))
            {
                Destroy(item.gameObject);
            }
        }


        int existCount = existAmmoItems.Length;
        if (existCount > 8) return;

        //for (int i = 0; i < 20; i++)
        //{
        //    Vector3 rdmPos = UtilsGame.GetCellCenterPos(new Vector3(Random.Range(0f, 512f), 0, Random.Range(0f, 512f)));
        //    if (IsSafeArea(rdmPos))
        //    {
        //        RaycastHit hit;
        //        if (Physics.Raycast(new Ray(new Vector3(rdmPos.x, 20f, rdmPos.z), -Vector3.up), out hit, 25f))
        //        {
        //            if (hit.point.y < 6f && hit.point.y > 0f)
        //            {
        //                Vector3 genPos = hit.point;
        //                Instantiate(Resources.Load<GameObject>("Prefabs/Items/Ammo"), genPos, new Quaternion(), null);
        //                break;
        //            }
        //        }
        //    }
        //}

        Vector3 genPos = UtilsGame.ScanRandomPos(this, true, Mathf.RoundToInt(SafeRadius), new Vector3(256f, 0f, 256f), new Vector2(0f, 6f));

        Instantiate(Resources.Load<GameObject>("Prefabs/Items/Ammo"), genPos, new Quaternion(), null);
    }


    #endregion














    /// <summary>
    /// 判定
    /// </summary>
    public void Judge()
    {
        if (isEnd) return;

        List<CharactorController> charactors = FindObjectsOfType<CharactorController>().ToList();

        if(charactors.Where(c => c.gameInput.Team == 0 && c.enabled == true).Count() < 1)
        {
            HiderLose();
        }

        if (charactors.Where(c => c.gameInput.Team == 1 && c.enabled == true).Count() < 1)
        {
            HiderWin();
        }

        if (remainTime < 0)
        {
            HiderWin();
        }
    }


    /// <summary>
    /// 游戏结果
    /// </summary>
    private void HiderWin()
    {
        if (isEnd) return;

        isEnd = true;

        winTeam = 0;

        CalBonus();

        StartCoroutine(CoGameEnd(3f));
    }
    
    private void HiderLose()
    {
        if (isEnd) return;

        isEnd = true;

        winTeam = 1;

        CalBonus();

        StartCoroutine(CoGameEnd(3f));
    }

    public void ForceEnd()
    {
        if (isEnd) return;

        isEnd = true;

        int fwinTeam = (Infomanager.playerTeam + 1) % 2;

        winTeam = fwinTeam;

        CalBonus();

        StartCoroutine(CoGameEnd(0f));
    }
    

    /// <summary>
    /// End
    /// </summary>
    /// <param name="tdelay"></param>
    /// <returns></returns>
    public IEnumerator CoGameEnd(float tdelay)
    {
        yield return new WaitForSeconds(tdelay);

        //stop record
        Recorder.StopRecord((path) => { }, (code,msg) => { });

        //statistics
        Infomanager.GetInstance().userData.totalClassicGames += 1;
        Infomanager.GetInstance().userData.todayClassicGames += 1;
        Infomanager.GetInstance().userData.todayKills += FindObjectOfType<PlayerInput>().GetComponent<CharactorController>().Kills;
        if (playerTeam == winTeam) Infomanager.GetInstance().userData.todayClassicWins += 1;

        //end window show
        if (playerTeam == winTeam)
        {
            uicanvaas.ShowWindowWinLose(true);
        }
        else
        {
            uicanvaas.ShowWindowWinLose(false);
        }
    }

    private void CalBonus()//Game.cs内调用
    {
        bonuses = new List<Bonus>();
        
        if(playerTeam == winTeam)
        {
            //奖励
            Bonus b = new Bonus();
            b.id = -1;
            b.name = "money";
            b.type = 0;
            b.amount = 200;
            bonuses.Add(b);
            //奖励
            Bonus b2 = new Bonus();
            b2.id = -1;
            b2.name = "diamonds";
            b2.type = 4;
            b2.amount = 50;
            bonuses.Add(b2);

            //段位
            Badge.levelPrevious = Infomanager.GetInstance().userData.level;
            Infomanager.GetInstance().userData.level = Mathf.Clamp(Infomanager.GetInstance().userData.level + 1, 0, Infomanager.maxLevel);
            Badge.levelCurrent = Infomanager.GetInstance().userData.level;

            //vip经验
            Infomanager.GetInstance().userData.vipExp += 10 ;
        }
        else
        {
            //奖励
            Bonus b = new Bonus();
            b.id = -1;
            b.name = "money";
            b.type = 0;
            b.amount = 100;
            bonuses.Add(b);
            //奖励
            Bonus b2 = new Bonus();
            b2.id = -1;
            b2.name = "diamonds";
            b2.type = 4;
            b2.amount = 25;
            bonuses.Add(b2);


            //段位
            Badge.levelPrevious = Infomanager.GetInstance().userData.level;
            Infomanager.GetInstance().userData.level = Mathf.Clamp(Infomanager.GetInstance().userData.level - 1, 0, Infomanager.maxLevel);
            Badge.levelCurrent = Infomanager.GetInstance().userData.level;


            //vip经验
            Infomanager.GetInstance().userData.vipExp += 10;
        }
    }

    public void GetBonus(int multipiler)
    {
        foreach(Bonus b in bonuses)
        {
            switch (b.type)
            {
                case 0://金币
                    Infomanager.GetInstance().userData.money += (b.amount * multipiler);
                    break;
                case 1://角色
                    //GiveSkin(Infomanager.GetInstance().skinInfos.FirstOrDefault(s => s.name == bonus.name));
                    break;
                case 2://装饰品
                    //GiveDecoration(Infomanager.GetInstance().decorationInfos.FirstOrDefault(d => d.name == bonus.name), false);
                    break;
                case 3://变身伪装
                    //GiveDisguise(Infomanager.GetInstance().disguiseObjInfos.FirstOrDefault(d => d.name == bonus.name), false);
                    break;
                case 4://钻石
                    Infomanager.GetInstance().userData.diamonds += (b.amount * multipiler);
                    break;
                default:
                    break;
            }
        }
    }



    /// <summary>
    /// PlayerDie
    /// </summary>
    public void PlayerDie()
    {
        uicanvaas.ShowObserverUI();
    }
    

    /// <summary>
    /// ESC TO MAINMENU
    /// </summary>
    public void ReturnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
