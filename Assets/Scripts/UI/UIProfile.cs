using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Refactorizar toda esta clase luego
public class UIProfile : MonoBehaviour
{
    public Text maxScoreTransit;

    public Text text;
    public GameObject signUpButton;
    public System.Action onClickSign;
    public InputField register;

    public static System.Action onLoginSuccesfull;
       
    public Text signInLog2;
    public bool cdSignIn = false;

    public bool skipOnlineSignin = true;

    private void Awake()
    {
        //GameManager.onLoadComplete += OnProfileLoad;
    }

  
    public void OnProfileLoad()
    {
        if (GameManager.instance.profile.idUser == -1) 
        {
            if (GameManager.instance.profile.registered == "false" || GameManager.instance.profile.registered == "")
            {
                onClickSign = PhpSignIn;
                signUpButton.SetActive(true);
                register.gameObject.SetActive(true);
            }
            else if (GameManager.instance.profile.registered == "Offline")
            {
                PhpSignIn();
            }
        }
        else
        {
            onLoginSuccesfull?.Invoke();
        }
    }

    void RefreshCdSignIn()
    {
        cdSignIn = false;
    }

    public void OnClickSignIn()
    {
        if (cdSignIn) return;

        cdSignIn = true;
        Invoke("RefreshCdSignIn", 1);
        if (onClickSign != null)
            onClickSign();
    }

    void PhpSignIn()
    {
        if (skipOnlineSignin)
        {
            onLoginSuccesfull?.Invoke();
            return;
        }

        print("register " + register.text);

        if (register.text != "")
            GameManager.instance.profile.playerName = register.text;

    }
    void PhpSignInClbk(WWW www)
    {

        if (GameManager.isOnline)
        {
            GameManager.instance.profile.idUser = int.Parse(www.text);
            GameManager.instance.profile.registered = "Online";
        }
        else
        {
            GameManager.instance.profile.registered = "Offline";
        }
        
        signUpButton.SetActive(false);
        register.gameObject.SetActive(false);
        ///ChangeLoginText("Welcome, " + GameManager.instance.profile.playerName);


        onLoginSuccesfull?.Invoke();
        
    }



   
    public void ChangeRecordTransit(string msg)
    {
        maxScoreTransit.text = msg;
    }


}
