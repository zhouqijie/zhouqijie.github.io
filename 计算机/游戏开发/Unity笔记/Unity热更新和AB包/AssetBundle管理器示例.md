# AssetBundle管理器示例    

```C#  
//#define EDITOR_CODE_HIDE
//#define WECHAT
#define ANDROID_APK


//* StreamingAssets 不能写入 随包发包   适合存放一些初始化的AssetBundle资源 如 登入页 加载页等

//* PersistentData 适合存放 运行程序下载的AssetBundle资源


//StreamingAssets目录必须在Assets根目录下，该目录下所有资源也会被打包到游戏里，不同于Resources目录，该目录下的资源不会进行压缩，同样是只读不可写的。
//Unity基本也没有提供从该路径下直接读取资源的方法，只有www可以加载audioClip、texture和二进制文件。但Unity提供了从该目录加载AssetBundle的方法，我们一般直接在这个目录下存放AssetBundle文件。可以通过Application.streamingAssetsPath访问该路径。

//PersistentDataPath
//该目录为应用程序沙盒目录，应用程序安装后才会出现。该目录独特之处在于是可写的，所以我们一般将下载的AssetBundle存放于此。使用Application.persistentDataPath访问。




using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using LitJson;


#if UNITY_EDITOR
using UnityEditor;
#endif

#if WECHAT
using WeChatWASM;
#endif



public class AssetBundleRequire
{
    /// <summary>
    /// 基本信息
    /// </summary>
    public string name;     //包名  
    public int length;      //文件长度
    public string url;      //下载地址
    public string devicePath;  //设备路径 -- （目前仅WX使用）  

    /// <summary>
    /// 网络请求
    /// </summary>
    public UnityWebRequest request;

    /// <summary>
    /// 下载完成回调
    /// </summary>
    public UnityAction savedCallBack;

    /// <summary>
    /// 已下载保存
    /// </summary>
    public bool isSaved = false;





    
    public float GetProgress()
    {
        if (isSaved)
        {
            return 1f;
        }
        else
        {
            if (request != null)
            {
                return request.downloadProgress;
            }
            else
            {
                return 0f;
            }
        }
    }
    public string GetProgressText()
    {
        if (isSaved)
        {
            return "(完毕)";
        }
        else
        {
            if (request != null)
            {
                return (request.downloadedBytes / 1000) + "kb/" + (length / 1000) + "kb";
            }
            else
            {
                return "(unknown)";
            }
        }

    }

    public AssetBundleRequire(string name, int length, string url)
    {
        this.name = name; this.length = length; this.url = url; this.devicePath = ""; this.savedCallBack = null;

#if UNITY_EDITOR && !EDITOR_CODE_HIDE
        this.devicePath = "";
#elif WECHAT
        this.devicePath = WX.env.USER_DATA_PATH + "/CWJLDMX/" + this.name;
#endif
    }
}

public class AssetBundleManager : MonoBehaviour
{
    // -------------------------------单例 ------------------------------------------------
    private static AssetBundleManager inst = null;
    public static AssetBundleManager Instance {
        get
        {
            if (inst == null)
                inst = FindObjectOfType<AssetBundleManager>();
            return inst;
        }
    }



    // ------------------------------ 容器 ------------------------------------------------

    [HideInInspector] public AssetBundleRequire[] basicAssetBundles = null;
    [HideInInspector] public AssetBundleRequire[] nonBasicAssetBundles = null;
    [HideInInspector] public List<AssetBundleRequire> appendAssetBundles = new List<AssetBundleRequire>();
    [HideInInspector] public Queue<AssetBundleRequire> remainRequires = null;
    [HideInInspector] public List<AssetBundleRequire> downloadingRequires = null;

    // ---------------------------------------- 外部接口 ----------------------------------------

    /// <summary>
    /// 基础包全部载入事件回调
    /// </summary>
    [HideInInspector] public UnityEvent onAllBasicBundlesLoaded = new UnityEvent();
    /// <summary>
    /// 下一场景异步加载完成回调
    /// </summary>
    [HideInInspector] public UnityEvent onNextSceneReady = new UnityEvent();

    /// <summary>
    /// 基本包已全部载入
    /// </summary>
    public bool AllLoaded => this.allLoaded;

    /// <summary>
    /// 新进入的玩家
    /// </summary>
    public bool IsNewEnter => this.isNewEnter;

    /// <summary>
    /// 基本包已下载字节数
    /// </summary>
    public int DownloadedBytes => this.GetDownloadedBytes();
    
    /// <summary>
    /// 基本包总字节数
    /// </summary>
    public int TotalBytes => this.GetTotalBytes();


    /// <summary>
    /// 新的附加AB包请求
    /// </summary>
    public AssetBundleRequire NewRequireBundle(string name, UnityAction callback)
    {
        AssetBundleRequire newRequire = nonBasicAssetBundles.FirstOrDefault(ab => ab.name == name.ToLower());

        Debug.Log("下载附加包：" + name.ToLower());

        if(newRequire != null)
        {
            // *** 附加到Append列表 *** 
            if (!appendAssetBundles.Contains(newRequire))
            {
                appendAssetBundles.Add(newRequire);
            }


            // *** 已下载或者下载 *** 
            if (newRequire.isSaved)// -- 已下载 -- 
            {
                Debug.Log("已下载过该AB包！");
                newRequire.savedCallBack = callback;
                newRequire.savedCallBack?.Invoke();

                return newRequire;
            }
            else //  -- 未下载 -- 
            {
                if (appendAssetBundles.Contains(newRequire))// -- 已经申请下载过 -- 
                {
                    Debug.Log("下载中");
                    newRequire.savedCallBack = callback;

                    return newRequire;
                }
                else // -- 未申请下载过 -- 
                {
                    Debug.Log("开始下载");
                    newRequire.savedCallBack = callback;

                    //加入下载队列
                    appendAssetBundles.Add(newRequire);
                    remainRequires.Enqueue(newRequire);

                    return newRequire;
                }
            }
        }
        else
        {
            Debug.LogAssertion("AB包名错误:" + name.ToLower());
            Debug.LogAssertion("可以下载的非必要包列表:" + string.Concat(nonBasicAssetBundles.Select(abr=>abr.name + "\n")));
            return null;
        }
        
    }


    // ---------------------------------------- 参数 ----------------------------------------
    
    [Header("同时下载的文件大小（参考值）")]
    public int refLength = 2000000; // 2mb //同时下载的文件长度参考
    [Header("下载AB包的服务器URL")]
    public string serverABUrl = "https://cdn.888xin.top/minigame/UnityWebZoo/AssetBundles/";
    [Header("AB包版本")]
    public string assetVersion = "";


    // ---------------------------------------- 状态相关 ----------------------------------------


    //STATUS
    [HideInInspector] public int runtime = 0;

    private bool allLoaded = false;

    private bool isNewEnter = false;


    // ---------------------------------------- 日志 ----------------------------------------
    private List<string> logList = new List<string>();











#if UNITY_EDITOR && !EDITOR_CODE_HIDE

#elif ANDROID_APK

#elif WECHAT
    private string localDir = WX.env.USER_DATA_PATH + "/CWJLDMX";

    //微信文件管理器
    private WXFileSystemManager mgr = null;
#endif

    void Awake()
    {
        inst = this;    
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);



        //比较资源版本 --> 清除文件缓存
        if (this.assetVersion != PlayerPrefs.GetString("assetVersion", ""))
        {
            CleanFilesCache_General();
            Debug.Log("资源版本比较不一致...清除资源文件...");
        }
        else
        {
            Debug.Log("资源版本比较一致...");
        }
        

        //（平台无关）生成基本/非基本AB包需求信息
        GenAbRequires();


        //（平台无关）判断是否已存在文件
        List<AssetBundleRequire> allabr = new List<AssetBundleRequire>(basicAssetBundles); allabr.AddRange(nonBasicAssetBundles);
        foreach (var require in allabr)
        {
            if (ExistFile_General(require))
            {
                require.isSaved = true;
            }
            else
            {
                require.isSaved = false;
            }
        }
        //必须下载的AB包
        remainRequires = new Queue<AssetBundleRequire>(basicAssetBundles.Where(r => r.isSaved == false).OrderBy(r => r.length));
        downloadingRequires = new List<AssetBundleRequire>();
        
        //新用户
        isNewEnter = basicAssetBundles.All(r => r.isSaved == false);

        //开始下载埋点
        if (isNewEnter)
        {
            MaiDian.Mai("number", 1, "version", Infomanager.MaiDianVersion, "enterGameLoading");
        }

#if UNITY_EDITOR && !EDITOR_CODE_HIDE

#elif ANDROID_APK  

#elif WECHAT
        WX.SetGameStage(0);
        
        // WX FILE MANAGER
        mgr = WX.GetFileSystemManager();
#endif

    }

    private void Update()
    {
        if (basicAssetBundles == null) return;

        //运行时间
        runtime += 1;

        //下载
        ProcessDownloading();


        //基本包加载完成
        if (allLoaded) return;
        if (!allLoaded && basicAssetBundles.All(rq => rq.isSaved == true))
        {
            allLoaded = true;


#if UNITY_EDITOR && !EDITOR_CODE_HIDE
            //Debug.Log("");
#elif ANDROID_APK  
            //Debug.Log("");
#elif WECHAT
            WX.SetGameStage(1);
#endif


            //下载完成埋点
            if (isNewEnter)
            {
                MaiDian.Mai("number", 2, "version", Infomanager.MaiDianVersion, "enterGameLoading");
            }



            //SET RES
            SetResourceBundle();




#if UNITY_EDITOR && !EDITOR_CODE_HIDE
            //Debug.Log("");
#elif ANDROID_APK  
            //Debug.Log("");
#elif WECHAT
            WX.ReportGameStart();
#endif

            //更新资源版本
            PlayerPrefs.SetString("assetVersion", this.assetVersion);


            ////自动异步加载下一场景
            //ReadyNextScene();

            //全部下载完成事件
            onAllBasicBundlesLoaded?.Invoke();
        }
    }













    //  -------------------------------------------------------------------------------------  文件下载和IO ----------------------------------------------------------------------------
    //（Editor下载到StreamAssets / 读取：直接从StreamAssets读取）
    //（安卓下载到Persistent目录 / 读取：可以从StreamingAsset读取也可以从Persistent目录读取）  
    //（微信小程序：使用提供的API接口）  


    /// <summary>
    /// 平台无关 读取AB包需求
    /// </summary>
    private void GenAbRequires()
    {
        JsonData jreq = JsonMapper.ToObject(Resources.Load<TextAsset>("AssetBundles").text);

        JsonData jbasicAbs = jreq["basic"];
        AssetBundleRequire[] requiresBasic = new AssetBundleRequire[jbasicAbs.Count];
        for (int i = 0; i < requiresBasic.Length; ++i)
        {
            requiresBasic[i] = new AssetBundleRequire((string)jbasicAbs[i]["file"], (int)jbasicAbs[i]["size"], serverABUrl + (string)jbasicAbs[i]["file"]);
        }
        this.basicAssetBundles = requiresBasic;


        if (jreq.ContainsKey("non-basic"))
        {
            JsonData jnonBasicAbs = jreq["non-basic"];
            AssetBundleRequire[] requiresNonBasic = new AssetBundleRequire[jnonBasicAbs.Count];
            for (int i = 0; i < requiresNonBasic.Length; ++i)
            {
                requiresNonBasic[i] = new AssetBundleRequire((string)jnonBasicAbs[i]["file"], (int)jnonBasicAbs[i]["size"], serverABUrl + (string)jnonBasicAbs[i]["file"]);
            }
            this.nonBasicAssetBundles = requiresNonBasic;
        }
        else
        {
            this.nonBasicAssetBundles = new AssetBundleRequire[0];
        }
    }

    /// <summary>
    /// 平台无关 清除文件缓存
    /// </summary>
    public void CleanFilesCache_General()
    {
#if UNITY_EDITOR && !EDITOR_CODE_HIDE
        DirectoryInfo streamingAssetDir = new DirectoryInfo(Application.streamingAssetsPath);
        foreach (var f in streamingAssetDir.GetFiles())
        {
            if (f.Extension.Contains("ab") || f.Extension.Contains("manifest"))
            {
                System.IO.File.Delete(f.FullName);
            }
        }
#elif ANDROID_APK
        DirectoryInfo persistantDir = new DirectoryInfo(Application.persistentDataPath);
        foreach (var f in persistantDir.GetFiles())
        {
            if (f.Extension.Contains("ab") || f.Extension.Contains("manifest"))
            {
                System.IO.File.Delete(f.FullName);
            }
        }
#elif WECHAT
        Debug.LogAssertion("删除所有文件 未实现");
#endif
    }

    /// <summary>
    /// 平台无关 开始下载
    /// </summary>
    public void StartDownload_General(AssetBundleRequire req)
    {
#if UNITY_EDITOR && !EDITOR_CODE_HIDE
        StartCoroutine(EditorDownLoadAndSave(req));
#elif ANDROID_APK
        StartCoroutine(AndroidDownloadAndSave(req));
#elif WECHAT
        StartCoroutine(WXDownLoadAndSave(req));
#endif
    }

    /// <summary>
    /// 平台无关 读取AssetBundle包
    /// </summary>
    public AssetBundle ReadAssetBundle_General(AssetBundleRequire req)
    {
#if UNITY_EDITOR && !EDITOR_CODE_HIDE
        return ReadBundleAtStreaming(req.name);
#elif ANDROID_APK
        return ReadBundleAndroid(req.name);
#elif WECHAT
        return ReadBundleAtFilePath(req.devicePath);
#endif
        return null;
    }

    /// <summary>
    /// 平台无关 是否存在文件
    /// </summary>
    public bool ExistFile_General(AssetBundleRequire require)
    {
#if UNITY_EDITOR && !EDITOR_CODE_HIDE
        return File.Exists(Application.streamingAssetsPath + "/" + require.name);
#elif ANDROID_APK

        // ---------------- 先在StreamingAsset中查找  --------------------------------
        UnityWebRequest localReq = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + require.name);
        localReq.SendWebRequest();
        //while (!localReq.isDone && localReq.error == null) { }
        if (localReq.error == null)
        {
            logList.Add("存在本地文件");
            localReq.downloadHandler.Dispose();
            return true;
        }

        // ---------------- 然后在Persistent下载中查找 --------------------------------
        bool existAtPersistent = File.Exists(Application.persistentDataPath + "/" + require.name);
        if (existAtPersistent)
        {
            logList.Add("存在本地文件");
            return true;
        }


#elif WECHAT
        return ExistWXFile(require.devicePath);
#endif

        logList.Add("不存在本地文件");
        return false;
    }


    /// <summary>
    /// 平台无关 比较清单
    /// </summary>
    public void CompareMinifest_General(AssetBundleRequire req)
    {
        //...
    }










    // -------------------------------------------------- Download && Save -----------------------------------------------------------------------------

    /// <summary>
    /// 加载AB包（从StreamingAsset目录）  
    /// </summary>
    private AssetBundle ReadBundleAtStreaming(string abname)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abname);
        return ab;
    }

    /// <summary>
    /// 编辑器下载，保存到StreamingAsset目录  
    /// </summary>
    IEnumerator EditorDownLoadAndSave(AssetBundleRequire abRequire)
    {
        //UnityWebRequestAssetBundle.GetAssetBundle(string uri)使用这个API下载回来的资源它是不支持原始数据访问的.

        UnityWebRequest request = UnityWebRequest.Get(abRequire.url);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.timeout = 300;
        abRequire.request = request;

        request.SendWebRequest();

        while (!request.isDone)
        {
            yield return null;
        }


        //下载状态.
        if (request.isNetworkError || request.isHttpError)
        {
            UIMiniPoper.PopText("网络错误！");
        }
        else
        {
            EditorSave(abRequire, request);
        }
    }
    private void EditorSave(AssetBundleRequire abRequire, UnityWebRequest request)
    {
        //构造文件流.
        FileStream fs = File.Create(Application.streamingAssetsPath + "/" + abRequire.name);

        //将字节流写入文件里,request.downloadHandler.data可以获取到下载资源的字节流.
        fs.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);

        //文件写入存储到硬盘，关闭文件流对象，销毁文件对象.
        fs.Flush();
        fs.Close();
        fs.Dispose();

        abRequire.isSaved = true;

        abRequire.savedCallBack?.Invoke();
        Debug.Log("文件" + abRequire.name + "已经下载保存");
    }

#if ANDROID_APK
    
    /// <summary>
    /// 安卓读取AB包  
    /// </summary>
    private AssetBundle ReadBundleAndroid(string abname)
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + abname);
        if(ab != null)
        {
            return ab;
        }

        ab = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + abname);
        return ab;
    }

    /// <summary>
    /// 安卓分包下载
    /// </summary>
    /// <param name="abRequire"></param>
    /// <returns></returns>
    IEnumerator AndroidDownloadAndSave(AssetBundleRequire abRequire)
    {
        //UnityWebRequestAssetBundle.GetAssetBundle(string uri)使用这个API下载回来的资源它是不支持原始数据访问的.

        UnityWebRequest request = UnityWebRequest.Get(abRequire.url);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.timeout = 300;
        abRequire.request = request;

        request.SendWebRequest();

        while (!request.isDone)
        {
            yield return null;
        }


        //下载状态.
        if (request.isNetworkError || request.isHttpError)
        {
            UIMiniPoper.PopText("网络错误！");
        }
        else
        {
            AndroidSave(abRequire, request);
        }
    }
    private void AndroidSave(AssetBundleRequire abRequire, UnityWebRequest request)
    {
        //构造文件流.
        FileStream fs = File.Create(Application.persistentDataPath + "/" + abRequire.name);

        //将字节流写入文件里,request.downloadHandler.data可以获取到下载资源的字节流.
        fs.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);

        //文件写入存储到硬盘，关闭文件流对象，销毁文件对象.
        fs.Flush();
        fs.Close();
        fs.Dispose();

        abRequire.isSaved = true;

        abRequire.savedCallBack?.Invoke();
        Debug.Log("文件" + abRequire.name + "已经下载保存");
    }

#endif

#if WECHAT
    bool ExistWXFile(string path)
    {
        var result = mgr.AccessSync(path);

        return result == "access:ok";
    }


    IEnumerator WXDownLoadAndSave(AssetBundleRequire abRequire)
    {
        //UnityWebRequestAssetBundle.GetAssetBundle(string uri)使用这个API下载回来的资源它是不支持原始数据访问的.

        UnityWebRequest request = UnityWebRequest.Get(abRequire.url);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.timeout = 300;
        abRequire.request = request;

        request.SendWebRequest();

        while (!request.isDone)
        {
            yield return null;
        }


        //下载状态.
        if (request.isNetworkError || request.isHttpError)
        {
            UIMiniPoper.PopText("网络错误！");
        }
        else
        {
            WXSave(abRequire, request);
        }
    }
    private void WXSave(AssetBundleRequire abRequire, UnityWebRequest request)
    {
        ////构造文件流.
        //FileStream fs = File.Create(Application.streamingAssetsPath + "/" + fileName);

        ////将字节流写入文件里,request.downloadHandler.data可以获取到下载资源的字节流.
        //fs.Write(request.downloadHandler.data, 0, request.downloadHandler.data.Length);

        ////文件写入存储到硬盘，关闭文件流对象，销毁文件对象.
        //fs.Flush();
        //fs.Close();
        //fs.Dispose();

        //Debug.Log(fileName + "下载完毕");

        mgr.MkdirSync(localDir, true);

        mgr.WriteFileSync(abRequire.devicePath, request.downloadHandler.data);

        abRequire.isSaved = true;
        
        abRequire.savedCallBack?.Invoke();
        Debug.Log("文件" + abRequire.name + "已经下载保存");
    }


    
    /// <summary>
    /// 加载AB包
    /// </summary>
    private AssetBundle ReadBundleAtFilePath(string localpath)
    {
        byte[] data = mgr.ReadFileSync(localpath);
        var ab = AssetBundle.LoadFromMemory(data);
        return ab;
    }
#endif




    /// <summary>
    /// 加载读取资源AB包
    /// </summary>
    private void SetResourceBundle()
    {
        AssetBundleRequire[] resourcesBundlesReq = basicAssetBundles.Where(abr => abr.name.Contains("resources")).ToArray();

        ResourceBundle.resAbs = resourcesBundlesReq.Select(req => ReadAssetBundle_General(req)).ToList();
    }



    /// <summary>
    /// 下载处理
    /// </summary>
    private void ProcessDownloading()
    {
        if (downloadingRequires.Count < 1 && remainRequires.Count < 1) return;


        //当前同时下载的文件总大小
        int currentSize = downloadingRequires.Sum(r => r.length);


        //检查下载队列
        if (downloadingRequires.Count > 0)
        {
            if (downloadingRequires.Any(r => r.isSaved == true))
            {
                var finishTask = downloadingRequires.FirstOrDefault(r => r.isSaved == true);

                if (finishTask != null)
                {
                    downloadingRequires.Remove(finishTask);
                }
            }
        }

        //检查待下载队列
        if(remainRequires.Count > 0)
        {
            //新的下载任务
            if (currentSize < refLength)
            {
                var newdownload = remainRequires.Dequeue();
                //加入加载队列
                downloadingRequires.Add(newdownload);
                //开始下载
                StartDownload_General(newdownload);
            }
        }

    }

    /// <summary>
    /// 所有AB包的总字节数
    /// </summary>
    /// <returns></returns>
    private int GetTotalBytes()
    {
        if (basicAssetBundles != null)
        {
            int sum = 0;
            foreach (var r in basicAssetBundles)
            {
                sum += r.length;
            }
            return sum;
        }
        else
        {
            return 99999;
        }
    }

    /// <summary>
    /// 已下载AB包的字节数
    /// </summary>
    /// <returns></returns>
    private int GetDownloadedBytes()
    {
        if (basicAssetBundles != null)
        {
            int sum = 0;
            foreach (var r in basicAssetBundles)
            {
                if (r.isSaved)
                {
                    sum += r.length;
                }
            }
            return sum;
        }
        else
        {
            return 0;
        }
    }








    // ****************************************************  场景加载相关 ******************************************************************

    /// <summary>
    /// 载入场景（没在AB包就默认为在主包中）
    /// </summary>
    /// <param name="sceneName"></param>
    /// <param name="callback"></param>
    public void LoadScene(string sceneName, UnityAction<AsyncOperation> callback = null)
    {
        StartCoroutine(CoLoadScene(sceneName, callback));
    }
    private List<AssetBundle> sceneAbLoaded = new List<AssetBundle>();
    private IEnumerator CoLoadScene(string sceneName, UnityAction<AsyncOperation> callback = null)
    {
        // *** 是否在BuildSetting中 ***  
        bool isInBuildinScenes = false;
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var s = SceneManager.GetSceneByBuildIndex(i);
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            string sName = System.IO.Path.GetFileNameWithoutExtension(path);
            //Debug.Log("scene path:" + path);
            //Debug.Log("scene name1:" + s.name);
            //Debug.Log("scene Name2:" + sName);
            if (sName == sceneName)
            {
                isInBuildinScenes = true;
            }
        }

        

        // *** 如果场景不在BuildSetting中就下载并加载AB包 ***
        if (isInBuildinScenes)
        {
        }
        else
        {
            var req = NewRequireBundle(sceneName.ToLower() + ".ab", () => { });

            yield return new WaitUntil(() => req.isSaved);

            AssetBundle ab = ReadAssetBundle_General(req);
            sceneAbLoaded.Add(ab);
        }



        // *** 开始异步加载场景 ***
        yield return new WaitForSeconds(0.1f);//如果没有这行会导致allowSceneActivation=false失效
        
        var nextSceneLoadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        nextSceneLoadOp.allowSceneActivation = false;

        yield return new WaitForSeconds(0.5f);

        while (nextSceneLoadOp.progress < 0.9f)
        {
            yield return null;
        }
        callback?.Invoke(nextSceneLoadOp);
    }

    /// <summary>
    /// 卸载场景包
    /// </summary>
    public void UnloadAllSceneAb()
    {
        AssetBundle[] arr = sceneAbLoaded.ToArray();
        foreach(var ab in arr)
        {
            Debug.Log("场景包：" + ab.name + " 正在卸载...");
            sceneAbLoaded.Remove(ab);
            ab.Unload(true);
        }
    }






    
    public void OnGUI()
    {
        if (allLoaded) return;

        // --------------------------------------------------------

        //if (allLoaded) return;

        //StringBuilder strb = new StringBuilder();
        //strb.Append("basic:\n");
        //foreach(var ab in this.basicAssetBundles)
        //{
        //    strb.Append(ab.name + "progress:" + ab.GetProgress() + "  isSvaed:" + ab.isSaved + "\n");
        //}
        //strb.Append("non - basic:\n");
        //foreach (var ab in this.nonBasicAssetBundles)
        //{
        //    strb.Append(ab.name + "progress:" + ab.GetProgress() + "  isSvaed:" + ab.isSaved + "\n");
        //}

        //GUILayout.TextArea(strb.ToString());

        // --------------------------------------------------------


        StringBuilder strb = new StringBuilder();
        foreach(var log in logList)
        {
            strb.Append(log);
            strb.Append("\n");
        }
        GUILayout.TextArea(strb.ToString());
    }
}



#if UNITY_EDITOR
[CustomEditor(typeof(AssetBundleManager))]
public class CustomAssetBundleManager : Editor
{
    private GUIStyle style = null;
    private AssetBundleManager mgr = null;
    private bool inited = false;
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(!inited)
        {
            mgr = target as AssetBundleManager;
            style = new GUIStyle() { richText = true };
            inited = true;
        }


        GUILayout.Space(50);
        GUILayout.BeginVertical("frameBox");

        //基本包

        GUILayout.Label(" *** 游戏运行所需基本包 ***");
        GUILayout.BeginVertical("frameBox");
        if (mgr.basicAssetBundles != null)
        {
            for (int i = 0; i < mgr.basicAssetBundles.Length; i++)
            {
                GUILayout.Space(20);
                var lastRect = GUILayoutUtility.GetLastRect();
                float progress = mgr.basicAssetBundles[i].GetProgress();
                bool isSaved = mgr.basicAssetBundles[i].isSaved;
                string colorRgb = isSaved ? "#FFFFFF" : "#444444";
                string progressText = mgr.basicAssetBundles[i].GetProgressText();

                GUI.DrawTexture(new Rect(lastRect.x + 300, lastRect.y, 204f, 20), Texture2D.grayTexture);
                GUI.DrawTexture(new Rect(lastRect.x + 302, lastRect.y + 5, 200f * progress , 10), Texture2D.whiteTexture);
                GUI.Label(lastRect, "<color=" + colorRgb + ">" + mgr.basicAssetBundles[i].name + "</color>", style);
                GUI.Label(new Rect(lastRect.x + 500, lastRect.y, 100, 20), "<color=" + colorRgb + ">" + ( isSaved ? "已下载" : ("下载中" + progressText) ) + "</color>", style);

            }
        }
        GUILayout.EndVertical();

        //可下载附加包

        GUILayout.Label(" *** 可下载附加包 ***");
        GUILayout.BeginVertical("frameBox");
        if (mgr.nonBasicAssetBundles != null)
        {
            for (int i = 0; i < mgr.nonBasicAssetBundles.Length; i++)
            {
                GUILayout.Space(20);
                var lastRect = GUILayoutUtility.GetLastRect();
                float progress = mgr.nonBasicAssetBundles[i].GetProgress();
                bool isSaved = mgr.nonBasicAssetBundles[i].isSaved;
                string colorRgb = isSaved ? "#FFFFFF" : "#444444";
                string progressText = mgr.nonBasicAssetBundles[i].GetProgressText();

                GUI.DrawTexture(new Rect(lastRect.x + 300, lastRect.y, 204f, 20), Texture2D.grayTexture);
                GUI.DrawTexture(new Rect(lastRect.x + 302, lastRect.y + 5, 200f * progress, 10), Texture2D.whiteTexture);
                GUI.Label(lastRect, "<color=" + colorRgb + ">" + mgr.nonBasicAssetBundles[i].name + "</color>", style);
                GUI.Label(new Rect(lastRect.x + 500, lastRect.y, 100, 20), "<color=" + colorRgb + ">" + (isSaved ? "已下载" : ("下载中" + progressText)) + "</color>", style);

            }
        }
        GUILayout.EndVertical();

        //已添加附加包

        GUILayout.Label(" *** 已添加附加包 ***");
        GUILayout.BeginVertical("frameBox");
        if(mgr.appendAssetBundles != null)
        {
            for (int i = 0; i < mgr.appendAssetBundles.Count; i++)
            {
                string colorRgb = mgr.appendAssetBundles[i].isSaved ? "#FFFFFF" : "#444444";
                GUILayout.Label("<color=" + colorRgb + ">" + mgr.appendAssetBundles[i].name + "</color>", style);
            }
        }
        GUILayout.EndVertical();

        //下载队列

        GUILayout.Label(" *** 待下载队列 ***");
        GUILayout.BeginVertical("frameBox");
        if (mgr.remainRequires != null)
        {
            foreach(var req in mgr.remainRequires)
            {
                GUILayout.Label("<color=#FFFFFF>" + req.name + "</color>", style);
            }
        }
        GUILayout.EndVertical();

        //下载中

        GUILayout.Label(" *** 下载中 ***");
        GUILayout.BeginVertical("frameBox");
        if (mgr.downloadingRequires != null)
        {
            foreach (var req in mgr.downloadingRequires)
            {
                GUILayout.Label("<color=#FFFFFF>" + req.name + "</color>", style);
            }
        }
        GUILayout.EndVertical();



        //。。。

        GUILayout.EndVertical();
        

        GUILayout.Label("<color=#333333>runtime:" + mgr.runtime + "</color>", style);
    }
}
#endif

```  