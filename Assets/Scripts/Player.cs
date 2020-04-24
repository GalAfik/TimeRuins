using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class Player : MonoBehaviour
	{
		[Serializable]
		public class ConfigurationData
		{
		}
		public ConfigurationData Conf = new ConfigurationData();
		public SpriteRenderer SpriteRenderer { get; set; }
		public Animator Animator { get; set; }
		public ParticleSystem ParticleSystem { get; set; }

private void Awake()
		{
			// Get references to the reference objects
			Animator = GetComponent<Animator>();
			SpriteRenderer = GetComponent<SpriteRenderer>();
			ParticleSystem = GetComponentInChildren<ParticleSystem>();

			// Disable the particle system when the game first starts
			ParticleSystem?.Pause();
		}
	}
}
