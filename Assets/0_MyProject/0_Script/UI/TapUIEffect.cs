using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class TapUIEffect : MonoBehaviour, IPointerClickHandler,IPointerUpHandler, IPointerDownHandler
{

	private RectTransform m_rectTransform;
	private  Vector3 m_vec3ScaleValue;

	[SerializeField]
	private Ease m_EaseEffect;

	public void OnPointerClick(PointerEventData eventData)
	{
	}

	public void OnPointerDown(PointerEventData eventData)
	{

	}

	public void OnPointerUp(PointerEventData eventData)
	{
		m_rectTransform.DOScale(Vector3.one, 0.25f).From(m_vec3ScaleValue).SetEase(m_EaseEffect);
	}


	void Start()
    {
		DOTween.Init(true, true, LogBehaviour.ErrorsOnly);

		m_vec3ScaleValue = new Vector3(0.75f, 0.75f, 0.75f);
		m_rectTransform = GetComponent<RectTransform>();
	}


}
