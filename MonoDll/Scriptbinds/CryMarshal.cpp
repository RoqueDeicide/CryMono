#include "stdafx.h"
#include "CryMarshal.h"

Scriptbind_CryMarshal::Scriptbind_CryMarshal()
{
	REGISTER_METHOD(AllocateMemory);
	REGISTER_METHOD(FreeMemory);
}
Scriptbind_CryMarshal::~Scriptbind_CryMarshal()
{}

void * Scriptbind_CryMarshal::AllocateMemory(unsigned __int64 size)
{
#ifdef WIN64
	return malloc(size);
#else
	return malloc((unsigned int)size);
#endif
}

void Scriptbind_CryMarshal::FreeMemory(void * pointer)
{
	free(pointer);
}