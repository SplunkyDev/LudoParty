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
	[SerializeField] private Sprite[] m_arrDiceSprite;

	 private Animator[] m_arrAnimController;
	 private Image[] m_arrDiceImage;

	private Vector2 m_vec2ScaleValue;
	private Image m_imgPrevAvatar, m_imgCurrentAvatar;

	private void RegisterToEvent()
	{
		if(EventManager.Instance == null)
		{
			Debug.LogError("[InGameUIManager] EventManager is null");
			return;
		}

		EventManager.Instance.RegisterEvent<EventHighlightCurrentPlayer>(HighlightCurrentPlayer);
		EventManager.Instance.RegisterEvent<EventDiceRollAnimationComplete>(DiceRollAnimationComplete);
	}

	private void DeregisterToEvent()
	{
		if (EventManager.Instance == null)
		{
			return;
		}

		EventManager.Instance.DeRegisterEvent<EventHighlightCurrentPlayer>(HighlightCurrentPlayer);
		EventManager.Instance.DeRegisterEvent<EventDiceRollAnimationComplete>(DiceRollAnimationComplete);
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
		m_vec2ScaleValue = new Vector2(0.85f, 0.85f);
		m_arrAnimController = new Animator[m_arrPlayerAvatar.Length];
		m_arrDiceImage = new Image[m_arrPlayerAvatar.Length];

		for (int i =0; i<m_arrPlayerAvatar.Length; i++)
		{
			m_arrAnimController[i] = m_arrPlayerAvatar[i].transform.GetComponentInParent<Animator>();
			m_arrDiceImage[i] = m_arrAnimController[i].transform.GetComponent<Image>();
		}
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
			m_imgPrevAvatar.transform.localScale = Vector3.one;

		}
		Debug.Log("[InGameUIManager] Player Token: " + data.EnumPlayerToken + " Player Avatar Index: " + (int)data.EnumPlayerToken);
		m_imgCurrentAvatar = m_arrPlayerAvatar[(int)data.EnumPlayerToken -1];
		m_imgCurrentAvatar.transform.DOScale(m_vec2ScaleValue, 0.75f).From(false).SetLoops(-1).SetEase(Ease.OutCirc);
	}

	public void DiceRollAnimationComplete(IEventBase a_Event)
	{
		EventDiceRollAnimationComplete data = a_Event as EventDiceRollAnimationComplete;
		if (data == null)
		{
			Debug.LogError("[InGameUIManager] Dice Roll  trigger is null");
			return;
		}

		m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken - 1].SetBool("RollDice", false);
		m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken - 1].enabled = false;

		Debug.Log("[InGameUIManager] The Dice Roll value: "+ GameManager.Instance.ICurrentDiceValue);
		//This will show the current roll sprite on the dice image
		m_arrDiceImage[(int)GameManager.Instance.EnumPlayerToken - 1].sprite = m_arrDiceSprite[GameManager.Instance.ICurrentDiceValue - 1];

		GameManager.Instance.CheckResult();
	}

	public void InputButton(string a_strInput)
	{
		switch (a_strInput)
		{
			case "RollDice":
				Debug.Log("[InGameUIManager] Roll Dice");
				GameManager.Instance.RollTheDice();
				AnimateDiceRoll();
				break;
			case "BackToMenu":
				break;
		}

	}

	private void AnimateDiceRoll()
	{
		m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken - 1].enabled = true;
		m_arrAnimController[(int)GameManager.Instance.EnumPlayerToken -1].SetBool("RollDice",true);
	}


}
