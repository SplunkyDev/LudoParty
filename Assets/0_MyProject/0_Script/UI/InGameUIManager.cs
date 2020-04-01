using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;
using UnityEngine.UI;

public class InGameUIManager : MonoBehaviour
{

	[Header("Player Avatar (Blue,Yellow,Red,Green)")]
	[SerializeField] private Image[] m_ImgPlayerAvatar;


    private void Start()
    {
		
	}

    private void Update()
    {
        
    }

	public void TweenInGameUI()
	{
		EventManager.Instance.TriggerEvent<EventShowInGameUI>(new EventShowInGameUI(true, eGameState.InGame));
	}
}
