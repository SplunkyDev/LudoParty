using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingRotate : MonoBehaviour
{
	private bool m_bActiveRotate = false;

	// Start is called before the first frame update
	public void EnableDisableEffect()
	{
		m_bActiveRotate = !m_bActiveRotate;
	}


	// Update is called once per frame
	void Update()
	{
		if (m_bActiveRotate)
		{
			transform.Rotate(Vector3.forward * 2);
		}
	}
}

