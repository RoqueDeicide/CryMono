﻿using System;
using System.Linq;

namespace CryCil
{
	public static partial class Interpolation
	{
		/// <summary>
		/// Cubic linear interpolation is a type of linear interpolation where the parameter is converted
		/// into one-dimensional cubic spline. This allows achieving results that closely resemble spherical
		/// interpolation when normalized.
		/// </summary>
		public static class Cubic
		{
			/// <summary>
			/// Applies linear cubic interpolation to the given vector.
			/// </summary>
			/// <param name="result">   Result of linear cubic interpolation.</param>
			/// <param name="first">    First vector.</param>
			/// <param name="second">   Second vector.</param>
			/// <param name="parameter">
			/// Parameter that determines position of resultant vector on the line that goes through the
			/// first and second vector relative to the first one.
			/// </param>
			public static void Apply(out Vector2 result, Vector2 first, Vector2 second, float parameter)
			{
				result = first + (second - first) * (parameter * parameter * (3.0f - 2.0f * parameter));
			}
			/// <summary>
			/// Creates a vector that is a result of linear cubic interpolation.
			/// </summary>
			/// <param name="first">    First vector.</param>
			/// <param name="second">   Second vector.</param>
			/// <param name="parameter">
			/// Parameter that determines position of resultant vector on the line that goes through the
			/// first and second vector relative to the first one.
			/// </param>
			/// <returns>Result of interpolation.</returns>
			public static Vector2 Create(Vector2 first, Vector2 second, float parameter)
			{
				return first + (second - first) * (parameter * parameter * (3.0f - 2.0f * parameter));
			}
			/// <summary>
			/// Applies linear cubic interpolation to the given vector.
			/// </summary>
			/// <param name="result">   Result of linear cubic interpolation.</param>
			/// <param name="first">    First vector.</param>
			/// <param name="second">   Second vector.</param>
			/// <param name="parameter">
			/// Parameter that determines position of resultant vector on the line that goes through the
			/// first and second vector relative to the first one.
			/// </param>
			public static void Apply(out Vector3 result, Vector3 first, Vector3 second, float parameter)
			{
				result = first + (second - first) * (parameter * parameter * (3.0f - 2.0f * parameter));
			}
			/// <summary>
			/// Creates a vector that is a result of linear cubic interpolation.
			/// </summary>
			/// <param name="first">    First vector.</param>
			/// <param name="second">   Second vector.</param>
			/// <param name="parameter">
			/// Parameter that determines position of resultant vector on the line that goes through the
			/// first and second vector relative to the first one.
			/// </param>
			/// <returns>Result of interpolation.</returns>
			public static Vector3 Create(Vector3 first, Vector3 second, float parameter)
			{
				return first + (second - first) * (parameter * parameter * (3.0f - 2.0f * parameter));
			}
			/// <summary>
			/// Applies linear cubic interpolation to the given vector.
			/// </summary>
			/// <param name="result">   Result of linear cubic interpolation.</param>
			/// <param name="first">    First vector.</param>
			/// <param name="second">   Second vector.</param>
			/// <param name="parameter">
			/// Parameter that determines position of resultant vector on the line that goes through the
			/// first and second vector relative to the first one.
			/// </param>
			public static void Apply(out Vector4 result, Vector4 first, Vector4 second, float parameter)
			{
				result = first + (second - first) * (parameter * parameter * (3.0f - 2.0f * parameter));
			}
			/// <summary>
			/// Creates a vector that is a result of linear cubic interpolation.
			/// </summary>
			/// <param name="first">    First vector.</param>
			/// <param name="second">   Second vector.</param>
			/// <param name="parameter">
			/// Parameter that determines position of resultant vector on the line that goes through the
			/// first and second vector relative to the first one.
			/// </param>
			/// <returns>Result of interpolation.</returns>
			public static Vector4 Create(Vector4 first, Vector4 second, float parameter)
			{
				return first + (second - first) * (parameter * parameter * (3.0f - 2.0f * parameter));
			}
		}
	}
}