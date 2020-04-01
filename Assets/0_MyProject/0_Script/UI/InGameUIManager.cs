using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;
using UnityEngine.UI;
using DG.Tweening;

public class InGameUIManager : MonoBehaviour
{

	[Header("Player Avatar (Blue,Yellow,Red,Green)")]
	[SerializeField] private Image[] m_arrPlayerAvatar;

	private Vector2 m_vec2ScaleValue = new Vector2(0.65f, 0.65f);
	private Image m_imgPrevAvatar, m_imgCurrentAvatar;

	private void RegisterToEvent()
	{
		if(EventManager.Instance == null)
		{
			Debug.LogError("[InGameUIManager] EventManager is null");
			return;
		}

		EventManager.Instance.RegisterEvent<EventHighlightCurrentPlayer>(HighlightCurrentPlayer);
	}

	private void DeregisterToEvent()
	{
		if (EventManager.Instance == null)
		{
			Debug.LogError("[InGameUIManager] EventManager is null");
			return;
		}

		EventManager.Instance.DeRegisterEvent<EventHighlightCurrentPlayer>(HighlightCurrentPlayer);
	}

	private void OnEnable()
	{
		RegisterToEvent();
	}

	private void OnDisable()
	{
		DeregisterToEvent();
	}

	private void Start()
    {
		DOTween.Init(true, false, LogBehaviour.Verbose);
	}

    private void Update()
    {
        
    }


	public void HighlightCurrentPlayer(IEventBase a_Event)
	{
		Debug.Log("[InGameUIManager] HighlightCurrentPlayer TRIGGERED");
		EventHighlightCurrentPlayer data = a_Event as EventHighlightCurrentPlayer;
		if(data == null)
		{
			Debug.LogError("[InGameUIManager] highlight trigger is null");
			return;
		}

		if(m_imgCurrentAvatar != null)
		{
			m_imgPrevAvatar = m_imgCurrentAvatar;
			m_imgPrevAvatar.transform.DOPause();
			
		}
		m_imgCurrentAvatar = m_arrPlayerAvatar[(int)data.EnumPlayerToken];
		m_imgCurrentAvatar.transform.DOScale(m_vec2ScaleValue, 1).From(true).SetLoops(1, LoopType.Yoyo);
	}


	public void InputButton(string a_strInput)
	{
		switch (a_strInput)
		{
			case "RollDice":
				break;
			case "BackToMenu":
				break;
		}

	}


}
