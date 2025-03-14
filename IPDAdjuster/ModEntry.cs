using UnityEngine;
using ResoniteModLoader;
using UnityFrooxEngineRunner;

namespace IPDAdjuster;

public class ModEntry : ResoniteMod 
{
	internal const string VERSION_CONSTANT = "1.0.0";
	public override string Name => "IPD Adjuster";
	public override string Author => "ErrorJan";
	public override string Version => VERSION_CONSTANT;
	public override string Link => "https://github.com/ErrorJan/ResoniteMod-IPDAdjuster/";
	private ModConfiguration? m_rmlConfig = null;
	private FrooxEngineRunner? m_fer;
	private Transform? m_vrCameraRoot;
	
	[AutoRegisterConfigKey]
	public static readonly 
		ModConfigurationKey<bool> modEnabled = 
			new( "modEnabled", 
				 "Should the mod be enabled?", 
				 () => true, 
				 false
			);
	
	[AutoRegisterConfigKey]
	public static readonly 
		ModConfigurationKey<float> ipdOverrideMultiplier = 
			new( "ipdOverrideMultiplier", 
				"What should the IPD be multiplied with?", 
				() => 1,
				false,
				( v ) => v > 0.001f | v < 3f
			);

	public override void OnEngineInit() 
	{
		m_rmlConfig = GetConfiguration();
		m_fer = Object.FindObjectOfType<FrooxEngineRunner>();
		m_vrCameraRoot = m_fer.VROutput.CameraRoot;

		ipdOverrideMultiplier.OnChanged += (o) => UpdateIPD();
		modEnabled.OnChanged += (o) => UpdateIPD();
		UpdateIPD();
	}

	public void UpdateIPD() 
	{
		if ( m_vrCameraRoot == null )
			return;
		
		if ( m_rmlConfig?.GetValue( modEnabled ) ?? false ) 
		{
			float ipdMul = m_rmlConfig?.GetValue( ipdOverrideMultiplier ) ?? 1.0f;
			m_vrCameraRoot.localScale = Vector3.one * ipdMul;
			return;
		}

		m_vrCameraRoot.localScale = Vector3.one;
	}
}
