#pragma once

#include "IMonoAliases.h"

//! Represents a method parameter specification.
//!
//! Represents a pair where first value is a pointer to the class wrapper that represents a type of the
//! parameter, and second value is postfix that specifies the kind of parameter's type.
//!
//! Check IMonoClass::GetMethod() overloads for more information.
typedef Pair<IMonoClass *, const char *> ClassSpec;

//! Defines interface of objects that wrap functionality of MonoClass type.
//!
//! General advice: Avoid dealing with generics when using this API: the embedded Mono API is way too
//! limited for this.
struct IMonoClass : public IMonoFunctionalityWrapper
{
	//! Gets the name of this class.
	__declspec(property(get = GetName))        const char                    *Name;
	//! Gets full name of this class.
	__declspec(property(get = GetFullName))    const char                    *FullName;
	//! Gets full name of this class.
	//!
	//! If this class is nested its name will be separated from declaring type with "+" instead of ".".
	__declspec(property(get = GetFullNameIL))  const char                    *FullNameIL;
	//! Gets the name space where this class is defined.
	__declspec(property(get = GetNameSpace))   const char                    *NameSpace;
	//! Gets assembly where this class is defined.
	__declspec(property(get = GetAssembly))    IMonoAssembly                 *Assembly;
	//! Gets the class where this class is defined.
	__declspec(property(get = GetBase))        IMonoClass                    *Base;
	//! Indicates whether this class is a value-type.
	__declspec(property(get = GetIsValueType)) bool                           IsValueType;
	//! Indicates whether this class is an enumeration.
	__declspec(property(get = GetIsEnum))      bool                           IsEnum;
	//! Indicates whether this class is a delegate.
	__declspec(property(get = GetIsDelegate))  bool                           IsDelegate;
	//! Gets the list of fields available through this class.
	__declspec(property(get = GetFields))      const List<IMonoField *>     &Fields;
	//! Gets the list of properties available through this class.
	__declspec(property(get = GetProperties))  const List<IMonoProperty *>  &Properties;
	//! Gets the list of events available through this class.
	__declspec(property(get = GetEvents))      const List<IMonoEvent *>     &Events;
	//! Gets the list of functions available through this class.
	__declspec(property(get = GetFunctions))   const List<IMonoFunction *>  &Functions;
	
