using Nexweron.Common.Attributes;
using UnityEngine;

namespace Nexweron.FragFilter
{
	[AddComponentMenu("Nexweron/FragFilter/FF ChromaKey Bg")]
	public class FFChromaKeyBg : FFComponent
	{
		private readonly int _keyColorID = Shader.PropertyToID("_KeyColor");
		private readonly int _bgModeID = Shader.PropertyToID("_BgMode");
		private readonly string[] _bgModeKeywords = {"_BGMODE_COLOR", "_BGMODE_TEXTURE"};
		
		private readonly int _bgColorID = Shader.PropertyToID("_BgColor");
		private readonly int _bgTexID = Shader.PropertyToID("_BgTex");
		
		private readonly int _dChromaID = Shader.PropertyToID("_DChroma");
		private readonly int _dChromaTID = Shader.PropertyToID("_DChromaT");
		private readonly int _chromaID = Shader.PropertyToID("_Chroma");
		private readonly int _lumaID = Shader.PropertyToID("_Luma");
		private readonly int _saturationID = Shader.PropertyToID("_Saturation");
		private readonly int _alphaID = Shader.PropertyToID("_Alpha");

		[SerializeField] Color m_keyColor = Color.green;
		private Color _keyColor = Color.green;
		public Color keyColor {
			get => _keyColor;
			set => SetColorProp(_keyColorID, ref _keyColor, ref m_keyColor, value);
		}
		
		public enum BgMode { Color, Texture }
		[SerializeField] BgMode m_bgMode = BgMode.Color;
		private BgMode _bgMode = BgMode.Color;
		public BgMode bgMode {
			get => _bgMode;
			set => SetKeywordEnumProp(_bgModeID, ref _bgMode, ref m_bgMode, value, _bgModeKeywords);
		}
		public int bgModeInt {
			get => (int)bgMode;
			set => bgMode = (BgMode)value;
		}
		
		[InspectorShowIf("m_bgMode", BgMode.Color)][InspectorIndent(1)]
		[SerializeField] Color m_bgColor = Color.white;
		private Color _bgColor = Color.white;
		public Color bgColor {
			get => _bgColor;
			set => SetColorProp(_bgColorID, ref _bgColor, ref m_bgColor, value);
		}
		
		[InspectorShowIf("m_bgMode", BgMode.Texture)][InspectorIndent(1)]
		[SerializeField] Texture m_bgTex;
		private Texture _bgTex;
		public Texture bgTex {
			get => _bgTex;
			set => SetTextureProp(_bgTexID, ref _bgTex, ref m_bgTex, value);
		}
		[InspectorShowIf("m_bgMode", BgMode.Texture)][InspectorRename("Tiling")][InspectorIndent(1)]
		[SerializeField] Vector2 m_bgTexScale = Vector2.one;
		private Vector2 _bgTexScale = Vector2.one;
		public Vector2 bgTexScale {
			get => _bgTexScale;
			set => SetTextureScaleProp(_bgTexID, ref _bgTexScale, ref m_bgTexScale, value);
		}
		[InspectorShowIf("m_bgMode", BgMode.Texture)][InspectorRename("Offset")][InspectorIndent(1)]
		[SerializeField] Vector2 m_bgTexOffset = Vector2.zero;
		private Vector2 _bgTexOffset = Vector2.zero;
		public Vector2 bgTexOffset {
			get => _bgTexOffset;
			set => SetTextureOffsetProp(_bgTexID, ref _bgTexOffset, ref m_bgTexOffset, value);
		}
		
		[Range(0, 1)]
		[SerializeField] float m_dChroma = 0.5f;
		private float _dChroma = 0.5f;
		public float dChroma {
			get => _dChroma;
			set => SetFloatProp(_dChromaID, ref _dChroma, ref m_dChroma, Mathf.Clamp(value, 0, 1));
		}
		
		[Range(0, 1)]
		[SerializeField] float m_dChromaT = 0.05f;
		private float _dChromaT = 0.05f;
		public float dChromaT {
			get => _dChromaT;
			set => SetFloatProp(_dChromaTID, ref _dChromaT, ref m_dChromaT, Mathf.Clamp(value, 0, 1));
		}
		
		[Range(0, 1)][Tooltip("Main (0.0) → Bg (1.0)")]
		[SerializeField] float m_chroma = 0.5f;
		private float _chroma = 0.5f;
		public float chroma {
			get => _chroma;
			set => SetFloatProp(_chromaID, ref _chroma, ref m_chroma, Mathf.Clamp(value, 0, 1));
        }
		
		[Range(0, 1)][Tooltip("Main (0.0) → Bg (1.0)")]
		[SerializeField] float m_luma = 0.5f;
		private float _luma = 0.5f;
		public float luma {
			get => _luma;
			set => SetFloatProp(_lumaID, ref _luma, ref m_luma, Mathf.Clamp(value, 0, 1));
		}
		
		[Range(0, 1)][Tooltip("(0.0) → Chroma (1.0)")]
		[SerializeField] float m_saturation = 1.0f;
		private float _saturation = 1.0f;
		public float saturation {
			get => _saturation;
			set => SetFloatProp(_saturationID, ref _saturation, ref m_saturation, Mathf.Clamp(value, 0, 1));
		}
		
		[Range(0, 1)][Tooltip("Chroma (0.0) → Bg (1.0)")]
		[SerializeField] float m_alpha = 1.0f;
		private float _alpha = 1.0f;
		public float alpha {
			get => _alpha;
			set => SetFloatProp(_alphaID, ref _alpha, ref m_alpha, Mathf.Clamp(value, 0, 1));
		}
		
		protected override Shader GetInternalShader() {
			return Shader.Find(@"Nexweron/Builtin/ChromaKey/UnlitBlendOff_ChromaKey_Bg");
		}
		
		protected override void UpdateMaterial() {
			SetColor(_keyColorID, m_keyColor);
			SetKeywordEnum(_bgModeID, (int)m_bgMode, _bgModeKeywords[0], _bgModeKeywords[(int)m_bgMode]);
			SetColor(_bgColorID, m_bgColor);
			SetTexture(_bgTexID, m_bgTex);
			SetTextureOffset(_bgTexID, m_bgTexOffset);
			SetTextureScale(_bgTexID, m_bgTexScale);
			SetFloat(_dChromaID, m_dChroma);
			SetFloat(_dChromaTID, m_dChromaT);
			SetFloat(_chromaID, m_chroma);
			SetFloat(_lumaID, m_luma);
			SetFloat(_saturationID, m_saturation);
			SetFloat(_alphaID, m_alpha);
		}

		protected override void UpdateSerialized() {
			base.UpdateSerialized();
			keyColor = m_keyColor;
			bgMode = m_bgMode;
			bgColor = m_bgColor;
			bgTex = m_bgTex;
			bgTexOffset = m_bgTexOffset;
			bgTexScale = m_bgTexScale;
			dChroma = m_dChroma;
			dChromaT = m_dChromaT;
			chroma = m_chroma;
			luma = m_luma;
			saturation = m_saturation;
			alpha = m_alpha;
		}
	}
}
