#if NETSTANDARD2_0
using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Renderite.Shared;
using Renderite.Unity;

using UnityEngine;

namespace IPDAdjuster;

[BepInPlugin(  ModInfo.ModRDName, ModInfo.ModName, ModInfo.ModVersion )]
public class BepInExMod : BaseUnityPlugin, IMessageDeserializer
{
	public static ManualLogSource ModLogger => _logger;
	
	private static ManualLogSource _logger = null!;
	private static string _currentResoniteQueueName = "";
	private static long _currentResoniteQueueCapacity = 0;
	private static ModMessagingManager? _messagingManager;
	
	private static RenderingManager? _renderer;
	private static Transform? _vrCameraRoot;
	private static bool _modEnabled = false;
	private static float _ipdOverrideMultiplier = 1.0f;
	private static BepInExMod? _instance;
	private static bool _initialized = false;

	private void Awake() 
	{
		_logger = base.Logger;
		_instance = this;
		
		Harmony harmony = new( ModInfo.ModRDName );
		harmony.CreateClassProcessor( typeof( QuickPatch ) ).Patch();
	}
	
	public static void InitializeLate() 
	{
		if ( _initialized || _instance == null )
			return;
		_initialized = true;
		
		_logger.LogInfo($"Plugin {ModInfo.ModRDName} is loaded! Initializing...");
		
		// Communicate with FrooxEngine for configs
		RenderingManager? manager = FindObjectOfType<RenderingManager>();
		if ( manager == null ) 
		{
			_logger.LogError( "Could not find RenderingManager! Initializing failed!" );
			return;
		} 
		
		object?[] args = { null, null };		
		AccessTools.Method( typeof( RenderingManager ), "GetConnectionParameters").Invoke( manager, args );
		if (args[0] == null || args[1] == null) 
		{
			_logger.LogError( "Could not find queuename or capacity! Initializing failed!" );
			return;
		}
		
		_renderer = manager;
		_vrCameraRoot = _renderer.VROutput.CameraRoot;
		
		_currentResoniteQueueName = (string)args[0]!;
		_currentResoniteQueueCapacity = (long)args[1]!;

		_messagingManager = new( PackerMemoryPool.Instance );
		_messagingManager.Connect( _currentResoniteQueueName + ModInfo.ModRDName, false, _currentResoniteQueueCapacity );
		_messagingManager.OnReceivingException += MessageException;
		_messagingManager.RegisterMessageHandler( _instance );
		_messagingManager.StartReceiving();
		
		_logger.LogInfo("Finished initializing!");
	}

	public void Deserialize( string messageID, MemoryUnpacker unpacker ) 
	{
		switch ( messageID ) 
		{
			case ModInfo.ModConfigKeys.ModEnabled:
				_modEnabled = unpacker.Read<bool>();
				_logger.LogInfo( $"ModEnabled: {_modEnabled}" );
				UpdateIPD();
				break;
			case ModInfo.ModConfigKeys.ipdOverrideMultiplier:
				_ipdOverrideMultiplier = unpacker.Read<float>();
				_logger.LogInfo( $"ipdOverrideMultiplier: {_ipdOverrideMultiplier}" );
				UpdateIPD();
				break;
			default:
				_logger.LogWarning( $"Unknown messageID: {messageID}" );
				break;
		}
	}
	
	public static void UpdateIPD() 
	{
		if ( _vrCameraRoot == null )
			return;
		
		if ( _modEnabled ) 
		{
			float ipdMul = _ipdOverrideMultiplier;
			_vrCameraRoot.localScale = Vector3.one * ipdMul;
			return;
		}

		_vrCameraRoot.localScale = Vector3.one;
	}

	private static void MessageException( Exception ex ) 
	{
		_logger.LogError( ex.Message );
	}
}

[HarmonyPatch( typeof( RenderingManager ), "InitializeHeadOutputs" )]
public static class QuickPatch 
{
	public static void Postfix() 
	{
		BepInExMod.InitializeLate();
	}
}
#endif
