/////////////////////////////////////////////////////////////////////////*
//Ink Studios Source File.
//Copyright (C), Ink Studios, 2011.
//////////////////////////////////////////////////////////////////////////
// IMonoObject interface for external projects, i.e. CryGame.
//////////////////////////////////////////////////////////////////////////
// 18/12/2011 : Created by Filip 'i59' Lundgren
////////////////////////////////////////////////////////////////////////*/
#ifndef __I_MONO_OBJECT_H__
#define __I_MONO_OBJECT_H__

#include <MonoCommon.h>

struct IMonoClass;

/// <summary>
/// The IMonoObject class is used to wrap native mono objects of any type, and to
/// convert C++ types to the Mono equivalent.
/// </summary>
struct IMonoObject
{
public:
	inline IMonoObject *CallMethod(const char *funcName);

	template<typename P1> 
	inline IMonoObject *CallMethod(const char *funcName, const P1 &p1);

	template<typename P1, typename P2> 
	inline IMonoObject *CallMethod(const char *funcName, const P1 &p1, const P2 &p2);

	template<typename P1, typename P2, typename P3> 
	inline IMonoObject *CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3);

	template<typename P1, typename P2, typename P3, typename P4> 
	inline IMonoObject *CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3, const P4 &p4);

	template<typename P1, typename P2, typename P3, typename P4, typename P5> 
	inline IMonoObject *CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3, const P4 &p4, const P5 &p5);

	template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
	inline IMonoObject *CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3, const P4 &p4, const P5 &p5, const P6 &p6);

	inline IMonoObject *GetPropertyValue(const char *fieldName, bool throwOnFail = true);
	inline void SetPropertyValue(const char *fieldName, IMonoObject *pNewValue, bool throwOnFail = true);
	inline IMonoObject *GetFieldValue(const char *fieldName, bool throwOnFail = true);
	inline void SetFieldValue(const char *fieldName, IMonoObject *pNewValue, bool throwOnFail = true);

	/// <summary>
	/// Releases the object. Warning: also destructed in managed code!
	/// </summary>
	virtual void Release(bool triggerGC = true) = 0;

	/// <summary>
	/// Gets the unboxed object and casts it to the requested type. (class T)
	/// </summary>
	template <class T>
	T Unbox() { return *(T *)UnboxObject(); }

	/// <summary>
	/// Gets the type of this Mono object.
	/// </summary>
	virtual EMonoAnyType GetType() = 0;

	virtual MonoAnyValue GetAnyValue() = 0;

	virtual const char *ToString() = 0;
	
	/// <summary>
	/// Returns the object as it is seen in managed code, can be passed directly across languages.
	/// </summary>
	virtual mono::object GetManagedObject() = 0;

	virtual IMonoClass *GetClass() = 0;

private:
	/// <summary>
	/// Unboxes the object and returns it as a void pointer. (Use Unbox() method to easily cast to the C++ type)
	/// </summary>
	virtual void *UnboxObject() = 0;
};

#include <IMonoClass.h>
#include <IMonoArray.h>

inline IMonoObject *IMonoObject::CallMethod(const char *funcName)
{
	mono::object result = GetClass()->Invoke(this->GetManagedObject(), funcName);

	if(result)
		return *result;

	return nullptr;
}

template<typename P1> 
inline IMonoObject *IMonoObject::CallMethod(const char *funcName, const P1 &p1)
{
	IMonoArray *pArgs = CreateMonoArray(1);
	pArgs->Insert(p1);

	mono::object result = GetClass()->InvokeArray(this->GetManagedObject(), funcName, pArgs);
	SAFE_RELEASE(pArgs);

	if(result)
		return *result;

	return nullptr;
};

