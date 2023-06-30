using System.Collections;
using UnityEngine;

namespace Snake
{
	public class AlphabetItem : Item
	{
		#region implemented abstract members of Item

		public override ItemKind Kind 
		{
			get 
			{
				return ItemKind.Alphabet;
			}
		}

		#endregion
	}
}