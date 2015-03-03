#pragma once

#include "IMonoAliases.h"

//! Represents a wrapper for a Mono property.
struct IMonoProperty : public IMonoMember
{
	//! Gets IMonoMethod that allows you to invoke a getter of the property.
	__declspec(property(get = GetGetter)) IMonoFunction *Getter;
	//! Gets IMonoMethod that allows you to invoke a setter of the property.
	__declspec(property(get = GetSetter)) IMonoFunction *Setter;
	//! Gets IMonoMethod that allows you to identify the signature of this property.
	__declspec(property(get = GetIdentifier)) IMonoFunction *Identifier;

	VIRTUAL_API virtual IMonoFunction *GetGetter() = 0;
	VIRTUAL_API virtual IMonoFunction *GetSetter() = 0;
	VIRTUAL_API virtual IMonoFunction *GetIdentifier() = 0;
};