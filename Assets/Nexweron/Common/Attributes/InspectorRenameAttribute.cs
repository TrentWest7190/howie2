using System;
using UnityEngine;

namespace Nexweron.Common.Attributes
{
	[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
	public class InspectorRenameAttribute : InspectorAttribute
	{
		/// <summary>
		///   <para>Name to display in the Inspector.</para>
		/// </summary>
		public readonly string displayName;

		/// <summary>
		///   <para>Specify a display name for an enum value.</para>
		/// </summary>
		/// <param name="displayName">The name to display.</param>
		public InspectorRenameAttribute(string displayName = null) : base() {
			this.displayName = displayName ?? "";
		}

	#if UNITY_EDITOR
		public override bool OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label) {
			label.text = displayName;
			return false;
		}
	#endif
	}
}
