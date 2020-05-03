using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

namespace TimeRuins
{
	public class TimeController : MonoBehaviour
	{
		public GameController GameController;
		public Player Player;
		public SpriteMask TimeMask;
		public Timer Timer; // The timer used for displaying how much time is left in the current time switch
		public float MaxTimelineSwitchTime; // How long the time switch lasts
		public float MaxTimelineSwitchTimeEasyMode; // How long the time switch lasts in easy mode
		public float ResetTime; // How long it takes the time switch power to recharge
		public float PauseForEffectTime; // How long the game should pause before and after initiating a time switch

		private float TimelineSwitchTime; // The applied version of the above variables
		HashSet<GameObject> PastItems;
		HashSet<GameObject> RuinsItems;
		private float ResetTimer;

		private bool IsPastActive = false;
		private float TimeInPast = 0; // The current amount of time spent in the past

		private void Awake()
		{
			// Set the amount of time the time switches last according to the set difficulty level
			// (players can turn on easy mode via the options menu)
			TimelineSwitchTime = GameController?.Difficulty == Difficulty.EASY ? MaxTimelineSwitchTimeEasyMode : MaxTimelineSwitchTime;

			// Fade out the timer to start off with
			StartCoroutine(FadeTo(Timer, 0f, 0f));
		}

		private void Start()
		{
			// Set the initial reset timer
			ResetTimer = ResetTime;

			// Split all objects in the scene into their respective layers
			PastItems = FindGameObjectsInLayer("Past");
			RuinsItems = FindGameObjectsInLayer("Ruins");

			// Disable the past layer
			SetColliders(PastItems, false);
		}

		// Update is called once per frame
		void Update()
		{
			// Handle switching timelines
			if (Input.GetButtonDown("Action") && !IsPastActive && ResetTimer == ResetTime) StartCoroutine(SwitchTimeline());

			// Set the timer to follow the current time in the past
			if (IsPastActive)
			{
				// Count time in the past
				TimeInPast += Time.unscaledDeltaTime;
				// Set the timer
				Timer?.SetMaxTimer(TimelineSwitchTime);
				Timer?.SetTimer(TimelineSwitchTime - TimeInPast);
			}
			else
			{
				// Recharge the timer
				if (ResetTimer < ResetTime) ResetTimer += Time.deltaTime;
				else
				{
					// Reset and fade out the timer UI
					ResetTimer = ResetTime;
					StartCoroutine(FadeTo(Timer, 0f, 0.2f));
				}

				// Reset both time in the past and the timer UI
				TimeInPast = 0;
				Timer?.SetMaxTimer(ResetTime);
				Timer?.SetTimer(ResetTimer);
			}
		}

		IEnumerator SwitchTimeline()
		{
			// Drain the recharge timer
			ResetTimer = 0;

			// Enable the past objects
			SetColliders(PastItems, true);
			SetColliders(RuinsItems, false);

			// Set the layer mask to active
			TimeMask?.gameObject.SetActive(true);

			
			// Check if the player is obstructed
			Collider2D hit = Physics2D.OverlapCircle( (Vector2)Player?.transform.position, 0.5f );

			// Start the timer
			float switchTime = (hit.CompareTag("Player")) ? TimelineSwitchTime : 0.5f;
			if (!hit.CompareTag("Player"))
			{
				// Freeze time if the player is overlapped by another object
				Time.timeScale = 0f;
			}
			else IsPastActive = true;

			// Fade in the timer UI
			StartCoroutine(FadeTo(Timer, 1f, 0.2f));

			// Pause time momentarily
			Time.timeScale = 0f;
			yield return new WaitForSecondsRealtime(PauseForEffectTime);
			Time.timeScale = 1f;

			// This is how long the time switch lasts
			yield return new WaitForSecondsRealtime(switchTime);

			// Pause time momentarily while time switches back
			Time.timeScale = 0f;

			// Switch back to the ruins
			SetColliders(RuinsItems, true);
			SetColliders(PastItems, false);

			// Hide the layer mask
			TimeMask?.gameObject.SetActive(false);

			// Pause time momentarily
			Time.timeScale = 0f;
			yield return new WaitForSecondsRealtime(PauseForEffectTime);
			Time.timeScale = 1f;

			// Set the past flag
			IsPastActive = false;
		}

		void SetColliders(HashSet<GameObject> timelineObjects, bool status)
		{
			foreach (GameObject item in timelineObjects)
			{
				// Set this item's collider status
				if (item.GetComponent<Collider2D>() != null) item.GetComponent<Collider2D>().enabled = status;
				// Set the status for this item's children
				foreach (Collider2D collider in item.GetComponentsInChildren<Collider2D>())
				{
					collider.enabled = status;
				}
			}
		}

		HashSet<GameObject> FindGameObjectsInLayer(String layerName)
		{
			// Initialize the empty result set
			HashSet<GameObject> objectsInLayer = new HashSet<GameObject>();
			// Return an array of all active and inactive GameObjects in the scene
			GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
			foreach (GameObject item in allGameObjects)
			{
				// If the object is in the provided layer, add it to the result set
				if (item.layer == LayerMask.NameToLayer(layerName)) objectsInLayer.Add(item);
			}
			// Return
			return objectsInLayer;
		}

		IEnumerator FadeTo(Timer timer, float aValue, float aTime)
		{
			float alpha = timer.GetComponentInChildren<Image>().material.color.a;
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
			{
				Color newColor = timer.GetComponentInChildren<Image>().material.color;
				newColor.a = Mathf.Lerp(alpha, aValue, t);
				timer.GetComponentInChildren<Image>().material.color = newColor;
				yield return new WaitForEndOfFrame();
			}
		}
	}
}
