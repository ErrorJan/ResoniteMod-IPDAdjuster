#if NET9_0
using System;
using Elements.Core;
using Renderite.Shared;
using ResoniteModLoader;
using FrooxEngine;
using HarmonyLib;

namespace IPDAdjuster;

public class ResoniteModLoaderMod : ResoniteMod, IMemoryPackerEntityPool
{
	public override string Name => ModInfo.ModName;
	public override string Author => ModInfo.ModAuthor;
	public override string Version => ModInfo.ModVersion;
	public override string Link => ModInfo.ModPage;
	
	private static ModConfiguration? _rmlConfig;
	private static string _currentResoniteQueueName = "";
	private static long _currentResoniteQueueCapacity = 0;
	private static ModMessagingManager? _messagingManager;

	public override void OnEngineInit() 
	{
		//Thread.Sleep( 30 * 1000 ); // For debugging
		//ManualHackyBreakPoint();
		Msg( "Mod loaded! Initializing..." );
	
		_rmlConfig = GetConfiguration();
		_rmlConfig?.Save( true );
	
		var renderiteMessagingHost = ( RenderiteMessagingHost? )AccessTools.Field( typeof( RenderSystem ), "_messagingHost" ).GetValue( Engine.Current.RenderSystem );
		if ( renderiteMessagingHost == null ) 
		{
			//_currentResoniteQueueName = RenderiteMessagingHost.GenerateQueueName();
			//_currentResoniteQueueCapacity = ModMessagingManager.DEFAULT_IPC_FILE_SIZE;
			Error( "Failed to initialize!" );
			return;
		} 
		else 
		{
			_currentResoniteQueueName = renderiteMessagingHost.QueueName;
			_currentResoniteQueueCapacity = renderiteMessagingHost.QueueCapacity;
		}
		
		_messagingManager = new( this );
		_messagingManager.Connect( _currentResoniteQueueName + ModInfo.ModRDName, true, _currentResoniteQueueCapacity );
		if ( _rmlConfig != null ) 
		{
			_messagingManager.SendMessage( ModInfo.ModConfigKeys.ModEnabled, _rmlConfig.GetValue( modEnabled ) );
			_messagingManager.SendMessage( ModInfo.ModConfigKeys.ipdOverrideMultiplier, _rmlConfig.GetValue( ipdOverrideMultiplier ) );
		}
			
		Msg( "Finished initializing!" );
	}
	
	[AutoRegisterConfigKey]
	public static readonly 
		ModConfigurationKey<bool> modEnabled = 
			new( "modEnabled", 
				 "Should the mod be enabled?", 
				 () => true, 
				 false,
				 valueValidator: ( obj ) => { _messagingManager?.SendMessage( ModInfo.ModConfigKeys.ModEnabled, obj ); return true; }
			);
	
	[AutoRegisterConfigKey]
	public static readonly 
		ModConfigurationKey<float> ipdOverrideMultiplier = 
			new( "ipdOverrideMultiplier", 
				"What should the IPD be multiplied with?", 
				() => 1,
				valueValidator: (obj) => 
				{
					bool check = Math.Abs( obj ) <= 10;
					if ( check )
						_messagingManager?.SendMessage( ModInfo.ModConfigKeys.ipdOverrideMultiplier, obj ); 
					return check;
				}
			);
	
	// A hacky way to be able to trigger the debugger
	/*public static void ManualHackyBreakPoint()
	{
		CommentBehaviour c = new CommentBehaviour();
		c.AssignConnector( null );
	}*/
	
	T IMemoryPackerEntityPool.Borrow<T>() => Pool<T>.Borrow();
	void IMemoryPackerEntityPool.Return<T>(T value) => Pool<T>.ReturnCleaned(ref value);
}
#endif