template<typename P1, typename P2> 
inline IMonoObject *IMonoObject::CallMethod(const char *funcName, const P1 &p1, const P2 &p2)
{
	IMonoArray *pArgs = CreateMonoArray(2);
	pArgs->Insert(p1);
	pArgs->Insert(p2);

	mono::object result = GetClass()->InvokeArray(this->GetManagedObject(), funcName, pArgs);
	SAFE_RELEASE(pArgs);

	if(result)
		return *result;

	return nullptr;
};

template<typename P1, typename P2, typename P3> 
inline IMonoObject *IMonoObject::CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3)
{
	IMonoArray *pArgs = CreateMonoArray(3);
	pArgs->Insert(p1);
	pArgs->Insert(p2);
	pArgs->Insert(p3);
	
	mono::object result = GetClass()->InvokeArray(this->GetManagedObject(), funcName, pArgs);
	SAFE_RELEASE(pArgs);

	if(result)
		return *result;

	return nullptr;
};

template<typename P1, typename P2, typename P3, typename P4> 
inline IMonoObject *IMonoObject::CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3, const P4 &p4)
{
	IMonoArray *pArgs = CreateMonoArray(4);
	pArgs->Insert(p1);
	pArgs->Insert(p2);
	pArgs->Insert(p3);
	pArgs->Insert(p4);
	
	mono::object result = GetClass()->InvokeArray(this->GetManagedObject(), funcName, pArgs);
	SAFE_RELEASE(pArgs);

	if(result)
		return *result;

	return nullptr;
};

template<typename P1, typename P2, typename P3, typename P4, typename P5> 
inline IMonoObject *IMonoObject::CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3, const P4 &p4, const P5 &p5)
{
	IMonoArray *pArgs = CreateMonoArray(5);
	pArgs->Insert(p1);
	pArgs->Insert(p2);
	pArgs->Insert(p3);
	pArgs->Insert(p4);
	pArgs->Insert(p5);
	
	mono::object result = GetClass()->InvokeArray(this->GetManagedObject(), funcName, pArgs);
	SAFE_RELEASE(pArgs);

	if(result)
		return *result;

	return nullptr;
};

template<typename P1, typename P2, typename P3, typename P4, typename P5, typename P6>
inline IMonoObject *IMonoObject::CallMethod(const char *funcName, const P1 &p1, const P2 &p2, const P3 &p3, const P4 &p4, const P5 &p5, const P6 &p6)
{
	IMonoArray *pArgs = CreateMonoArray(6);
	pArgs->Insert(p1);
	pArgs->Insert(p2);
	pArgs->Insert(p3);
	pArgs->Insert(p4);
	pArgs->Insert(p5);
	pArgs->Insert(p6);
	
	mono::object result = GetClass()->InvokeArray(this->GetManagedObject(), funcName, pArgs);
	SAFE_RELEASE(pArgs);

	if(result)
		return *result;

	return nullptr;
};

inline IMonoObject *IMonoObject::GetPropertyValue(const char *propertyName, bool throwOnFail)
{
	mono::object result = GetClass()->GetPropertyValue(this->GetManagedObject(), propertyName, throwOnFail);

	if(result)
		return *result;

	return nullptr;
}

inline void IMonoObject::SetPropertyValue(const char *propertyName, IMonoObject *pNewValue, bool throwOnFail)
{
	GetClass()->SetPropertyValue(this->GetManagedObject(), propertyName, (pNewValue != nullptr ? pNewValue->GetManagedObject() : nullptr), throwOnFail);
}

inline IMonoObject *IMonoObject::GetFieldValue(const char *fieldName, bool throwOnFail)
{
	mono::object result = GetClass()->GetFieldValue(this->GetManagedObject(), fieldName, throwOnFail);

	if(result)
		return *result;

	return nullptr;
}

inline void IMonoObject::SetFieldValue(const char *fieldName, IMonoObject *pNewValue, bool throwOnFail)
{
	GetClass()->SetFieldValue(this->GetManagedObject(), fieldName, (pNewValue != nullptr ? pNewValue->GetManagedObject() : nullptr), throwOnFail);
}

#endif //__I_MONO_OBJECT_H__