using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;

public class TokenData : MonoBehaviour
{
	//This type is to identify which token it is
	[SerializeField] private GameUtility.Base.eTokenType m_enumTokenType = GameUtility.Base.eTokenType.None;
	public eTokenType EnumTokenType { get => m_enumTokenType; }
	[SerializeField] private int m_iTokenID = 0;
	public int ITokenID { get => m_iTokenID;}

	private int m_iCurrentPathIndex = -1; //-1 means it is at home
	public int ICurrentPathIndex { get => m_iCurrentPathIndex; set => m_iCurrentPathIndex = value; }

	private bool m_bInSpecial;
	public bool BInSpecial { get => m_bInSpecial; set => m_bInSpecial = value; }

	private GameUtility.Base.eTokenState m_enumTokenState = GameUtility.Base.eTokenState.House;
	public eTokenState EnumTokenState { get => m_enumTokenState; set => m_enumTokenState = value; }

	private bool m_bCanBeUsed = false;
	public bool BCanBeUsed { get => m_bCanBeUsed; set => m_bCanBeUsed = value; }

	private void OnEnable()
	{
		if(TokenManager.Instance == null)
		{
			Debug.LogError("[TokenData] Cannot find TokenManager");
			return;
		}

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
