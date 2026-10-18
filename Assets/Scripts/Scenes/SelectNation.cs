using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectNation : MonoBehaviour
{
    [SerializeField]
    private string nextScene = "Map01";

    [SerializeField]
    private int index = 0;

    [Header("Leader Portrait")]
    [SerializeField]
    private Image leaderPortrait; //the big full-body portrait on NationAttributesPanel

    [SerializeField]
    private Sprite[] leaderPortraits = new Sprite[5]; //0-England 1-France 2-Spain 3-Netherland 4-Portugal

    [Header("Default Selection")]
    [SerializeField]
    private Toggle defaultNationToggle; //England

    // Toggle.onValueChanged fires for every card as the scene wakes up, including the
    // ones switching off, so whichever fires last would win. Start() runs after all of
    // them and pins the opening selection to England.
    private void Start()
    {
        if (defaultNationToggle != null)
            defaultNationToggle.isOn = true;

        SelectEuropeNation(0);
    }

    public void SelectEuropeNation(int i)
    {
        index = i;
        Settings.playerNationId = index;

        ShowLeaderPortrait(index);
    }

    private void ShowLeaderPortrait(int i)
    {
        if (leaderPortrait == null)
            return;

        if (i < 0 || i >= leaderPortraits.Length || leaderPortraits[i] == null)
        {
            leaderPortrait.enabled = false;
            return;
        }

        leaderPortrait.sprite = leaderPortraits[i];
        leaderPortrait.enabled = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(nextScene);
    }
}
