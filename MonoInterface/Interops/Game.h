#pragma once

#include "IMonoInterface.h"

struct GameInterop : public IMonoInterop<true, true>
{
	virtual const char *GetName() override { return "Game"; }
	virtual const char *GetNameSpace() override { return "CryCil"; }

	virtual void OnRunTimeInitialized() override;

	static bool get_IsEditor();
	static bool get_IsEditing();
	static bool get_IsMultiplayer();
	static bool get_IsServer();
	static bool get_IsClient();
	static bool get_IsDedicatedServer();
	static bool get_IsFullMotionVideoPlaying();
	static bool get_IsCutscenePlaying();
};