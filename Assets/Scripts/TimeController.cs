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
		public Timer Timer; // The timer used for displaying how much time is left in the current time switch
		public float MaxTimelineSwitchTime; // How long the time switch lasts
		public float MaxTimelineSwitchTimeEasyMode; // How long the time switch lasts in easy mode
		public float ResetTime; // How long it takes the time switch power to recharge

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

			// Set the initial capacity of the tiemr
			Timer?.SetMaxTimer(TimelineSwitchTime);
			StartCoroutine(FadeTo(Timer, 0f, 0.01f));
		}

		private void Start()
		{
			// Set the initial reset timer
			ResetTimer = ResetTime;

			// Split all objects in the scene into their respective layers
			PastItems = FindGameObjectsInLayer("Past");
			RuinsItems = FindGameObjectsInLayer("Ruins");

			// Disable the past layer
			SetTimelineStatus(PastItems, false);
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
				Timer?.SetTimer(ResetTimer);
			}
		}

		IEnumerator SwitchTimeline()
		{
			// Drain the recharge timer
			ResetTimer = 0;

			// Set the player animation
			Player?.Animator?.SetTrigger("Action");
			if (Player.ParticleSystem != null)
			{
				Player.ParticleSystem.Clear();
				Player.ParticleSystem.Play();
			}

			// Enable the past objects
			SetTimelineStatus(PastItems, true);
			// Disable the ruins objects
			SetTimelineStatus(RuinsItems, false);


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

			yield return new WaitForSecondsRealtime(switchTime);
			Time.timeScale = 1f;

			// Switch back to the ruins
			SetTimelineStatus(RuinsItems, true);
			SetTimelineStatus(PastItems, false);
			IsPastActive = false;
		}

		void SetTimelineStatus(HashSet<GameObject> timelineObjects, bool status)
		{
			foreach (GameObject item in timelineObjects)
			{
				item.SetActive(status);
			}
		}

		void FadeLayer(HashSet<GameObject> timelineObjects, float alpha, float time)
		{
			foreach (GameObject item in timelineObjects)
			{
				StartCoroutine(FadeTo(item, alpha, time));
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

		IEnumerator FadeTo(GameObject obj, float aValue, float aTime)
		{
			float alpha = obj.GetComponent<Renderer>().material.color.a;
			for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
			{
				Color newColor = obj.GetComponent<Renderer>().material.color;
				newColor.a = Mathf.Lerp(alpha, aValue, t);
				obj.GetComponent<Renderer>().material.color = newColor;
				yield return new WaitForEndOfFrame();
			}
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
