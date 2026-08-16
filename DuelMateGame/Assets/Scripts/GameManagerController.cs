using DuelMate.Core.Models;
using DuelMate.Core.Services;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerController : MonoBehaviour
{
    [Header("Player Cards")]
    public Button playerCard1;
    public Button playerCard2;
    public Button playerCard3;

    public TextMeshProUGUI playerCard1Text;
    public TextMeshProUGUI playerCard2Text;
    public TextMeshProUGUI playerCard3Text;

    [Header("HP Displays")]
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI opponentHPText;

    [Header("Opponent Cards")]
    public GameObject opponentCard1;
    public GameObject opponentCard2;
    public GameObject opponentCard3;

    [Header("Opponent Card Texts")]
    public TextMeshProUGUI opponentCard1Text;
    public TextMeshProUGUI opponentCard2Text;
    public TextMeshProUGUI opponentCard3Text;

    private GameObject[] opponentCardObjects;
    private TextMeshProUGUI[] opponentCardTexts;

    // To track which card player selected
    private int selectedCardIndex = -1;

    private GameManager game;
    private SimpleAI ai;

    private void Start()
    {
        // Hook up the card buttons
        playerCard1.onClick.AddListener(() => OnCardClicked(0));
        playerCard2.onClick.AddListener(() => OnCardClicked(1));
        playerCard3.onClick.AddListener(() => OnCardClicked(2));

        opponentCardObjects = new[] { opponentCard1, opponentCard2, opponentCard3 };
        opponentCardTexts = new[] { opponentCard1Text, opponentCard2Text, opponentCard3Text };
    }

    private void Awake()
    {
        game = new GameManager();
        game.StartGame();
        UpdateUIWithNewHand();

        ai = new SimpleAI();
    }

    private void OnCardClicked(int index)
    {
        if (selectedCardIndex == index)
        {
            ConfirmCardSelection(index);
        }
        else
        {
            selectedCardIndex = index;
            HighlightSelectedCard(index);
        }
    }

    private void HighlightSelectedCard(int index)
    {
        // For now, just log. Later, do animation or glow effect
        Debug.Log($"Selected card {index + 1}");
    }

    private void ConfirmCardSelection(int index)
    {
        Debug.Log($"Confirmed card {index + 1}");

        // NEW: start of a new round → clear last round’s opponent text(s)
        ResetOpponentRoundVisuals();

        // AI picks its card first so both cards can be resolved together.
        int aiCardIndex = ai.ChooseCardIndex(game.Player2);
        if (aiCardIndex == -1)
        {
            Debug.LogWarning("AI had no card to play; skipping round.");
            selectedCardIndex = -1;
            return;
        }

        // Resolve the round: both cards are committed and compared at once.
        RoundResult result = game.PlayRound(index, aiCardIndex);
        Debug.Log($"You played {result.Player1Card.Type}, opponent played {result.Player2Card.Type} " +
                   $"(you took {result.Player1DamageTaken} dmg, opponent took {result.Player2DamageTaken} dmg)");

        // Start the flip animation revealing the opponent's card.
        StartCoroutine(FlipOpponentCard(aiCardIndex, result.Player2Card.Type.ToString()));

        // Update UI
        UpdateHPDisplay();

        selectedCardIndex = -1;

        if (result.GameOver)
        {
            string outcome = result.Winner == game.Player1 ? "You win!" : "Opponent wins!";
            Debug.Log($"Game over: {outcome}");
            // TODO: show a proper end-of-game screen instead of just logging.
            return;
        }

        game.DrawHand(game.Player1);
        game.DrawHand(game.Player2); // AI gets next hand too
        UpdateUIWithNewHand();
    }

    private void UpdateUIWithNewHand()
    {
        var hand = game.Player1.Hand;

        playerCard1Text.text = hand[0].Type.ToString();
        playerCard2Text.text = hand[1].Type.ToString();
        playerCard3Text.text = hand[2].Type.ToString();
    }

    private IEnumerator FlipOpponentCard(int cardIndex, string revealedText)
    {
        GameObject card = opponentCardObjects[cardIndex];
        TextMeshProUGUI text = opponentCardTexts[cardIndex];

        // Hide text first
        text.text = "";
        text.gameObject.SetActive(false);

        // Flip: scale X from 1 → 0
        float duration = 0.2f;
        float t = 0f;
        Vector3 originalScale = card.transform.localScale;

        while (t < duration)
        {
            float scale = Mathf.Lerp(1f, 0f, t / duration);
            card.transform.localScale = new Vector3(scale, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        // Midpoint: update text and flip sprite side if needed
        Debug.Log($"Flipping opponent card {cardIndex} to show: {revealedText}");
        text.text = revealedText;
        text.gameObject.SetActive(true);

        // Flip back: scale X from 0 → 1
        t = 0f;
        while (t < duration)
        {
            float scale = Mathf.Lerp(0f, 1f, t / duration);
            card.transform.localScale = new Vector3(scale, 1f, 1f);
            t += Time.deltaTime;
            yield return null;
        }

        card.transform.localScale = originalScale;
    }

    private void UpdateHPDisplay()
    {
        playerHPText.text = $"HP: {game.Player1.Health}";
        opponentHPText.text = $"HP: {game.Player2.Health}";
    }

    // Clears ALL opponent card texts and ensures cards start from a clean scale.
    // We call this the moment the player CONFIRMS a new card (start of new round).
    private void ResetOpponentRoundVisuals()
    {
        if (opponentCardTexts != null)
        {
            foreach (var t in opponentCardTexts)
            {
                if (!t) continue;
                t.text = string.Empty;
                t.gameObject.SetActive(false); // hides the "front" text so the back is what you see
            }
        }

        if (opponentCardObjects != null)
        {
            foreach (var go in opponentCardObjects)
            {
                if (!go) continue;
                // make sure we aren't mid-flip from a previous animation
                go.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }

}
