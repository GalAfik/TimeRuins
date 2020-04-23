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
			public float MoveSpeed; // How fast the player moves
			public float JumpForce; // How high the player can jump
			public float Gravity; // How fast the player falls
		}
		public ConfigurationData Conf = new ConfigurationData();
	}
}
