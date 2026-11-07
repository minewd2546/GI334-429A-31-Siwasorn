using UnityEngine;

public class DialogManager : MonoBehaviour
{
    [SerializeField]
    private Dialog[] dialogs; //0 - Naval

    [SerializeField]
    private DialogData[] dialogData;

    public static DialogManager instance;

    void Awake()
    {
        instance = this;
    }

    public void GoToEuropeQuestion()
    {
        dialogs[0].gameObject.SetActive(true);
        dialogs[0].DialogGoToEuropeInit(dialogData[0]);
    }

    public void YesGoToEurope()
    {
        dialogs[0].gameObject.SetActive(false);
        EuropeManager.instance.AllowToGoToEurope();
    }

    public void NoGoToEurope()
    {
        dialogs[0].gameObject.SetActive(false);
    }

    public void ToggleAiMoveDialog(bool flag)
    {
        if (flag)
            dialogs[1].NotiText.text = ($"Year:{GameManager.instance.Year}\nAI is moving...");

        dialogs[1].gameObject.SetActive(flag);
    }

    public void ToggleNavalDialog(bool flag)
    {
        dialogs[0].gameObject.SetActive(flag);
    }

    public void ShowCombatResult(Unit winner, Unit loser)
    {
        dialogs[0].gameObject.SetActive(true);
        dialogs[0].DialogCombatResultInit(winner, loser);
    }

    #region Army Dialog
    public void CombatAnalysisInit(Unit ourUnit, Unit enemyUnit)
    {
        dialogs[9].DialogCombatAnalysisInit(ourUnit, enemyUnit);
    }

    public void ToggleCombatPanel(bool flag)
    {
        dialogs[9].gameObject.SetActive(flag);
    }
    #endregion

    #region Foreign Dialog
    public void ToggleForeignDialog(bool flag)
    {
        dialogs[3].gameObject.SetActive(flag);

        if (flag)
            dialogs[3].DialogNativeIntroInit(GameManager.instance.CurForeignFaction);
        else
            GameManager.instance.CurForeignFaction = null;
    }

    public void MakePeaceWithNation(bool flag)
    {
        GameManager.instance.CurForeignFaction.MetByPlayer = true;
        GameManager.instance.CurForeignFaction.AtPeaceWithPlayer = flag;

        if (flag)
            ToggleForeignDialog(false);
        else
            dialogs[3].ThisMeansWar(GameManager.instance.CurForeignFaction);
    }
    #endregion

    #region Colony Dialog
    public void ToggleColonyDialog(bool flag)
    {
        dialogs[6].gameObject.SetActive(flag);
    }

    public void ShowColonyClearLandText()
    {
        ToggleColonyDialog(true);
        string text = "You must build a settlement on cleared land or hill, not forest or mountain. Press <color=yellow>P</color> to clear forest.";
        dialogs[6].ShowNotiText(text);
    }

    public void ShowColonyNotEnoughToolsText(int n)
    {
        ToggleColonyDialog(true);
        string text = $"You don't have enough tools. This unit currently has only <color=yellow>{n}</color> tools." +
            $"You must have at least 20 tools. Buy more tools from Europe or get it from Blacksmith's house";

        dialogs[6].ShowNotiText(text);
    }

    public void ShowColonyHasOtherTownAround()
    {
        ToggleColonyDialog(true);
        string text = $"You can't build a settlement next to another settlement or village.";
        dialogs[6].ShowNotiText(text);
    }
    #endregion

    #region King Dialog
    public void ToggleKingDialog(bool flag)
    {
        dialogs[5].gameObject.SetActive(flag);
    }

    public void ShowFirstKingQuestDialogText()
    {
        ToggleKingDialog(true);
        string text = "Establish few colonies and sell these raw materials to our market: <color=yellow>Sugar</color>," +
            " <color=yellow>Tobacco</color> and <color=yellow>Cotton</color>.";
        dialogs[5].ShowNotiText(text);
    }
    #endregion
}
