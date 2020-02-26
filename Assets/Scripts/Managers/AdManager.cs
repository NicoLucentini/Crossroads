using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    public delegate void OnPositiveResult();
    public event OnPositiveResult onPositiveResult;
    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this);
    }

    void Start ()
    {   
        Advertisement.Initialize("1469859", true);
	}
    public void AdShow(OnPositiveResult clbk)
    {
#if UNITY_ANDROID
        StartCoroutine(ShowAd(clbk));
#endif
    }
    public IEnumerator ShowAd(OnPositiveResult clbk)
    {
        print("Adv ini: " + Advertisement.isInitialized);

        while (!Advertisement.IsReady())
            yield return new WaitForEndOfFrame();

        ShowOptions so = new ShowOptions();
        so.resultCallback = OnShowResultado;
        onPositiveResult = clbk;
        Advertisement.Show("rewardedVideo",so);

        

#if UNITY_EDITOR
            var currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
            yield return new WaitUntil( () => !Advertisement.isShowing );
            Time.timeScale = currentTimeScale;
#endif
    }
     
        void OnShowResultado(ShowResult sr)
        {
            if(sr == ShowResult.Failed)
            {
                print("ShowResult.Failed");
            }
            else if(sr == ShowResult.Finished)
            {
                onPositiveResult?.Invoke();
            }
            else if(sr == ShowResult.Skipped)
            {
                print("ShowResult.Skipped");
            }
        }
    }
