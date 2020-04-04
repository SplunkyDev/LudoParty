using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;
using UnityEngine.UI;
using DG.Tweening;


public class InGameUIManager : MonoBehaviour
{
	private static InGameUIManager m_instance;
	public static InGameUIManager Instance
	{
		get
		{
			if(m_instance == null)
			{
				m_instance = FindObjectOfType<InGameUIManager>();
			}
			return m_instance;
		}
	}

	[Header("Player Avatar (Blue,Yellow,Red,Green)")]
	[SerializeField] private Image[] m_arrPlayerAvatar;
	[SerializeField] private Sprite[] m_arrDiceSprite;

	private Animator[] m_arrAnimController;
	private Image[] m_arrDiceImage;

	private Vector2 m_vec2ScaleValue;
	private Image m_imgPrevAvatar, m_imgCurrentAvatar, m_imgCurrentDiceRoll;


	public GameObject m_gResultPrefab;
	public GameObject m_gGridResult;
	public Text m_textGameInstruction;

	private void RegisterToEvent()
	{
		if(EventManager.Instance == null)
		{
			Debug.LogError("[InGameUIManager] EventManager is null");
			return;
		}

		EventManager.Instance.RegisterEvent<EventHighlightCurrentPlayer>(HighlightCurrentPlayer);
		EventManager.Instance.RegisterEvent<EventDiceRollAnimationComplete>(DiceRollAnimationComplete);
		EventManager.Instance.RegisterEvent<EventPlayerTurnChanged>(PlayerTurnChanged);
		EventManager.Instance.RegisterEvent<EventOpponentDiceRoll>(DiceRollAnimation);
	}

	private void DeregisterToEvent()
	{
		if (EventManager.Instance == null)
		{
			return;
		}

		EventManager.Instance.DeRegisterEvent<EventHighlightCurrentPlayer>(HighlightCurrentPlayer);
		EventManager.Instance.DeRegisterEvent<EventDiceRollAnimationComplete>(DiceRollAnimationComplete);
		EventManager.Instance.DeRegisterEvent<EventPlayerTurnChanged>(PlayerTurnChanged);
		EventManager.Instance.DeRegisterEvent<EventOpponentDiceRoll>(DiceRollAnimation);
	}

	private  void Awake()
	{
		if(m_instance == null)
		{
			m_instance = this;
		}
	}

	private void OnEnable()
	{
		RegisterToEvent();
	}

	private void OnDisable()
	{
		DeregisterToEvent();
	}

	private void Start()
    {
		DOTween.Init(true, false, LogBehaviour.Verbose);
		m_vec2ScaleValue = new Vector2(0.85f, 0.85f);
		m_arrAnimController = new Animator[m_arrPlayerAvatar.Length];
		m_arrDiceImage = new Image[m_arrPlayerAvatar.Length];

		for (int i =0; i<m_arrPlayerAvatar.Length; i++)
		{
			m_arrAnimController[i] = m_arrPlayerAvatar[i].transform.GetComponentInParent<Animator>();
			m_arrDiceImage[i] = m_arrAnimController[i].transform.GetComponent<Image>();
		}
	}

    private void Update()
    {
        
    }

	public void ShowGameResults(List<PlayerData> a_lstPlayerData)
	{
		GameObject gResultPanel;
		Text txtPlayer;

		EventManager.Instance.TriggerEvent<EventShowGameCompleteUI>(new EventShowGameCompleteUI(true, eGameState.GameComplete));
		for (int i = 0; i < a_lstPlayerData.Count; i++)
		{
			gResultPanel = Instantiate(m_gResultPrefab, Vector3.zero, Quaternion.identity, m_gGridResult.transform);
			gResultPanel.transform.GetChild(0).gameObject.GetComponent<Text>().text = i == 0 ? "1st" : "2nd";
			switch (a_lstPlayerData[i].m_enumPlayerTurn)
			{
				case ePlayerTurn.PlayerOne:
					txtPlayer = gResultPanel.transform.GetChild(1).gameObject.GetComponent<Text>();
					txtPlayer.text = "Player One";
					SetColourToTheName(i);
					break;
				case ePlayerTurn.PlayerTwo:
					txtPlayer = gResultPanel.transform.GetChild(1).gameObject.GetComponent<Text>();
					txtPlayer.text = "Player Two";
					SetColourToTheName(i);
					break;
				case ePlayerTurn.PlayerThree:
					txtPlayer = gResultPanel.transform.GetChild(1).gameObject.GetComponent<Text>();
					txtPlayer.text = "Player Three";
					SetColourToTheName(i);
					break;
				case ePlayerTurn.PlayerFour:
					txtPlayer = gResultPanel.transform.GetChild(1).gameObject.GetComponent<Text>();
					txtPlayer.text = "Player Four";
					SetColourToTheName(i);
					break;
			}

		}

		void SetColourToTheName(int a_iIndex)
		{
			switch (a_lstPlayerData[a_iIndex].m_enumPlayerToken)
			{
				case ePlayerToken.Blue:
					txtPlayer.color = Color.blue;
					break;
				case ePlayerToken.Yellow:
					txtPlayer.color = Color.yellow;
					break;
				case ePlayerToken.red:
					txtPlayer.color = Color.red;
					break;
				case ePlayerToken.Green:
					txtPlayer.color = Color.green;
					break;
			}
		}
	}

