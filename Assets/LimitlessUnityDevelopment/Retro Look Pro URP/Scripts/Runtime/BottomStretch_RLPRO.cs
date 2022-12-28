using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BottomStretch_RLPRO : ScriptableRendererFeature
{
	BottomStretch_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new BottomStretch_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class BottomStretch_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int TimeV = Shader.PropertyToID("Time");
		static readonly int _NoiseBottomHeightV = Shader.PropertyToID("_NoiseBottomHeight");
		static readonly int frequencyV = Shader.PropertyToID("frequency");
		static readonly int amplitudeV = Shader.PropertyToID("amplitude");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		BottomStretch retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;

		private float T;

		public BottomStretch_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/BottomStretchEffect_RLPRO");
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
			retroEffect = stack.GetComponent<BottomStretch>();
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
			if (retroEffect.distort.value)
			{
				shaderPass = retroEffect.distortRandomly.value ? 1 : 0;
			}
			else
				shaderPass = 2;

			T += Time.deltaTime;
			RetroEffectMaterial.SetFloat(TimeV, T);
			RetroEffectMaterial.SetFloat(_NoiseBottomHeightV, retroEffect.height.value);
			RetroEffectMaterial.SetFloat(frequencyV, retroEffect.frequency.value);
			RetroEffectMaterial.SetFloat(amplitudeV, retroEffect.amplitude.value);

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


