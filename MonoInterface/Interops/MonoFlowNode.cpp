#include "stdafx.h"

#include "MonoFlowNode.h"

IMonoClass *GetFlowNodeClass()
{
	return MonoEnv->Cryambly->GetClass("CryCil.Engine.Logic", "FlowNode");
}

RAW_THUNK typedef mono::object(*CreateThunk)(IFlowGraph *, ushort, ushort);

MonoFlowNode::MonoFlowNode(TFlowNodeTypeId typeId, SActivationInfo *info, bool &cancel)
	: refCount(0)
	, objHandle(-1)
	, targetsEntity(false)
{
	static CreateThunk thunk = CreateThunk(GetFlowNodeClass()->GetFunction("Create")->RawThunk);

	mono::object obj = thunk(info->pGraph, typeId, info->myID);
	if (!obj)
	{
		delete this;
		cancel = true;
	}
	this->objHandle = MonoEnv->GC->Keep(obj);
	cancel = false;
}

RAW_THUNK typedef void(*NodeReleaseThunk)(mono::object);

MonoFlowNode::~MonoFlowNode()
{
	static NodeReleaseThunk thunk = NodeReleaseThunk(GetFlowNodeClass()->GetFunction("Release")->RawThunk);
	
	if (this->objHandle.IsValid)
	{
		thunk(this->objHandle.Object);
	}
}

struct MonoFlowPortConfig
{
	const char *name;
	const char *humanName;
	const char *description;
	const char *enumConfig;
	MonoFlowData defaultValue;

	SInputPortConfig ToInput()
	{
		SInputPortConfig conf;
		conf.name = name;
		conf.humanName = humanName;
		conf.description = description;
		conf.sUIConfig = enumConfig;
		switch (defaultValue.DataType)
		{
		case eFDT_Int:
			conf.defaultData = TFlowInputData(this->defaultValue.Integer32);
			break;
		case eFDT_Float:
			conf.defaultData = TFlowInputData(this->defaultValue.Float);
			break;
		case eFDT_EntityId:
			conf.defaultData = TFlowInputData(this->defaultValue.EntityId);
			break;
		case eFDT_String:
			conf.defaultData = TFlowInputData(string(this->defaultValue.text));
			break;
		case eFDT_Vec3:
		{
			Vec3 vector(this->defaultValue.Vector3[0], this->defaultValue.Vector3[1], this->defaultValue.Vector3[2]);
			conf.defaultData = TFlowInputData(vector);
			break;
		}
		case eFDT_Bool:
			conf.defaultData = TFlowInputData(this->defaultValue.Bool);
			break;
		default:
			break;
		}
		return conf;
	}
	SOutputPortConfig ToOutput() const
	{
		SOutputPortConfig conf;
		conf.name = name;
		conf.humanName = humanName;
		conf.description = description;
		conf.type = this->defaultValue.DataType;
		return conf;
	}
};

struct MonoFlowNodeConfig
{
	const char *description;
	mono::Array inputs;
	mono::Array outputs;
	unsigned int flags;
};

typedef void(*GetConfigurationThunk)(mono::object, MonoFlowNodeConfig *);

void MonoFlowNode::GetConfiguration(SFlowNodeConfig &config)
{
	static GetConfigurationThunk thunk =
		GetConfigurationThunk(GetFlowNodeClass()->GetFunction("GetConfiguration", 1)->RawThunk);

	if (this->nodeConfig.sUIClassName != nullptr || !this->objHandle.IsValid)
	{
		config = this->nodeConfig;
		return;
	}

	MonoFlowNodeConfig conf;
	thunk(this->objHandle.Object, &conf);

	this->targetsEntity = (conf.flags & EFLN_TARGET_ENTITY) != 0;

	// Pin those arrays just in case.
	MonoGCHandle inputsGC  = MonoEnv->GC->Pin(conf.inputs);
	MonoGCHandle outputsGC = MonoEnv->GC->Pin(conf.outputs);

	IMonoArray<MonoFlowPortConfig> inputs = conf.inputs;
	IMonoArray<MonoFlowPortConfig> outputs = conf.outputs;
	// Currently the memory that is allocated here is never released, since I think CryEngine expects these arrays to be
	// global arrays.
	SInputPortConfig *inputConfs = new SInputPortConfig[inputs.Length + 1];
	SOutputPortConfig *outputConfs = new SOutputPortConfig[outputs.Length + 1];
	// Transfer and convert configs for input ports.
	for (int i = 0, count = inputs.Length; i < count; i++)
	{
		inputConfs[i] = inputs[i].ToInput();
	}
	// Transfer and convert configs for output ports.
	for (int i = 0, count = outputs.Length; i < count; i++)
	{
		outputConfs[i] = outputs[i].ToOutput();
	}

	config.nFlags = conf.flags;
	config.sDescription = conf.description;
	config.sUIClassName = "";
	config.pInputPorts = inputConfs;
	config.pOutputPorts = outputConfs;
	this->nodeConfig = config;
}

