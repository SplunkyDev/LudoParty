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

	[SerializeField] private List<TokenData> m_lstBlueToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstYellowToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstRedToken = new List<TokenData>();
	[SerializeField] private List<TokenData> m_lstGreenToken = new List<TokenData>();

	private List<Transform> m_lstTokenMovePoints = new List<Transform>();
	private bool m_bMoveTweenComplete = false;
	private TokenData m_TokenToMove;
	private Vector2 m_vec2Scalevalue = new Vector2(0.65f, 0.65f);

    // Start is called before the first frame update
    void Start()
    {
		DOTween.Init(true, false, LogBehaviour.Verbose);
    }

	public void CheckvalidTokenMovement(int a_iDiceValue)
	{
		switch (GameManager.Instance.EnumPlayerTurn)
		{

			case GameUtility.Base.ePlayerTurn.Blue:
				AnimateValidTokens(m_lstBlueToken, a_iDiceValue);
				break;
			case GameUtility.Base.ePlayerTurn.Yellow:
				AnimateValidTokens(m_lstYellowToken, a_iDiceValue);
				break;
			case GameUtility.Base.ePlayerTurn.red:
				AnimateValidTokens(m_lstRedToken, a_iDiceValue);
				break;
			case GameUtility.Base.ePlayerTurn.Green:
				AnimateValidTokens(m_lstGreenToken, a_iDiceValue);
				break;
			default:
				break;
		}

	}
		

	//This method is used to check which tokens can be moved
	private void AnimateValidTokens(List<TokenData> a_lstToken, int a_iDiceValue)
	{
		for (int i = 0; i < a_lstToken.Count; i++)
		{
			switch (a_lstToken[i].EnumTokenState)
			{
				case GameUtility.Base.eTokenState.House:
					if (a_iDiceValue == 6)
					{
						a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(true).SetLoops(3, LoopType.Yoyo);
					}
					break;
				case GameUtility.Base.eTokenState.InRoute:
				case GameUtility.Base.eTokenState.InHideOut:
					a_lstToken[i].BCanBeUsed = true;
					a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(true).SetLoops(3, LoopType.Yoyo);
					break;
				case GameUtility.Base.eTokenState.InStairwayToHeaven:
					if (PathManager.Instance.ValidateMovement(m_lstBlueToken[i], a_iDiceValue))
					{
						a_lstToken[i].BCanBeUsed = true;
						a_lstToken[i].transform.DOScale(m_vec2Scalevalue, 0.5f).From(true).SetLoops(3, LoopType.Yoyo);
					}
					break;
			}
		}
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
