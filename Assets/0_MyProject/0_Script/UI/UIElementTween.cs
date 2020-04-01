using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using GameUtility.Base;

public class UIElementTween : MonoBehaviour
{
	[Tooltip("None, default scale effect")]
	public eUITweenMoveState m_eUITweenMoveState;
	public Ease m_easeType;
	[Tooltip("SelfTweenTime will override the default time if AllowSelfTween is checked")]
	public bool m_bAllowSelfTween;
	public float m_fSelfTweenTime;

	private RectTransform m_rectTransform;
	private Vector3 m_vec3OffScreenHorizontal, m_vec3OffScreenVertical;
	private Vector3 m_vec3ScreenPosition;
    // Start is called before the first frame update
    private void Awake()
    {
		DOTween.Init(true, true, LogBehaviour.ErrorsOnly);
		m_rectTransform = GetComponent<RectTransform>();
		m_vec3ScreenPosition = m_rectTransform.anchoredPosition;
		m_vec3OffScreenHorizontal = new Vector3(1000,0,0);
		m_vec3OffScreenVertical = new Vector3(0, 1000, 0);
		ElementTweenOut(0);
	}

	public void ElementTweenIn(float a_fTweenTime)
	{
		switch (m_eUITweenMoveState)
		{
			case eUITweenMoveState.None:
				m_rectTransform.DOScale(Vector3.one, m_bAllowSelfTween == true ? m_fSelfTweenTime : a_fTweenTime).From(Vector3.zero).SetEase(m_easeType);
				break;
			case eUITweenMoveState.Horizontal_Left:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition, m_bAllowSelfTween == true ? m_fSelfTweenTime : a_fTweenTime).From(m_vec3ScreenPosition - m_vec3OffScreenHorizontal).SetEase(m_easeType);
				break;
			case eUITweenMoveState.Horizontal_Right:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition, m_bAllowSelfTween == true ? m_fSelfTweenTime : a_fTweenTime).From(m_vec3ScreenPosition + m_vec3OffScreenHorizontal).SetEase(m_easeType);
				break;
			case eUITweenMoveState.Vertical_Up:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition, m_bAllowSelfTween == true ? m_fSelfTweenTime : a_fTweenTime).From(m_vec3ScreenPosition + m_vec3OffScreenVertical).SetEase(m_easeType);
				break;
			case eUITweenMoveState.vertical_Down:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition, m_bAllowSelfTween == true ? m_fSelfTweenTime : a_fTweenTime).From(m_vec3ScreenPosition - m_vec3OffScreenVertical).SetEase(m_easeType);
				break;
			default:
				break;
		}

	}

	public void ElementTweenOut(float a_fTweenTime)
	{
		switch (m_eUITweenMoveState)
		{
			case eUITweenMoveState.None:
				m_rectTransform.DOScale(Vector3.zero, a_fTweenTime).SetEase(m_easeType);
				break;
			case eUITweenMoveState.Horizontal_Left:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition - m_vec3OffScreenHorizontal, a_fTweenTime).SetEase(Ease.InSine);
				break;
			case eUITweenMoveState.Horizontal_Right:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition + m_vec3OffScreenHorizontal, a_fTweenTime).SetEase(Ease.InSine);
				break;
			case eUITweenMoveState.Vertical_Up:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition + m_vec3OffScreenVertical, a_fTweenTime).SetEase(Ease.InSine);
				break;
			case eUITweenMoveState.vertical_Down:
				m_rectTransform.DOAnchorPos(m_vec3ScreenPosition - m_vec3OffScreenVertical, a_fTweenTime).SetEase(Ease.InSine);
				break;
			default:
				break;
		}
	}

}