	//! Gets one of the constructors that can accept specified number of arguments.
	//!
	//! It's not easy to predict which one of the constructors will be acquired when there are several
	//! of them that accept the same number of arguments.
	__forceinline const IMonoConstructor *GetConstructor(int paramCount = 0) const;			// Defined in IMonoConstructor.
	//! Gets constructor that can accept arguments of specified types.
	//!
	//! @param types An array of System.Type objects that specify constructor signature to use.
	__forceinline const IMonoConstructor *GetConstructor(IMonoArray<> &types) const;		// Defined in IMonoConstructor.
	//! Gets constructor that can accept arguments of specified types.
	//!
	//! Refer to documentation of corresponding GetMethod() overload for details.
	//!
	//! @param classes A list IMonoClass wrappers that specify constructor signature to use.
	__forceinline const IMonoConstructor *GetConstructor(List<IMonoClass *> &classes) const;// Defined in IMonoConstructor.
	//! Gets constructor that can accept arguments of specified types.
	//!
	//! Refer to documentation of corresponding GetMethod() overload for details.
	//!
	//! @param specifiedClasses A list of classes and postfixes that specify constructor signature
	//!                         to use.
	__forceinline const IMonoConstructor *GetConstructor(List<ClassSpec> &specifiedClasses) const;// Defined in IMonoConstructor.
	//! Gets the constructor that matches given description.
	//!
	//! Refer to documentation of corresponding GetMethod() overload for details.
	//!
	//! @param params Text that describes types arguments the method should take.
	//!
	//! @returns A pointer to the wrapper to the found constructor. Null is returned if
	//!          no constructor matching the description was found.
	__forceinline const IMonoConstructor *GetConstructor(const char *params) const;			// Defined in IMonoConstructor.
	//! Gets a constructor defined in this class.
	//!
	//! Refer to documentation of corresponding GetMethod() overload for details.
	//!
	//! @param paramTypeNames A list of full type names that specify the parameters the constructor
	//!                       accepts.
	__forceinline const IMonoConstructor *GetConstructor(List<const char *> &paramTypeNames) const;	// Defined in IMonoConstructor.
	//! Gets method that can accept arguments of specified types.
	//!
	//! @param name  Name of the method to get. If null, then any name will suffice.
	//! @param types An array of System.Type objects that specify method signature to use.
	VIRTUAL_API virtual const IMonoFunction *GetFunction(const char *name, IMonoArray<> &types) const = 0;
	//! Gets method that can accept arguments of specified types.
	//!
	//! This method does not bother with checking how arguments are passed to the method.
	//!
	//! Use it when you have a lot overloads that just accept simple values.
	//!
	//! Some types are remapped for this method: any arrays are mapped to System.Array and
	//! pointers are mapped to IntPtr.
	//!
	//! For instance:
	//! @code{.cs}
	//! void Add(sbyte);
	//! void Add(short);
	//! void Add(int);
	//! void Add(long);
	//! void Add(int[]);
	//! @endcode
	//!
	//! To get the last one in the above list, use the following code:
	//!
	//! @code{.cpp}
	//! List<IMonoClass *> type = List<IMonoClass *>(1);
	//! type.Add(MonoEnv->CoreLibrary->GetClass("System", "Array"));
	//! klass->GetMethod("Add", type);
	//! @endcode
	//!
	//! @param name    Name of the method to get. If null, then any name will suffice.
	//! @param classes A list IMonoClass wrappers that specify method signature to use.
	VIRTUAL_API virtual const IMonoFunction *GetFunction(const char *name, List<IMonoClass *> &classes) const = 0;
	//! Gets method that can accept arguments of specified types.
	//!
	//! Postfixes allow you to specify what kind of parameter to use.
	//!
	//! Available postfixes:
	//!     1) &  - specifies that parameter is passed by reference using either "ref" or "out"
	//!             keyword. When combining this postfix with others put it at the end, i.e
	//!             "[,]&" specifies an array with two dimensions that is passed by reference.
	//!     2) *  - specifies that parameter is a pointer. Bare in mind that some pointer types
	//!             may not be allowed.
	//!     1) [] - Specifies an array type. When working with multi-dimensional arrays, put
	//!             N - 1 number of commas between the brackets where N = number of dimensions.
	//!
	//! @param name             Name of the method to get. If null, then any name will suffice.
	//! @param specifiedClasses A list of classes and postfixes that specify method signature to use.
	VIRTUAL_API virtual const IMonoFunction *GetFunction(const char *name, List<ClassSpec> &specifiedClasses) const = 0;
	//! Gets the method that matches given description.
	//!
	//! The easiest way to learn the signature of the method is to use the following code:
	//!
	//! @code{.cs}
	//!
	//! MethodInfo method = typeof(Foo).GetMethod("Boo");
	//!
	//! StringBuilder builder = new StringBuilder(20);
	//!
	//! Type[] types = method.GetParameters().Select(x = > x.ParameterType).ToArray();
	//!
	//! builder.Append(types[0].FullName);
	//!
	//! for(int i = 1; i < types.Length; i++)
	//! {
	//!     builder.Append(',');
	//!     builder.Append(types[i].FullName);
	//! }
	//!
	//! string ourParameterList = builder.ToString();
	//!
	//! @endcode
	//!
	//! Examples:
	//!
	//! C# method signature: SetupNumber(out int result, ref double value, ref void *ptr, ref object[,] pars, Foo.Boo objectOfNestedType);
	//! C++ search: GetMethod("SetupNumber", "System.Int32&,System.Double&,System.Void*&,System.Object[,]&,Foo+Boo");
	//!
	//! @param name   Name of the method to find. If null, then any name will suffice.
	//! @param params Text that describes types arguments the method should take.
	//!
	//! @returns A pointer to the wrapper to the found method. Null is returned if
	//!          no method matching the description was found.
	VIRTUAL_API virtual const IMonoFunction *GetFunction(const char *name, const char *params) const = 0;
	//! Gets a method defined in this class.
	//!
	//! Examples:
	//!
	//! C# method signature: SetupNumber(out int result, ref double value, ref void *ptr, ref object[,] pars);
	//!
	//! @code{.cpp}
	//!
	//! List<const char *> typeNames = List(4);
	//!
	//! typeNames.Add("System.Int32&");
	//! typeNames.Add("System.Double&");
	//! typeNames.Add("System.Void*&");
	//! typeNames.Add("System.Object[,]&");
	//!
	//! IMonoMethod *method = ourClass->GetMethod("SetupNumber", typeNames);
	//!
	//! @endcode
	//!
	//! @param name           Name of the method to find. If null, then any name will suffice.
	//! @param paramTypeNames A list of full type names that specify the parameters the method accepts.
	VIRTUAL_API virtual const IMonoFunction *GetFunction(const char *name, List<const char *> &paramTypeNames) const = 0;
	//! Gets the first method that matches given description.
	//!
	//! @param name       Name of the method to find. If null, then any name will suffice.
	//! @param paramCount Number of arguments the method should take. If it's equal to -1 then parameter
	//!                   count is ignored.
	VIRTUAL_API virtual const IMonoFunction *GetFunction(const char *name, int paramCount = 0) const = 0;
	//! Gets an array of functions that matches given description.
	//!
	//! @param name       Name of the functions to find.
	//! @param paramCount Number of arguments the functions should take.
	//!
	//! @returns A pointer to the first found method. You should release resultant array once
	//!          you don't need it anymore.
	VIRTUAL_API virtual List<IMonoFunction *> GetFunctions(const char *name, int paramCount) const = 0;
	//! Gets an array of overload of the method.
	//!
	//! @param name       Name of the method which overloads to find.
	//!
	//! @returns A pointer to the first found method. You should release resultant array once
	//!          you don't need it anymore.
	VIRTUAL_API virtual List<IMonoFunction *> GetFunctions(const char *name) const = 0;
	//! Gets a metadata wrapper for the field of this class.
	//!
	//! @param name Name of the field to get.
	//!
	//! @returns A pointer to the object that implements IMonoField that represents the field of interest,
	//!          if found, otherwise null.
	VIRTUAL_API virtual const IMonoField *GetField(const char *name) const = 0;
	//! Gets the value of the object's field.
	//!
	//! @param obj   Object which field to get. Use nullptr when working with a static field.
	//! @param name  Name of the field which value to get.
	//! @param value Pointer to the object that will contain the value of the field.
	//!
	//! @seealso IMonoHandle::SetField
	VIRTUAL_API virtual void GetField(mono::object obj, const char *name, void *value) const = 0;
	//! Sets the value of the object's field.
	//!
	//! @param obj   Object which field to set. Use nullptr when working with a static field.
	//! @param name  Name of the field which value to set.
	//! @param value New value to assign to the field. This pointer can be a mono::object
	//!              when setting the managed-typed fields and it can be a pointer to
	//!              value-type object when setting the value-typed fields.
	//!
	//! @seealso IMonoHandle::SetField
	VIRTUAL_API virtual void SetField(mono::object obj, const char *name, void *value) const = 0;
	//! Gets the value of the object's field.
	//!
	//! @param obj   Object which field to get. Use nullptr when working with a static field.
	//! @param field Wrapper that identifies the field which value to get.
	//! @param value Pointer to the object that will contain the value of the field.
	//!
	//! @seealso IMonoHandle::SetField
	VIRTUAL_API virtual void GetField(mono::object obj, const IMonoField *field, void *value) const = 0;
	//! Sets the value of the object's field.
	//!
	//! @param obj   Object which field to set. Use nullptr when working with a static field.
	//! @param field Wrapper that identifies the field which value to set.
	//! @param value New value to assign to the field. This pointer can be a mono::object
	//!              when setting the managed-typed fields and it can be a pointer to
	//!              value-type object when setting the value-typed fields.
	//!
	//! @seealso IMonoHandle::SetField
	VIRTUAL_API virtual void SetField(mono::object obj, const IMonoField *field, void *value) const = 0;
	//! Gets a wrapper for a property defined in this class.
	//!
	//! @param name Name of the property to get.
	VIRTUAL_API virtual const IMonoProperty *GetProperty(const char *name) const = 0;
	//! Gets a wrapper for a property defined in this class.
	//!
	//! @param name  Name of the property to get. If null, then any name will suffice.
	//! @param types An array of System.Type objects that specify property signature to use.
	VIRTUAL_API virtual const IMonoProperty *GetProperty(const char *name, IMonoArray<> &types) const = 0;
	//! Gets a wrapper for a property defined in this class.
	//!
	//! @param name    Name of the property to get. If null, then any name will suffice.
	//! @param classes A list IMonoClass wrappers that specify property signature to use.
	VIRTUAL_API virtual const IMonoProperty *GetProperty(const char *name, List<IMonoClass *> &classes) const = 0;
	//! Gets a wrapper for a property defined in this class.
	//!
	//! @param name             Name of the property to get. If null, then any name will suffice.
	//! @param specifiedClasses A list of classes and postfixes that specify property signature to use.
	VIRTUAL_API virtual const IMonoProperty *GetProperty(const char *name, List<ClassSpec> &specifiedClasses) const = 0;
	//! Gets a wrapper for a property defined in this class.
	//!
	//! @param name           Name of the property to get. If null, then any name will suffice.
	//! @param paramTypeNames A list of full type names that specify the parameters the property accepts.
	VIRTUAL_API virtual const IMonoProperty *GetProperty(const char *name,
												   List<const char *> &paramTypeNames) const = 0;
	//! Gets a wrapper for a property defined in this class.
	//!
	//! @param name       Name of the property to get. If null, then any name will suffice.
	//! @param paramCount Number of arguments the getter or setter of the property should take. If it's
	//!                   equal to -1 then parameter count is ignored.
	VIRTUAL_API virtual const IMonoProperty *GetProperty(const char *name, int paramCount) const = 0;
	//! Gets one of the events defined in this class.
	//!
	//! @param name Name of the event to get.
	VIRTUAL_API virtual const IMonoEvent *GetEvent(const char *name) const = 0;
	//! Gets the class or struct that is defined in this one.
	//!
	//! @param name Name of the class to get.
	VIRTUAL_API virtual const IMonoClass *GetNestedType(const char *name) const = 0;
	//! Determines whether this class inherits from specified class.
	//!
	//! Entire inheritance path will be searched for the specified class.
	//!
	//! @param nameSpace Full name of the name space where the class is located.
	//! @param className Name of the class.
	//!
	//! @returns True, if this class is a subclass of specified one.
	VIRTUAL_API virtual bool Inherits(const char *nameSpace, const char *className) const = 0;
	//! Determines whether this class inherits from specified class.
	//!
	//! Entire inheritance path will be searched for the specified class.
	//!
	//! @param klass Pointer to the wrapper that represents the class the fact of inheritance from which
	//!              must be determined.
	//!
	//! @returns True, if this class is a subclass of specified one.
	VIRTUAL_API virtual bool Inherits(IMonoClass *klass) const = 0;
	//! Determines whether this class inherits from specified class.
	//!
	//! @param nameSpace Full name of the name space where the class is located.
	//! @param className Name of the class.
	//! @param direct    Indicates whether only direct base class should be checked. If false, the entire
	//!                  inheritance path will be searched for specified class.
	//!
	//! @returns True, if this class is a subclass of specified one.
	VIRTUAL_API virtual bool Inherits(const char *nameSpace, const char *className, bool direct) const = 0;
	//! Determines whether this class inherits from specified class.
	//!
	//! @param klass  Pointer to the wrapper that represents the class the fact of inheritance from which
	//!               must be determined.
	//! @param direct Indicates whether only direct base class should be checked. If false, the entire
	//!               inheritance path will be searched for specified class.
	//!
	//! @returns True, if this class is a subclass of specified one.
	VIRTUAL_API virtual bool Inherits(IMonoClass *klass, bool direct) const = 0;
	//! Determines whether this class implements a certain interface.
	//!
	//! @param nameSpace         Full name of the name space where the interface is located.
	//! @param interfaceName     Name of the interface.
	//! @param searchBaseClasses Indicates whether we should look if base classes implement
	//!                          this interface.
	//!
	//! @returns True, if this class implements specified interface.
	VIRTUAL_API virtual bool Implements(const char *nameSpace, const char *interfaceName,
										bool searchBaseClasses = true) const = 0;
	//! Determines whether this class implements a certain interface.
	//!
	//! @param interfacePtr      Pointer to the wrapper that represents the interface the fact of
	//!                          implementation of which must be determined.
	//! @param searchBaseClasses Indicates whether we should look if base classes implement
	//!                          this interface.
	//!
	//! @returns True, if this class implements specified interface.
	VIRTUAL_API virtual bool Implements(IMonoClass *interfacePtr, bool searchBaseClasses = true) const = 0;
	//! Boxes given value.
	//!
	//! @returns Null if this class is not a value-type, or reference to the boxed object, if it is.
	VIRTUAL_API virtual mono::object Box(void *value) const = 0;
	//! Gets an instance of type System.Type that represents this class.
	VIRTUAL_API virtual mono::type GetType() const = 0;
	//! Gets an instance of type System.Type that represents an array of instances of this class.
	VIRTUAL_API virtual mono::type MakeArrayType() const = 0;
	//! Gets an instance of type System.Type that represents an array of instances of this class.
	//!
	//! @param rank Number of dimensions in the array.
	VIRTUAL_API virtual mono::type MakeArrayType(int rank) const = 0;
	//! Gets an instance of type System.Type that represents a reference to objects of this class.
	VIRTUAL_API virtual mono::type MakeByRefType() const = 0;
	//! Gets an instance of type System.Type that represents a pointer to objects of this class.
	VIRTUAL_API virtual mono::type MakePointerType() const = 0;
// 		//! Creates a generic type instantiation where type arguments are substituted with given types.
// 		//!
// 		//! Cache the resultant type, since inflation is quite costly.
// 		//!
// 		//! @param types A list of types to use when inflating this class.
// 		//!
// 		//! @returns A pointer to the cached wrapper that represents inflated generic type, if this type is
// 		//!          generic and no exceptions were raised during the execution, otherwise this pointer will
// 		//!          be returned.
// 		VIRTUAL_API virtual IMonoClass *Inflate(List<IMonoClass *> &types) = 0;

