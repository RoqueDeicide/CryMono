#include "stdafx.h"

#include "Console.h"

#undef GetCommandLine

void ConsoleInterop::InitializeInterops()
{
	auto klass = this->GetInteropClass(MonoEnv->Cryambly);
	executeCommand = ExecuteCommandThunk(klass->GetFunction("ExecuteMonoCommand", -1)->UnmanagedThunk);

	REGISTER_METHOD(RegisterCommandInternal);
	REGISTER_METHOD(UnregisterCommandInternal);
	REGISTER_METHOD_N("ExecuteCommand(string,bool,bool)", ExecuteCommand);

	REGISTER_METHOD_N("RegisterVariable(string,float&,float,CryCil.Engine.ConsoleFlags,string)", RegisterVariable);
	REGISTER_METHOD_N("RegisterVariable(string,int&,int,CryCil.Engine.ConsoleFlags,string)", RegisterVariableIntRef);

	REGISTER_METHOD_N("RegisterVariableInternal(string,float,CryCil.Engine.ConsoleFlags,intptr,string)", RegisterVariableFloat);
	REGISTER_METHOD_N("RegisterVariableInternal(string,int,CryCil.Engine.ConsoleFlags,intptr,string)", RegisterVariableInt);
	REGISTER_METHOD_N("RegisterVariableInternal(string,string,CryCil.Engine.ConsoleFlags,intptr,string)", RegisterVariableString);

	REGISTER_METHOD(GetVariable);
}

void ConsoleInterop::MonoCommand(IConsoleCmdArgs *args)
{
	mono::exception ex;
	executeCommand(ToMonoString(args->GetCommandLine()), &ex);
}

void ConsoleInterop::RegisterCommandInternal(mono::string name, mono::string help, EVarFlags flags)
{
	if (gEnv && gEnv->pConsole && name)
	{
		gEnv->pConsole->AddCommand(ToNativeString(name), MonoCommand, flags, help ? ToNativeString(help) : nullptr);
	}
}

void ConsoleInterop::UnregisterCommandInternal(mono::string name)
{
	if (gEnv && gEnv->pConsole)
	{
		gEnv->pConsole->RemoveCommand(NtText(name));
	}
}

void ConsoleInterop::ExecuteCommand(mono::string command, bool silent, bool deferExecution)
{
	if (gEnv && gEnv->pConsole)
	{
		if (!command)
		{
			ArgumentNullException("Name of the command to execute cannot be null.").Throw();
		}
		gEnv->pConsole->ExecuteString(NtText(command), silent, deferExecution);
	}
}

ExecuteCommandThunk ConsoleInterop::executeCommand;

#define GetCommandLine GetCommandLineA

ICVar *ConsoleInterop::RegisterVariableFloat(mono::string name, float value, EVarFlags flags, ConsoleVarFunc thunk, mono::string help)
{
	if (gEnv && gEnv->pConsole)
	{
		return gEnv->pConsole->RegisterFloat
			(ToNativeString(name), value, flags, help ? ToNativeString(help) : nullptr, thunk);
	}
	return nullptr;
}

ICVar *ConsoleInterop::RegisterVariableInt(mono::string name, int value, EVarFlags flags, ConsoleVarFunc thunk, mono::string help)
{
	if (gEnv && gEnv->pConsole)
	{
		return gEnv->pConsole->RegisterInt
			(ToNativeString(name), value, flags, help ? ToNativeString(help) : nullptr, thunk);
	}
	return nullptr;
}

ICVar *ConsoleInterop::RegisterVariableString(mono::string name, mono::string value, EVarFlags flags, ConsoleVarFunc thunk, mono::string help)
{
	if (gEnv && gEnv->pConsole)
	{
		const char *val = value ? ToNativeString(value) : nullptr;
		const char *h = help ? ToNativeString(help) : nullptr;
		return gEnv->pConsole->RegisterString(ToNativeString(name), val, flags, h, thunk);
	}
	return nullptr;
}

ICVar *ConsoleInterop::RegisterVariable(mono::string name, float *field, float value, EVarFlags flags, mono::string help)
{
	if (gEnv && gEnv->pConsole)
	{
		if (!name)
		{
			ArgumentNullException("Cannot register a console variable using a null name.").Throw();
		}
		return gEnv->pConsole->Register
			(ToNativeString(name), field, value, flags, help ? ToNativeString(help) : nullptr);
	}
	return nullptr;
}

ICVar *ConsoleInterop::RegisterVariableIntRef(mono::string name, int *field, int value, EVarFlags flags, mono::string help)
{
	if (gEnv && gEnv->pConsole)
	{
		if (!name)
		{
			ArgumentNullException("Cannot register a console variable using a null name.").Throw();
		}
		return gEnv->pConsole->Register
			(ToNativeString(name), field, value, flags, help ? ToNativeString(help) : nullptr);
	}
	return nullptr;
}

void ConsoleInterop::UnregisterVariable(mono::string name, bool _delete)
{
	if (gEnv && gEnv->pConsole)
	{
		if (!name)
		{
			ArgumentNullException("Name of the console variable to unregister cannot be null.").Throw();
		}
		gEnv->pConsole->UnregisterVariable(NtText(name), _delete);
	}
}

ICVar *ConsoleInterop::GetVariable(mono::string name)
{
	if (gEnv && gEnv->pConsole)
	{
		if (!name)
		{
			ArgumentNullException("Cannot get a console variable using a null name.").Throw();
		}
		return gEnv->pConsole->GetCVar(NtText(name));
	}
	return nullptr;
}
