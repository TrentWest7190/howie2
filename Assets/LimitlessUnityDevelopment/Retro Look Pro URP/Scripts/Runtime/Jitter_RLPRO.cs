using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Jitter_RLPRO : ScriptableRendererFeature
{
	Jitter_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new Jitter_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class Jitter_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int screenLinesNumV = Shader.PropertyToID("screenLinesNum");
		static readonly int time_V = Shader.PropertyToID("time_");
		static readonly int twitchHFreqV = Shader.PropertyToID("twitchHFreq");
		static readonly int twitchVFreqV = Shader.PropertyToID("twitchVFreq");
		static readonly int jitterHAmountV = Shader.PropertyToID("jitterHAmount");
		static readonly int jitterVAmountV = Shader.PropertyToID("jitterVAmount");
		static readonly int jitterVSpeedV = Shader.PropertyToID("jitterVSpeed");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		Jitter retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;
		private float _time;

		public Jitter_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/JitterEffect_RLPRO");
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
			retroEffect = stack.GetComponent<Jitter>();
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
			if (retroEffect.unscaledTime.value) { _time = Time.unscaledTime; }
			else _time = Time.time;

			RetroEffectMaterial.SetFloat(screenLinesNumV, retroEffect.stretchResolution.value);
			RetroEffectMaterial.SetFloat(time_V, _time);
			ParamSwitch(RetroEffectMaterial, retroEffect.twitchHorizontal.value, "VHS_TWITCH_H_ON");
			RetroEffectMaterial.SetFloat(twitchHFreqV, retroEffect.horizontalFreq.value);
			ParamSwitch(RetroEffectMaterial, retroEffect.twitchVertical.value, "VHS_TWITCH_V_ON");
			RetroEffectMaterial.SetFloat(twitchVFreqV, retroEffect.verticalFreq.value);
			ParamSwitch(RetroEffectMaterial, retroEffect.stretch.value, "VHS_STRETCH_ON");

			ParamSwitch(RetroEffectMaterial, retroEffect.jitterHorizontal.value, "VHS_JITTER_H_ON");
			RetroEffectMaterial.SetFloat(jitterHAmountV, retroEffect.jitterHorizontalAmount.value);

			ParamSwitch(RetroEffectMaterial, retroEffect.jitterVertical.value, "VHS_JITTER_V_ON");
			RetroEffectMaterial.SetFloat(jitterVAmountV, retroEffect.jitterVerticalAmount.value);
			RetroEffectMaterial.SetFloat(jitterVSpeedV, retroEffect.jitterVerticalSpeed.value);

			cmd.SetGlobalTexture(MainTexId, source);

			cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


			cmd.Blit(source, destination);
			cmd.Blit(destination, source, RetroEffectMaterial, shaderPass);
		}
		private void ParamSwitch(Material mat, bool paramValue, string paramName)
		{
			if (paramValue) mat.EnableKeyword(paramName);
			else mat.DisableKeyword(paramName);
		}
	}

}


