using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using GameUtility.Base;

[System.Serializable]
public class Sounds
{
	public string name;
	public AudioClip clip;

	[Range(0f, 1f)]
	public float volume;
	[Range(.1f, 3f)]
	public float pitch;
	[HideInInspector]
	public AudioSource source;

	public bool loop;
}


public class AudioManager : MonoBehaviour
{
	public Sounds[] m_Sounds;
	public Sounds m_gamebackgroundSound;

	private static AudioManager m_Instance;

	public static AudioManager Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = FindObjectOfType<AudioManager>();
			}

			return m_Instance;
		}
			
	}

	private void Awake()
	{
		if (m_Instance == null)
		{
			m_Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
			return;
		}
	}

	private void OnEnable()
	{
		for (int i = 0; i < m_Sounds.Length; i++)
		{
			if (m_Sounds[i].name == null || m_Sounds[i].name == "") { m_Sounds[i].name = m_Sounds[i].clip.name; }
			m_Sounds[i].source = gameObject.AddComponent<AudioSource>();
			m_Sounds[i].source.clip = m_Sounds[i].clip;
			m_Sounds[i].source.volume = m_Sounds[i].volume;
			m_Sounds[i].source.pitch = m_Sounds[i].pitch;
		}

		PlayGameBackGroundSound();
	}

	void PlayGameBackGroundSound()
	{
		m_gamebackgroundSound.source = gameObject.AddComponent<AudioSource>();
		m_gamebackgroundSound.source.clip = m_gamebackgroundSound.clip;
		m_gamebackgroundSound.source.volume = m_gamebackgroundSound.volume;
		m_gamebackgroundSound.source.pitch = m_gamebackgroundSound.pitch;
		m_gamebackgroundSound.source.loop = m_gamebackgroundSound.loop;
		m_gamebackgroundSound.source.Play();
	}

	public void PlaySound(string name)
	{
		Sounds s = Array.Find(m_Sounds, m_Sounds => m_Sounds.name == name);
		s.source.Play();
	}

}
