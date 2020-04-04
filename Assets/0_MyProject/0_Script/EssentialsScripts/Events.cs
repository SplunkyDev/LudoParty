using GameUtility.Base;
using UnityEngine;
using System.Collections.Generic;

#region GameMechanics

//Event to start a new session as all details have been got
public class EventStartGameSession : IEventBase
{
	public EventStartGameSession()
	{

	}
}

public class EventDiceRollAnimationComplete : IEventBase
{
	public EventDiceRollAnimationComplete() { }
}

public class EventPlayerTurnChanged : IEventBase
{
	public EventPlayerTurnChanged() { }
}

public class EventPlayerFinished : IEventBase
{
	private TokenData m_refTokenData;
	public TokenData RefTokenData { get => m_refTokenData;}

	public EventPlayerFinished(TokenData a_refTokenData)
	{
		m_refTokenData = a_refTokenData;
	}

}

public class EventTokenScaleFactor : IEventBase
{
	private List<GameObject> m_lstTokenGameObject;
	public List<GameObject> LTokenGameObject { get => m_lstTokenGameObject; }
	private Vector2 m_vec2ScaleValue;
	public Vector2 Vec2ScaleValue { get => m_vec2ScaleValue; }
	private eScaleType m_enumScaleType;
	public eScaleType EScaleType { get => m_enumScaleType; }

	public EventTokenScaleFactor(List<GameObject> a_lstTokenGameObject, Vector2 a_vec2ScaleValue, eScaleType a_enumScaleType)
	{
		m_lstTokenGameObject = a_lstTokenGameObject;
		m_vec2ScaleValue = a_vec2ScaleValue;
		m_enumScaleType = a_enumScaleType;
	}

}
#endregion

#region MultiplayerFeatures
//Event to serialize the data into json
public class EventSerializeGameEssentialData : IEventBase
{
	public EventSerializeGameEssentialData()
	{

	}
}

//Event to deserialize the data from json
public class EventDeserializeGameEssentialData : IEventBase
{
	private string m_strJsonData;
	public string StrJsonData { get => m_strJsonData; }
	public EventDeserializeGameEssentialData(string a_strJsonData)
	{
		m_strJsonData = a_strJsonData;
	}

}

//Event to insert message to json to send it to room  server
public class EventInsertInGameMessage : IEventBase
{
	private eMessageType[] m_eMessageType;
	public eMessageType[] EMessageType { get => m_eMessageType; }
	public EventInsertInGameMessage(eMessageType[] a_eMessageType)
	{
		m_eMessageType = a_eMessageType;
	}

}

//Event to read message from json got from room  server
public class EventReadInGameMessage : IEventBase
{
	private string m_strInGameMessage;
	public string StrInGameMessage { get => m_strInGameMessage; }
	public EventReadInGameMessage(string a_strInGameMessage)
	{
		m_strInGameMessage = a_strInGameMessage;
	}

}

//Event setting the player turn to this deviice
public class EventDevicePlayerTurn : IEventBase
{
	private ePlayerTurn m_enumPlayerTurn;
	public ePlayerTurn EnumPlayerTurn { get => m_enumPlayerTurn;  }

	public EventDevicePlayerTurn(ePlayerTurn a_enumPlayerTurn)
	{
		m_enumPlayerTurn = a_enumPlayerTurn;
	}

}

//Event when opponent has left
public class EventOpponentLeftRoom : IEventBase
{
	public EventOpponentLeftRoom()
	{

	}
}

public class EventTokenSelectedInMultiplayer : IEventBase
{
	private TokenData m_refTokenData;
	public TokenData RefTokenData { get => m_refTokenData;}

	public EventTokenSelectedInMultiplayer(TokenData a_refTokenData)
	{
		m_refTokenData = a_refTokenData;
	}

}

#region AppWarp

public class EventOpponentDiceRollAnimation : IEventBase
{
	public EventOpponentDiceRollAnimation(int a_iDiceRoll)
	{		
	}
}

public class EventOpponentDiceRoll : IEventBase
{
	private int m_iDiceRoll;
	public int IDiceRoll { get => m_iDiceRoll;}

	public EventOpponentDiceRoll(int a_iDiceRoll)
	{
		m_iDiceRoll = a_iDiceRoll;
	}
}

