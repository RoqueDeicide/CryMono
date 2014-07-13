﻿using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CryEngine
{
	/// <summary>
	/// Represents Euler angles.
	/// </summary>
	public struct EulerAngles
	{
		#region Fields
		public float Pitch;
		public float Roll;
		public float Yaw;
		#endregion
		#region Properties
		/// <summary>
		/// Gets or sets component of angles specified by given index.
		/// </summary>
		/// <param name="index">Index of component.</param>
		public float this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return this.Pitch;
					case 1:
						return this.Roll;
					case 2:
						return this.Yaw;
					default:
						throw new IndexOutOfRangeException("[Angle3.this[]] Index out of range.");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						this.Pitch = value;
						break;
					case 1:
						this.Roll = value;
						break;
					case 2:
						this.Yaw = value;
						break;
					default:
						throw new IndexOutOfRangeException("[Angle3.this[]] Index out of range.");
				}
			}
		}
		#endregion
		#region Interface
		#region Constructors
		/// <summary>
		/// Creates new instance of <see cref="EulerAngles" /> struct.
		/// </summary>
		/// <param name="pitch">Angle of rotation around X-axis.</param>
		/// <param name="roll">Angle of rotation around Y-axis.</param>
		/// <param name="yaw">Angle of rotation around Z-axis.</param>
		public EulerAngles(float pitch, float roll, float yaw)
		{
			this.Pitch = pitch;
			this.Roll = roll;
			this.Yaw = yaw;
		}
		/// <summary>
		/// Creates new instance of <see cref="EulerAngles" /> struct.
		/// </summary>
		/// <param name="matrix">
		/// Matrix that defines new instance.
		/// </param>
		public EulerAngles(Matrix33 matrix)
		{
			// Assert matrix being orthonormal.
			this.Roll = (float)Math.Asin(Math.Max(-1.0f, Math.Min(1.0f, -matrix.M20)));
			if (Math.Abs(Math.Abs(this.Roll) - (float)(Math.PI * 0.5)) < 0.01f)
			{
				this.Pitch = 0;
				this.Yaw = (float)Math.Atan2(-matrix.M01, matrix.M11);
			}
			else
			{
				this.Pitch = (float)Math.Atan2(matrix.M21, matrix.M22);
				this.Yaw = (float)Math.Atan2(matrix.M10, matrix.M00);
			}
		}
		#endregion
		#region Operators
		#region Comparison Operators
		/// <summary>
		/// Determines whether two instances of <see
		/// cref="EulerAngles" /> struct are equal.
		/// </summary>
		/// <param name="l">Left operand.</param>
		/// <param name="r">Right operand.</param>
		/// <returns>
		/// True, if objects are equal, otherwise false.
		/// </returns>
		public static bool operator ==(EulerAngles l, EulerAngles r)
		{
			return l.Pitch == r.Pitch && l.Roll == r.Roll && l.Yaw == r.Yaw;
		}
		/// <summary>
		/// Determines whether two instances of <see
		/// cref="EulerAngles" /> struct are not equal.
		/// </summary>
		/// <param name="l">Left operand.</param>
		/// <param name="r">Right operand.</param>
		/// <returns>
		/// True, if objects are not equal, otherwise false.
		/// </returns>
		public static bool operator !=(EulerAngles l, EulerAngles r)
		{
			return l.Pitch != r.Pitch && l.Roll != r.Roll && l.Yaw != r.Yaw;
		}
		#endregion
		#region Arithmetic Operators
		/// <summary>
		/// Multiplies given instance of <see cref="EulerAngles" />
		/// struct by given amount.
		/// </summary>
		/// <param name="l">Left operand.</param>
		/// <param name="r">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static EulerAngles operator *(EulerAngles l, float r)
		{
			return new EulerAngles(l.Pitch * r, l.Roll * r, l.Yaw * r);
		}
		/// <summary>
		/// Multiplies given instance of <see cref="EulerAngles" />
		/// struct by given amount.
		/// </summary>
		/// <param name="l">Left operand.</param>
		/// <param name="r">Right operand.</param>
		/// <returns>Result of multiplication.</returns>
		public static EulerAngles operator *(float l, EulerAngles r)
		{
			return new EulerAngles(r.Pitch * l, r.Roll * l, r.Yaw * l);
		}
		/// <summary>
		/// Divides given instance of <see cref="EulerAngles" />
		/// struct by given amount.
		/// </summary>
		/// <param name="l">Left operand.</param>
		/// <param name="r">Right operand.</param>
		/// <returns>Result of division.</returns>
		public static EulerAngles operator /(EulerAngles l, float r)
		{
			return new EulerAngles(l.Pitch / r, l.Roll / r, l.Yaw / r);
		}
		/// <summary>
		/// Divides given instance of <see cref="EulerAngles" />
		/// struct by given amount.
		/// </summary>
		/// <param name="l">Left operand.</param>
		/// <param name="r">Right operand.</param>
		/// <returns>Result of division.</returns>
		public static EulerAngles operator /(float l, EulerAngles r)
		{
			return new EulerAngles(r.Pitch / l, r.Roll / l, r.Yaw / l);
		}
		/// <summary>
		/// Creates negated <see cref="EulerAngles" /> instance.
		/// </summary>
		/// <param name="angle">Object to negate.</param>
		/// <returns>Result.</returns>
		public static EulerAngles operator -(EulerAngles angle)
		{
			return new EulerAngles(-angle.Pitch, -angle.Roll, -angle.Yaw);
		}
		#endregion
		#region Conversion Operators
		/// <summary>
		/// Converts unit quaternion to angles.
		/// </summary>
		/// <param name="quaternion">Quaternion to convert.</param>
		/// <returns>Result of conversion.</returns>
		public static explicit operator EulerAngles(Quaternion quaternion)
		{
			EulerAngles result = new EulerAngles();
			result.Roll = System.Convert.ToSingle(Math.Asin(Math.Max(-1.0f, Math.Min(1.0f, -(quaternion.V.X * quaternion.V.Z - quaternion.W * quaternion.V.Y) * 2))));
			if (Math.Abs(Math.Abs(result.Roll) - (float)(Math.PI * 0.5)) < 0.01f)
			{
				result.Pitch = 0;
				result.Yaw = (float)(Math.Atan2(-2 * (quaternion.V.X * quaternion.V.Y - quaternion.W * quaternion.V.Z), 1 - (quaternion.V.X * quaternion.V.X + quaternion.V.Z * quaternion.V.Z) * 2));
			}
			else
			{
				result.Pitch = (float)(Math.Atan2(-2 * (quaternion.V.Y * quaternion.V.Z - quaternion.W * quaternion.V.X), 1 - (quaternion.V.X * quaternion.V.X + quaternion.V.Y * quaternion.V.Y) * 2));
				result.Yaw = (float)(Math.Atan2(-2 * (quaternion.V.X * quaternion.V.Y - quaternion.W * quaternion.V.Z), 1 - (quaternion.V.Z * quaternion.V.Z + quaternion.V.Y * quaternion.V.Y) * 2));
			}
			return result;
		}
		/// <summary>
		/// Creates vector representation of given Euler angle.
		/// </summary>
		/// <param name="angle">Angle to convert.</param>
		/// <returns>Equivalent of Euler angle.</returns>
		public static explicit operator Vector3(EulerAngles angle)
		{
			return new Vector3(angle.Pitch, angle.Roll, angle.Yaw);
		}
		#endregion
		#endregion
		#region Overrides
		/// <summary>
		/// Gets hash code of this object.
		/// </summary>
		/// <returns>Hash code of this object.</returns>
		public override int GetHashCode()
		{
			return this.Pitch.GetHashCode() * 11 + this.Roll.GetHashCode() * 37 + this.Yaw.GetHashCode() * 23;
		}
		/// <summary>
		/// Checks equality of this object and given one.
		/// </summary>
		/// <param name="obj">Given object.</param>
		/// <returns>
		/// True, if given object is <see cref="EulerAngles" /> equal
		/// to this instance, otherwise false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (obj is EulerAngles)
			{
				EulerAngles o = (EulerAngles)obj;
				return o.Pitch == this.Pitch && this.Roll == o.Roll && this.Yaw == o.Yaw;
			}
			return false;
		}
		#endregion
		#endregion
		#region Utilities
		#endregion
	}
}