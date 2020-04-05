using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameUtility.Base;

public class TokenManager : MonoBehaviour
{
	private static TokenManager m_instance;
	public static TokenManager Instance
	{
		get
		{
			if (m_instance == null)
			{
				m_instance = FindObjectOfType<TokenManager>();
			}
			return m_instance;
		}
	}


	[Header("Start Postion Token")]
	[SerializeField] private List<Transform> m_lstStartBlueTokenPosition = new List<Transform>();
	[SerializeField] private List<Transform> m_lstStartYellowTokenPosition = new List<Transform>();
	[SerializeField] private List<Transform> m_lstStartRedTokenPosition = new List<Transform>();
	[SerializeField] private List<Transform> m_lstStartGreenTokenPosition = new List<Transform>();

	[Header("Token References")]
	[SerializeField] private List<TokenData> m_lstBlueToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstYellowToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstRedToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstGreenToken = new List<TokenData>();


	private List<SpriteRenderer> m_lstBlueTokenSprite = new List<SpriteRenderer>();
	private List<SpriteRenderer> m_lstYellowTokenSprite = new List<SpriteRenderer>();
	private List<SpriteRenderer> m_lstRedTokenSprite = new List<SpriteRenderer>();
	private List<SpriteRenderer> m_lstGreenTokenSprite = new List<SpriteRenderer>();

	[Header("Token Layer")]
	[SerializeField] private LayerMask m_layerMask;

	private List<GameObject> m_lstTokengameobject = new List<GameObject>();
	private List<Transform> m_lstTokenMovePoints = new List<Transform>();
	private bool m_bMoveTweenComplete = false;
	private const float FTOKENJUMPVALUE = 0.15f;
	private Vector2 m_vec2Scalevalue = new Vector2(0.6f, 0.6f);
	private Vector2 m_vec2ScaleShared = new Vector2(0.4f, 0.4f);
	private Vector2 m_Vec3TokenOrginalScale = new Vector2(0.5f, 0.5f);
	private TweenParams m_tweenScaleEffect;

	[SerializeField]private TokenData m_refCurrentToken;
	public TokenData RefCurrentToken { get => m_refCurrentToken; }
	private string m_strTokenDataJson = string.Empty;
	public string StrTokenDataJson { get => m_strTokenDataJson; }

	private const int TOKENSPERPLAYER = 4;
	private List<eMessageType> m_lstMessageType = new List<eMessageType>();

	public delegate void m_delResetToken();
	//This event will be called to reset all token BCanBeUsed to false;
	public m_delResetToken m_OnResetToken;


	private Dictionary<int, Transform> m_dicAllTokensInSpecialTile = new Dictionary<int, Transform>();

	private void RegisterToEvents()
	{
		if (EventManager.Instance == null)
		{
			Debug.LogError("[TokenManager] EventManager is null");
			return;
		}
		EventManager.Instance.RegisterEvent<EventTouchActive>(InputReceived);
		EventManager.Instance.RegisterEvent<EventTokenScaleFactor>(TokenScaleFactor);
		EventManager.Instance.RegisterEvent<EventTokenSelectedInMultiplayer>(TokenSelectedOverTheNetwork);
	}

	private void DereegiterToEvents()
	{

		if (EventManager.Instance == null)
			return;

		EventManager.Instance.DeRegisterEvent<EventTokenScaleFactor>(TokenScaleFactor);
		EventManager.Instance.DeRegisterEvent<EventTouchActive>(InputReceived);
		EventManager.Instance.DeRegisterEvent<EventTokenSelectedInMultiplayer>(TokenSelectedOverTheNetwork);
	}

	private void OnEnable()
	{
		RegisterToEvents();

		//Reseting to init postion on game start to resized  spritepositions
		for (int i = 0; i < 4; i++)
		{
			m_lstBlueToken[i].transform.position = m_lstStartBlueTokenPosition[i].position;
			m_lstYellowToken[i].transform.position = m_lstStartYellowTokenPosition[i].position;
			m_lstRedToken[i].transform.position = m_lstStartRedTokenPosition[i].position;
			m_lstGreenToken[i].transform.position = m_lstStartGreenTokenPosition[i].position;

			m_lstBlueTokenSprite.Add(m_lstBlueToken[i].transform.GetChild(0).GetComponent<SpriteRenderer>());
			m_lstYellowTokenSprite.Add(m_lstYellowToken[i].transform.GetChild(0).GetComponent<SpriteRenderer>());
			m_lstRedTokenSprite.Add(m_lstRedToken[i].transform.GetChild(0).GetComponent<SpriteRenderer>());
			m_lstGreenTokenSprite.Add(m_lstGreenToken[i].transform.GetChild(0).GetComponent<SpriteRenderer>());

		}


	}


