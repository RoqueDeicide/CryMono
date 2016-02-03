﻿using System;
using System.Reflection;
using CryCil.RunTime.Registration;

namespace CryCil.Engine.Logic
{
	/// <summary>
	/// Encapsulates description of the editable entity property.
	/// </summary>
	public struct EditablePropertyInfo
	{
		#region Fields
		private readonly string name;
		private readonly EditablePropertyType type;
		private readonly string editType;
		private readonly string description;
		private readonly uint flags;
		private readonly Vector2 limits;
		private readonly string defaultValue;
		#endregion
		#region Properties
		/// <summary>
		/// Name of the property.
		/// </summary>
		public string Name
		{
			get { return this.name; }
		}
		/// <summary>
		/// Type of the property.
		/// </summary>
		public EditablePropertyType Type
		{
			get { return this.type; }
		}
		/// <summary>
		/// Prefix that designates which edit control to use.
		/// </summary>
		public string EditType
		{
			get { return this.editType; }
		}
		/// <summary>
		/// Description of the property.
		/// </summary>
		public string Description
		{
			get { return this.description; }
		}
		/// <summary>
		/// A set of flags that describes the property.
		/// </summary>
		/// <remarks>Not used.</remarks>
		public uint Flags
		{
			get { return this.flags; }
		}
		/// <summary>
		/// 2D vector where <see cref="Vector2.X"/> field designates minimal value this property can take
		/// and <see cref="Vector2.Y"/> field designates maximal one.
		/// </summary>
		public Vector2 Limits
		{
			get { return this.limits; }
		}
		/// <summary>
		/// Text representation of the default value of this property.
		/// </summary>
		public string DefaultValue
		{
			get { return this.defaultValue; }
		}
		#endregion
		#region Construction
		/// <summary>
		/// Creates new instance of this type.
		/// </summary>
		/// <param name="property">An object that represents the editable property.</param>
		/// <exception cref="Exception">
		/// Invalid member was passed to EditablePropertyInfo constructor.
		/// </exception>
		/// <exception cref="Exception">
		/// Ui control specified for the property cannot be used for it.
		/// </exception>
		/// <exception cref="TypeLoadException">The custom attribute type cannot be loaded.</exception>
		public EditablePropertyInfo(EditableProperty property)
		{
			MemberInfo member = property.Member;
			Type memberType = member.GetAssociatedType();
			EditablePropertyAttribute attribute = member.GetAttribute<EditablePropertyAttribute>();
			if (attribute == null)
			{
				throw new Exception("Invalid member was passed to EditablePropertyInfo constructor.");
			}
			this.name = member.Name;
			this.type = property.Type;

			var typeDesc = EntityRegistry.EditablePropertyUiMap[(int)attribute.UiControl];
			// Gotta validate that type.
			if (typeDesc.ManagedType != memberType)
			{
				string error =
					string.Format("Ui control specified for the property {0} of type {1} cannot be used: it works only " +
								  "with properties of type {2}",
								  member.Name, memberType.FullName, typeDesc.ManagedType.FullName);
				throw new Exception(error);
			}
			this.editType = typeDesc.Prefix;

			this.description = attribute.Description;
			this.flags = 0;
			this.limits = attribute.Limits;

			if (attribute.DefaultValue == null)
			{
				this.defaultValue = string.Empty;
			}
			else if (attribute.DefaultValue.GetType() != typeDesc.ManagedType)
			{
				string error =
					string.Format("Default value for the property {0} of type {1} is of incompatible type {2}.",
								  member.Name, memberType.FullName, attribute.DefaultValue.GetType().FullName);
				throw new Exception(error);
			}
			else
			{
				this.defaultValue = attribute.DefaultValue.ToString();
			}
		}
		/// <summary>
		/// Creates new property info for a folder.
		/// </summary>
		/// <param name="folderName">Name of the folder.</param>
		/// <param name="start">     Indicates whether this is a beginning of the folder.</param>
		public EditablePropertyInfo(string folderName, bool start)
			: this()
		{
			this.name = folderName;
			this.type = start ? EditablePropertyType.FolderBegin : EditablePropertyType.FolderEnd;
		}
		#endregion
	}
}