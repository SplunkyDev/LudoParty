using System.Collections;
using System.Collections.Generic;
using GameUtility.Base;
using UnityEngine;

public class GameManager : MBSingleton<GameManager>
{

	private const int MAXTRY = 3;

	private PlayerData m_RefCurrentPlayer;
	private int m_iCurrentDiceValue = 0, m_iTotalRolls = MAXTRY;
	private List<PlayerData> m_lstPlayerData = new List<PlayerData>();

	private eGameState m_enumGameState;
	public eGameState EnumGameState { get => m_enumGameState; set => m_enumGameState = value; }

	private ePlayerTurn m_enumCurrentPlayerTurn = ePlayerTurn.PlayerOne;
	private ePlayerTurn m_enumPlayerTurn = ePlayerTurn.PlayerOne;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn; }

	public ePlayerToken EnumPlayerToken
	{
		get
		{
			for (int i = 0; i < m_lstPlayerData.Count; i++)
			{
				if (m_lstPlayerData[i].m_enumPlayerTurn == EnumPlayerTurn)
				{
					return m_lstPlayerData[i].m_enumPlayerToken;
				}				
			}
			return ePlayerToken.None;
		}
	}


	public void SetPlayerData(PlayerData a_PlayerData)
	{
		//Setting the value of how many turns until the player gets a forced six if not got until then
		a_PlayerData.m_iRollSixIn = Random.Range(5, 10);
		m_lstPlayerData.Add(a_PlayerData);
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	public void RollTheDice()
	{
		//Let's Roll a D20, ;-P
		m_iCurrentDiceValue = UnityEngine.Random.Range(1, 6);
	}

	private void CheckPlayerPlayCondition()
	{
	
		//Checking how many 6 dice value has been got by player
		if(m_iCurrentDiceValue == 6)
		{
			UpdatePlayerSixPossiblity();
			if (m_iTotalRolls <= 0)
			{
				ChangePlayerTurn();
			}
		}
		else 
		{
			if (!TokenManager.Instance.CheckValidTokenMovement(m_iCurrentDiceValue))
			{
				ChangePlayerTurn();
			}

			//If SIX has not been got by the player 
			IncreasePossiblityofGettingSix();
		}
	}

	private void ChangePlayerTurn()
	{
		//TODO: change to next player turn

		if (m_enumCurrentPlayerTurn != m_enumPlayerTurn)
		{
			m_enumCurrentPlayerTurn = m_enumPlayerTurn;
			m_iTotalRolls = MAXTRY;
		}

		//Get Reference to PlayerData, dont need to loop through players when ever update in data is required
		CurrentPlayerData();
	}



	#region Modify_PlayerData
	private void UpdatePlayerSixPossiblity()
	{
		m_RefCurrentPlayer.m_iRollSixIn = Random.Range(5, 10);
	}

	private void IncreasePossiblityofGettingSix()
	{
		m_RefCurrentPlayer.m_iRollSixIn--;
	}

	private void CurrentPlayerData()
	{
		for (int i = 0; i < m_lstPlayerData.Count; i++)
		{
			if (m_lstPlayerData[i].m_enumPlayerTurn == m_enumCurrentPlayerTurn)
			{
				m_RefCurrentPlayer = m_lstPlayerData[i];
			}
		}
	}
	#endregion
}