public class EventFirstPlayerEntered : IEventBase
{
	private string m_strUserName = string.Empty;
	public string StrUsername { get => m_strUserName; }
	public EventFirstPlayerEntered(string a_strUsername)
	{
		m_strUserName = a_strUsername;
	}
}

public class EventGenerateNextPlayer : IEventBase
{
	private string m_strUserName = string.Empty;
	public string StrUsername { get => m_strUserName; } 
	public EventGenerateNextPlayer(string a_strUsername)
	{
		m_strUserName = a_strUsername;
	}
}

public class EventOpponentTokenSelected : IEventBase
{
	private TokenData m_refTokenSelected;
	public TokenData TokenSelected { get => m_refTokenSelected; }

	public EventOpponentTokenSelected(TokenData a_refTokenSelected)
	{
		m_refTokenSelected = a_refTokenSelected;
	}
}

public class EventInitializeNetworkApi : IEventBase
{
	public EventInitializeNetworkApi()
	{

	}
}

public class EventConnectToServer : IEventBase
{
	public EventConnectToServer()
	{

	}
}

public class EventDisonnectFromServer : IEventBase
{
	public EventDisonnectFromServer()
	{

	}
}


public class EventSubscribeRoom : IEventBase
{
	public EventSubscribeRoom()
	{

	}
}

public class EventJoinRoom : IEventBase
{
	public EventJoinRoom()
	{

	}
}

public class EventReconnectServer : IEventBase
{
	public EventReconnectServer()
	{

	}
}
#endregion
#endregion

#region UI_EVENTS
//Event to call Game Complete UI
public class EventShowGameCompleteUI : IEventBase
{
	private bool m_bShowUI = false;
	public bool BShowUI { get => m_bShowUI; }
	private eGameState m_eGameState;
	public eGameState EGameState { get => m_eGameState; }



	public EventShowGameCompleteUI(bool a_bShowUI, eGameState a_eGameState)
	{
		m_bShowUI = a_bShowUI;
		m_eGameState = a_eGameState;
	}
}

//Event to show in MENu UI
public class EventShowMenuUI : IEventBase
{
	private bool m_bShowUI = false;
	public bool BShowUI { get => m_bShowUI; }
	private eGameState m_eGameState;
	public eGameState EGameState { get => m_eGameState; }

	public EventShowMenuUI(bool a_bShowUI, eGameState a_eGameState)
	{
		m_bShowUI = a_bShowUI;
		m_eGameState = a_eGameState;
	}
}


//Event to show in game UI
public class EventShowInGameUI : IEventBase
{
	private bool m_bShowUI = false;
	public bool BShowUI { get => m_bShowUI; }
	private eGameState m_eGameState;
	public eGameState EGameState { get => m_eGameState; }

	public EventShowInGameUI(bool a_bShowUI, eGameState a_eGameState)
	{
		m_bShowUI = a_bShowUI;
		m_eGameState = a_eGameState;
	}
}

public class EventHighlightCurrentPlayer : IEventBase
{
	private ePlayerToken m_enumPlayerToken;
	public ePlayerToken EnumPlayerToken { get => m_enumPlayerToken;  }

	public EventHighlightCurrentPlayer(ePlayerToken a_enumPlayerToken)
	{
		m_enumPlayerToken = a_enumPlayerToken;
	}
}

#endregion

#region TouchEvents
public class EventTouchActive : IEventBase
{
	private bool m_bTouch = false;
	public bool BTouch { get => m_bTouch;}

	private Vector3 m_vec3TouchPosition;
	public Vector3 Vec3TouchPosition { get => m_vec3TouchPosition;}

	public EventTouchActive(bool a_bTouch, Vector3 a_vec3TouchPosi)
	{
		m_bTouch = a_bTouch;
		m_vec3TouchPosition = a_vec3TouchPosi;
	}
}

public class EventTouchMove : IEventBase
{
	private bool m_bTouch = false;
	public bool BTouchMove { get => m_bTouch; }

	private Vector3 m_vec3DeltaPosi;
	public Vector3 Vec3DeltaPostion { get => m_vec3DeltaPosi; }
	public EventTouchMove(bool a_bTouch, Vector3 a_vec3DeltaPosi)
	{
		m_bTouch = a_bTouch;
		m_vec3DeltaPosi = a_vec3DeltaPosi;
	}
}
#endregion