	private void OnDisable()
	{
		DereegiterToEvents();
	}

	// Start is called before the first frame update
	void Start()
	{
		DOTween.Init(true, false, LogBehaviour.Verbose);
		m_tweenScaleEffect = new TweenParams().SetLoops(-1).SetEase(Ease.OutCirc);
	}


	private void Update()
	{
	}

	public bool CheckValidTokenMovement(int a_iDiceValue)
	{

		Debug.Log("[TokenManager][CheckValidTokenMovement]");
		bool bValid = false;
		//Resets all the token before checking their movable state, basically checking if the player can move it at their turn after making the roll
		if (m_OnResetToken != null)
		{
			m_OnResetToken.Invoke();
		}

		Debug.Log("[TokenManager][CheckValidTokenMovement] PlayerToken: " + GameManager.Instance.EnumPlayerToken.ToString());
		switch (GameManager.Instance.EnumPlayerToken)
		{
			case GameUtility.Base.ePlayerToken.None:
				Debug.LogError("[TokenManager] Could not retrieve Player Token State");
				break;
			case GameUtility.Base.ePlayerToken.Blue:
				bValid = AnimateValidTokens(m_lstBlueToken, a_iDiceValue);
				break;
			case GameUtility.Base.ePlayerToken.Yellow:
				bValid = AnimateValidTokens(m_lstYellowToken, a_iDiceValue);
				break;
			case GameUtility.Base.ePlayerToken.red:
				bValid = AnimateValidTokens(m_lstRedToken, a_iDiceValue);
				break;
			case GameUtility.Base.ePlayerToken.Green:
				bValid = AnimateValidTokens(m_lstGreenToken, a_iDiceValue);
				break;
			default:
				break;
		}

		return bValid;

	}


