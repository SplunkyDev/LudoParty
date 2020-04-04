using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;

[System.Serializable]
public class TokenData : MonoBehaviour
{

	public TokenData (eTokenType a_enumTokenType, int a_iTokenID, bool a_bCanBeUsed)
	{
		m_enumTokenType = a_enumTokenType;
		m_iTokenID = a_iTokenID;
		m_bCanBeUsed = a_bCanBeUsed;
	}

	//This type is to identify which token it is
	[SerializeField] private GameUtility.Base.eTokenType m_enumTokenType = GameUtility.Base.eTokenType.None;
	public eTokenType EnumTokenType { get => m_enumTokenType; }
	[SerializeField] private int m_iTokenID = 0;
	public int ITokenID { get => m_iTokenID;}

	[SerializeField]private int m_iCurrentPathIndex = -1; //-1 means it is at home
	public int ICurrentPathIndex { get => m_iCurrentPathIndex; set => m_iCurrentPathIndex = value; }

	[SerializeField]private GameUtility.Base.eTokenState m_enumTokenState = GameUtility.Base.eTokenState.House;
	public eTokenState EnumTokenState { get => m_enumTokenState; set => m_enumTokenState = value; }

	[SerializeField] private bool m_bCanBeUsed = false;
	public bool BCanBeUsed { get => m_bCanBeUsed; set => m_bCanBeUsed = value; }

	[SerializeField] private Vector2 m_vec2PositionOnTile;
	public Vector2 Vec2PositionOnTile { get => m_vec2PositionOnTile; set => m_vec2PositionOnTile = value; }

	[SerializeField] private SpriteRenderer m_spriteRenderer;

	private void OnEnable()
	{
		if(TokenManager.Instance == null)
		{
			Debug.LogError("[TokenData] Cannot find TokenManager");
			return;
		}

		Vec2PositionOnTile = transform.position;
		m_spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
		TokenManager.Instance.m_OnResetToken += ResetTokenUsability;
	}

	private void OnDisable()
	{
		if (TokenManager.Instance == null)
		{
			Debug.LogError("[TokenData] Cannot find TokenManager");
			return;
		}

		TokenManager.Instance.m_OnResetToken -= ResetTokenUsability;
	}

	
	private void ResetTokenUsability()
	{
		m_bCanBeUsed = false;
		m_spriteRenderer.sortingOrder = 0;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