	VIRTUAL_API virtual const char *GetName() const = 0;
	VIRTUAL_API virtual const char *GetNameSpace() const = 0;
	VIRTUAL_API virtual const char *GetFullName() const = 0;
	VIRTUAL_API virtual const char *GetFullNameIL() const = 0;

	VIRTUAL_API virtual bool GetIsValueType() const = 0;
	VIRTUAL_API virtual bool GetIsEnum() const = 0;
	VIRTUAL_API virtual bool GetIsDelegate() const = 0;

	VIRTUAL_API virtual const IMonoAssembly *GetAssembly() const = 0;

	VIRTUAL_API virtual const IMonoClass *GetBase() const = 0;

	VIRTUAL_API virtual const List<IMonoField *>    &GetFields() const = 0;
	VIRTUAL_API virtual const List<IMonoProperty *> &GetProperties() const = 0;
	VIRTUAL_API virtual const List<IMonoEvent *>    &GetEvents() const = 0;
	VIRTUAL_API virtual const List<IMonoFunction *> &GetFunctions() const = 0;

	//! Gets the value of the object's field.
	//!
	//! @param obj   Object which field to get. Use nullptr when working with a static field.
	//! @param name  Name of the field which value to get.
	//!
	//! @seealso IMonoHandle::SetField
	template <typename FieldType> FieldType GetField(mono::object obj, const char *name) const
	{
		FieldType valueContainer;

		this->GetField(obj, name, &valueContainer);

		return valueContainer;
	}
	//! Sets the value of the object's field.
	//!
	//! @param obj   Object which field to set. Use nullptr when working with a static field.
	//! @param name  Name of the field which value to set.
	//! @param value New value to assign to the field.
	//!
	//! @seealso IMonoHandle::SetField
	template <typename FieldType> void AssignField(mono::object obj, const char *name, FieldType value) const
	{
		this->SetField(obj, name, &value);
	}
	//! Sets the value of the object's field.
	//!
	//! @param obj   Object which field to set. Use nullptr when working with a static field.
	//! @param name  Name of the field which value to set.
	//! @param value New value to assign to the field.
	//!
	//! @seealso IMonoHandle::SetField
	void AssignField(mono::object obj, const char *name, mono::object value) const
	{
		this->SetField(obj, name, value);
	}
	//! Gets the value of the object's field.
	//!
	//! @param obj   Object which field to get. Use nullptr when working with a static field.
	//! @param field Wrapper that identifies the field which value to get.
	//!
	//! @seealso IMonoHandle::SetField
	template <typename FieldType> FieldType GetField(mono::object obj, const IMonoField *field) const
	{
		FieldType valueContainer;

		this->GetField(obj, field, &valueContainer);

		return valueContainer;
	}
	//! Sets the value of the object's field.
	//!
	//! @param obj   Object which field to set. Use nullptr when working with a static field.
	//! @param field Wrapper that identifies the field which value to set.
	//! @param value New value to assign to the field.
	//!
	//! @seealso IMonoHandle::SetField
	template <typename FieldType> void AssignField(mono::object obj, const IMonoField *field, FieldType value) const
	{
		this->SetField(obj, field, &value);
	}
	//! Sets the value of the object's field.
	//!
	//! @param obj   Object which field to set. Use nullptr when working with a static field.
	//! @param field Wrapper that identifies the field which value to set.
	//! @param value New value to assign to the field.
	//!
	//! @seealso IMonoHandle::SetField
	void AssignField(mono::object obj, IMonoField *field, mono::object value) const
	{
		this->SetField(obj, field, value);
	}
};