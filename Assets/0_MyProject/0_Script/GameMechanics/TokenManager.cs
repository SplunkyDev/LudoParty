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

	[Header("Token Layer")]
	[SerializeField] private LayerMask m_layerMask;

	private List<Transform> m_lstTokenMovePoints = new List<Transform>();
	private bool m_bMoveTweenComplete = false;
	private TokenData m_TokenToMove;
	private Vector2 m_vec2Scalevalue = new Vector2(0.95f, 0.95f);
	private TweenParams m_tweenScaleEffect;
	private TokenData m_refCurrentToken;

	private const int TOKENSPERPLAYER = 4;

	public delegate void m_delResetToken();
	//This event will be called to reset all token BCanBeUsed to false;
	public m_delResetToken m_OnResetToken;


	private void RegisterToEvents()
	{
		EventManager.Instance.RegisterEvent<EventTouchActive>(InputReceived);
	}

	private void DereegiterToEvents()
	{

		if (EventManager.Instance == null)
			return;

		EventManager.Instance.DeRegisterEvent<EventTouchActive>(InputReceived);
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

	public bool CheckValidTokenMovement(int a_iDiceValue)
	{
		
		Debug.Log("[TokenManager][CheckValidTokenMovement]");
		bool bValid = false;
		//Resets all the token before checking their movable state, basically checking if the player can move it at their turn after making the roll
		if (m_OnResetToken != null)
		{
			m_OnResetToken.Invoke();
		}

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
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					}
					break;
				case GameUtility.Base.eTokenState.InRoute:
				case GameUtility.Base.eTokenState.InHideOut:
					bValid = a_lstToken[i].BCanBeUsed = true;
					a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					break;
				case GameUtility.Base.eTokenState.InStairwayToHeaven:
					if (PathManager.Instance.ValidateMovement(m_lstBlueToken[i], a_iDiceValue))
					{
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(false).SetAs(m_tweenScaleEffect).SetId("ScaleEffect");
					}
					break;
			}
		}

		return bValid;
	}


	private void InputReceived(IEventBase a_Event)
	{
		EventTouchActive data = a_Event as EventTouchActive;
		if(data == null)
		{
			Debug.LogError("[TokenManager] Touch Active trigger null");
			return;
		}

		Debug.Log("[TokenManager] Input detected");
		if(data.BTouch)
		{
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
				TokenSelected(hit.transform.parent.gameObject.GetComponent<TokenData>(),GameManager.Instance.ICurrentDiceValue);
			}
		}
	}

	//When the dice has been rolled and the token has been selected this will be called
	private void TokenSelected(TokenData a_refTokenData, int a_iDiceValue)
	{
		

		if (!a_refTokenData.BCanBeUsed)
		{
			Debug.LogError("[TokenManager] This Token cannot be moved");
			return;
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
				m_TokenToMove = m_lstBlueToken[a_refTokenData.ITokenID];
				break;
			case GameUtility.Base.eTokenType.Yellow:
				m_TokenToMove = m_lstYellowToken[a_refTokenData.ITokenID];
				break;
			case GameUtility.Base.eTokenType.Red:
				m_TokenToMove = m_lstRedToken[a_refTokenData.ITokenID];
				break;
			case GameUtility.Base.eTokenType.Green:
				m_TokenToMove = m_lstGreenToken[a_refTokenData.ITokenID];
				break;
			default:
				break;
		}
		//calling the coroutine
		StartCoroutine(PlayerTurn(0.15f));

		//This coroutine is local to this method
		IEnumerator PlayerTurn(float a_fDelay)
		{
			yield return new WaitForSeconds(a_fDelay);
			m_lstTokenMovePoints = PathManager.Instance.TokenStateUpdate(m_TokenToMove, a_iDiceValue);
			Debug.Log("<color=red>[TokenManager]  m_TokenToMove: " + m_TokenToMove.ICurrentPathIndex+"</color>");
			for (int i = 0; i < m_lstTokenMovePoints.Count; i++)
			{
				m_bMoveTweenComplete = false;
				m_TokenToMove.transform.DOMove((Vector2)m_lstTokenMovePoints[i].transform.position, 5, false).SetSpeedBased(true).OnComplete(MoveTweenComplete);
				while (!m_bMoveTweenComplete)
				{
					yield return null;
				}
			}

			GameManager.Instance.CurrentPlayer.m_ePlayerState = ePlayerState.PlayerRollDice;
			if (m_TokenToMove.EnumTokenState != eTokenState.House)
			{
				m_refCurrentToken = m_TokenToMove;
				Debug.Log("<color=red>[TokenManager] Current Token InRoute,check if other token present in same tile: m_refCurrentToken: " + m_refCurrentToken.ICurrentPathIndex + "</color>");
				CheckIfTileContainsOtherTokens();
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
		
		if(m_refCurrentToken != null)
		{
			Debug.Log("[TokenManager] checking current token tile position with other tokens, send them HOME");
			switch (m_refCurrentToken.EnumTokenType)
			{
				case eTokenType.Blue:
					for(int i = 0;i< TOKENSPERPLAYER; i++)
					{
						if(m_lstRedToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstBlueToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstRedToken[i].transform.DOMove((Vector2)m_lstStartRedTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstGreenToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstBlueToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstGreenToken[i].transform.DOMove((Vector2)m_lstStartGreenTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstYellowToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							Debug.Log("[TokenManager] Yellow Token not in hiding, its in danger! Current Token: "+m_refCurrentToken.ICurrentPathIndex+" Checking Token: "+ m_lstYellowToken[i].ICurrentPathIndex);
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstBlueToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								Debug.Log("[TokenManager] YELLOW: Gotchya GO HOME!");
								m_lstYellowToken[i].transform.DOMove((Vector2)m_lstStartYellowTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}
					}
					break;
				case eTokenType.Yellow:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{
						if (m_lstRedToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstYellowToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstRedToken[i].transform.DOMove((Vector2)m_lstStartRedTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstGreenToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstYellowToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstGreenToken[i].transform.DOMove((Vector2)m_lstStartGreenTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstBlueToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							Debug.Log("[TokenManager] Blue Token not in hiding, its in danger! Current Token: " + m_lstYellowToken[m_refCurrentToken.ITokenID].ICurrentPathIndex + " Checking Token: " + m_lstBlueToken[i].ICurrentPathIndex);
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstYellowToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								Debug.Log("[TokenManager] BLUE: Gotchya GO HOME!");
								m_lstBlueToken[i].transform.DOMove((Vector2)m_lstStartBlueTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}
					}
					break;
				case eTokenType.Red:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{
						if (m_lstYellowToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstRedToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstYellowToken[i].transform.DOMove((Vector2)m_lstStartYellowTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstGreenToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstGreenToken[i].ICurrentPathIndex == m_lstRedToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstGreenToken[i].transform.DOMove((Vector2)m_lstStartGreenTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstBlueToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstRedToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstBlueToken[i].transform.DOMove((Vector2)m_lstStartBlueTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}
					}
					break;
				case eTokenType.Green:
					for (int i = 0; i < TOKENSPERPLAYER; i++)
					{
						if (m_lstYellowToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstYellowToken[i].ICurrentPathIndex == m_lstGreenToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstYellowToken[i].transform.DOMove((Vector2)m_lstStartYellowTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstRedToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstRedToken[i].ICurrentPathIndex == m_lstGreenToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstRedToken[i].transform.DOMove((Vector2)m_lstStartRedTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}

						if (m_lstBlueToken[i].EnumTokenState != eTokenState.InHideOut)
						{
							if (m_lstBlueToken[i].ICurrentPathIndex == m_lstGreenToken[m_refCurrentToken.ITokenID].ICurrentPathIndex)
							{
								m_lstBlueToken[i].transform.DOMove((Vector2)m_lstStartBlueTokenPosition[i].position, 5, false).SetSpeedBased(true);
							}
						}
					}
					break;
			}

		}
	}

}
