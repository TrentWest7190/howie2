using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PictureCorrection_RLPRO : ScriptableRendererFeature
{
	PictureCorrection_RLPROPass RetroPass;
	public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


	public override void Create()
	{
		RetroPass = new PictureCorrection_RLPROPass(Event);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
		RetroPass.Setup(renderer.cameraColorTarget);
		renderer.EnqueuePass(RetroPass);
	}
	public class PictureCorrection_RLPROPass : ScriptableRenderPass
	{
		static readonly string k_RenderTag = "Renderr Glitch1 Effect";
		static readonly int MainTexId = Shader.PropertyToID("_InputTexture");
		static readonly int signalAdjustY = Shader.PropertyToID("signalAdjustY");
		static readonly int signalAdjustI = Shader.PropertyToID("signalAdjustI");
		static readonly int signalAdjustQ = Shader.PropertyToID("signalAdjustQ");
		static readonly int signalShiftY = Shader.PropertyToID("signalShiftY");
		static readonly int signalShiftI = Shader.PropertyToID("signalShiftI");
		static readonly int signalShiftQ = Shader.PropertyToID("signalShiftQ");
		static readonly int gammaCorection = Shader.PropertyToID("gammaCorection");
		static readonly int TempTargetId = Shader.PropertyToID("Glitch1rr");

		PictureCorrection retroEffect;
		Material RetroEffectMaterial;
		RenderTargetIdentifier currentTarget;

		public PictureCorrection_RLPROPass(RenderPassEvent evt)
		{
			renderPassEvent = evt;
			var shader = Shader.Find("Hidden/Shader/PictureCorrectionEffect_RLPRO");
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
			retroEffect = stack.GetComponent<PictureCorrection>();
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


			RetroEffectMaterial.SetFloat(signalAdjustY, retroEffect.signalAdjustY.value);
			RetroEffectMaterial.SetFloat(signalAdjustI, retroEffect.signalAdjustI.value);
			RetroEffectMaterial.SetFloat(signalAdjustQ, retroEffect.signalAdjustQ.value);
			RetroEffectMaterial.SetFloat(signalShiftY, retroEffect.signalShiftY.value);
			RetroEffectMaterial.SetFloat(signalShiftI, retroEffect.signalShiftI.value);
			RetroEffectMaterial.SetFloat(signalShiftQ, retroEffect.signalShiftQ.value);
			RetroEffectMaterial.SetFloat(gammaCorection, retroEffect.gammaCorection.value);

			cmd.Blit(destination, source, RetroEffectMaterial, 0);
		}
	}

}


