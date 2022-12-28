using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OldFilm_RLPRO : ScriptableRendererFeature
{
	OldFilm_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new OldFilm_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class OldFilm_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int TV = Shader.PropertyToID("T");
		static readonly int FPSV = Shader.PropertyToID("FPS");
		static readonly int ContrastV = Shader.PropertyToID("Contrast");
		static readonly int BurnV = Shader.PropertyToID("Burn");
		static readonly int SceneCutV = Shader.PropertyToID("SceneCut");
		static readonly int FadeV = Shader.PropertyToID("Fade");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		OldFilm retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;
		private float T;

		public OldFilm_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/OldFilmEffect_RLPRO");
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
			retroEffect = stack.GetComponent<OldFilm>();
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

			int shaderPass = 0;
			T += Time.deltaTime;
			if (T > 100) T = 0;
			RetroEffectMaterial.SetFloat(TV, T);
			RetroEffectMaterial.SetFloat(FPSV, retroEffect.fps.value);
			RetroEffectMaterial.SetFloat(ContrastV, retroEffect.contrast.value);
			RetroEffectMaterial.SetFloat(BurnV, retroEffect.burn.value);
			RetroEffectMaterial.SetFloat(SceneCutV, retroEffect.sceneCut.value);
			RetroEffectMaterial.SetFloat(FadeV, retroEffect.fade.value);
			cmd.SetGlobalTexture(MainTexId, source);
			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);
			cmd.Blit(source, destination);
			cmd.Blit(destination, source, RetroEffectMaterial, shaderPass);
		}
	}

}


