using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RetroLookPro.Enums;
public class TVEffect_RLPRO : ScriptableRendererFeature
{
	TVEffect_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new TVEffect_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class TVEffect_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int fade = Shader.PropertyToID("fade");
		static readonly int scale = Shader.PropertyToID("scale");
		static readonly int hardScan = Shader.PropertyToID("hardScan");
		static readonly int hardPix = Shader.PropertyToID("hardPix");
		static readonly int resScale = Shader.PropertyToID("resScale");
		static readonly int maskDark = Shader.PropertyToID("maskDark");
		static readonly int maskLight = Shader.PropertyToID("maskLight");
		static readonly int warp = Shader.PropertyToID("warp");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		TVEffect retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;
		private float T;
		public TVEffect_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/TV_RLPRO");
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
			retroEffect = stack.GetComponent<TVEffect>();
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


			RetroEffectMaterial.SetFloat(fade, retroEffect.fade.value);
			RetroEffectMaterial.SetFloat(scale, retroEffect.scale.value);
			RetroEffectMaterial.SetFloat(hardScan, retroEffect.hardScan.value);
			RetroEffectMaterial.SetFloat(hardPix, retroEffect.hardPix.value);
			RetroEffectMaterial.SetFloat(resScale, retroEffect.resScale.value);
			RetroEffectMaterial.SetFloat(maskDark, retroEffect.maskDark.value);
			RetroEffectMaterial.SetFloat(maskLight, retroEffect.maskLight.value);
			RetroEffectMaterial.SetVector(warp, retroEffect.warp.value);

			cmd.Blit(destination, source, RetroEffectMaterial, retroEffect.warpMode == WarpMode.SimpleWarp ? 0 : 1);
		}
	}

}