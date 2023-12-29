using Nexweron.Common.Attributes;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[AddComponentMenu("Nexweron/FragFilter/FF Filter HSBC")]
	public class FFFilterHSBC : FFComponent
	{
		private readonly int _baseColorID = Shader.PropertyToID("_BaseColor");
		private readonly int _tintColorID = Shader.PropertyToID("_TintColor");
		private readonly int _filterModeID = Shader.PropertyToID("_FilterMode");
		private readonly string[] _filterModeKeywords = {"_FILTERMODE_HS", "_FILTERMODE_BC", "_FILTERMODE_HSBC"};
		private readonly int _hueID = Shader.PropertyToID("_Hue");
		private readonly int _saturationID = Shader.PropertyToID("_Saturation");
		private readonly int _brightnessID = Shader.PropertyToID("_Brightness");
		private readonly int _contrastID = Shader.PropertyToID("_Contrast");
		
		[SerializeField] Color m_baseColor = Color.white;
		private Color _baseColor = Color.white;
		public Color baseColor {
			get => _baseColor;
			set => SetColorProp(_baseColorID, ref _baseColor, ref m_baseColor, value);
		}

		[Tooltip("Add color by alpha value")]
		[SerializeField] Color m_tintColor = new Color(1,1,1,0);
		private Color _tintColor = new Color(1,1,1,0);
		public Color tintColor {
			get => _tintColor;
			set => SetColorProp(_tintColorID, ref _tintColor, ref m_tintColor, value);
		}
		
		public enum FilterMode { HS, BC, HSBC }
		[SerializeField] FilterMode m_filterMode = FilterMode.HS;
		private FilterMode _filterMode = FilterMode.HS;
		public FilterMode filterMode {
			get => _filterMode;
			set => SetKeywordEnumProp(_filterModeID, ref _filterMode, ref m_filterMode, value, _filterModeKeywords);
		}
		public int filterModeInt {
			get => (int)filterMode;
			set => filterMode = (FilterMode)value;
		}
		
		[Range(0, 360)][InspectorShowIf("m_filterMode", FilterMode.HS, FilterMode.HSBC)]
		[SerializeField] int m_hue = 0;
		private int _hue = 0;
		public float hue {
			get => _hue;
			set => SetIntProp(_hueID, ref _hue, ref m_hue, (int)Mathf.Clamp(value, 0, 360));
		}
		
		[Range(-1, 1)][InspectorShowIf("m_filterMode", FilterMode.HS, FilterMode.HSBC)]
		[SerializeField] float m_saturation = 0.0f;
		private float _saturation = 0.0f;
		public float saturation {
			get => _saturation;
			set => SetFloatProp(_saturationID, ref _saturation, ref m_saturation, Mathf.Clamp(value, -1, 1));
		}
		
		[Range(-1, 1)][InspectorShowIf("m_filterMode", FilterMode.BC, FilterMode.HSBC)]
		[SerializeField] float m_brightness = 0.0f;
		private float _brightness = 0.0f;
		public float brightness {
			get => _brightness;
			set => SetFloatProp(_brightnessID, ref _brightness, ref m_brightness, Mathf.Clamp(value, -1, 1));
		}
		
		[Range(-1, 1)][InspectorShowIf("m_filterMode", FilterMode.BC, FilterMode.HSBC)]
		[SerializeField] float m_contrast = 0.0f;
		private float _contrast = 0.0f;
		public float contrast {
			get => _contrast;
			set => SetFloatProp(_contrastID, ref _contrast, ref m_contrast, Mathf.Clamp(value, -1, 1));
		}
		
		protected override Shader  GetInternalShader() {
			return Shader.Find(@"Nexweron/Builtin/Filter/UnlitBlendOff_FilterHSBC");
		}
		
		protected override void UpdateMaterial() {
			SetColor(_baseColorID, m_baseColor);
			SetColor(_tintColorID, m_tintColor);
			SetKeywordEnum(_filterModeID, (int)m_filterMode, _filterModeKeywords[0], _filterModeKeywords[(int)m_filterMode]);
			SetInt(_hueID, m_hue);
			SetFloat(_saturationID, m_saturation);
			SetFloat(_brightnessID, m_brightness);
			SetFloat(_contrastID, m_contrast);
		}

		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			baseColor = m_baseColor;
			tintColor = m_tintColor;
			filterMode = m_filterMode;
			hue = m_hue;
			saturation = m_saturation;
			brightness = m_brightness;
			contrast = m_contrast;
		}
	}
}