RAW_THUNK typedef bool(*SaveLoadThunk)(mono::object, IXmlNode *);

bool MonoFlowNode::SerializeXML(SActivationInfo *, const XmlNodeRef& root, bool reading)
{
	static SaveLoadThunk save = SaveLoadThunk(GetFlowNodeClass()->GetFunction("SaveData")->RawThunk);
	static SaveLoadThunk load = SaveLoadThunk(GetFlowNodeClass()->GetFunction("LoadData")->RawThunk);

	if (!this->objHandle.IsValid)
	{
		return true;
	}

	if (reading)
	{
		return load(this->objHandle.Object, root);
	}
	else
	{
		return save(this->objHandle.Object, root);
	}
}

RAW_THUNK typedef void(*SerializeThunk)(mono::object, ISerialize *);

void MonoFlowNode::Serialize(SActivationInfo *, TSerialize ser)
{
	static SerializeThunk thunk = SerializeThunk(GetFlowNodeClass()->GetFunction("Serialize")->RawThunk);

	if (!this->objHandle.IsValid)
	{
		return;
	}

	thunk(this->objHandle.Object, *reinterpret_cast<ISerialize **>(&ser));
}

RAW_THUNK typedef void(*PostSerializeThunk)(mono::object);

void MonoFlowNode::PostSerialize(SActivationInfo *)
{
	static PostSerializeThunk thunk = PostSerializeThunk(GetFlowNodeClass()->GetFunction("PostSerialize")->RawThunk);

	if (!this->objHandle.IsValid)
	{
		return;
	}

	thunk(this->objHandle.Object);
}

typedef void(*UpdateNodeThunk)(mono::object);
typedef void(*ActivatePortsThunk)(mono::object, mono::Array, mono::Array);
typedef void(*PrecacheResourcesThunk)(mono::object);
typedef void(*InitializeNodeThunk)(mono::object);
typedef void(*PostInitializeNodeThunk)(mono::object);
typedef void(*SetEntityIdNodeThunk)(mono::object, EntityId);
typedef void(*SuspendNodeThunk)(mono::object);
typedef void(*ResumeNodeThunk)(mono::object);
typedef void(*ConnectInputPortNodeThunk)(mono::object, byte);
typedef void(*DisconnectInputPortNodeThunk)(mono::object, byte);
typedef void(*ConnectOutputPortNodeThunk)(mono::object, byte);
typedef void(*DisconnectOutputPortNodeThunk)(mono::object, byte);

