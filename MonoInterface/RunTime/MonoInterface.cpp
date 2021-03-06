#include "stdafx.h"
#include "MonoInterface.h"
#include "RunTime/AllInterops.h"

#if 1
#define InterfaceMessage CryLogAlways
#else
#define InterfaceMessage(...) void(0)
#endif

#include <mono/utils/mono-logger.h>

#pragma region Property Methods
//! Returns a pointer to app domain.
void *MonoInterface::GetAppDomain()
{
	return this->appDomain;
}

const Text &MonoInterface::GetBasePath()
{
	return this->executablePath;
}

const Text &MonoInterface::GetProjectPath()
{
	return this->projectPath;
}

void MonoInterface::SetMonoLogLevel(MonoLog::Level logLevel)
{
	mono_trace_set_level_string(monoLogLevels[logLevel]);
}
#pragma endregion
#pragma region External Triggers
//! Triggers registration of FlowGraph nodes.
void MonoInterface::RegisterFlowGraphNodes()
{
	if (!this->running)
	{
		return;
	}
	mono::exception ex;
	MonoInterfaceThunks::TriggerFlowNodesRegistration(&ex);
}
//! Shuts down Mono run-time environment.
void MonoInterface::Shutdown()
{
	CryLogAlways("Checking activity before shutdown.");
	
	if (!this->running)
	{
		return;
	}
	
	CryLogAlways("About to broadcast shutdown event.");
	
	this->broadcaster->Shutdown();
	
	CryLogAlways("About to send shutdown event to Cryambly.");
	
	mono::exception ex;
	MonoInterfaceThunks::Shutdown(&ex);
	
	this->framework->UnregisterListener(this);
	gEnv->pSystem->GetISystemEventDispatcher()->RemoveListener(this);
	
	delete this->assemblies;
	delete this->gc;
	delete this->objs;
	delete this->funcs;
	MonoClassCache::Dispose();
	
	CryLogAlways("Shutting down jit.");
	
	mono_jit_cleanup(this->appDomain);
	
	CryLogAlways("No more running.");
	
	this->running = false;
	
	CryLogAlways("Deleting broadcaster.");
	
	delete this->broadcaster;
}
#pragma endregion
#pragma region Interaction with Run-Time
//! Handles exception that occurred during managed method invocation.
void MonoInterface::HandleException(mono::exception exception)
{
	if (!this->running)
	{
		return;
	}
	mono::exception ex;
	MonoInterfaceThunks::DisplayException(exception, &ex);
}
#pragma endregion
#pragma region Listeners
//! Registers new object that receives notifications about CryCIL events.
void MonoInterface::AddListener(IMonoSystemListener *listener)
{
	if (this->broadcaster)
	{
		this->broadcaster->listeners.Add(listener);
	}
}
//! Unregisters an object that receives notifications about CryCIL events.
void MonoInterface::RemoveListener(IMonoSystemListener *listener)
{
	if (this->broadcaster)
	{
		this->broadcaster->RemoveListener(listener);
	}
}
#pragma endregion
#pragma region IGameFrameworkListener Implementation.
//! Triggers Update event in MonoInterface object in Cryambly.
void MonoInterface::OnPostUpdate(float)
{
	// Notify everything about the update.
	if (this->running)
	{
		this->broadcaster->Update();
		
		mono::exception ex;
		MonoInterfaceThunks::Update(&ex);
		
		this->broadcaster->PostUpdate();
	}
}
//! Not used.
void MonoInterface::OnSaveGame(ISaveGame*) {}
//! Not used.
void MonoInterface::OnLoadGame(ILoadGame*) {}
//! Not used.
void MonoInterface::OnLevelEnd(const char*) {}
//! Not used.
void MonoInterface::OnActionEvent(const SActionEvent&) {}
#pragma endregion
#pragma region ISystemEventListener Implementation
//! Reacts to system events.
//!
//! @param event  Identifier of the event.
//! @param wparam First parameter that can supply extra information about the event.
//! @param lparam Second parameter that can supply extra information about the event.
void MonoInterface::OnSystemEvent(ESystemEvent, UINT_PTR, UINT_PTR)
{
// 	switch (event)
// 	{
// 		case ESYSTEM_EVENT_CHANGE_FOCUS:
// 		{
// 			CryLogAlways("The window has lost/gained focus.");
// 		}
// 		break;
// 	default:
// 		break;
// 	}
}

