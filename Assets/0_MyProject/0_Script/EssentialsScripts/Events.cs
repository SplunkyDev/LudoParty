using GameUtility.Base;
using UnityEngine;

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