void MonoFlowNode::ProcessEvent(EFlowEvent event, SActivationInfo *actInfo)
{
	IMonoClass *klass = GetFlowNodeClass();
	static UpdateNodeThunk update = UpdateNodeThunk(klass->GetFunction("Update", -1)->RawThunk);
	static ActivatePortsThunk activate = ActivatePortsThunk(klass->GetFunction("Activate", -1)->RawThunk);
	static PrecacheResourcesThunk precache =
		PrecacheResourcesThunk(klass->GetFunction("PrecacheResourcesInternal", -1)->RawThunk);
	static InitializeNodeThunk init = InitializeNodeThunk(klass->GetFunction("Initialize", -1)->RawThunk);
	static PostInitializeNodeThunk postInit = PostInitializeNodeThunk(klass->GetFunction("PostInitialize", -1)->RawThunk);
	static SetEntityIdNodeThunk setEnt = SetEntityIdNodeThunk(klass->GetFunction("SetEntityId", -1)->RawThunk);
	static SuspendNodeThunk suspend = SuspendNodeThunk(klass->GetFunction("Suspend", -1)->RawThunk);
	static ResumeNodeThunk resume = ResumeNodeThunk(klass->GetFunction("Resume", -1)->RawThunk);
	static ConnectInputPortNodeThunk connectInput =
		ConnectInputPortNodeThunk(klass->GetFunction("ConnectInputPort", -1)->RawThunk);
	static DisconnectInputPortNodeThunk disconnectInput =
		DisconnectInputPortNodeThunk(klass->GetFunction("DisconnectInputPort", -1)->RawThunk);
	static ConnectOutputPortNodeThunk connectOutput =
		ConnectOutputPortNodeThunk(klass->GetFunction("ConnectOutputPort", -1)->RawThunk);
	static DisconnectOutputPortNodeThunk disconnectOutput =
		DisconnectOutputPortNodeThunk(klass->GetFunction("DisconnectOutputPort", -1)->RawThunk);

	if (!this->objHandle.IsValid)
	{
		return;
	}

	switch (event)
	{
	case eFE_Update:
		update(this->objHandle.Object);
		break;
	case eFE_Activate:
	{
		List<byte> listIds(64);
		
		auto data = actInfo->pGraph->GetNodeData(actInfo->myID);
		int portCount = data->GetNumInputPorts();
		if (this->targetsEntity)
		{
			// If this node targets an entity then it will have an extra node that is activated separately via
			// eFE_SetEntityId event.
			portCount--;
		}
		for (byte i = 0; i < portCount; i++)
		{
			if (actInfo->pInputPorts[i].IsUserFlagSet())		// Was it activated?
			{
				listIds.Add(i);
			}
		}
		
		IMonoArray<byte> ids =
			MonoEnv->Objects->Arrays->Create(listIds.Length, MonoEnv->CoreLibrary->Byte);
		IMonoArray<MonoFlowData> values =
			MonoEnv->Objects->Arrays->Create(listIds.Length,
			MonoEnv->Cryambly->GetClass("CryCil.Engine.Logic", "FlowData"));
		
		MonoGCHandle arrayHandle0 = MonoEnv->GC->Pin(ids);
		MonoGCHandle arrayHandle1 = MonoEnv->GC->Pin(values);

		for (byte i = 0; i < listIds.Length; i++)
		{
			byte id = listIds[i];
			ids[i] = id;
			values[i] = actInfo->pInputPorts[id];
		}

		activate(this->objHandle.Object, ids, values);

		break;
	}
	case eFE_FinalActivate:
		break;
	case eFE_PrecacheResources:
		precache(this->objHandle.Object);
		break;
	case eFE_Initialize:
		init(this->objHandle.Object);
		break;
	case eFE_FinalInitialize:
		postInit(this->objHandle.Object);
		break;
	case eFE_SetEntityId:
		setEnt(this->objHandle.Object, actInfo->pEntity->GetId());
		break;
	case eFE_Suspend:
		suspend(this->objHandle.Object);
		break;
	case eFE_Resume:
		resume(this->objHandle.Object);
		break;
	case eFE_ConnectInputPort:
		connectInput(this->objHandle.Object, actInfo->connectPort);
		break;
	case eFE_DisconnectInputPort:
		disconnectInput(this->objHandle.Object, actInfo->connectPort);
		break;
	case eFE_ConnectOutputPort:
		connectOutput(this->objHandle.Object, actInfo->connectPort);
		break;
	case eFE_DisconnectOutputPort:
		disconnectOutput(this->objHandle.Object, actInfo->connectPort);
		break;
	case eFE_DontDoAnythingWithThisPlease:
		break;
	default:
		break;
	}
}

void MonoFlowNode::GetMemoryUsage(ICrySizer *) const
{
	
}