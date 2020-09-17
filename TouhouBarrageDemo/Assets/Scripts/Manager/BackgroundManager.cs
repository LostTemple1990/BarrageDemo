using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager
{
    private static BackgroundManager _instance = new BackgroundManager();

    public static BackgroundManager GetInstance()
    {
        return _instance;
    }

    private BackgroundBase _currentBg;

    private Dictionary<string, IParser> _bgCfgDic;
    private Dictionary<string, string> _stageBg;

    private StageBgCfg _curLoadedBgCfg;

    public BackgroundManager()
    {
        
    }

    public void Init()
    {
        _bgCfgDic = DataManager.GetInstance().GetDatasByName("StageBgCfgs") as Dictionary<string, IParser>;
        _stageBg = new Dictionary<string, string>();
        _stageBg.Add("Stage1", "Stage1Bg");
    }

    /// <summary>
    /// 获取关卡默认的背景
    /// </summary>
    /// <param name="stageName"></param>
    /// <returns></returns>
    public string GetStageDefaultBgId(string stageName)
    {
        string bgId;
        if (!_stageBg.TryGetValue(stageName, out bgId))
            bgId = "Stage1Bg";
        return bgId;
    }

    /// <summary>
    /// 加载关卡的默认背景
    /// </summary>
    /// <param name="stageName"></param>
    public void LoadStageDefaultBg(string stageName)
    {
        string bgId = GetStageDefaultBgId(stageName);
        _curLoadedBgCfg = GetBackgroundCfgById(bgId);
        if (_curLoadedBgCfg != null)
        {
            //LoadBackgroundByBgName(_curLoadedBgCfg.sceneName);
            LoadBackgroundAsyncByBgName(_curLoadedBgCfg.sceneName);
        }
    }

    /// <summary>
    /// 加载背景 -- 同步方法
    /// </summary>
    /// <param name="id"></param>
    public void LoadBackground(string id)
    {
        StageBgCfg cfg = GetBackgroundCfgById(id);
        if (cfg == null)
            Logger.LogError("Background cfg with id " + id + " is not exist");
        SceneManager.LoadScene(cfg.sceneName, LoadSceneMode.Single);
    }

    /// <summary>
    /// 直接加载场景
    /// 场景加载完成后
    /// 对应场景的Start()方法会调用OnLoadBackgroundSceneComplete()方法，表示加载完成
    /// </summary>
    /// <param name="name"></param>
    private void LoadBackgroundByBgName(string name)
    {
        SceneManager.LoadScene(name, LoadSceneMode.Single);
    }

    private void LoadBackgroundAsyncByBgName(string name)
    {
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);
    }

    public StageBgCfg GetBackgroundCfgById(string id)
    {
        IParser tmpCfg;
        if (!_bgCfgDic.TryGetValue(id, out tmpCfg))
            return null;
        return tmpCfg as StageBgCfg;
    }

    public void OnLoadBackgroundSceneComplete(Transform backgroundRootTf)
    {
        System.Type classType = System.Type.GetType(_curLoadedBgCfg.className);
        _currentBg = System.Activator.CreateInstance(classType) as BackgroundBase;
        _currentBg.Init(backgroundRootTf);
        _curLoadedBgCfg = null;
        CommandManager.GetInstance().RunCommand(CommandConsts.STGLoadStageDefaultBgComplete);
    }

    public void Update()
    {
        int curFrame = STGStageManager.GetInstance().GetFrameSinceStageStart();
        if (_currentBg != null)
            _currentBg.Update(curFrame);
    }

    public void Clear()
    {
        if (_currentBg != null)
        {
            _currentBg.Clear();
            _currentBg = null;
        }
    }
}
