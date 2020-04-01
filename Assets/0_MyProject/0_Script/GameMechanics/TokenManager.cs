using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
	[SerializeField] private List<Transform> m_lstStarttRedTokenPosition = new List<Transform>();
	[SerializeField] private List<Transform> m_lstStartGreenTokenPosition = new List<Transform>();

	[Header("Token References")]
	[SerializeField] private List<TokenData> m_lstBlueToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstYellowToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstRedToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstGreenToken = new List<TokenData>();

	private List<Transform> m_lstTokenMovePoints = new List<Transform>();
	private bool m_bMoveTweenComplete = false;
	private TokenData m_TokenToMove;
	private Vector2 m_vec2Scalevalue = new Vector2(0.65f, 0.65f);

	public delegate void m_delResetToken();
	//This event will be called to reset all token BCanBeUsed to false;
	public m_delResetToken m_OnResetToken;

    // Start is called before the first frame update
    void Start()
    {
		DOTween.Init(true, false, LogBehaviour.Verbose);
    }

	public bool CheckValidTokenMovement(int a_iDiceValue)
	{
		bool bValid = false;
		//Resets all the token before checking their movable state, basically checking if the player can move it at their turn after making the roll
		if(m_OnResetToken != null)
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
		bool bValid = false;

		for (int i = 0; i < a_lstToken.Count; i++)
		{
			switch (a_lstToken[i].EnumTokenState)
			{
				case GameUtility.Base.eTokenState.House:
					if (a_iDiceValue == 6)
					{
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(true).SetLoops(3, LoopType.Yoyo);
					}
					break;
				case GameUtility.Base.eTokenState.InRoute:
				case GameUtility.Base.eTokenState.InHideOut:
					bValid = a_lstToken[i].BCanBeUsed = true;
					a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(true).SetLoops(3, LoopType.Yoyo);
					break;
				case GameUtility.Base.eTokenState.InStairwayToHeaven:
					if (PathManager.Instance.ValidateMovement(m_lstBlueToken[i], a_iDiceValue))
					{
						bValid = a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(true).SetLoops(3, LoopType.Yoyo);
					}
					break;
			}
		}

		return bValid;
	}

	//When the dice has been rolled and the token has been selected this will be called
	public void TokenSelected(TokenData a_refTokenData, int a_iDiceValue)
	{
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
			for (int i = 0; i < m_lstTokenMovePoints.Count; i++)
			{
				m_bMoveTweenComplete = false;
				m_TokenToMove.transform.DOMove(m_lstTokenMovePoints[i].position, 5, false).SetSpeedBased(true).OnComplete(MoveTweenComplete);
				while (!m_bMoveTweenComplete)
				{
					yield return null;
				}
			}
		}
	}

	

	private void MoveTweenComplete()
	{
		m_bMoveTweenComplete = true;
	}

	// Update is called once per frame
	void Update()
    {
    }
}