	//This method is used to check which tokens can be moved
	private bool AnimateValidTokens(List<TokenData> a_lstToken, int a_iDiceValue)
	{
		Debug.Log("[TokenManager][AnimateValidTokens]");
		bool bValid = false;

		for (int i = 0; i < a_lstToken.Count; i++)
		{
			switch (a_lstToken[i].EnumTokenState)
			{

				case GameUtility.Base.eTokenState.House:
					if (a_iDiceValue == 6)
					{
						UpdateSortOrder(i);
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOMoveY((a_lstToken[i].transform.position.y + FTOKENJUMPVALUE), 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					}
					break;
				case GameUtility.Base.eTokenState.InRoute:
				case GameUtility.Base.eTokenState.InHideOut:
					UpdateSortOrder(i);
					bValid = a_lstToken[i].BCanBeUsed = true;
					a_lstToken[i].transform.DOMoveY((a_lstToken[i].transform.position.y + FTOKENJUMPVALUE), 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					break;
				case GameUtility.Base.eTokenState.InStairwayToHeaven:
					if (PathManager.Instance.ValidateMovement(a_lstToken[i], a_iDiceValue))
					{
						UpdateSortOrder(i);
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOMoveY((a_lstToken[i].transform.position.y + FTOKENJUMPVALUE), 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					}
					break;
				case GameUtility.Base.eTokenState.EntryToStairway:
					Debug.Log("<color=red>[TokeManager][AnimateValidTokens] Getting closer to heaven:" + a_lstToken[i].EnumTokenType + "</color>");
					if (PathManager.Instance.ValidateMovement(a_lstToken[i], a_iDiceValue))
					{
						UpdateSortOrder(i);
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOMoveY((a_lstToken[i].transform.position.y + FTOKENJUMPVALUE), 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					}
					break;
			}


		}

		void UpdateSortOrder(int a_index)
		{
			switch (a_lstToken[a_index].EnumTokenType)
			{
				case eTokenType.Blue:
					m_lstBlueTokenSprite[a_index].sortingOrder = 1;
					break;
				case eTokenType.Yellow:
					m_lstYellowTokenSprite[a_index].sortingOrder = 1;
					break;
				case eTokenType.Red:
					m_lstRedTokenSprite[a_index].sortingOrder = 1;
					break;
				case eTokenType.Green:
					m_lstGreenTokenSprite[a_index].sortingOrder = 1;
					break;
			}

		}

		return bValid;
	}


	private void InputReceived(IEventBase a_Event)
	{
		EventTouchActive data = a_Event as EventTouchActive;
		if (data == null)
		{
			Debug.LogError("[TokenManager] Touch Active trigger null");
			return;
		}

		if (data.BTouch)
		{
			if (GameManager.Instance.BOnlineMultiplayer)
			{
				//DO not accept input from device if the player turn is not of this device
				if (GameManager.Instance.EnumPlayerTurn != GameManager.Instance.EnumMyPlayerTurn)
				{
					return;
				}
			}
			RaycastFromScreen(data.Vec3TouchPosition);
		}


	}

	private void RaycastFromScreen(Vector3 a_vec3Position)
	{
		Vector2 vec2ray = Camera.main.ScreenToWorldPoint(a_vec3Position);
		RaycastHit2D hit = Physics2D.Raycast(vec2ray, Vector2.zero, m_layerMask);
		if (hit.collider != null)
		{
			if (hit.transform.CompareTag("Token"))
			{
				Debug.Log("[TokenManger] Token selected");
				TokenSelected(hit.transform.parent.gameObject.GetComponent<TokenData>(), GameManager.Instance.ICurrentDiceValue);
			}
		}
	}

	private void TokenScaleFactor(IEventBase a_Event)
	{
		EventTokenScaleFactor data = a_Event as EventTokenScaleFactor;
		if (data == null)
		{
			Debug.LogError("[TokenManager] Scale factor Value trigger null");
			return;
		}

		switch (data.EScaleType)
		{
			case eScaleType.None:
				break;
			case eScaleType.TokenType:
				TokenData refTokenData = data.LTokenGameObject[0].GetComponent<TokenData>();
				if (refTokenData == null)
				{
					Debug.LogError("[TokenManager] TokenData null, cannot set scale value");
					return;
				}
				ScaleTokenType(refTokenData);
				break;
			case eScaleType.SharedTile:
				for (int i = 0; i < data.LTokenGameObject.Count; i++)
				{
					Debug.Log("<color=green>[TokenManager][TokenScaleFactor] Shared Tile</color>");
					data.LTokenGameObject[i].transform.localScale = data.Vec2ScaleValue;
					Vector2 vec2RandomPosition = Random.insideUnitCircle * 0.15f;
					data.LTokenGameObject[i].transform.position += new Vector3(vec2RandomPosition.x, vec2RandomPosition.y, data.LTokenGameObject[i].transform.position.z);
				}
				break;
		}


		//Reseting scale to original size after highlighting them
		void ScaleTokenType(TokenData a_refTokenData)
		{
			switch (a_refTokenData.EnumTokenType)
			{
				case eTokenType.None:
					break;
				case eTokenType.Blue:
					for (int i = 0; i < m_lstBlueToken.Count; i++)
					{
						m_lstBlueToken[i].gameObject.transform.localScale = data.Vec2ScaleValue;
					}
					break;
				case eTokenType.Yellow:
					for (int i = 0; i < m_lstYellowToken.Count; i++)
					{
						m_lstYellowToken[i].gameObject.transform.localScale = data.Vec2ScaleValue;
					}
					break;
				case eTokenType.Red:
					for (int i = 0; i < m_lstRedToken.Count; i++)
					{
						m_lstRedToken[i].gameObject.transform.localScale = data.Vec2ScaleValue;
					}
					break;
				case eTokenType.Green:
					for (int i = 0; i < m_lstGreenToken.Count; i++)
					{
						m_lstGreenToken[i].gameObject.transform.localScale = data.Vec2ScaleValue;
					}
					break;
			}

		}
	}


	public void TokenSelectedOverTheNetwork(IEventBase a_Event)
	{
		EventTokenSelectedInMultiplayer data = a_Event as EventTokenSelectedInMultiplayer;
		if (data == null)
		{
			Debug.LogError("[TokenManager] EventTokenSelectedInMultiplayer null");
			return;
		}

		TokenSelected(data.RefTokenData, GameManager.Instance.ICurrentDiceValue);
	}

	//When the dice has been rolled and the token has been selected this will be called
	private void TokenSelected(TokenData a_refTokenData, int a_iDiceValue)
	{
		
		if (!a_refTokenData.BCanBeUsed)
		{
			Debug.LogError("[TokenManager] This Token cannot be moved");
			return;
		}

		//Online call to affect the other device toke also
		if (GameManager.Instance.BOnlineMultiplayer && GameManager.Instance.CurrentPlayer.m_enumPlayerTurn == GameManager.Instance.EnumMyPlayerTurn)
		{
			Debug.Log("[TokenManager][TokenSelected] This is local device sending token selected over network");

			TokenDataContainer refTokenDataContainer = new TokenDataContainer(a_refTokenData);
			m_strTokenDataJson = JsonUtility.ToJson(refTokenDataContainer);
			Debug.Log("<color=green>[TokenManager] serializing token data to send  over network: " + m_strTokenDataJson + "</color>");

			m_lstMessageType.Clear();
			m_lstMessageType.Add(eMessageType.PlayerTokenSelected);
			EventManager.Instance.TriggerEvent<EventInsertInGameMessage>(new EventInsertInGameMessage(m_lstMessageType.ToArray()));
		}

		

		//Resets all the token after checking their movable state, making sure no user tried to mobe any other token
		if (m_OnResetToken != null)
		{
			m_OnResetToken.Invoke();
		}

		Debug.Log("[TokenManager] Scale Effect Tween Paused: " + DOTween.Pause("ScaleEffect"));
		switch (a_refTokenData.EnumTokenType)
		{
			case GameUtility.Base.eTokenType.None:
				break;
			case GameUtility.Base.eTokenType.Blue:
				m_refCurrentToken = m_lstBlueToken[a_refTokenData.ITokenID];
				
				break;
			case GameUtility.Base.eTokenType.Yellow:
				m_refCurrentToken = m_lstYellowToken[a_refTokenData.ITokenID];
				break;
			case GameUtility.Base.eTokenType.Red:
				m_refCurrentToken = m_lstRedToken[a_refTokenData.ITokenID];
				break;
			case GameUtility.Base.eTokenType.Green:
				m_refCurrentToken = m_lstGreenToken[a_refTokenData.ITokenID];
				break;
			default:
				break;
		}

	
		m_lstTokengameobject.Add(m_refCurrentToken.gameObject);
		EventManager.Instance.TriggerEvent<EventTokenScaleFactor>(new EventTokenScaleFactor(m_lstTokengameobject, m_Vec3TokenOrginalScale, eScaleType.TokenType));
		m_lstTokengameobject.Clear();

		//calling the coroutine
		StartCoroutine(PlayerTurn(0.15f));

		//This coroutine is local to this method
		IEnumerator PlayerTurn(float a_fDelay)
		{
			yield return new WaitForSeconds(a_fDelay);
			m_lstTokenMovePoints = PathManager.Instance.TokenStateUpdate(RefCurrentToken, a_iDiceValue);
			if (m_lstTokenMovePoints != null)
			{
				Debug.Log("<color=red>[TokenManager]  m_refCurrentToken: " + RefCurrentToken.ICurrentPathIndex + "</color>");
				for (int i = 0; i < m_lstTokenMovePoints.Count; i++)
				{
					m_bMoveTweenComplete = false;
					RefCurrentToken.transform.DOMove((Vector2)m_lstTokenMovePoints[i].transform.position, 5, false).SetSpeedBased(true).OnComplete(MoveTweenComplete);
					while (!m_bMoveTweenComplete)
					{
						yield return null;
					}
				}
			}

			RefCurrentToken.Vec2PositionOnTile = RefCurrentToken.transform.position;
			if (RefCurrentToken.EnumTokenState == eTokenState.InRoute || RefCurrentToken.EnumTokenState == eTokenState.InHideOut)
			{
				Debug.Log("<color=red>[TokenManager] Current Token InRoute,check if other token present in same tile: m_refCurrentToken: " + RefCurrentToken.ICurrentPathIndex +"TokenState: "+ RefCurrentToken.EnumTokenState + "</color>");
				CheckIfTileContainsOtherTokens();			
			}

			//Checks if all Tokens are in heaven and the player has finished his game
			if (RefCurrentToken.EnumTokenState == eTokenState.InHeaven)
			{
				EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
				int iBlueTokensFinished = 0, iYellowTokensFinished = 0, iRedTokensFinished =0, iGreenTokensFinished = 0;

				switch (RefCurrentToken.EnumTokenType)
				{
					case eTokenType.Blue:
						for(int i = 0;i<m_lstBlueToken.Count;i++)
						{
							if(m_lstBlueToken[i].EnumTokenState == eTokenState.InHeaven)
							{
								iBlueTokensFinished++;
								if (iBlueTokensFinished >= TOKENSPERPLAYER)
								{
									EventManager.Instance.TriggerEvent<EventPlayerFinished>(new EventPlayerFinished(RefCurrentToken));
								}
							}
						}
						break;
					case eTokenType.Yellow:
						for (int i = 0; i < m_lstBlueToken.Count; i++)
						{
							if (m_lstYellowToken[i].EnumTokenState == eTokenState.InHeaven)
							{
								iYellowTokensFinished++;
								if (iYellowTokensFinished >= TOKENSPERPLAYER)
								{
									EventManager.Instance.TriggerEvent<EventPlayerFinished>(new EventPlayerFinished(RefCurrentToken));
								}
							}
						}
						break;
					case eTokenType.Red:
						for (int i = 0; i < m_lstBlueToken.Count; i++)
						{
							if (m_lstRedToken[i].EnumTokenState == eTokenState.InHeaven)
							{
								iRedTokensFinished++;
								if (iRedTokensFinished >= TOKENSPERPLAYER)
								{
									EventManager.Instance.TriggerEvent<EventPlayerFinished>(new EventPlayerFinished(RefCurrentToken));
								}
							}
						}
						break;
					case eTokenType.Green:
						for (int i = 0; i < m_lstBlueToken.Count; i++)
						{
							if (m_lstGreenToken[i].EnumTokenState == eTokenState.InHeaven)
							{
								iGreenTokensFinished++;
								if (iGreenTokensFinished >= TOKENSPERPLAYER)
								{
									EventManager.Instance.TriggerEvent<EventPlayerFinished>(new EventPlayerFinished(RefCurrentToken));
								}
							}
						}
						break;
				}

				
			
			}
			GameManager.Instance.CheckPlayerChangeCondtion();
		}

	}

	private void MoveTweenComplete()
	{	
		m_bMoveTweenComplete = true;
	}

	//After the token has been moved, if other tokens are present move them back to thrier respective homes
	private void CheckIfTileContainsOtherTokens()
	{
		
		if(RefCurrentToken != null)
		{
			Debug.Log("[TokenManager] checking current token tile position with other tokens, send them HOME");
			switch (RefCurrentToken.EnumTokenType)
			{
				case eTokenType.Blue:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{
						if (m_lstBlueToken[i].EnumTokenState == RefCurrentToken.EnumTokenState)
						{
							if (m_lstBlueToken[i].ITokenID != RefCurrentToken.ITokenID)
							{
								if (m_lstBlueToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
								{
									m_lstTokengameobject.Add(m_lstBlueToken[i].gameObject);
								}
							}
						}
						

						if (m_lstRedToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstRedToken[i].transform.DOMove((Vector2)m_lstStartRedTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstRedToken[i].EnumTokenState = eTokenState.House;
								m_lstRedToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstRedToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstRedToken[i].gameObject);
							}
						}

						if (m_lstGreenToken[i].EnumTokenState == eTokenState.InRoute )
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstGreenToken[i].transform.DOMove((Vector2)m_lstStartGreenTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstGreenToken[i].EnumTokenState = eTokenState.House;
								m_lstGreenToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstGreenToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstGreenToken[i].gameObject);
							}
						}

						if (m_lstYellowToken[i].EnumTokenState == eTokenState.InRoute)
						{
							Debug.Log("[TokenManager] Yellow Token not in hiding, its in danger! Current Token: "+RefCurrentToken.ICurrentPathIndex+" Checking Token: "+ m_lstYellowToken[i].ICurrentPathIndex);
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								Debug.Log("[TokenManager] YELLOW: Gotchya GO HOME!");
								m_lstYellowToken[i].transform.DOMove((Vector2)m_lstStartYellowTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstYellowToken[i].EnumTokenState = eTokenState.House;
								m_lstYellowToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstYellowToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstBlueToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstYellowToken[i].gameObject);
							}
						}
					}
					break;
				case eTokenType.Yellow:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{
						if (m_lstYellowToken[i].EnumTokenState == RefCurrentToken.EnumTokenState)
						{
							//Sharing a tile with same token
							if (m_lstYellowToken[i].ITokenID != RefCurrentToken.ITokenID)
							{
								if (m_lstYellowToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
								{
									m_lstTokengameobject.Add(m_lstYellowToken[i].gameObject);
								}
							}
						}
						

						if (m_lstRedToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstRedToken[i].transform.DOMove((Vector2)m_lstStartRedTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstRedToken[i].EnumTokenState = eTokenState.House;
								m_lstRedToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstRedToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstRedToken[i].gameObject);
							}
						}

						if (m_lstGreenToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstGreenToken[i].transform.DOMove((Vector2)m_lstStartGreenTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstGreenToken[i].EnumTokenState = eTokenState.House;
								m_lstGreenToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstGreenToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstGreenToken[i].gameObject);
							}
						}

						if (m_lstBlueToken[i].EnumTokenState == eTokenState.InRoute)
						{
							Debug.Log("[TokenManager] Blue Token not in hiding, its in danger! Current Token: " + m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex + " Checking Token: " + m_lstBlueToken[i].ICurrentPathIndex);
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								Debug.Log("[TokenManager] BLUE: Gotchya GO HOME!");
								m_lstBlueToken[i].transform.DOMove((Vector2)m_lstStartBlueTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstBlueToken[i].EnumTokenState = eTokenState.House;
								m_lstBlueToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstBlueToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstYellowToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstBlueToken[i].gameObject);
							}
						}
					}
					break;
				case eTokenType.Red:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{


						if (m_lstRedToken[i].EnumTokenState == RefCurrentToken.EnumTokenState)
						{
							//Sharing a tile with same token
							if (m_lstRedToken[i].ITokenID != RefCurrentToken.ITokenID)
							{
								if (m_lstRedToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
								{
									m_lstTokengameobject.Add(m_lstRedToken[i].gameObject);
								}
							}
						}
						


						if (m_lstYellowToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstYellowToken[i].transform.DOMove((Vector2)m_lstStartYellowTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstYellowToken[i].EnumTokenState = eTokenState.House;
								m_lstYellowToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstYellowToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstYellowToken[i].gameObject);
							}
						}

						if (m_lstGreenToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstGreenToken[i].transform.DOMove((Vector2)m_lstStartGreenTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstGreenToken[i].EnumTokenState = eTokenState.House;
								m_lstGreenToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstGreenToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstGreenToken[i].gameObject);
							}
						}

						if (m_lstBlueToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstBlueToken[i].transform.DOMove((Vector2)m_lstStartBlueTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstBlueToken[i].EnumTokenState = eTokenState.House;
								m_lstBlueToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstBlueToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstRedToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstBlueToken[i].gameObject);
							}
						}
					}
					break;
				case eTokenType.Green:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{
						if (m_lstGreenToken[i].EnumTokenState == RefCurrentToken.EnumTokenState)
						{
							//Sharing a tile with same token
							if (m_lstGreenToken[i].ITokenID != RefCurrentToken.ITokenID)
							{
								if (m_lstGreenToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
								{
									m_lstTokengameobject.Add(m_lstGreenToken[i].gameObject);
								}
							}
						}
						

						if (m_lstYellowToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstYellowToken[i].transform.DOMove((Vector2)m_lstStartYellowTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstYellowToken[i].EnumTokenState = eTokenState.House;
								m_lstYellowToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstYellowToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstYellowToken[i].gameObject);
							}
						}
						
						if (m_lstRedToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstRedToken[i].transform.DOMove((Vector2)m_lstStartRedTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstRedToken[i].EnumTokenState = eTokenState.House;
								m_lstRedToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if (m_lstRedToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstRedToken[i].gameObject);
							}
						}

						if (m_lstBlueToken[i].EnumTokenState == eTokenState.InRoute)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstBlueToken[i].transform.DOMove((Vector2)m_lstStartBlueTokenPosition[i].position, 5, false).SetSpeedBased(true);
								m_lstBlueToken[i].EnumTokenState = eTokenState.House;
								m_lstBlueToken[i].ICurrentPathIndex = -1;
								EventManager.Instance.TriggerEvent<EventLetCurrentPlayerPlayAgain>(new EventLetCurrentPlayerPlayAgain(true));
							}
						}
						else if(m_lstBlueToken[i].EnumTokenState == eTokenState.InHideOut)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstGreenToken[RefCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstTokengameobject.Add(m_lstBlueToken[i].gameObject);
							}
						}
					}
					break;
			}


			if (m_lstTokengameobject.Count > 0)
			{
				Debug.Log("<color=green>[TokenManager][CheckIfTileContainsOtherTokens] More than one token inside same hHdeOut</color>");
				m_lstTokengameobject.Add(RefCurrentToken.gameObject);
				EventManager.Instance.TriggerEvent<EventTokenScaleFactor>(new EventTokenScaleFactor(m_lstTokengameobject, m_vec2ScaleShared, eScaleType.SharedTile));
				m_lstTokengameobject.Clear();
			}

			

		}
	}

}
