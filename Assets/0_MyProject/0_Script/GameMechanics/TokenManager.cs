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

    // Start is called before the first frame update
    void Start()
    {
		DOTween.Init(true, false, LogBehaviour.Verbose);
    }
	
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
