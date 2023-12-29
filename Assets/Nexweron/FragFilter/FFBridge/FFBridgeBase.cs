using Nexweron.Common.Attributes;
using UnityEngine;

namespace Nexweron.FragFilter
{
	public class FFBridgeBase : MonoBehaviour, IFFBridge
	{
		[SerializeField][InspectorRename("FF Controller")]
		private FFController m_ffController;
		protected FFController _ffController;
		
		public FFController ffController {
			get => _ffController;
			set {
				if (_ffController != value) {
					_ffController = value ? value.TrySetBridge(this) : _ffController.SetBridgeNull(this);
					m_ffController = _ffController;
					UpdateFFController();
				}
				// bridge can be lost on deserialization
				else if (_ffController) _ffController.TrySetBridge(this);
			}
		}
		protected virtual void UpdateFFController() {
			_isModified = true;
		}

		public virtual bool isValid => ffController != null;
		
		protected bool _isModified = false;
		protected bool CheckModified(bool checker) {
			return _isModified = _isModified || checker;
		}

		protected virtual void Start() {
			UpdateSerialized();
		}

		protected virtual void OnDestroy() {
			ffController = null;
		}
		
		protected virtual void UpdateSerialized() {
			ffController = m_ffController;
		}
		
	#if UNITY_EDITOR
		protected virtual void OnValidate() {
			UpdateSerialized();
		}
	#endif
	}
}