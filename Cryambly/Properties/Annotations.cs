﻿using System;
using System.Linq;

// ReSharper disable UnusedMember.Global

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable UnusedAutoPropertyAccessor.Global

// ReSharper disable IntroduceOptionalParameters.Global

// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable InconsistentNaming

// ReSharper disable CheckNamespace
namespace CryCil.Annotations
{
	/// <summary>
	/// Indicates that the value of the marked element could be <c>null</c> sometimes, so the check for
	/// <c>null</c> is necessary before its usage.
	/// </summary>
	/// <example>
	/// <code>
	/// [CanBeNull]
	/// public object Test()
	/// {
	///     return null;
	/// }
	///
	/// public void UseTest()
	/// {
	///     var p = Test();
	///     var s = p.ToString(); // Warning: Possible 'System.NullReferenceException'
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
					AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class CanBeNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that the value of the marked element could never be <c>null</c>.
	/// </summary>
	/// <example>
	/// <code>
	/// [NotNull]
	/// public object Foo()
	/// {
	///   return null; // Warning: Possible 'null' assignment
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
					AttributeTargets.Delegate | AttributeTargets.Field | AttributeTargets.Event)]
	public sealed class NotNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that collection or enumerable value does not contain null elements.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
					AttributeTargets.Delegate | AttributeTargets.Field)]
	public sealed class ItemNotNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that collection or enumerable value can contain null elements.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property |
					AttributeTargets.Delegate | AttributeTargets.Field)]
	public sealed class ItemCanBeNullAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that the marked method builds string by format pattern and (optional) arguments.
	/// Parameter, which contains format string, should be given in constructor. The format string should be
	/// in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form.
	/// </summary>
	/// <example>
	/// <code>
	/// [StringFormatMethod("message")]
	/// public void ShowError(string message, params object[] args)
	/// {
	///     /* do something */
	/// }
	///
	/// public void Foo()
	/// {
	///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method |
					AttributeTargets.Property | AttributeTargets.Delegate)]
	public sealed class StringFormatMethodAttribute : Attribute
	{
		/// <param name="formatParameterName">
		/// Specifies which parameter of an annotated method should be treated as format-string
		/// </param>
		public StringFormatMethodAttribute(string formatParameterName)
		{
			this.FormatParameterName = formatParameterName;
		}
		/// <summary>
		/// Gets the name of the parameter that is used as a format string.
		/// </summary>
		public string FormatParameterName { get; private set; }
	}

	/// <summary>
	/// For a parameter that is expected to be one of the limited set of values. Specify fields of which
	/// type should be used as values for this parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class ValueProviderAttribute : Attribute
	{
		/// <summary>
		/// Creates an object of this type.
		/// </summary>
		/// <param name="name">
		/// The name of the parameter, property or field that is expected to only accept a limited set of
		/// values.
		/// </param>
		public ValueProviderAttribute(string name)
		{
			this.Name = name;
		}
		/// <summary>
		/// Gets the name of the parameter, property or field that is expected to only accept a limited set
		/// of values.
		/// </summary>
		[NotNull]
		public string Name { get; private set; }
	}

	/// <summary>
	/// Indicates that the function argument should be string literal and match one of the parameters of the
	/// caller function. For example, ReSharper annotates the parameter of
	/// <see cref="System.ArgumentNullException"/>.
	/// </summary>
	/// <example>
	/// <code>
	/// public void Foo(string param)
	/// {
	///     if (param == null)
	///     {
	///         throw new ArgumentNullException("par"); // Warning: Cannot resolve symbol
	///     }
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class InvokerParameterNameAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that the method is contained in a type that implements
	/// <c>System.ComponentModel.INotifyPropertyChanged</c> interface and this method is used to notify that
	/// some property value changed.
	/// </summary>
	/// <remarks>
	/// The method should be non-static and conform to one of the supported signatures:
	/// <list>
	/// <item><c>NotifyChanged(string)</c></item>
	/// <item><c>NotifyChanged(params string[])</c></item>
	/// <item><c>NotifyChanged{T}(Expression{Func{T}})</c></item>
	/// <item><c>NotifyChanged{T,U}(Expression{Func{T,U}})</c></item>
	/// <item><c>SetProperty{T}(ref T, T, string)</c></item>
	/// </list>
	/// </remarks>
	/// <example>
	/// <code>
	/// public class Foo : INotifyPropertyChanged
	/// {
	///     public event PropertyChangedEventHandler PropertyChanged;
	///     [NotifyPropertyChangedInvocator]
	///     protected virtual void NotifyChanged(string propertyName) { ... }
	///
	///     private string _name;
	///     public string Name
	///     {
	///         get { return _name; }
	///         set { _name = value; NotifyChanged("LastName"); /* Warning */ }
	///     }
	/// }
	/// </code>
	/// Examples of generated notifications:
	/// <list>
	/// <item><c>NotifyChanged("Property")</c></item>
	/// <item><c>NotifyChanged(() =&gt; Property)</c></item>
	/// <item><c>NotifyChanged((VM x) =&gt; x.Property)</c></item>
	/// <item><c>SetProperty(ref myField, value, "Property")</c></item>
	/// </list>
	/// </example>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		public NotifyPropertyChangedInvocatorAttribute()
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="parameterName">
		/// The name of the parameter that represents the name(s) of changed propert(y|ies).
		/// </param>
		public NotifyPropertyChangedInvocatorAttribute(string parameterName)
		{
			this.ParameterName = parameterName;
		}
		/// <summary>
		/// Gets the name of the parameter that represents the name(s) of changed propert(y|ies).
		/// </summary>
		public string ParameterName { get; private set; }
	}

	/// <summary>
	/// Describes dependency between method input and output.
	/// </summary>
	/// <syntax>
	/// <p>Function Definition Table syntax:</p>
	/// <list>
	/// <item>FDT ::= FDTRow [;FDTRow]*</item>
	/// <item>FDTRow ::= Input =&gt; Output | Output &lt;= Input</item>
	/// <item>Input ::= ParameterName: Value [, Input]*</item>
	/// <item>Output ::= [ParameterName: Value]* {halt|stop|void|nothing|Value}</item>
	/// <item>Value ::= true | false | null | notnull | canbenull</item>
	/// </list>
	/// If method has single input parameter, it's name could be omitted. <br/> Using <c>halt</c> (or
	/// <c>void</c>/ <c>nothing</c>, which is the same) for method output means that the method doesn't
	/// return normally. <br/><c>canbenull</c> annotation is only applicable for output parameters. <br/>
	/// You can use multiple <c>[ContractAnnotation]</c> for each FDT row, or use single attribute with rows
	/// separated by semicolon. <br/>
	/// </syntax>
	/// <examples>
	/// <list>
	/// <item>
	/// <code>
	/// [ContractAnnotation("=&gt; halt")]
	/// public void TerminationMethod()
	/// </code>
	/// </item>
	/// <item>
	/// <code>
	/// [ContractAnnotation("halt &lt;= condition: false")]
	/// public void Assert(bool condition, string text) // regular assertion method
	/// </code>
	/// </item>
	/// <item>
	/// <code>
	/// [ContractAnnotation("s:null =&gt; true")]
	/// public bool IsNullOrEmpty(string s) // string.IsNullOrEmpty()
	/// </code>
	/// </item>
	/// <item>
	/// <code>
	/// // A method that returns null if the parameter is null,
	/// // and not null if the parameter is not null
	/// [ContractAnnotation("null =&gt; null; notnull =&gt; notnull")]
	/// public object Transform(object data)
	/// </code>
	/// </item>
	/// <item>
	/// <code>
	/// [ContractAnnotation("s:null=&gt;false; =&gt;true,result:notnull; =&gt;false, result:null")]
	/// public bool TryParse(string s, out Person result)
	/// </code>
	/// </item>
	/// </list>
	/// </examples>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public sealed class ContractAnnotationAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="contract">A string that defines a contract.</param>
		public ContractAnnotationAttribute([NotNull] string contract)
			: this(contract, false)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="contract">       A string that defines a contract.</param>
		/// <param name="forceFullStates">Unknown.</param>
		public ContractAnnotationAttribute([NotNull] string contract, bool forceFullStates)
		{
			this.Contract = contract;
			this.ForceFullStates = forceFullStates;
		}
		/// <summary>
		/// Gets a string that defines a contract.
		/// </summary>
		public string Contract { get; private set; }
		/// <summary>
		/// Unknown.
		/// </summary>
		public bool ForceFullStates { get; private set; }
	}

	/// <summary>
	/// Indicates that marked element should be localized or not.
	/// </summary>
	/// <example>
	/// <code>
	/// [LocalizationRequiredAttribute(true)]
	/// public class Foo
	/// {
	///     private string str = "my string"; // Warning: Localizable string
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class LocalizationRequiredAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		public LocalizationRequiredAttribute() : this(true)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="required">Indicates whether localization is required.</param>
		public LocalizationRequiredAttribute(bool required)
		{
			this.Required = required;
		}
		/// <summary>
		/// Indicates whether localization is required.
		/// </summary>
		public bool Required { get; private set; }
	}

	/// <summary>
	/// Indicates that the value of the marked type (or its derivatives) cannot be compared using '==' or
	/// '!=' operators and <c>Equals()</c> should be used instead. However, using '==' or '!=' for
	/// comparison with <c>null</c> is always permitted.
	/// </summary>
	/// <example>
	/// <code>
	/// [CannotApplyEqualityOperator]
	/// class NoEquality
	/// {
	/// }
	///
	/// class UsesNoEquality
	/// {
	///     public void Test()
	///     {
	///         var ca1 = new NoEquality();
	///         var ca2 = new NoEquality();
	///         if (ca1 != null) // OK
	///         {
	///             bool condition = ca1 == ca2; // Warning
	///         }
	///     }
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class CannotApplyEqualityOperatorAttribute : Attribute
	{
	}

	/// <summary>
	/// When applied to a target attribute, specifies a requirement for any type marked with the target
	/// attribute to implement or inherit specific type or types.
	/// </summary>
	/// <example>
	/// <code>
	/// [BaseTypeRequired(typeof(IComponent)] // Specify requirement
	/// public class ComponentAttribute : Attribute
	/// {
	/// }
	///
	/// [Component] // ComponentAttribute requires implementing IComponent interface
	/// public class MyComponent : IComponent
	/// {
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	[BaseTypeRequired(typeof(Attribute))]
	public sealed class BaseTypeRequiredAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="baseType">The required base type.</param>
		public BaseTypeRequiredAttribute([NotNull] Type baseType)
		{
			this.BaseType = baseType;
		}
		/// <summary>
		/// Gets the type that must be inherited by the class that is marked by this attribute.
		/// </summary>
		[NotNull]
		public Type BaseType { get; private set; }
	}

	/// <summary>
	/// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external library), so
	/// this symbol will not be marked as unused (as well as by other usage inspections).
	/// </summary>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class UsedImplicitlyAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		public UsedImplicitlyAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="useKindFlags">A set of flags that specify how the implicit usage is done.</param>
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="targetFlags">
		/// A set of flags that specify which parts of the marked entity are used implicitly.
		/// </param>
		public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="useKindFlags">A set of flags that specify how the implicit usage is done.</param>
		/// <param name="targetFlags"> 
		/// A set of flags that specify which parts of the marked entity are used implicitly.
		/// </param>
		public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			this.UseKindFlags = useKindFlags;
			this.TargetFlags = targetFlags;
		}
		/// <summary>
		/// Gets a set of flags that specify how the implicit usage is done.
		/// </summary>
		public ImplicitUseKindFlags UseKindFlags { get; private set; }
		/// <summary>
		/// Gets a set of flags that specify which parts of the marked entity are used implicitly.
		/// </summary>
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}

	/// <summary>
	/// Should be used on attributes and causes ReSharper to not mark symbols marked with such attributes as
	/// unused (as well as by other usage inspections)
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter)]
	public sealed class MeansImplicitUseAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		public MeansImplicitUseAttribute()
			: this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="useKindFlags">A set of flags that specify how the implicit usage is done.</param>
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
			: this(useKindFlags, ImplicitUseTargetFlags.Default)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="targetFlags">
		/// A set of flags that specify which parts of the marked entity are used implicitly.
		/// </param>
		public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
			: this(ImplicitUseKindFlags.Default, targetFlags)
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="useKindFlags">A set of flags that specify how the implicit usage is done.</param>
		/// <param name="targetFlags"> 
		/// A set of flags that specify which parts of the marked entity are used implicitly.
		/// </param>
		public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
		{
			this.UseKindFlags = useKindFlags;
			this.TargetFlags = targetFlags;
		}
		/// <summary>
		/// Gets a set of flags that specify how the implicit usage is done.
		/// </summary>
		public ImplicitUseKindFlags UseKindFlags { get; private set; }
		/// <summary>
		/// Gets a set of flags that specify which parts of the marked entity are used implicitly.
		/// </summary>
		public ImplicitUseTargetFlags TargetFlags { get; private set; }
	}
	/// <summary>
	/// Enumeration of flags that specify the kind of implicit usage.
	/// </summary>
	[Flags]
	public enum ImplicitUseKindFlags
	{
		/// <summary>
		/// Default set of flags.
		/// </summary>
		Default = Access | Assign | InstantiatedWithFixedConstructorSignature,
		/// <summary>
		/// Only entity marked with attribute considered used.
		/// </summary>
		Access = 1,
		/// <summary>
		/// Indicates implicit assignment to a member.
		/// </summary>
		Assign = 2,
		/// <summary>
		/// Indicates implicit instantiation of a type with fixed constructor signature. That means any
		/// unused constructor parameters won't be reported as such.
		/// </summary>
		InstantiatedWithFixedConstructorSignature = 4,
		/// <summary>
		/// Indicates implicit instantiation of a type.
		/// </summary>
		InstantiatedNoFixedConstructorSignature = 8,
	}

	/// <summary>
	/// Specify what is considered used implicitly when marked with <see cref="MeansImplicitUseAttribute"/>
	/// or <see cref="UsedImplicitlyAttribute"/>.
	/// </summary>
	[Flags]
	public enum ImplicitUseTargetFlags
	{
		/// <summary>
		/// Default set of flags.
		/// </summary>
		Default = Itself,
		/// <summary>
		/// The entity that is marked with attribute is considered used.
		/// </summary>
		Itself = 1,
		/// <summary>
		/// Members of entity marked with attribute are considered used.
		/// </summary>
		Members = 2,
		/// <summary>
		/// Entity marked with attribute and all its members considered used.
		/// </summary>
		WithMembers = Itself | Members
	}

	/// <summary>
	/// This attribute is intended to mark publicly available API which should not be removed and so is
	/// treated as used.
	/// </summary>
	[MeansImplicitUse(ImplicitUseTargetFlags.WithMembers)]
	public sealed class PublicAPIAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		public PublicAPIAttribute()
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="comment">A simple message.</param>
		public PublicAPIAttribute([NotNull] string comment)
		{
			this.Comment = comment;
		}
		/// <summary>
		/// Gets a message that is associated with the marked code entity.
		/// </summary>
		public string Comment { get; private set; }
	}

	/// <summary>
	/// Tells code analysis engine if the parameter is completely handled when the invoked method is on
	/// stack. If the parameter is a delegate, indicates that delegate is executed while the method is
	/// executed. If the parameter is an enumerable, indicates that it is enumerated while the method is
	/// executed.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class InstantHandleAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that a method does not make any observable state changes. The same as
	/// <c>System.Diagnostics.Contracts.PureAttribute</c>.
	/// </summary>
	/// <example>
	/// <code>
	/// [Pure]
	/// private int Multiply(int x, int y)
	/// {
	///     return x * y;
	/// }
	///
	/// public void Foo()
	/// {
	///     const int a = 2, b = 2;
	///     Multiply(a, b); // Waring: Return value of pure method is not used
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class PureAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that a parameter is a path to a file or a folder within a web project. Path can be
	/// relative or absolute, starting from web root (~).
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class PathReferenceAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		public PathReferenceAttribute()
		{
		}
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="basePath">A base path to use, if provided path is relative.</param>
		public PathReferenceAttribute([PathReference] string basePath)
		{
			this.BasePath = basePath;
		}
		/// <summary>
		/// Gets a base path to use, if provided path is relative.
		/// </summary>
		public string BasePath { get; private set; }
	}

	/// <summary>
	/// An extension method marked with this attribute is processed by ReSharper code completion as a
	/// 'Source Template'. When extension method is completed over some expression, it's source code is
	/// automatically expanded like a template at call site.
	/// </summary>
	/// <remarks>
	/// Template method body can contain valid source code and/or special comments starting with '$'. Text
	/// inside these comments is added as source code when the template is applied. Template parameters can
	/// be used either as additional method parameters or as identifiers wrapped in two '$' signs. Use the
	/// <see cref="MacroAttribute"/> attribute to specify macros for parameters.
	/// </remarks>
	/// <example>
	/// In this example, the 'forEach' method is a source template available over all values of enumerable
	/// types, producing ordinary C# 'foreach' statement and placing caret inside block:
	/// <code>
	/// [SourceTemplate]
	/// public static void forEach&lt;T&gt;(this IEnumerable&lt;T&gt; xs)
	/// {
	///     foreach (var x in xs)
	///     {
	///         //$ $END$
	///     }
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class SourceTemplateAttribute : Attribute
	{
	}

	/// <summary>
	/// Allows specifying a macro for a parameter of a <see cref="SourceTemplateAttribute">source
	/// template</see>.
	/// </summary>
	/// <remarks>
	/// You can apply the attribute on the whole method or on any of its additional parameters. The macro
	/// expression is defined in the <see cref="MacroAttribute.Expression"/> property. When applied on a
	/// method, the target template parameter is defined in the <see cref="MacroAttribute.Target"/>
	/// property. To apply the macro silently for the parameter, set the
	/// <see cref="MacroAttribute.Editable"/> property value = -1.
	/// </remarks>
	/// <example>
	/// Applying the attribute on a source template method:
	/// <code>
	/// [SourceTemplate, Macro(Target = "item", Expression = "suggestVariableName()")]
	/// public static void forEach&lt;T&gt;(this IEnumerable&lt;T&gt; collection)
	/// {
	///     foreach (var item in collection)
	///     {
	///         //$ $END$
	///     }
	/// }
	/// </code>
	/// Applying the attribute on a template method parameter:
	/// <code>
	/// [SourceTemplate]
	/// public static void something(this Entity x, [Macro(Expression = "guid()", Editable = -1)] string newguid)
	/// {
	///     /*$ var $x$Id = "$newguid$" + x.ToString();
	///     x.DoSomething($x$Id); */
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method, AllowMultiple = true)]
	public sealed class MacroAttribute : Attribute
	{
		/// <summary>
		/// Allows specifying a macro that will be executed for a <see cref="SourceTemplateAttribute">source
		/// template</see> parameter when the template is expanded.
		/// </summary>
		public string Expression { get; set; }

		/// <summary>
		/// Allows specifying which occurrence of the target parameter becomes editable when the template is
		/// deployed.
		/// </summary>
		/// <remarks>
		/// If the target parameter is used several times in the template, only one occurrence becomes
		/// editable; other occurrences are changed synchronously. To specify the zero-based index of the
		/// editable occurrence, use values &gt;= 0. To make the parameter non-editable when the template is
		/// expanded, use -1.
		/// </remarks>
		/// &gt;
		public int Editable { get; set; }

		/// <summary>
		/// Identifies the target parameter of a <see cref="SourceTemplateAttribute">source template</see>
		/// if the <see cref="MacroAttribute"/> is applied on a template method.
		/// </summary>
		public string Target { get; set; }
	}
	/// <summary>
	/// Indicates how method invocation affects content of the collection.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class CollectionAccessAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="collectionAccessType">
		/// A set of flags that specify how the collection is accessed.
		/// </param>
		public CollectionAccessAttribute(CollectionAccessType collectionAccessType)
		{
			this.CollectionAccessType = collectionAccessType;
		}
		/// <summary>
		/// Gets a set of flags that specify how the collection is accessed.
		/// </summary>
		public CollectionAccessType CollectionAccessType { get; private set; }
	}
	/// <summary>
	/// Enumeration of flags that specify how the collection is accessed.
	/// </summary>
	[Flags]
	public enum CollectionAccessType
	{
		/// <summary>
		/// Method does not use or modify content of the collection.
		/// </summary>
		None = 0,
		/// <summary>
		/// Method only reads content of the collection but does not modify it.
		/// </summary>
		Read = 1,
		/// <summary>
		/// Method can change content of the collection but does not add new elements.
		/// </summary>
		ModifyExistingContent = 2,
		/// <summary>
		/// Method can add new elements to the collection.
		/// </summary>
		UpdatedContent = ModifyExistingContent | 4
	}

	/// <summary>
	/// Indicates that the marked method is assertion method, i.e. it halts control flow if one of the
	/// conditions is satisfied. To set the condition, mark one of the parameters with
	/// <see cref="AssertionConditionAttribute"/> attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class AssertionMethodAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates the condition parameter of the assertion method. The method itself should be marked by
	/// <see cref="AssertionMethodAttribute"/> attribute. The mandatory argument of the attribute is the
	/// assertion type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class AssertionConditionAttribute : Attribute
	{
		/// <summary>
		/// Creates a new object of this type.
		/// </summary>
		/// <param name="conditionType">
		/// Specifies the boolean operation to perform when checking assertion.
		/// </param>
		public AssertionConditionAttribute(AssertionConditionType conditionType)
		{
			this.ConditionType = conditionType;
		}
		/// <summary>
		/// Gets a value that specifies the boolean operation to perform when checking assertion.
		/// </summary>
		public AssertionConditionType ConditionType { get; private set; }
	}

	/// <summary>
	/// Specifies assertion type. If the assertion method argument satisfies the condition, then the
	/// execution continues. Otherwise, execution is assumed to be halted.
	/// </summary>
	public enum AssertionConditionType
	{
		/// <summary>
		/// Marked parameter should be evaluated to true.
		/// </summary>
		IS_TRUE = 0,
		/// <summary>
		/// Marked parameter should be evaluated to false.
		/// </summary>
		IS_FALSE = 1,
		/// <summary>
		/// Marked parameter should be evaluated to null value.
		/// </summary>
		IS_NULL = 2,
		/// <summary>
		/// Marked parameter should be evaluated to not null value.
		/// </summary>
		IS_NOT_NULL = 3,
	}

	/// <summary>
	/// Indicates that the marked method unconditionally terminates control flow execution. For example, it
	/// could unconditionally throw exception.
	/// </summary>
	[Obsolete("Use [ContractAnnotation('=> halt')] instead")]
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class TerminatesProgramAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that method is pure LINQ method, with postponed enumeration (like Enumerable.Select,
	/// .Where). This annotation allows inference of [InstantHandle] annotation for parameters of delegate
	/// type by analyzing LINQ method chains.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class LinqTunnelAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that IEnumerable, passed as parameter, is not enumerated.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class NoEnumerationAttribute : Attribute
	{
	}

	/// <summary>
	/// Indicates that parameter is regular expression pattern.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class RegexPatternAttribute : Attribute
	{
	}

	/// <summary>
	/// XAML attribute. Indicates the type that has <c>ItemsSource</c> property and should be treated as
	/// <c>ItemsControl</c>-derived type, to enable inner items <c>DataContext</c> type resolve.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class XamlItemsControlAttribute : Attribute
	{
	}

	/// <summary>
	/// XAML attribute. Indicates the property of some <c>BindingBase</c>-derived type, that is used to bind
	/// some item of <c>ItemsControl</c>-derived type. This annotation will enable the <c>DataContext</c>
	/// type resolve for XAML bindings for such properties.
	/// </summary>
	/// <remarks>
	/// Property should have the tree ancestor of the <c>ItemsControl</c> type or marked with the
	/// <see cref="XamlItemsControlAttribute"/> attribute.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property)]
	public sealed class XamlItemBindingOfItemsControlAttribute : Attribute
	{
	}
	/// <summary>
	/// Prevents the Member Reordering feature from tossing members of the marked class.
	/// </summary>
	/// <remarks>The attribute must be mentioned in your member reordering patterns</remarks>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class NoReorder : Attribute
	{
	}
}