using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum GameType
{
    Tutorial,
    Match,
    MissionA,
    MissionB,
    MissionC
}

public class Game : MonoBehaviour
{
    [HideInInspector] public static Game instance;

    [HideInInspector] public static float identityDistanceFac = 0.75f;

    [HideInInspector] public GameType gameType;

    private bool isEnd = false;
    public bool IsEnd { get { return isEnd; } }


    //time and record
    [HideInInspector] public float timeCountDown;
    private float gameTime = 0f;
    private bool recordedStopped = false;


    [HideInInspector] public float viewDistance;

    private UICanvas uiCanvas;
    private CameraSphere camSphere;

    [HideInInspector] public List<IShipController> units;
    [HideInInspector] public PlayerController player;
    [HideInInspector] public List<IShipController> allies;
    [HideInInspector] public List<IShipController> enemies;


    //mission support
    [HideInInspector] public bool isCallingSupport = false;
    private AIController supportShip = null;
    [HideInInspector] public bool CanCallSupport { get { return !(supportShip != null && supportShip.Enabled); } }


    //---Reward---
    [HideInInspector] public List<LevelReward> rewards;

    [HideInInspector] public float acheveProgress;
    //------


    private void Awake()
    {
        instance = this;


        

        //Get Infomanager
        InfoManager infomanager = InfoManager.GetInstance();

        
        switch (InfoManager.GetInstance().userData.currentMission)
        {
            case 0:
                gameType = GameType.MissionA;
                break;
            case 1:
                gameType = GameType.MissionB;
                break;
            case 2:
                gameType = GameType.MissionC;
                break;
            default:
                break;
        }
        switch (InfoManager.currentGameType)
        {
            case 99:
                gameType = GameType.Match;
                break;
            case -1:
                gameType = GameType.Tutorial;
                break;
            default:
                break;
        }

        //Init CamSphere
        if (FindObjectOfType<UICanvas>() == null)
        {
            camSphere = Instantiate(Resources.Load<GameObject>("Prefabs/CameraSphere")).GetComponent<CameraSphere>();
        }

        //Init canvas
        uiCanvas = FindObjectOfType<UICanvas>();
        if (uiCanvas == null)
        {
            uiCanvas = Instantiate(Resources.Load<GameObject>("Prefabs/Canvas")).GetComponent<UICanvas>();
        }
        Text textMission = uiCanvas.transform.Find("MISSION").GetComponent<Text>();
        



        //Water Reflect Set
        UnityStandardAssetsCustom.MyWater water = FindObjectOfType<UnityStandardAssetsCustom.MyWater>();

        if(water != null && InfoManager.devicePerformance < 1)
        {
            water.waterMode = UnityStandardAssetsCustom.MyWater.WaterMode.Simple;
        }



        switch (gameType)
        {
            case GameType.Match:
                {
                    viewDistance = 10000f;

                    camSphere.DisableRain();

                    //-----
                    textMission.text = "<b>任务目标：歼灭所有敌人</b>";

                    //-----ships list-----
                    //...

                    units = new List<IShipController>();
                    allies = new List<IShipController>();
                    enemies = new List<IShipController>();

                    //------Instantiate-----
                    List<SpawnPoint> pointsTeam0 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 0).OrderBy(p => p.transform.position.sqrMagnitude).ToList();
                    List<SpawnPoint> pointsTeam1 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 1).OrderBy(p => p.transform.position.sqrMagnitude).ToList();

                    //=====player Config=====
                    ShipConfig playerShipConfig = infomanager.userData.currentShip;
                    int playerWeaponLvl = playerShipConfig.activeWeaponCom != null ? playerShipConfig.activeWeaponCom.lvl : 0;
                    int playerArmorLvl = playerShipConfig.activeArmorCom != null ? playerShipConfig.activeArmorCom.lvl : 0;
                    int playerEngineLvl = playerShipConfig.activeEngineCom != null ? playerShipConfig.activeEngineCom.lvl : 0;
                    int playerTrueLvl = Mathf.RoundToInt(playerShipConfig.lvl * 0.5f + playerWeaponLvl * 0.2f + playerArmorLvl * 0.2f + playerEngineLvl * 0.1f);


                    //=====Spawn foreach=====

                    //json文件最大最小rank
                    int minRankInInfo = infomanager.shipInfos.Where(s => s.id < 90).Min(s => s.ranklvl);
                    int maxRankInInfo = infomanager.shipInfos.Where(s => s.id < 90).Max(s => s.ranklvl);

                    //玩家船只rank
                    int playerShipRank = playerShipConfig.shipInfo.ranklvl;

                    //玩家装备rank
                    int playerComRankW = playerShipConfig.activeWeaponCom != null ? playerShipConfig.activeWeaponCom.EquipmentInfo.ranklvl : 1;
                    int playerComRankA = playerShipConfig.activeArmorCom != null ? playerShipConfig.activeArmorCom.EquipmentInfo.ranklvl : 1;
                    int playerComRankE = playerShipConfig.activeEngineCom != null ? playerShipConfig.activeEngineCom.EquipmentInfo.ranklvl : 1;
                    int playerComRank = Mathf.RoundToInt( playerComRankW * 0.4f + playerComRankA * 0.4f + playerComRankE * 0.2f);

                    //敌人船只大小rank
                    int aiShipMinRank;
                    int aiShipMaxRank;

                    if(playerShipRank == minRankInInfo)
                    {
                        aiShipMinRank = playerShipRank;
                        aiShipMaxRank = playerShipRank + 1;
                    }
                    else if(playerShipRank == maxRankInInfo)
                    {
                        aiShipMinRank = playerShipRank - 1;
                        aiShipMaxRank = playerShipRank;
                    }
                    else
                    {
                        if(Random.value > 0.5f)//壮丁房
                        {
                            aiShipMinRank = playerShipRank;
                            aiShipMaxRank = playerShipRank + 1;
                        }
                        else//班长房
                        {
                            aiShipMinRank = playerShipRank - 1;
                            aiShipMaxRank = playerShipRank;
                        }
                    }
                    Debug.Log("稀有度：" + aiShipMinRank + "-" + aiShipMaxRank);

                    //敌人等级
                    int[] aiLvlRange = new int[2] { Mathf.Clamp(playerTrueLvl - 4, 0, 29), Mathf.Clamp(playerTrueLvl + 4, 0, 29) };


                    //-------------Spawn-------------------------------------
                    int playerPosIndexIfNotCaptain = Random.Range(1, pointsTeam0.Count);
                    int currentnameIndex = 1;

                    for (int i = 0;i < pointsTeam0.Count; i++)
                    {
                        if(i == 0)
                        {
                            if (playerShipRank == aiShipMaxRank)//玩家班长
                            {
                                SpawnPlayer(playerShipConfig, pointsTeam0[i].transform);
                            }
                            else//其他AI班长
                            {
                                var arr = infomanager.shipInfos.Where(s => s.id < 90).Where(s => s.ranklvl == aiShipMaxRank).ToArray();
                                int aishipIndex = arr[Random.Range(0, arr.Length)].id;// if (aishipIndex > 90) throw new UnityException("Spawn DD0?");
                                SpawnAIBattleShip(pointsTeam0[i].transform, aishipIndex, playerComRank, aiLvlRange, InfoManager.playerNames[currentnameIndex], 0, AIMode.Hunt, 0.5f); currentnameIndex++;
                            }
                        }
                        else
                        {
                            if(playerShipRank != aiShipMaxRank && i == playerPosIndexIfNotCaptain)
                            {
                                SpawnPlayer(playerShipConfig, pointsTeam0[i].transform);
                            }
                            else
                            {
                                var arr = infomanager.shipInfos.Where(s => s.id < 90).Where(s => s.ranklvl == aiShipMinRank).ToArray();
                                int aishipIndex = arr[Random.Range(0, arr.Length)].id;// if (aishipIndex > 90) throw new UnityException("Spawn DD1?");
                                SpawnAIBattleShip(pointsTeam0[i].transform, aishipIndex, playerComRank, aiLvlRange, InfoManager.playerNames[currentnameIndex], 0, AIMode.Hunt); currentnameIndex++;
                            }
                        }
                    }
                    

                    for (int i = 0; i < pointsTeam1.Count; i++)
                    {
                        if(i == 0)//敌方班长
                        {
                            var arr = infomanager.shipInfos.Where(s => s.id < 90).Where(s => s.ranklvl == aiShipMaxRank).ToArray();
                            int aishipIndex = arr[Random.Range(0, arr.Length)].id;// if (aishipIndex > 90) throw new UnityException("Spawn DD2?");
                            SpawnAIBattleShip(pointsTeam1[i].transform, aishipIndex, playerComRank, aiLvlRange, InfoManager.playerNames[currentnameIndex], 1, AIMode.Hunt, 0.5f); currentnameIndex++;
                        }
                        else//敌方
                        {
                            var arr = infomanager.shipInfos.Where(s => s.id < 90).Where(s => s.ranklvl == aiShipMinRank).ToArray();
                            int aishipIndex = arr[Random.Range(0, arr.Length)].id;// if (aishipIndex > 90) throw new UnityException("Spawn DD3?");
                            SpawnAIBattleShip(pointsTeam1[i].transform, aishipIndex, playerComRank, aiLvlRange, InfoManager.playerNames[currentnameIndex], 1, AIMode.Hunt); currentnameIndex++;
                        }
                    }
                    

                }
                break;
            case GameType.MissionA:
                {
                    viewDistance = 6000f;

                    camSphere.DisableRain();

                    //-----
                    textMission.text = "<b>任务目标：歼灭所有敌人</b>";
                    //-----ships list-----
                    //...

                    units = new List<IShipController>();
                    allies = new List<IShipController>();
                    enemies = new List<IShipController>();

                    //------Instantiate-----
                    SpawnPoint[] pointsTeam0 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 0).ToArray();
                    SpawnPoint[] pointsTeam1 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 1).ToArray();

                    //player
                    ShipConfig playerShipConfig = infomanager.userData.currentShip;
                    SpawnPlayer(playerShipConfig, pointsTeam0[0].transform);
                    

                    for(int i = 0; i < pointsTeam1.Length; i++)
                    {
                        //spawn evey
                        if(i == 0)
                        {
                            StartCoroutine(CoSpawnBoss("克洛诺斯", 0f, pointsTeam1[i].transform));
                            continue;
                        }
                        SpawnDestroyer(pointsTeam1[i].transform, AIMode.Hunt);
                    }
                    
                }
                break;
            case GameType.MissionB:
                {
                    viewDistance = 4000f;

                    camSphere.DisableRain();

                    //-----
                    textMission.text = "<b>任务目标：在敌人的进攻中存活</b>";

                    //-----ships list-----
                    //...

                    units = new List<IShipController>();
                    allies = new List<IShipController>();
                    enemies = new List<IShipController>();
                    
                    //---Only---Instantiate---player
                    SpawnPoint[] pointsTeam0 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 0).ToArray();
                    ShipConfig playerShipConfig = infomanager.userData.currentShip;
                    SpawnPlayer(playerShipConfig, pointsTeam0[0].transform);


                    StartCoroutine(CoSpawnDestoyers(1f, 30f));
                    StartCoroutine(CoSpawnTorpedoShips(1f, 20f));
                    StartCoroutine(CoSpawnBoss("阿波罗", 90f, null));
                }
                break;
            case GameType.MissionC:
                {
                    viewDistance = 4000f;


                    //-----
                    textMission.text = "<b>任务目标：支援友军并在敌人的进攻中存活</b>";

                    //-----ships list-----
                    //...

                    units = new List<IShipController>();
                    allies = new List<IShipController>();
                    enemies = new List<IShipController>();

                    //-----Instantiate---player
                    SpawnPoint[] pointsTeam0 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 0).ToArray();

                    ShipConfig playerShipConfig = infomanager.userData.currentShip;
                    SpawnPlayer(playerShipConfig, pointsTeam0[0].transform);

                    //-----Instantiate--ally

                    //---
                    int level = Mathf.Clamp(Mathf.RoundToInt(InfoManager.GetInstance().userData.currentLvl * 1.5f), 1, 30); // 1-20 => 1-30

                    ShipConfig allyShipConfig = new ShipConfig();
                    allyShipConfig.shipInfo = InfoManager.GetInstance().shipInfos.FirstOrDefault(s => s.displayName == "波塞冬");
                    allyShipConfig.lvl = level;

                    ShipWeaponCom weaponCom = new ShipWeaponCom();
                    int allyComIndex1 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().weaponInfos.Count);
                    weaponCom.weaponInfo = InfoManager.GetInstance().weaponInfos[allyComIndex1];
                    weaponCom.lvl = level;

                    ShipArmorCom armorCom = new ShipArmorCom();
                    int allyComIndex2 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().armorInfos.Count);
                    armorCom.armorInfo = InfoManager.GetInstance().armorInfos[allyComIndex2];
                    armorCom.lvl = level;

                    ShipEngineCom engineCom = new ShipEngineCom();
                    int allyComIndex3 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().engineInfos.Count);
                    engineCom.engineInfo = InfoManager.GetInstance().engineInfos[allyComIndex3];
                    engineCom.lvl = level;

                    allyShipConfig.activeWeaponCom = weaponCom;
                    allyShipConfig.activeArmorCom = armorCom;
                    allyShipConfig.activeEngineCom = engineCom;
                    allyShipConfig.lvl = level;

                    GameObject allyShip = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/波塞冬"), pointsTeam0[1].transform.position, new Quaternion());
                    //height
                    allyShip.transform.position = new Vector3(allyShip.transform.position.x, allyShip.GetComponentInChildren<Float>().BalanceHeight, allyShip.transform.position.z);

                    allyShip.GetComponent<Hull>().shipConfig = allyShipConfig;

                    allyShip.AddComponent<AIController>();
                    allyShip.GetComponent<AIController>().Name = "支援目标";
                    allyShip.GetComponent<AIController>().team = 0;
                    allyShip.GetComponent<AIController>().aiMode = AIMode.MoveTo;
                    allyShip.tag = "Untagged";

                    InitShip(allyShip.GetComponent<AIController>());

                    //-----this ally add to list-----
                    allies.Add(allyShip.GetComponent<AIController>());
                    units.Add(allyShip.GetComponent<AIController>());


                    StartCoroutine(CoSpawnDestoyers(1f, 15f));
                    StartCoroutine(CoSpawnBoss("雅典娜", 90f, null));
                }
                break;
            default:
                {
                    viewDistance = 8000f;

                    //-----
                    textMission.text = "教程：歼灭敌人";
                    //---

                    camSphere.DisableRain();

                    //Player
                    GameObject point = new GameObject();
                    point.transform.position = new Vector3();
                    point.transform.rotation = new Quaternion();

                    GameObject tuPlayer = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/雅典娜"), point.transform.position, point.transform.rotation);
                    //height
                    tuPlayer.transform.position = new Vector3(tuPlayer.transform.position.x, -tuPlayer.GetComponentInChildren<Float>().BalanceHeight, tuPlayer.transform.position.z);

                    tuPlayer.AddComponent<PlayerController>();
                    tuPlayer.tag = "Player";
                    ShipConfig playerShipConfig = infomanager.userData.currentShip;

                    tuPlayer.GetComponent<Hull>().shipConfig = playerShipConfig;

                    InitShip(tuPlayer.GetComponent<IShipController>());

                    player = tuPlayer.GetComponent<PlayerController>();

                    //Target
                    GameObject point2 = new GameObject();
                    point2.transform.position = new Vector3(1000f, 0f, 500f);
                    point2.transform.rotation = new Quaternion();

                    GameObject tuDD = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/驱逐舰"), point2.transform.position, point2.transform.rotation);
                    //height
                    tuDD.transform.position = new Vector3(tuDD.transform.position.x, -tuDD.GetComponentInChildren<Float>().BalanceHeight, tuDD.transform.position.z);

                    tuDD.AddComponent<AIController>();
                    tuDD.GetComponent<AIController>().team = 1;
                    tuDD.GetComponent<AIController>().aiMode = AIMode.Guard;

                    tuDD.tag = "Untagged";

                    tuDD.GetComponent<Hull>().shipConfig = playerShipConfig;

                    InitShip(tuDD.GetComponent<IShipController>(), 1f);
                }
                break;
        }
    }


    void Start()
    {
        //Audio Source
        Instantiate(Resources.Load<GameObject>("Prefabs/BGM"));
        //音量
        foreach (var asc in FindObjectsOfType<AudioSource>())
        {
            asc.volume = SoundPlayer.GetVolume();
        }

        //录制
        Recorder.StartRecord(() => { }, (code, msg) => { }, 600);



        switch (gameType)
        {
            case GameType.Match:
                timeCountDown = 180f;
                break;
            case GameType.MissionA:
                timeCountDown = 180f;
                break;
            case GameType.MissionB:
                timeCountDown = 180f;
                break;
            case GameType.MissionC:
                timeCountDown = 180f;
                break;
            default:
                break;
        }
    }
    

    void Update()
    {

        //Test
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    Debug.Log("队友数：" + allies.Count + " 敌人数：" + enemies.Count);
        //}

        //Time
        gameTime += Time.deltaTime;
        timeCountDown = Mathf.Clamp(timeCountDown - Time.deltaTime, 0f, 999f);

        if(timeCountDown < 1f)
        {
            Judge();
        }

        if(!recordedStopped && gameTime > 60f)
        {
            recordedStopped = true;
            Recorder.StopRecord((path) => { }, (code, msg) => { });
        }
    }


    public void Judge()
    {
        if (isEnd) return;


        //匹配和任务的最后一个敌方AI
        switch (gameType)
        {
            case GameType.Match:
            case GameType.MissionA:
                {
                    if (FindObjectsOfType<AIController>().ToArray().Where(ship => ship.Team == 1 && ship.Enabled).ToArray().Length == 1)
                    {
                        foreach (var ship in FindObjectsOfType<AIController>().Where(s => s.Team == 0))
                        {
                            foreach (var turret in ship.GetComponentsInChildren<Turret>())
                            {
                                turret.enabled = false;
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
        

        //胜负判断
        switch (gameType)
        {
            case GameType.Match:
                {
                    List<IShipController> team0List = new List<IShipController>();
                    team0List.Add(player);
                    team0List.AddRange(allies);

                    if (team0List.Where(enemy => enemy.Enabled == true).ToArray().Length < 1)
                    {
                        Lose();
                    }
                    if (enemies.Where(enemy => enemy.Enabled == true).ToArray().Length < 1)
                    {
                        Win();
                    }
                }
                break;
            case GameType.MissionA:
                {
                    if (player.Enabled == false || timeCountDown < 1f)
                    {
                        Lose();
                    }
                    if (enemies.Where(enemy => enemy.Enabled == true).ToArray().Length < 1)
                    {
                        Win();
                    }
                }
                break;
            case GameType.MissionB:
                {
                    if (player.Enabled == false)
                    {
                        Lose();
                    }
                    if (timeCountDown < 1f)
                    {
                        Win();
                    }
                }
                break;
            case GameType.MissionC:
                {
                    if(FindObjectsOfType<AIController>().FirstOrDefault(c => c.team == 0).Enabled == false || player.Enabled == false)
                    {
                        Lose();
                    }
                    if (timeCountDown < 1f)
                    {
                        Win();
                    }
                }
                break;
            default:
                break;
        }
    }


    public void CalAssetsRewardAndProgress(bool win)
    {
        //普通物资和碎片
        switch (gameType)
        {
            case GameType.Match:
                {
                    rewards = new List<LevelReward>();
                    LevelReward money = new LevelReward(); money.name = "金币"; money.count = Mathf.RoundToInt(200f * Utils.CalFac(InfoManager.GetInstance().userData.currentShip.lvl));
                    LevelReward mat = new LevelReward(); mat.name = "材料"; mat.count = Mathf.RoundToInt(2f * InfoManager.GetInstance().userData.currentShip.lvl);
                    //LevelReward debris = new LevelReward(); debris.name = "碎片"; debris.count = 1; debris.index = 1;

                    rewards.Add(money);
                    rewards.Add(mat);
                    //rewards.Add(debris);
                    
                }
                break;
            case GameType.MissionA:
                {
                    int level = InfoManager.GetInstance().userData.currentLvl;
                    rewards = new List<LevelReward>();
                    LevelReward money = new LevelReward(); money.name = "金币"; money.count = Mathf.RoundToInt(100f * level);
                    LevelReward mat = new LevelReward(); mat.name = "材料"; mat.count = Mathf.RoundToInt(1f * level);
                    LevelReward debris = new LevelReward(); debris.name = "碎片"; debris.count = 1; debris.index = 1;//1//克洛诺斯

                    rewards.Add(money);
                    rewards.Add(mat);
                    rewards.Add(debris);
                    
                }
                break;
            case GameType.MissionB:
                {
                    int missionLevel = InfoManager.GetInstance().userData.currentLvl;
                    rewards = new List<LevelReward>();
                    LevelReward money = new LevelReward(); money.name = "金币"; money.count = Mathf.RoundToInt(200f * missionLevel);
                    LevelReward mat = new LevelReward(); mat.name = "材料"; mat.count = Mathf.RoundToInt(2f * missionLevel);
                    LevelReward debris = new LevelReward(); debris.name = "碎片"; debris.count = 1; debris.index = Random.Range(2, 4);// 2 ,3 阿波罗或者埃尔忒米斯

                    rewards.Add(money);
                    rewards.Add(mat);
                    rewards.Add(debris);
                    
                }
                break;
            case GameType.MissionC:
                {
                    int level = InfoManager.GetInstance().userData.currentLvl;
                    rewards = new List<LevelReward>();
                    LevelReward money = new LevelReward(); money.name = "金币"; money.count = Mathf.RoundToInt(300f * level);
                    LevelReward mat = new LevelReward(); mat.name = "材料"; mat.count = Mathf.RoundToInt(3f * level);
                    LevelReward debris = new LevelReward(); debris.name = "碎片"; debris.count = 1; debris.index = 5;//5//雅典娜

                    rewards.Add(money);
                    rewards.Add(mat);
                    rewards.Add(debris);
                    
                }
                break;
            default:
                break;
        }

        //输赢差异
        if (win)
        {
            //胜利埋点
            if(this.gameType != GameType.Tutorial)
            {
                MaiDian.Mai("number", InfoManager.GetInstance().userData.totalGames, "version", InfoManager.Version, "endLevel");
            }


            //资源翻倍
            foreach (var reward in rewards)
            {
                reward.count = reward.count * 2;
            }

            //远征进度
            InfoManager.GetInstance().userData.achieveProgress = Mathf.Clamp(InfoManager.GetInstance().userData.achieveProgress + 50, 0, 1000);

            //关卡解锁和装备掉落

            var equipsC = InfoManager.GetInstance().allEquipmentInfos.Where(e => e.rank == "C").ToList();
            var equipsB = InfoManager.GetInstance().allEquipmentInfos.Where(e => e.rank == "B").ToList();
            var equipsA = InfoManager.GetInstance().allEquipmentInfos.Where(e => e.rank == "A").ToList();
            var equipsS = InfoManager.GetInstance().allEquipmentInfos.Where(e => e.rank == "S").ToList();


            switch (this.gameType)
            {
                case GameType.MissionA:
                    {
                        //关卡进度
                        InfoManager.GetInstance().userData.currentMission += 1;
                    }
                    break;
                case GameType.MissionB:
                    {
                        
                        //关卡进度
                        InfoManager.GetInstance().userData.currentMission += 1;
                    }
                    break;
                case GameType.MissionC:
                    {

                        //关卡进度
                        InfoManager.GetInstance().userData.currentMission = 0;
                        InfoManager.GetInstance().userData.currentLvl = Mathf.Clamp(InfoManager.GetInstance().userData.currentLvl + 1, 1, 20);
                    }
                    break;
            }

            switch (this.gameType)
            {
                case GameType.MissionA:
                case GameType.MissionB:
                case GameType.MissionC:
                    {
                        if(InfoManager.GetInstance().userData.currentLvl <= 5)
                        {
                            LevelReward com = new LevelReward();
                            com.name = equipsC[Random.Range(0, equipsC.Count)].displayName;
                            com.count = 1;
                            rewards.Add(com);
                        }
                        else if (InfoManager.GetInstance().userData.currentLvl <= 10)
                        {
                            LevelReward com = new LevelReward();
                            com.name = equipsB[Random.Range(0, equipsC.Count)].displayName;
                            com.count = 1;
                            rewards.Add(com);
                        }
                        else if (InfoManager.GetInstance().userData.currentLvl <= 15)
                        {
                            LevelReward com = new LevelReward();
                            com.name = equipsA[Random.Range(0, equipsC.Count)].displayName;
                            com.count = 1;
                            rewards.Add(com);
                        }
                        else if (InfoManager.GetInstance().userData.currentLvl <= 20)
                        {
                            LevelReward com = new LevelReward();
                            com.name = equipsS[Random.Range(0, equipsC.Count)].displayName;
                            com.count = 1;
                            rewards.Add(com);
                        }
                    }
                    break;
                default:
                    break;

            }
        }
        else
        {
            //远征进度
            InfoManager.GetInstance().userData.achieveProgress = Mathf.Clamp(InfoManager.GetInstance().userData.achieveProgress + 25, 0, 1000);
        }
    }

    public void GetAssetsReward()
    {
        //物资
        foreach (LevelReward reward in rewards)
        {
            switch (reward.name)
            {
                case "金币":
                    InfoManager.GetInstance().userData.money += reward.count;
                    break;
                case "材料":
                    InfoManager.GetInstance().userData.equipmentMat += reward.count;
                    break;
                case "碎片":
                    InfoManager.GetInstance().userData.debris[reward.index] += reward.count;
                    break;
                default:
                    var equipInfo = InfoManager.GetInstance().allEquipmentInfos.FirstOrDefault(e => e.displayName == reward.name);
                    if (equipInfo != null)
                    {
                        MainMenuCanvas.GiveNewCom(equipInfo);
                    }
                    break;
            }
        }
    }





    //---------------------------------------------------------------------战舰生成--------------------------------------------------------------------------
    //---------------------------------------------------------------------战舰生成--------------------------------------------------------------------------

    public GameObject SpawnPlayer(ShipConfig playerShipConfig, Transform point)
    {
        GameObject playerShip = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/" + playerShipConfig.shipInfo.displayName), point.transform.position, point.transform.rotation);
        //height
        playerShip.transform.position = new Vector3(playerShip.transform.position.x, -playerShip.GetComponentInChildren<Float>().BalanceHeight, playerShip.transform.position.z);

        playerShip.GetComponent<Hull>().shipConfig = playerShipConfig;

        playerShip.AddComponent<PlayerController>();
        playerShip.GetComponent<PlayerController>().Name = InfoManager.GetInstance().userData.name;
        playerShip.tag = "Player";

        player = playerShip.GetComponent<PlayerController>();
        units.Add(player);

        InitShip(playerShip.GetComponent<PlayerController>());

        return playerShip;
    }

    public GameObject SpawnAIBattleShip(Transform point, int shipInfoId, int comRankLvl, int[] lvlRange, string name, int team = 0, AIMode  mode = AIMode.Hunt, float powerMultip = 1f)
    {
        ShipConfig aiShipConfig = new ShipConfig();
        aiShipConfig.shipInfo = InfoManager.GetInstance().shipInfos.FirstOrDefault(s => s.id == shipInfoId);
        aiShipConfig.lvl = Random.Range(lvlRange[0], lvlRange[1]);

        ShipWeaponCom weaponCom = new ShipWeaponCom();
        var weaponComsThisRank = InfoManager.GetInstance().weaponInfos.Where(w => w.ranklvl == comRankLvl).ToArray();
        weaponCom.weaponInfo = weaponComsThisRank[Random.Range(0, weaponComsThisRank.Length)];
        weaponCom.lvl = Random.Range(lvlRange[0], lvlRange[1]);

        ShipArmorCom armorCom = new ShipArmorCom();
        var armorComsThisRank = InfoManager.GetInstance().armorInfos.Where(w => w.ranklvl == comRankLvl).ToArray();
        armorCom.armorInfo = armorComsThisRank[Random.Range(0, armorComsThisRank.Length)];
        armorCom.lvl = Random.Range(lvlRange[0], lvlRange[1]);

        ShipEngineCom engineCom = new ShipEngineCom();
        var engineComsThisRank = InfoManager.GetInstance().engineInfos.Where(w => w.ranklvl == comRankLvl).ToArray();
        engineCom.engineInfo = engineComsThisRank[Random.Range(0, engineComsThisRank.Length)];
        engineCom.lvl = Random.Range(lvlRange[0], lvlRange[1]);

        aiShipConfig.activeWeaponCom = weaponCom;
        aiShipConfig.activeArmorCom = armorCom;
        aiShipConfig.activeEngineCom = engineCom;
        aiShipConfig.lvl = Random.Range(lvlRange[0], lvlRange[1]);

        GameObject aiShipGameObj = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/" + aiShipConfig.shipInfo.displayName), point.transform.position, point.transform.rotation);
        //height
        aiShipGameObj.transform.position = new Vector3(aiShipGameObj.transform.position.x, -aiShipGameObj.GetComponentInChildren<Float>().BalanceHeight, aiShipGameObj.transform.position.z);

        aiShipGameObj.GetComponent<Hull>().shipConfig = aiShipConfig;

        aiShipGameObj.AddComponent<AIController>();
        aiShipGameObj.GetComponent<AIController>().Name = name;
        aiShipGameObj.GetComponent<AIController>().team = team;
        aiShipGameObj.GetComponent<AIController>().aiMode = mode;


        //******重要：战列舰武器最远攻击距离等于识别距离 ****
        aiShipGameObj.GetComponent<AIController>().primarydistance = this.viewDistance * Game.identityDistanceFac;
        aiShipGameObj.GetComponent<AIController>().secondarydistance = this.viewDistance * Game.identityDistanceFac * 0.5f;

        aiShipGameObj.tag = "Untagged";

        if(team == 0)
        {
            allies.Add(aiShipGameObj.GetComponent<AIController>());
        }
        else
        {
            enemies.Add(aiShipGameObj.GetComponent<AIController>());
        }
        units.Add(aiShipGameObj.GetComponent<AIController>());

        
        InitShip(aiShipGameObj.GetComponent<AIController>(), powerMultip);

        return aiShipGameObj;
    }


    /// <summary>
    /// 等级：任务等级，地点：spawnpoint01
    /// </summary>
    public GameObject SpawnDestroyer(Transform point, AIMode mode = AIMode.Hunt, int team = 1)
    {
        Debug.Log("DD Spawn");

        //enemys
        int level = Mathf.Clamp(Mathf.RoundToInt(InfoManager.GetInstance().userData.currentLvl * 1.5f), 1, 30); // 1-20 => 1-30

        //Power Multip(模拟rank差异)
        float powerMultip = (InfoManager.GetInstance().userData.currentLvl + 1) * 0.5f;//1 - 10

        ShipConfig enemyShipConfig = new ShipConfig();
        enemyShipConfig.shipInfo = InfoManager.GetInstance().shipInfos.FirstOrDefault(s => s.id == 99);
        enemyShipConfig.lvl = level;

        //ShipWeaponCom weaponCom = new ShipWeaponCom();
        //int enemyComIndex1 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().weaponInfos.Count);
        //weaponCom.weaponInfo = InfoManager.GetInstance().weaponInfos[enemyComIndex1];
        //weaponCom.lvl = level;

        //ShipArmorCom armorCom = new ShipArmorCom();
        //int enemyComIndex2 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().armorInfos.Count);
        //armorCom.armorInfo = InfoManager.GetInstance().armorInfos[enemyComIndex2];
        //armorCom.lvl = level;

        //ShipEngineCom engineCom = new ShipEngineCom();
        //int enemyComIndex3 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().engineInfos.Count);
        //engineCom.engineInfo = InfoManager.GetInstance().engineInfos[enemyComIndex3];
        //engineCom.lvl = level;

        enemyShipConfig.activeWeaponCom = null;//武器组件对副炮/鱼雷无用
        enemyShipConfig.activeArmorCom = null;
        enemyShipConfig.activeEngineCom = null;
        enemyShipConfig.lvl = level;

        GameObject enemyShip0 = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/" + enemyShipConfig.shipInfo.displayName), point.transform.position, point.transform.rotation);
        //height
        enemyShip0.transform.position = new Vector3(enemyShip0.transform.position.x, -enemyShip0.GetComponentInChildren<Float>().BalanceHeight, enemyShip0.transform.position.z);

        enemyShip0.GetComponent<Hull>().shipConfig = enemyShipConfig;

        enemyShip0.AddComponent<AIController>();
        enemyShip0.GetComponent<AIController>().team = team;
        enemyShip0.GetComponent<AIController>().aiMode = mode;

        //******重要：驱逐舰武器最远攻击距离等于识别距离 * 0.75 ****
        enemyShip0.GetComponent<AIController>().secondarydistance = this.viewDistance * Game.identityDistanceFac;

        enemyShip0.GetComponent<AIController>().Name = "驱逐舰(等级" + level + ")";
        enemyShip0.tag = "Untagged";

        enemies.Add(enemyShip0.GetComponent<AIController>());
        units.Add(enemyShip0.GetComponent<AIController>());

        //Init
        InitShip(enemyShip0.GetComponent<AIController>(), powerMultip, true);

        return enemyShip0;
    }

    public GameObject SpawnTorpedoShip(Transform point, AIMode mode = AIMode.Hunt, int team = 1)
    {
        Debug.Log("TorpedoShip Spawn");

        //enemys
        int level = Mathf.Clamp(Mathf.RoundToInt(InfoManager.GetInstance().userData.currentLvl * 1.5f), 1, 30); // 1-20 => 1-30

        //Power Multip(模拟rank差异)
        float powerMultip = (InfoManager.GetInstance().userData.currentLvl + 1) * 0.5f;//1 - 10

        ShipConfig enemyShipConfig = new ShipConfig();
        enemyShipConfig.shipInfo = InfoManager.GetInstance().shipInfos.FirstOrDefault(s => s.id == 98);
        enemyShipConfig.lvl = level;

        //ShipWeaponCom weaponCom = new ShipWeaponCom();
        //int enemyComIndex1 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().weaponInfos.Count);
        //weaponCom.weaponInfo = InfoManager.GetInstance().weaponInfos[enemyComIndex1];
        //weaponCom.lvl = level;

        //ShipArmorCom armorCom = new ShipArmorCom();
        //int enemyComIndex2 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().armorInfos.Count);
        //armorCom.armorInfo = InfoManager.GetInstance().armorInfos[enemyComIndex2];
        //armorCom.lvl = level;

        //ShipEngineCom engineCom = new ShipEngineCom();
        //int enemyComIndex3 = Mathf.FloorToInt((level / 31f) * InfoManager.GetInstance().engineInfos.Count);
        //engineCom.engineInfo = InfoManager.GetInstance().engineInfos[enemyComIndex3];
        //engineCom.lvl = level;

        enemyShipConfig.activeWeaponCom = null;//武器组件对副炮/鱼雷无用
        enemyShipConfig.activeArmorCom = null;
        enemyShipConfig.activeEngineCom = null;
        enemyShipConfig.lvl = level;

        GameObject enemyShip0 = Instantiate(Resources.Load<GameObject>("Prefabs/Ships/" + enemyShipConfig.shipInfo.displayName), point.transform.position, point.transform.rotation);
        enemyShip0.GetComponent<Hull>().shipConfig = enemyShipConfig;

        enemyShip0.AddComponent<AIController>();
        enemyShip0.GetComponent<AIController>().team = team;
        enemyShip0.GetComponent<AIController>().aiMode = mode;
        enemyShip0.GetComponent<AIController>().Name = "鱼雷艇(等级" + level + ")";
        enemyShip0.tag = "Untagged";

        enemies.Add(enemyShip0.GetComponent<AIController>());
        units.Add(enemyShip0.GetComponent<AIController>());

        //Init
        InitShip(enemyShip0.GetComponent<AIController>(), powerMultip, true);

        return enemyShip0;
    }


    public IEnumerator CoSpawnDestoyers(float delay, float interval)
    {
        yield return new WaitForSeconds(delay);

        while (!isEnd)
        {
            SpawnPoint[] pointsTeam1 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 1).Where(p => (p.transform.position - player.transform.position).magnitude > 2000f).OrderBy(p => (p.transform.position - player.transform.position).sqrMagnitude).Skip(1).ToArray();

            SpawnPoint rdmpoint = pointsTeam1[Random.Range(0, Mathf.Clamp(pointsTeam1.Length / 4, 1, 99))];

            SpawnDestroyer(rdmpoint.transform, AIMode.Hunt);

            yield return new WaitForSeconds(interval);
        }
    }

    public IEnumerator CoSpawnTorpedoShips(float delay, float interval)
    {
        yield return new WaitForSeconds(delay);

        while (!isEnd)
        {
            SpawnPoint[] pointsTeam1 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 1).Where(p => (p.transform.position - player.transform.position).magnitude > 2000f).OrderBy(p => (p.transform.position - player.transform.position).sqrMagnitude).Skip(1).ToArray();

            SpawnPoint rdmpoint = pointsTeam1[Random.Range(0, Mathf.Clamp(pointsTeam1.Length / 4, 1, 99))];

            SpawnTorpedoShip(rdmpoint.transform, AIMode.Hunt);

            yield return new WaitForSeconds(interval);
        }
    }

    public IEnumerator CoSpawnSupport()
    {
        yield return null;

        ShipInfo supportInfo = InfoManager.GetInstance().userData.currentShip.shipInfo;
        int[] levelrange = new int[2] { InfoManager.GetInstance().userData.currentShip.lvl, InfoManager.GetInstance().userData.currentShip.lvl };

        GameObject o = SpawnAIBattleShip(player.transform, supportInfo.id, supportInfo.ranklvl, levelrange, "援军", 0, AIMode.Hunt, 0.8f);
        o.transform.position += (player.transform.right * 80f).OnOceanPlane();

        this.supportShip = o.GetComponent<AIController>();
    }

    public IEnumerator CoSpawnBoss(string shipName, float delay, Transform pos = null)
    {
        yield return new WaitForSeconds(delay);

        Transform bossPoint = pos;
        if (pos == null)
        {
            SpawnPoint[] pointsTeam1 = FindObjectsOfType<SpawnPoint>().Where(p => p.team == 1).Where(p => (p.transform.position - player.transform.position).magnitude > 2000f).OrderBy(p => (p.transform.position - player.transform.position).sqrMagnitude).Skip(1).ToArray();
            bossPoint = pointsTeam1[0].transform;
        }

        int rankLvl = InfoManager.GetInstance().shipInfos.Find(s => s.displayName == shipName).ranklvl;

        float powerMultip = ((InfoManager.GetInstance().userData.currentLvl + 1) * 0.5f);

        switch (rankLvl) //抵消rank因素（并不完全抵消）
        {
            case 1:
                powerMultip *= 0.75f * 1f * 1f; //0.75f 全局难度系数  1f 抵消rank差异 1f 关卡难度系数
                break;
            case 2:
                powerMultip *= 0.75f * 0.33f * 1.15f;
                break;
            case 3:
                powerMultip *= 0.75f * 0.15f * 1.25f;
                break;
            case 4:
                powerMultip *= 0.75f * 0.09f * 1.5f;
                break;
            default:
                break;
        }

        Debug.Log("==BOSS== (shipAndComRank: " + rankLvl + "multip:" + powerMultip + ")");

        int level = Mathf.Clamp(Mathf.RoundToInt(InfoManager.GetInstance().userData.currentLvl * 1.5f), 1, 30); // 1-20 => 1-30
        int[] lvlrange = new int[2] { level, level };
        GameObject o = SpawnAIBattleShip(bossPoint, InfoManager.GetInstance().shipInfos.Find(s => s.displayName == shipName).id, rankLvl, lvlrange, shipName + "(等级" + level + ")", 1, AIMode.Hunt, powerMultip);
        (o.GetComponents<Ability>().FirstOrDefault(a => a.Index == 1) as MonoBehaviour).enabled = false;
        (o.GetComponents<Ability>().FirstOrDefault(a => a.Index == 2) as MonoBehaviour).enabled = false;
    }



    private void InitShip(IShipController ship, float powerMultip = 1f, bool disableSpeedAdd = false)
    {
        Hull hull = ship.GameObj.GetComponent<Hull>();
        hull.shipPowerMultip = powerMultip;

        //---设置属性---

        //初始化血量
        //Debug.Log("Cal(1):" + Utils.CalFac(1));
        hull.MaxHP = hull.shipConfig.shipInfo.hp * Utils.CalFac(hull.shipConfig.lvl); //if (ship is AIController) { Debug.Log("战舰裸血量: " + hull.MaxHP + "系数: " + Utils.CalFac(hull.shipConfig.lvl)); }
        if (hull.shipConfig.activeArmorCom != null)//装甲加血
        {
            hull.MaxHP += hull.shipConfig.activeArmorCom.armorInfo.hp * Utils.CalFac(hull.shipConfig.activeArmorCom.lvl); //if (ship is AIController) { Debug.Log("装甲血量加成后：" + hull.MaxHP); }
        }
        if(hull.shipConfig.activeEngineCom != null)//引擎加血
        {
            hull.MaxHP += hull.shipConfig.activeEngineCom.engineInfo.hp * Utils.CalFac(hull.shipConfig.activeEngineCom.lvl);
        }
        if(hull.shipConfig.crew != null)//船员加血（%）
        {
            hull.MaxHP *= Utils.CalFacCrew(hull.shipConfig.crew.crewInfo.hp, hull.shipConfig.crew.lvl);
        }

        if(ship is PlayerController)//玩家生命倍数
        {
            hull.MaxHP *= 5f;
        }

        hull.MaxHP *= hull.shipPowerMultip;
        hull.HP = hull.MaxHP;

        //初始化防御
        ShipConfig shipConfig = hull.shipConfig;
        ShipArmorCom armorCom = hull.shipConfig.activeArmorCom;
        float armor = shipConfig.shipInfo.defense * Utils.CalFac(shipConfig.lvl);
        if (armorCom != null)//装甲加防御
        {
            armor += armorCom.armorInfo.defense * Utils.CalFac(armorCom.lvl);
        }
        if (hull.shipConfig.crew != null)//船员加防御（%）
        {
            armor *= Utils.CalFacCrew(hull.shipConfig.crew.crewInfo.defense, hull.shipConfig.crew.lvl);
        }
        armor *= hull.shipPowerMultip;
        hull.Armor = armor;

        //初始化炮塔
        foreach (var item in hull.GetComponentsInChildren<Turret>())
        {
            item.TurretInit(hull.shipPowerMultip);
        }

        //初始化引擎
        ship.GameObj.GetComponent<Engine>().thrustPower = ship.GameObj.GetComponent<Hull>().shipConfig.shipInfo.speed / 4f;
        if(!disableSpeedAdd && hull.shipConfig.activeEngineCom != null)//装备加速（%）
        {
            ship.GameObj.GetComponent<Engine>().thrustPower *= 1f + (hull.shipConfig.activeEngineCom.engineInfo.speed * 0.001f) * Utils.CalFac(hull.shipConfig.activeEngineCom.lvl);
        }
        if (!disableSpeedAdd && hull.shipConfig.crew != null)//船员加速（%）
        {
            ship.GameObj.GetComponent<Engine>().thrustPower *= Utils.CalFacCrew(hull.shipConfig.crew.crewInfo.speed, hull.shipConfig.crew.lvl);
        }
    }


    //---------------------------------------------------------------------场景相关---------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 返回主界面
    /// </summary>
    public void ReturnMainMenuScene()
    {
        SceneManager.LoadScene("SceneMainMenu");
        InfoManager.showAchieveOnLoad = true;
    }


    //---------------------------------------------------------------------游戏相关---------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------------------------------------------------

    //WinLose


    private void Win()
    {
        isEnd = true;
        CalAssetsRewardAndProgress(true);
        uiCanvas.ShowWindowWinLose(0);
    }

    private void Lose()
    {
        isEnd = true;
        CalAssetsRewardAndProgress(false);
        uiCanvas.ShowWindowWinLose(1);
    }

    public void Leave()//立即结算
    {
        isEnd = true;
        CalAssetsRewardAndProgress(false);
        uiCanvas.ShowWindowWinLose(2);
    }

    public void ExitGame()//强制退出游戏
    {
        isEnd = true;
        ReturnMainMenuScene();
        InfoManager.showAchieveOnLoad = false;
    }

    public void OnGUI()
    {
        //GUILayout.TextArea("InfoManager.MissionType:" + InfoManager.currentMissionType + "游戏模式：" + gameType.ToString());
    }








}
