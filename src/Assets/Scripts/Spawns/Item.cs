using System.Collections;
using UnityEngine;

namespace Snake
{
	public enum ItemKind
	{
		Number,
		Bonus,
		Reverse,
		Alphabet
	}

	[RequireComponent (typeof (SpriteRenderer))]
	[RequireComponent (typeof (BoxCollider2D))]
	public abstract class Item : MonoBehaviour
	{
		public abstract ItemKind Kind { get; }

		public virtual void Hide ()
		{
			gameObject.SetActive (false);

			transform.localPosition = Vector2.zero;
			transform.localRotation = Quaternion.identity;
		}
	}
}