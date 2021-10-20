using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BKRacing
{
	public class CanvasController : MonoBehaviour
	{
		[SerializeField]
		private float fadeOutTime = 0.75f;

		public void StartGame()
		{
			FadeAll(1, 0);
			var buttons = GetComponentsInChildren<Button>();

			foreach (var button in buttons)
			{
				button.interactable = false;
			}

			Invoke(nameof(EnableGame), fadeOutTime);
		}

		public void RestartGame()
		{
			Invoke(nameof(ResetGame), fadeOutTime * 0.5f);
		}

		public void FadeAll(float from, float to)
		{
			var images = GetComponentsInChildren<Image>();

			foreach (var image in images)
			{
				StartCoroutine(Fade(image, fadeOutTime, from, to));
			}
		}

		private IEnumerator Fade(Image image, float time, float from, float to)
		{
			float t = 0;

			while (t < time)
			{
				var color = Color.white;
				color.a = Mathf.Lerp(from, to, t / time);
				image.color = color;
				t += Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
		}

		private void EnableGame()
		{
			Game.Instance.ReadyToStart = true;
			gameObject.SetActive(false);
		}

		private void ResetGame()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
