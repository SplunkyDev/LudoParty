using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameUtility.Base;

public class TouchInputManager : MBSingleton<TouchInputManager>
{
	private int m_iTouchID = -1;
	private Vector3 m_vec3BeganPosi;
	private bool m_bPlayerReady = false;

	void Awake()
	{
	}

	// Start is called before the first frame update
	void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
#if UNITY_STANDALONE
		MouseInput();
#elif UNITY_ANDROID
		TouchInput();
#endif

	}

	private void TouchInput()
	{

		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (m_iTouchID == -1)
			{
				m_iTouchID = touch.fingerId;
			}


			if (m_iTouchID != touch.fingerId)
				return;

			switch (touch.phase)
			{
				case TouchPhase.Began:
					m_vec3BeganPosi = touch.position;
					m_bPlayerReady = true;
					EventManager.Instance.TriggerEvent<EventTouchActive>(new EventTouchActive(true, m_vec3BeganPosi));

					break;
				case TouchPhase.Moved:

					if (Vector3.Distance(m_vec3BeganPosi, touch.position) < 1)
						return;

					Vector3 vec3Offset = (new Vector3(touch.deltaPosition.x, 0, 0));
					EventManager.Instance.TriggerEvent<EventTouchMove>(new EventTouchMove(true, vec3Offset));
					break;
				case TouchPhase.Stationary:
					EventManager.Instance.TriggerEvent<EventTouchMove>(new EventTouchMove(false, Vector3.zero));
					break;
				case TouchPhase.Ended:
					if (m_iTouchID != touch.fingerId)
					{
						Debug.LogError("[TouchManager] Finger ID doesn't match");
						return;
					}
					m_iTouchID = -1;

					if(m_bPlayerReady)
					{
						//TODD: Stop any running process	
					}
					m_bPlayerReady = false;

					EventManager.Instance.TriggerEvent<EventTouchMove>(new EventTouchMove(false, Vector3.zero));
					EventManager.Instance.TriggerEvent<EventTouchActive>(new EventTouchActive(false, Vector3.zero));
					//Debug.Log("[TouchInputManager] Touch phase: ENDED");
					break;
				case TouchPhase.Canceled:
					EventManager.Instance.TriggerEvent<EventTouchMove>(new EventTouchMove(false, Vector3.zero));
					EventManager.Instance.TriggerEvent<EventTouchActive>(new EventTouchActive(false, Vector3.zero));
					//Debug.Log("[TouchInputManager] Touch phase: CANCELLED");
					break;
			}

		}
		else
		{
			m_iTouchID = -1;
		}
	}

	private void MouseInput()
	{

	}
}
