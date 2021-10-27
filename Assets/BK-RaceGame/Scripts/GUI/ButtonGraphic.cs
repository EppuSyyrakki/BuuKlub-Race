using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BKRacing.GUI
{
	public class ButtonGraphic : MonoBehaviour
	{
		private enum Type
		{
			Play,
			Replay
		}

		[SerializeField]
		private Type buttonType;

		void Start()
		{
			var image = GetComponent<Image>();
			var button = GetComponent<Button>();
			var uiButton = buttonType == Type.Play ? Game.Instance.PlayButton : Game.Instance.ReplayButton;

			var ss = new SpriteState
			{
				disabledSprite = uiButton.hoverSprite,
				highlightedSprite = uiButton.hoverSprite,
				pressedSprite = uiButton.hoverSprite,
				selectedSprite = uiButton.hoverSprite
			};

			image.sprite = uiButton.normalSprite;
			button.targetGraphic = image;
			button.spriteState = ss;
		}
	}
}