#pragma endregion
#pragma region Default Listeners
void MonoInterface::RegisterDefaultListeners() const
{
	this->broadcaster->listeners.Add(new TimingInterop());			// This interop has to be first to make
																	// sure that all timings are up-to-date.
	this->broadcaster->listeners.Add(new MathHelpersInterop());
#ifdef _DEBUG
	this->broadcaster->listeners.Add(new DebugEventReporter());
#endif // _DEBUG
	this->broadcaster->listeners.Add(new CryMarshalInterop());
	this->broadcaster->listeners.Add(new MeshOpsInterop());
	this->broadcaster->listeners.Add(new BatchOps());
	this->broadcaster->listeners.Add(new MouseInterop());
	this->broadcaster->listeners.Add(new InputInterop());
	this->broadcaster->listeners.Add(new ConsoleInterop());
	this->broadcaster->listeners.Add(new ConsoleVariableInterop());
	this->broadcaster->listeners.Add(new DebugDrawInterop());
	this->broadcaster->listeners.Add(new ProfilingInterop());
	this->broadcaster->listeners.Add(new ArchiveStreamInterop());
	this->broadcaster->listeners.Add(new CryArchiveInterop());
	this->broadcaster->listeners.Add(new CryFilesInterop());
	this->broadcaster->listeners.Add(new AliasesInterop());
	this->broadcaster->listeners.Add(new CryPakInterop());
	this->broadcaster->listeners.Add(new AuxiliaryGeometryInterop());
	this->broadcaster->listeners.Add(new RendererInterop());
	this->broadcaster->listeners.Add(new TextureInterop());
	this->broadcaster->listeners.Add(new CryFontInterop());
	this->broadcaster->listeners.Add(new LocaleInterop());
	this->broadcaster->listeners.Add(new LevelsInterop());
	this->broadcaster->listeners.Add(new LevelSystemInterop());
	this->broadcaster->listeners.Add(new LevelInterop());
	this->broadcaster->listeners.Add(new TerrainInterop());
	this->broadcaster->listeners.Add(new OceanInterop());
	this->broadcaster->listeners.Add(new CryXmlNodeInterop());
	this->broadcaster->listeners.Add(new MaterialInterop());
	this->broadcaster->listeners.Add(new SubMaterialsInterop());
	this->broadcaster->listeners.Add(new GeneralExtensionsInterop());
	this->broadcaster->listeners.Add(new RenderShaderResourcesInterop());
	this->broadcaster->listeners.Add(new ShaderParametersInterop());
	this->broadcaster->listeners.Add(new ShaderInterop());
	this->broadcaster->listeners.Add(new SurfaceTypeInterop());
	this->broadcaster->listeners.Add(new SurfaceTypeEnumeratorInterop());
	this->broadcaster->listeners.Add(new MaterialLayerCollectionInterop());
	this->broadcaster->listeners.Add(new MaterialLayerInterop());
	this->broadcaster->listeners.Add(new StackStringInterop());
	this->broadcaster->listeners.Add(new ParticleParametersInterop());
	this->broadcaster->listeners.Add(new ParticleEmitterInterop());
	this->broadcaster->listeners.Add(new ParticleEffectInterop());
	this->broadcaster->listeners.Add(new ParticleEffectIteratorInterop());
	this->broadcaster->listeners.Add(new SplineInterop());
	this->broadcaster->listeners.Add(new CrySyncInterop());
	this->broadcaster->listeners.Add(new TimeOfDayInterop());
	this->broadcaster->listeners.Add(new FlowGraphInterop());
	this->broadcaster->listeners.Add(new EntityIdInterop());
	this->broadcaster->listeners.Add(new EntityPoolInterop());
	this->broadcaster->listeners.Add(new EntitySystemInterop());
	this->broadcaster->listeners.Add(new NetEntityInterop());
	this->broadcaster->listeners.Add(new CryEntityInterop());
	this->broadcaster->listeners.Add(new EntitySlotsInterop());
	this->broadcaster->listeners.Add(new MonoEntityInterop());
	this->broadcaster->listeners.Add(new GameInterop());
	this->broadcaster->listeners.Add(new ActionMapHandlerInterop());
	this->broadcaster->listeners.Add(new ActionMapsInterop());
	this->broadcaster->listeners.Add(new CryActionMapInterop());
	this->broadcaster->listeners.Add(new CryInputActionInterop());
	this->broadcaster->listeners.Add(new LightSourceInterop());
	this->broadcaster->listeners.Add(new PhysicalWorldInterop());
	this->broadcaster->listeners.Add(new PhysicalEntityInterop());
	this->broadcaster->listeners.Add(new LatticeInterop());
	this->broadcaster->listeners.Add(new IGeometryInterop());
	this->broadcaster->listeners.Add(new WriteLockCondInterop());
	this->broadcaster->listeners.Add(new PhysicalBodyInterop());
	this->broadcaster->listeners.Add(new RayInterop());
	this->broadcaster->listeners.Add(new StaticSubObjectInterop());
	this->broadcaster->listeners.Add(new StaticObjectInterop());
	this->broadcaster->listeners.Add(new CMeshInterop());
	this->broadcaster->listeners.Add(new IndexedMeshInterop());
	this->broadcaster->listeners.Add(new RenderMeshInterop());
	this->broadcaster->listeners.Add(new CharacterInterop());
	this->broadcaster->listeners.Add(new DefaultSkeletonInterop());
	this->broadcaster->listeners.Add(new CharacterInterop());
	this->broadcaster->listeners.Add(new ParametricSamplerInterop());
	this->broadcaster->listeners.Add(new CharacterAnimationInterop());
	this->broadcaster->listeners.Add(new SkeletonAnimationInterop());
	this->broadcaster->listeners.Add(new SkeletonPoseInterop());
	this->broadcaster->listeners.Add(new AnimationSetInterop());
	this->broadcaster->listeners.Add(new FacialEffectorControllerInterop());
	this->broadcaster->listeners.Add(new FacialEffectorInterop());
	this->broadcaster->listeners.Add(new CustomMarshalingInterop());
	this->broadcaster->listeners.Add(new FaceIdentifierInterop());
	this->broadcaster->listeners.Add(new FacialEffectorsLibraryInterop());
	this->broadcaster->listeners.Add(new FacialModelInterop());
	this->broadcaster->listeners.Add(new FaceStateInterop());
	this->broadcaster->listeners.Add(new PhonemeLibraryInterop());
	this->broadcaster->listeners.Add(new FacialSentenceInterop());
	this->broadcaster->listeners.Add(new FacialAnimationChannelInterop());
	this->broadcaster->listeners.Add(new FacialAnimationSequenceInterop());
	this->broadcaster->listeners.Add(new FacialAnimationSkeletonAnimationEntryInterop());
	this->broadcaster->listeners.Add(new FacialAnimationSoundEntryInterop());
	this->broadcaster->listeners.Add(new FaceInterop());
	this->broadcaster->listeners.Add(new FacialAnimationInterop());
	this->broadcaster->listeners.Add(new AttachmentManagerInterop());
	this->broadcaster->listeners.Add(new AttachedObjectsCommonsInterop());
	this->broadcaster->listeners.Add(new AttachmentRowSimulationParametersInterop());
	this->broadcaster->listeners.Add(new AttachmentSimulationParametersInterop());
	this->broadcaster->listeners.Add(new AttachmentSocketInterop());
	this->broadcaster->listeners.Add(new AttachmentSkinInterop());
	this->broadcaster->listeners.Add(new AttachmentProxyInterop());
	this->broadcaster->listeners.Add(new AudioSystemInterop());
	this->broadcaster->listeners.Add(new CryAudioProxyInterop());
	this->broadcaster->listeners.Add(new DecalInterop());
	this->broadcaster->listeners.Add(new CryEntityAreaProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityAudioProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityCameraProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityPhysicalProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityRenderProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityRopeProxyInterop());
	this->broadcaster->listeners.Add(new CryEntitySubstitutionProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityTriggerProxyInterop());
	this->broadcaster->listeners.Add(new CryEntityProxiesInterop());
	this->broadcaster->listeners.Add(new GameRulesInterop());
	this->broadcaster->listeners.Add(new PostProcessingInterop());
	this->broadcaster->listeners.Add(new CryViewInterop());
	this->broadcaster->listeners.Add(new ViewControllerInterop());
	this->broadcaster->listeners.Add(new SystemEventsInterop());
	this->broadcaster->listeners.Add(new CryNetChannelInterop());
	this->broadcaster->listeners.Add(new ChannelIdInterop());
	this->broadcaster->listeners.Add(new MeshObjectInterop());
}
#pragma endregion
#pragma region Thunks Initialization
	void MonoInterface::InitializeThunks()
	{
		this->InitializeMonoInterfaceThunks();
		this->InitializeAssemblyCollectionThunks();
	}
	void MonoInterface::InitializeMonoInterfaceThunks()
	{
		InterfaceMessage("Initializing mono interface thunks.");

		MonoInterfaceThunks::DisplayException =
			this->GetMethodThunk<DisplayExceptionThunk>
			(this->cryambly, "CryCil.RunTime", "MonoInterface", "DisplayException", "System.Object");
		MonoInterfaceThunks::Initialize =
			this->GetMethodThunk<InitializeThunk>
			(this->cryambly, "CryCil.RunTime", "MonoInterface", "Initialize", nullptr);
		MonoInterfaceThunks::TriggerFlowNodesRegistration =
			this->GetMethodThunk<RegisterFlowNodesThunk>
			(this->cryambly, "CryCil.RunTime", "MonoInterface", "RegisterFlowGraphNodeTypes", nullptr);
		MonoInterfaceThunks::Shutdown =
			this->GetMethodThunk<ShutDownThunk>
			(this->cryambly, "CryCil.RunTime", "MonoInterface", "Shutdown", nullptr);
		MonoInterfaceThunks::Update =
			this->GetMethodThunk<UpdateThunk>
			(this->cryambly, "CryCil.RunTime", "MonoInterface", "Update", nullptr);
	}
	void MonoInterface::InitializeAssemblyCollectionThunks()
	{
		AssemblyCollectionThunks::LookUpAssembly =
			this->GetMethodThunk<LookUpAssemblyThunk>
			(this->cryambly, "CryCil.RunTime", "AssemblyLookUp", "LookUpAssembly", "System.String");
	}
	template<typename MethodSignature>
	MethodSignature MonoInterface::GetMethodThunk(IMonoAssembly *assembly, const char *nameSpace,
												  const char *className, const char *methodName,
												  const char *params)
	{
		InterfaceMessage("Getting a thunk for a method %s(%s) in the class %s.%s.", methodName, params, nameSpace, className);

		IMonoClass *klass = assembly->GetClass(nameSpace, className);

		InterfaceMessage("Got the class wrapper.");

		const IMonoFunction *function = klass->GetFunction(methodName, params);

		InterfaceMessage("Got the function: %p.", function);

		void *thunk = function->UnmanagedThunk;

		InterfaceMessage("Got the thunk.");

		return MethodSignature(thunk);
	}
#pragma endregion

#pragma region Thunks Initialization
	void MonoInterface::OnCompilationStartingBind()
	{
		_this->broadcaster->OnCompilationStarting();
	}
	void MonoInterface::OnCompilationCompleteBind(bool success)
	{
		_this->broadcaster->OnCompilationComplete(success);
	}
	mono::Array MonoInterface::GetSubscribedStagesBind()
	{
		int stagesCount;
		int *indices = _this->broadcaster->GetSubscribedStagesInfo(stagesCount);
		IMonoClass *SystemInt32 = MonoClassCache::Wrap(mono_get_int32_class());
		IMonoArray<int> result = MonoEnv->Objects->Arrays->Create(stagesCount, SystemInt32);
		for (int i = 0; i < stagesCount; i++)
		{
			result[i] = indices[i];
		}
		return result;
	}
	void MonoInterface::OnInitializationStageBind(int stageIndex)
	{
		_this->broadcaster->OnInitializationStage(stageIndex);
	}

	MonoInterface *MonoInterface::_this;
#pragma endregion