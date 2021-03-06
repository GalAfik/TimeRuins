﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TimeRuins
{
	public class GameController : MonoBehaviour
	{
		public DialogController DialogController;
		public Difficulty Difficulty { get; set; }

		public void ToggleEasyMode()
		{
			if (Difficulty == Difficulty.NORMAL) Difficulty = Difficulty.EASY;
			else Difficulty = Difficulty.NORMAL;
		}

		public void ToggleMusic()
		{
			// TODO : mute the audiosource
		}

		public void ToggleSFX()
		{
			// TODO : toggle SFX
		}
	}

	public enum Difficulty { NORMAL, EASY };
}
