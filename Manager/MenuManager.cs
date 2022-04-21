using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

// see https://docs.unity3d.com/2022.1/Documentation/Manual/UIE-create-tabbed-menu-for-runtime.html for tabs

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    VisualElement ve;
    List<Label> lblScores;
    GameManager.eGameStarte oldState;
    Label lblGamestate;



    private void OnEnable()
    {
        Debug.Log("timescale=0 " + GameManager.GetState());
        Time.timeScale = 0;

        ve = GetComponent<UIDocument>().rootVisualElement;
        lblGamestate= ve.Q<Label>("game-state");
        lblGamestate.text = GameManager.GetState().ToString();

        var buttons = ve.Query<Button>();
        foreach (var btn in buttons.ToList())
        {
            btn.RegisterCallback<ClickEvent>((evt) => buttonClicked(btn,evt));
        }

        lblScores = ve.Query<Label>("lbl_score").ToList();

        foreach(var lblScore in lblScores)
        {
            lblScore.text = "0";
        }


        GameManager.StateChanged += GameManager_StateChanged;
        GameManager.ScoreChanged += GameManager_ScoreChanged;
    }

    private void GameManager_ScoreChanged(int newScore)
    {
        foreach(var lblScore in lblScores)
        {
            lblScore.text = newScore + "";
        }
    }

    private void OnDisable()
    {
        GameManager.StateChanged -= GameManager_StateChanged;
        GameManager.ScoreChanged -= GameManager_ScoreChanged;
    }

    private void GameManager_StateChanged(GameManager.eGameStarte newState)
    {

        // disable all except the new state? 
        if(newState == GameManager.eGameStarte.Playing && oldState == GameManager.eGameStarte.StartScreen)
        {
            ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.None;
        }
        else if (newState == GameManager.eGameStarte.StartScreen)
        {
            ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.Flex;
        }

        oldState = newState;
        ve.Q<Label>("game-state").text = newState.ToString();
    }

    private void buttonClicked(Button sender ,ClickEvent evt)
    {
        Debug.Log("clicked: " + sender.name);

        switch (sender.name)
        {
            case "cmd_play":
                GameManager.StartGame();
                break;
            case "cmd_openOption":
                GameManager.PauseGame();
                ve.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.Flex;
                ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.None;
                break;
            case "cmd_closeOption":
                GameManager.ResumeGame();
                ve.Q<VisualElement>("OptionsMenu").style.display = DisplayStyle.None;
                if (oldState == GameManager.eGameStarte.StartScreen)
                {
                    ve.Q<VisualElement>("StartScreenBackground").style.display = DisplayStyle.Flex;
                }
                break;
            case "cmd_close":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }
}
