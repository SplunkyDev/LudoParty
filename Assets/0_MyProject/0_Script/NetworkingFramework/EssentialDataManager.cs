using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class EssentialDataManager : MBSingleton<EssentialDataManager>
{

	#region Data
	//App Data
	private const string m_strAppKey = "3947794a91eff135d088c30441ee9f1c995b5f5a32f4843825d0f73c311ea510";
	public string AppKey { get => m_strAppKey; }
	private const string m_strSecretKey = "38148b9167c527644ba2e59f34cb34bc4ddc0d2273f2578f123502cc67c11276";
	public string SecretKey { get => m_strSecretKey; }
	private string m_strMatchId;
	public string MatchId { get => m_strMatchId; }

	//Game Data
	private bool m_bOnlineMode = false;
	public bool BOnlineMode { get => m_bOnlineMode; set => m_bOnlineMode = value; }


	//Network Data
	private const string m_strRoomName = "LudoFunTim";
	public string RoomName { get => m_strRoomName; }
	private const string m_strRoomNo = "493171307";
	public string RoomID { get => m_strRoomNo;}
	private int m_iMaxPlayers = 2;
	public int MaxPlayers { get => m_iMaxPlayers; }

	private ePlayerTurn m_enumPlayerTurn, m_enumOppoentPlayerTurn;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn; }
	public ePlayerTurn EnumOppoentPlayerTurn { get => m_enumOppoentPlayerTurn; set => m_enumOppoentPlayerTurn = value; }
	#endregion


	private string m_strJsonData = string.Empty;


	private EssentialDataManager()
	{

	}

    private void Awake()
    {
        
    }

	private void OnEnable()
	{
		RegisterEvents();
	}

	private void OnDisable()
	{
		DeregisterEvents();
	}

	private void RegisterEvents()
	{
	}

	private void DeregisterEvents()
	{
		if(EventManager.Instance)
		{		
		}
	}

}



