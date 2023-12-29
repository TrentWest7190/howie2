using System;
using UnityEngine;

namespace Nexweron.TargetRender
{
	[Serializable]
	public class SubTarget
	{
		protected Texture _defaultTexture;
		public Texture defaultTexture => _defaultTexture;
		
		protected bool _isCachedDefaultTexture = false;
		public bool isCachedDefaultTexture => _isCachedDefaultTexture;

		public virtual bool isValidTarget => false;

		protected bool _isModified = false;
		protected void SetModified() {
			if(_valueTexture) _isModified = true;
		}
		
		private bool _isSelected = false;
		protected bool isSelected => _isSelected;
		
		public virtual void SetSelected(bool value) {
			if (_isSelected == value) return;
			if (!value) RevertDefaultTexture(true);
			_isSelected = value;
			SetModified();
		}
		
		protected Texture _valueTexture;
		protected int _valueTextureHashCode = int.MinValue;
		
		public virtual void SetValueTexture(Texture texture) {
			var hashCode = texture && texture != _defaultTexture ? texture.GetHashCode() : int.MinValue;
			if (_valueTextureHashCode != hashCode) {
				_valueTextureHashCode = hashCode;
				_valueTexture = texture;
				_isModified = true;
			}
		}
		public void UpdateValueTexture() {
			if (_isSelected && _isModified && isValidTarget) {
				if (_valueTexture) {
					SetTargetTexture(_valueTexture);
				}
				else {
					RevertDefaultTexture();
				}
			}
			_isModified = false;
		}
		
		// return isCached
		public bool ReCacheDefaultTexture() {
			if (!isValidTarget) return false;
			CacheDefaultTexture();
			return true;
		}
		
		protected virtual Texture GetDefaultTexture() { return null; }
		protected virtual void CacheDefaultTexture() { }
		protected virtual void RevertDefaultTexture(bool clearCache = false) { }
		public virtual Texture GetTargetTexture() { return null; }
		protected virtual void SetTargetTexture(Texture texture) { }
		
		public virtual void UpdateSerialized() { }
	}

	[Serializable]
	public class ApiOnlyTarget : SubTarget
	{ }
}
