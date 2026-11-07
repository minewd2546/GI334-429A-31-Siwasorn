using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour
{
    [SerializeField]
    private TMP_Text notiText;
    public TMP_Text NotiText { get { return notiText; } set { notiText = value; } }

    [SerializeField]
    private TMP_Text yesText;

    [SerializeField]
    private TMP_Text noText;

    [SerializeField]
    private Button btnYes;

    [SerializeField]
    private Button btnNo;

    [SerializeField]
    private Button btnOkay;

    [SerializeField]
    private TMP_Text okayText;

    [SerializeField]
    private Image charImage;

    [Header("Combat Dialog")]
    //Attacker
    [SerializeField]
    private Image attackerFactionImage;

    [SerializeField]
    private Image[] attackerImage;

    [SerializeField]
    private TMP_Text[] attackerNameText;

    [SerializeField]
    private TMP_Text[] attackerStrText;

    //Defender
    [SerializeField]
    private Image defenderFactionImage;

    [SerializeField]
    private Image[] defenderImage;

    [SerializeField]
    private TMP_Text[] defenderNameText;

    [SerializeField]
    private TMP_Text[] defenderStrText;

    public void DialogCombatAnalysisInit(Unit attacker, Unit defender)
    {
        Debug.Log(attackerFactionImage);
        attackerFactionImage.sprite = attacker.Faction.ShieldIcon;
        attackerImage[0].sprite = attacker.UnitSprite.sprite;
        attackerNameText[0].text = attacker.UnitName;
        attackerStrText[0].text = attacker.Strength.ToString();

        defenderFactionImage.sprite = defender.Faction.ShieldIcon;
        defenderImage[0].sprite = defender.UnitSprite.sprite;
        defenderNameText[0].text = defender.UnitName;
        defenderStrText[0].text = defender.Strength.ToString();
    }

    public void DialogCombatResultInit(Unit winner, Unit loser)
    {
        if (notiText != null)
            notiText.text =
                ($"<color=yellow>{winner.Faction.Nation} {winner.UnitName}</color> defeats " +
                $"<color=yellow>{loser.Faction.Nation} {loser.UnitName}</color>");

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(true);
    }

    public void DialogNativeIntroInit(Faction faction)
    {
        if (charImage != null && faction.LeaderSprite != null)
        {
            charImage.sprite = faction.LeaderSprite;

            //viceroys are tall cutouts (44.24); native portraits are square and must stay off the text
            RectTransform charRect = charImage.rectTransform;
            charRect.anchoredPosition = faction.IsEuropean ? new Vector2(-665, 0) : new Vector2(-736, 0);
            charRect.sizeDelta = faction.IsEuropean ? new Vector2(768, 1024) : new Vector2(448, 1024);
        }

        if (notiText != null)
        {
            if (faction.IsEuropean)
                notiText.text =
                    ($"Greetings and welcome to <color=yellow>New {faction.Nation}</color>. " +
                    $"We have justly claimed all of this land in the name of King of " +
                    $"<color=yellow>{faction.Nation}</color>. Please do not interfere with us. " +
                    $"We propose a <color=yellow>peace treaty</color>. Do you agree?");
            else
                notiText.text =
                    ($"The <color=yellow>{faction.Nation}</color> tribe welcomes you." +
                    $"To celebrate our friendship, we generously offer you " +
                    $"the land you now occupy as a gift. Will you accept our treaty " +
                    $"and live with us in peace as brothers?");
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(true);

        if (btnNo != null)
            btnNo.gameObject.SetActive(true);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(false);
    }

    public void ShowNotiText(string s)
    {
        notiText.text = s;

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(true);
    }

    public void ThisMeansWar(Faction faction)
    {
        if (notiText != null)
        {
            if (faction.IsEuropean)
                notiText.text =
                    ($"This means you are at war with <color=yellow>{faction.Nation}</color> already. " +
                    $"See you on the battlefield!");
            else
                notiText.text =
                    ($"My tribe, <color=yellow>{faction.Nation}</color> will kill all of you. " +
                    $"Prepare to Die!!!");
        }

        if (btnYes != null)
            btnYes.gameObject.SetActive(false);

        if (btnNo != null)
            btnNo.gameObject.SetActive(false);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(true);
    }

    public void DialogGoToEuropeInit(DialogData data)
    {
        if (notiText != null)
            notiText.text = data.question;

        if (yesText != null)
            yesText.text = data.answers[0];

        if (noText != null)
            noText.text = data.answers[1];

        //the naval dialog also shows combat results, which hide Yes/No
        if (btnYes != null)
            btnYes.gameObject.SetActive(true);

        if (btnNo != null)
            btnNo.gameObject.SetActive(true);

        if (btnOkay != null)
            btnOkay.gameObject.SetActive(false);
    }
}
