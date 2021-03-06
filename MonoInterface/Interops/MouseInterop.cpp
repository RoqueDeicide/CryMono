#include "stdafx.h"

#include "MouseInterop.h"

RAW_THUNK typedef void(*positionThunk)(int, int);
RAW_THUNK typedef void(*wheelDeltaThunk)(int);

void MouseInterop::InitializeInterops()
{
	REGISTER_METHOD(IncrementCounter);
	REGISTER_METHOD(DecrementCounter);
	REGISTER_METHOD(GetAbsolutePosition);
	REGISTER_METHOD(SetAbsolutePosition);
	REGISTER_METHOD(GetClientPosition);
	REGISTER_METHOD(SetClientPosition);
	REGISTER_METHOD(Reset);
	REGISTER_METHOD(ConfineCursor);
	REGISTER_METHOD(Hide);
	REGISTER_METHOD(UseSystemCursor);

	gEnv->pHardwareMouse->AddListener(this);
}

mono::object Box(Vec2_tpl<int> vector)
{
	static IMonoClass *vector2Int32 = MonoEnv->Cryambly->GetClass("CryCil", "Vector2Int32");
	return vector2Int32->Box(&vector);
}

void MouseInterop::OnHardwareMouseEvent(int iX, int iY, EHARDWAREMOUSEEVENT eHardwareMouseEvent, int wheelDelta /*= 0*/)
{
	static IMonoClass *klass = this->GetInteropClass(MonoEnv->Cryambly);
	static positionThunk rmbd =
		positionThunk(klass->GetFunction("OnRightMouseButtonDown", -1)->UnmanagedThunk);
	static positionThunk rmbu =
		positionThunk(klass->GetFunction("OnRightMouseButtonUp", -1)->UnmanagedThunk);
	static positionThunk rmbdd =
		positionThunk(klass->GetFunction("OnRightMouseButtonDoubleClick", -1)->UnmanagedThunk);
	static positionThunk lmbd =
		positionThunk(klass->GetFunction("OnLeftMouseButtonDown", -1)->UnmanagedThunk);
	static positionThunk lmbu =
		positionThunk(klass->GetFunction("OnLeftMouseButtonUp", -1)->UnmanagedThunk);
	static positionThunk lmbdd =
		positionThunk(klass->GetFunction("OnLeftMouseButtonDoubleClick", -1)->UnmanagedThunk);
	static positionThunk mmbd =
		positionThunk(klass->GetFunction("OnMiddleMouseButtonDown", -1)->UnmanagedThunk);
	static positionThunk mmbu =
		positionThunk(klass->GetFunction("OnMiddleMouseButtonUp", -1)->UnmanagedThunk);
	static positionThunk mmbdd =
		positionThunk(klass->GetFunction("OnMiddleMouseButtonDoubleClick", -1)->UnmanagedThunk);
	
	static positionThunk moveM = positionThunk(klass->GetFunction("OnMove", -1)->UnmanagedThunk);
	
	static wheelDeltaThunk wheel = wheelDeltaThunk(klass->GetFunction("OnWheel", -1)->UnmanagedThunk);

	switch (eHardwareMouseEvent)
	{
	case HARDWAREMOUSEEVENT_MOVE:
		moveM(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_LBUTTONDOWN:
		rmbd(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_LBUTTONUP:
		rmbu(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_LBUTTONDOUBLECLICK:
		rmbdd(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_RBUTTONDOWN:
		lmbd(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_RBUTTONUP:
		lmbu(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_RBUTTONDOUBLECLICK:
		lmbdd(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_MBUTTONDOWN:
		mmbd(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_MBUTTONUP:
		mmbu(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_MBUTTONDOUBLECLICK:
		mmbdd(iX, iY);
		break;
	case HARDWAREMOUSEEVENT_WHEEL:
		wheel(wheelDelta);
		break;
	default:
		break;
	}
}

void MouseInterop::IncrementCounter()
{
	gEnv->pHardwareMouse->IncrementCounter();
}

void MouseInterop::DecrementCounter()
{
	gEnv->pHardwareMouse->DecrementCounter();
}

Vec2 MouseInterop::GetAbsolutePosition()
{
	Vec2 value;
	gEnv->pHardwareMouse->GetHardwareMousePosition(&value.x, &value.y);
	return value;
}

void MouseInterop::SetAbsolutePosition(Vec2 value)
{
	gEnv->pHardwareMouse->SetHardwareMousePosition(value.x, value.y);
}

Vec2 MouseInterop::GetClientPosition()
{
	Vec2 value;
	gEnv->pHardwareMouse->GetHardwareMouseClientPosition(&value.x, &value.y);
	return value;
}

void MouseInterop::SetClientPosition(Vec2 value)
{
	gEnv->pHardwareMouse->SetHardwareMouseClientPosition(value.x, value.y);
}

void MouseInterop::Reset(bool visibleByDefault)
{
	gEnv->pHardwareMouse->Reset(visibleByDefault);
}

void MouseInterop::ConfineCursor(bool confine)
{
	gEnv->pHardwareMouse->ConfineCursor(confine);
}

void MouseInterop::Hide(bool hide)
{
	gEnv->pHardwareMouse->Hide(hide);
}

void MouseInterop::UseSystemCursor(bool useSystemCursor)
{
#if CRY_PLATFORM_WINDOWS
	gEnv->pHardwareMouse->UseSystemCursor(useSystemCursor);
#endif
}
