using System;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[ExecuteAlways]
	public abstract class MaterialComponent : MonoBehaviour
	{
		[SerializeField][HideInInspector]
		private Shader _internalShader;
		private Shader internalShader => _internalShader ? _internalShader : _internalShader = GetInternalShader();
		protected virtual Shader GetInternalShader() {
			return Shader.Find(@"UI/Default");
		}
		
		private Material _material;
		public Material internalMaterial => _material;
		
		protected bool _hasModifiedProps = false;
		public bool hasModifiedProps => _hasModifiedProps;
		public void SetModified() {
			_hasModifiedProps = true;
		}
		
		protected virtual void Awake() {
			if (!_material) {
				_material = new Material(internalShader) { name = $"Internal_{internalShader.name}" };
				UpdateMaterial();
			}
		}
		protected virtual void UpdateMaterial() { }
		
		// Setters
		protected void SetColorProp(int nameID, ref Color prop, ref Color mProp, Color value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetColor(nameID, value);
		}
		protected void SetColor(int nameID, Color value) {
			if (_material) {
				_material.SetColor(nameID, value);
				SetModified();
			}
		}
		
		protected void SetVectorProp(int nameID, ref Vector4 prop, ref Vector4 mProp, Vector4 value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetVector(nameID, value);
		}
		protected void SetVector(int nameID, Vector4 value) {
			if (_material) {
				_material.SetVector(nameID, value);
				SetModified();
			}
		}
		
		protected void SetFloatProp(int nameID, ref float prop, ref float mProp, float value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetFloat(nameID, value);
		}
		protected void SetFloat(int nameID, float value) {
			if (_material) {
				_material.SetFloat(nameID, value);
				SetModified();
			}
		}
		
		protected void SetTextureProp(int nameID, ref Texture prop, ref Texture mProp, Texture value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetTexture(nameID, value);
		}
		protected void SetTexture(int nameID, Texture value) {
			if (_material) {
				_material.SetTexture(nameID, value);
				SetModified();
			}
		}
		
		protected void SetTextureOffsetProp(int nameID, ref Vector2 prop, ref Vector2 mProp, Vector2 value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetTextureOffset(nameID, value);
		}
		protected void SetTextureOffset(int nameID, Vector2 value) {
			if (_material) {
				_material.SetTextureOffset(nameID, value);
				SetModified();
			}
		}
		
		protected void SetTextureScaleProp(int nameID, ref Vector2 prop, ref Vector2 mProp, Vector2 value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetTextureScale(nameID, value);
		}
		protected void SetTextureScale(int nameID, Vector2 value) {
			if (_material) {
				_material.SetTextureScale(nameID, value);
				SetModified();
			}
		}
		
		protected void SetIntProp(int nameID, ref int prop, ref int mProp, int value) {
			mProp = value;
			if (prop == value) return;
			prop = value;
			SetInt(nameID, value);
		}
		protected void SetInt(int nameID, int value) {
			if (_material) {
				_material.SetInt(nameID, value);
				SetModified();
			}
		}
		
		protected void SetKeywordEnumProp<TEnum>(int nameID, ref TEnum prop, ref TEnum mProp, TEnum value, string[] keywords) where TEnum : Enum {
			if (Equals(prop, value)) return;
			var intValue = Convert.ToInt32(value);
			if (intValue < 0 || intValue >= keywords.Length) {
				Debug.LogError($"Keywords array has no index {intValue}");
				return;
			}
			var intPrev = Convert.ToInt32(prop);
			prop = mProp = value;

			SetKeywordEnum(nameID, intValue, keywords[intPrev], keywords[intValue]);
		}
		protected void SetKeywordEnum(int nameID, int value, string disableKeyword, string enableKeyword) {
			if (_material) {
				_material.SetInt(nameID, value);
				_material.DisableKeyword(disableKeyword);
				_material.EnableKeyword(enableKeyword);
				SetModified();
			}
		}
		
		protected virtual void OnDestroy() {
			if (internalMaterial) {
				DestroyImmediate(internalMaterial);
			}
		}
		
		protected virtual void UpdateSerialized() { }
		
	#if UNITY_EDITOR
		protected virtual void OnValidate() {
			UpdateSerialized();
		}
	#endif
	}
}