		private void HighlightCurrentPlayer(IEventBase a_Event)
	{
		Debug.Log("[InGameUIManager] HighlightCurrentPlayer TRIGGERED");
		EventHighlightCurrentPlayer data = a_Event as EventHighlightCurrentPlayer;
		if(data == null)
		{
			Debug.LogError("[InGameUIManager] highlight trigger is null");
			return;
		}

		if(m_imgCurrentAvatar != null)
		{
			m_imgPrevAvatar = m_imgCurrentAvatar;
			m_imgPrevAvatar.transform.DOPause();
			m_imgPrevAvatar.transform.localScale = Vector3.one;

		}
		Debug.Log("[InGameUIManager] Player Token: " + data.EnumPlayerToken + " Player Avatar Index: " + (int)data.EnumPlayerToken);
		m_imgCurrentAvatar = m_arrPlayerAvatar[(int)data.EnumPlayerToken -1];
		m_imgCurrentAvatar.transform.DOScale(m_vec2ScaleValue, 0.75f).From(false).SetLoops(-1).SetEase(Ease.OutCirc);
	}

	private void DiceRollAnimationComplete(IEventBase a_Event)
	{
		EventDiceRollAnimationComplete data = a_Event as EventDiceRollAnimationComplete;
		if (data == null)
		{
			Debug.LogError("[InGameUIManager] Dice Roll  trigger is null");
			return;
		}

		StartCoroutine(AnimationSequence());

		IEnumerator AnimationSequence()
		{
			m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken - 1].SetBool("RollDice", false);
			yield return new WaitForSeconds(0.25f);
			m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken - 1].enabled = false;
			yield return new WaitForEndOfFrame();

			m_imgCurrentDiceRoll = m_arrDiceImage[(int)GameManager.Instance.EnumPlayerToken - 1];
			GameManager.Instance.CheckResult();
			Debug.Log("[InGameUIManager] The Dice Roll value: " + GameManager.Instance.ICurrentDiceValue);

			yield return new WaitForEndOfFrame();

			//This will show the current roll sprite on the dice image
			m_imgCurrentDiceRoll.sprite = m_arrDiceSprite[GameManager.Instance.ICurrentDiceValue - 1];

			switch (GameManager.Instance.CurrentPlayer.m_ePlayerState)
			{
				case ePlayerState.PlayerRollDice:
					Instructions("Roll Dice");
					break;
				case ePlayerState.PlayerMoveToken:
					Instructions("Select Token");
					break;
			}
		}

		
	}

	private void PlayerTurnChanged(IEventBase a_Event)
	{
		Debug.Log("[InGameUIManager] Player Turn Changed TRIGGERED");
		EventPlayerTurnChanged data = a_Event as EventPlayerTurnChanged;
		if (data == null)
		{
			Debug.LogError("[InGameUIManager] Player Turn Changed trigger is null");
			return;
		}

		EventManager.Instance.TriggerEvent<EventHighlightCurrentPlayer>(new EventHighlightCurrentPlayer(GameManager.Instance.EnumPlayerToken));
	}

	public void InputButton(string a_strInput)
	{
		switch (a_strInput)
		{
			case "RollDice":

				if (GameManager.Instance.CurrentPlayer.m_ePlayerState != ePlayerState.PlayerRollDice)
					return;

				if (GameManager.Instance.BOnlineMultiplayer)
				{
					//DO not accept input from device if the player turn is not of this device
					if (GameManager.Instance.EnumPlayerTurn != GameManager.Instance.EnumMyPlayerTurn)
					{
						return;
					}
				}

				Debug.Log("[InGameUIManager] Roll Dice");
				GameManager.Instance.RollTheDice();
				StartCoroutine(AnimateDiceRoll());
				break;
			case "BackToMenu":
				if (GameManager.Instance.EnumGameState != eGameState.InGame)
					return;

				if (GameManager.Instance.BOnlineMultiplayer)
				{
					//TODO: Disconnect from server
				}
				GameManager.Instance.LoadToGame(0);
				break;
			case "BackAfterGC":
				if (GameManager.Instance.EnumGameState!= eGameState.GameComplete)
						return;

				if(GameManager.Instance.BOnlineMultiplayer)
				{
					//TODO: Disconnect from server
				}
				GameManager.Instance.LoadToGame(0);
				break;
		}

	}

	private void DiceRollAnimation(IEventBase a_Event)
	{
		EventOpponentDiceRoll data = a_Event as EventOpponentDiceRoll;
		if (data == null)
		{
			Debug.LogError("[InGamaUIManager] Event Oppoennt dice roll null");
			return;
		}
		StartCoroutine(AnimateDiceRoll());
	}


	private IEnumerator AnimateDiceRoll()
	{
		Debug.Log("[InGameUIManager][AnimateDiceRoll] EnumPlayerToken: "+ GameManager.Instance.EnumPlayerToken +" Value for Token: "+((int)GameManager.Instance.EnumPlayerToken - 1));
		m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken - 1].enabled = true;
		yield return new WaitForSeconds(0.25f);
		m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken -1].SetBool("RollDice",true);
	}


	public void Instructions(string a_strInstructions)
	{
		m_textGameInstruction.text = a_strInstructions;
	}
}