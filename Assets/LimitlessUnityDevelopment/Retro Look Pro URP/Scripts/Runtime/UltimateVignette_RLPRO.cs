using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RetroLookPro.Enums;

public class UltimateVignette_RLPRO : ScriptableRendererFeature
{
	UltimateVignette_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new UltimateVignette_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class UltimateVignette_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int _Params = Shader.PropertyToID("_Params");
		static readonly int _InnerColor = Shader.PropertyToID("_InnerColor");
		static readonly int _Center = Shader.PropertyToID("_Center");
		static readonly int _Params1 = Shader.PropertyToID("_Params1");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		UltimateVignette retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;

		public UltimateVignette_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/UltimateVignetteEffect_RLPRO");
			if (shader == null)
			{
				Debug.LogError("Shader not found.");
				return;
			}
			RetroEffectMaterial = CoreUtils.CreateEngineMaterial(shader);
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (RetroEffectMaterial == null)
			{
				Debug.LogError("Material not created.");
				return;
			}
			if (!renderingData.cameraData.postProcessEnabled) return;

			var stack = VolumeManager.instance.stack;
			retroEffect = stack.GetComponent<UltimateVignette>();
			if (retroEffect == null) { return; }
			if (!retroEffect.IsActive()) { return; }

			var cmd = CommandBufferPool.Get(k_RenderTag);
			Render(cmd, ref renderingData);
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
		}

		public void Setup(in RenderTargetIdentifier currentTarget)
		{
			this.currentTarget = currentTarget;
		}

		void Render(CommandBuffer cmd, ref RenderingData renderingData)
		{
			ref var cameraData = ref renderingData.cameraData;
			var source = currentTarget;
			int destination = TempTargetId;

			cmd.SetGlobalTexture(MainTexId, source);
			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);
			RetroEffectMaterial.DisableKeyword("VIGNETTE_CIRCLE");
			RetroEffectMaterial.DisableKeyword("VIGNETTE_ROUNDEDCORNERS");
			switch (retroEffect.vignetteShape.value)
			{
				case VignetteShape.circle:
					RetroEffectMaterial.EnableKeyword("VIGNETTE_CIRCLE");
					break;
				case VignetteShape.roundedCorners:
					RetroEffectMaterial.EnableKeyword("VIGNETTE_ROUNDEDCORNERS");
					break;
			}
			RetroEffectMaterial.SetVector(_Params, new Vector4(retroEffect.edgeSoftness.value * 0.01f, retroEffect.vignetteAmount.value * 0.02f, retroEffect.innerColorAlpha.value * 0.01f, retroEffect.edgeBlend.value * 0.01f));
			RetroEffectMaterial.SetColor(_InnerColor, retroEffect.innerColor.value);
			RetroEffectMaterial.SetVector(_Center, retroEffect.center.value);
			RetroEffectMaterial.SetVector(_Params1, new Vector2(retroEffect.vignetteFineTune.value, 0.8f));
			cmd.Blit(destination, source, RetroEffectMaterial, 0);
		}
	}

}


