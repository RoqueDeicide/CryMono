﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using CryCil.Annotations;
using CryCil.Engine.Physics.Primitives;
using CryCil.Geometry;

namespace CryCil.Engine.Physics
{
	/// <summary>
	/// Represents an object that defines the shape of geometric object that is used by the physics
	/// subsystem.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct GeometryShape
	{
		#region Fields
		private readonly IntPtr handle;
		#endregion
		#region Properties
		/// <summary>
		/// Indicates whether this instance is usable.
		/// </summary>
		public bool IsValid => this.handle != IntPtr.Zero;
		/// <summary>
		/// Gets the type of this geometry object, it can be a triangular mesh or some primitive.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public GeometryTypes GeometryType
		{
			get
			{
				this.AssertInstance();

				return GetGeometryType(this.handle);
			}
		}
		/// <summary>
		/// Gets the pointer to the internal structure that represents this geometric object. See Remarks
		/// for details.
		/// </summary>
		/// <remarks>
		/// You can cast returned pointer to one of the corresponding primitives that is specified by
		/// <see cref="P:CryCil.Engine.Physics.GeometryShape.GeometryType"/>.
		/// </remarks>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public Primitive.BasePrimitive* Data
		{
			get
			{
				this.AssertInstance();

				return GetData(this.handle);
			}
		}
		/// <summary>
		/// Gets the primitive that represents a bounding box for this geometry object.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public Primitive.Box BoundingBox
		{
			get
			{
				this.AssertInstance();

				Primitive.Box box;
				GetBBox(this.handle, out box);
				return box;
			}
		}
		/// <summary>
		/// Gets the value that indicates whether this geometric object is convex.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public bool IsConvex
		{
			get
			{
				this.AssertInstance();

				return IsConvexInternal(this.handle, MathHelpers.ZeroTolerance) != 0;
			}
		}
		/// <summary>
		/// Gets the number of primitive objects that comprise this one.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public int PrimitiveCount
		{
			get
			{
				this.AssertInstance();

				return GetPrimitiveCount(this.handle);
			}
		}
		/// <summary>
		/// Gets the volume of this geometric object.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public float Volume
		{
			get
			{
				this.AssertInstance();

				return GetVolume(this.handle);
			}
		}
		/// <summary>
		/// Gets the coordinates of center of mass of this geometric object.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public Vector3 Center
		{
			get
			{
				this.AssertInstance();

				return GetCenter(this.handle);
			}
		}
		/// <summary>
		/// Gets the number of subtractions this object has survived so far.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public int SurvivedSubtractions
		{
			get
			{
				this.AssertInstance();

				return GetSubtractionsCount(this.handle);
			}
		}
		/// <summary>
		/// Gets or sets the object that represents the foreign data that can be associated with this
		/// geometric object.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public ForeignData ForeignData
		{
			get
			{
				this.AssertInstance();

				return GetForeignData(this.handle);
			}
			set
			{
				this.AssertInstance();

				SetForeignData(this.handle, value);
			}
		}
		/// <summary>
		/// Gets the number of edges that don't belong to exactly 2 triangles. Only valid for triangular
		/// meshes.
		/// </summary>
		/// <remarks>
		/// Having edges that belong to not exactly 2 triangles is allowed, but cannot be used for anything
		/// that is not a static geometry.
		/// </remarks>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public int ErroneousEdgeCount
		{
			get
			{
				this.AssertInstance();

				return GetErrorCount(this.handle);
			}
		}
		/// <summary>
		/// Gets the value that indicates whether this geometry object doesn't have excessive depth of
		/// bounding volume tree.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public bool IsSane
		{
			get
			{
				this.AssertInstance();

				return SanityCheck(this.handle) != 0;
			}
		}
		#endregion
		#region Construction
		internal GeometryShape(IntPtr handle)
		{
			this.handle = handle;
		}
		/// <summary>
		/// Creates a new geometry object.
		/// </summary>
		/// <param name="vertices">              An array of vertices the mesh consists of.</param>
		/// <param name="indices">               
		/// An array of indices of vertices that form the triangles that form the mesh.
		/// </param>
		/// <param name="flags">                 A set of flags that specify the geometry.</param>
		/// <param name="approximationTolerance">
		/// A tolerance value to use when checking whether provided geometry resembles a primitive geometric
		/// object.
		/// </param>
		/// <param name="minTrianglesPerNode">   
		/// Minimal number of triangle that can be present in a single bounding volume tree node. If one of
		/// the nodes has less then this number of triangles after split, a different one will be attempted.
		/// </param>
		/// <param name="maxTrianglesPerNode">   
		/// Maximal number of triangle that can be present in a single bounding volume tree node. If one of
		/// the nodes has more then this number of triangles, it gets split.
		/// </param>
		/// <param name="favorAabb">             
		/// When several BV trees are requested, it selects the one with the smallest volume; This field
		/// scales AABB's volume down.
		/// </param>
		/// <exception cref="ArgumentException">
		/// An array of vertices needs to have at least 3 vertices.
		/// </exception>
		/// <exception cref="ArgumentNullException">An array of indexes cannot be null.</exception>
		/// <exception cref="ArgumentException">
		/// Length of the array of indexes must be divisible by 3.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Minimal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Maximal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An array of vertices cannot be longer then 65535.
		/// </exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public GeometryShape(Vector3[] vertices, ushort[] indices, PhysicsMeshFlags flags,
							 float approximationTolerance = 0.05f, int minTrianglesPerNode = 2, int maxTrianglesPerNode = 4,
							 float favorAabb = 1)
		{
			this.handle = IntPtr.Zero;

			if (vertices.IsNullOrTooSmall(3))
			{
				throw new ArgumentException("An array of vertices needs to have at least 3 vertices.", nameof(vertices));
			}
			if (indices == null)
			{
				throw new ArgumentNullException(nameof(indices), "An array of indexes cannot be null.");
			}
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Length of the array of indexes must be divisible by 3.", nameof(indices));
			}
			if (minTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minTrianglesPerNode),
													  "Minimal number of triangles per node must be greater then 0.");
			}
			if (maxTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxTrianglesPerNode),
													  "Maximal number of triangles per node must be greater then 0.");
			}
			if (vertices.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(vertices), "An array of vertices cannot be longer then 65535.");
			}

			fixed (Vector3* verticesPtr = vertices)
			fixed (ushort* indexesPtr = indices)
			{
				this.handle = CreateMesh(verticesPtr, indexesPtr, null, null, indices.Length / 3, flags,
										 approximationTolerance, minTrianglesPerNode, maxTrianglesPerNode, favorAabb);
			}
		}
		/// <summary>
		/// Creates a new geometry object.
		/// </summary>
		/// <param name="vertices">              An array of vertices the mesh consists of.</param>
		/// <param name="indices">               
		/// An array of indices of vertices that form the triangles that form the mesh.
		/// </param>
		/// <param name="flags">                 A set of flags that specify the geometry.</param>
		/// <param name="bvParams">              
		/// An object that provides the parameters that specify how to build the bounding volume tree.
		/// </param>
		/// <param name="approximationTolerance">
		/// A tolerance value to use when checking whether provided geometry resembles a primitive geometric
		/// object.
		/// </param>
		/// <exception cref="ArgumentException">
		/// An array of vertices needs to have at least 3 vertices.
		/// </exception>
		/// <exception cref="ArgumentNullException">An array of indexes cannot be null.</exception>
		/// <exception cref="ArgumentException">
		/// Length of the array of indexes must be divisible by 3.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Minimal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Maximal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An array of vertices cannot be longer then 65535.
		/// </exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public GeometryShape(Vector3[] vertices, ushort[] indices, PhysicsMeshFlags flags,
							 BoundingVolumeParameters bvParams, float approximationTolerance = 0.05f)
		{
			this.handle = IntPtr.Zero;

			if (vertices.IsNullOrTooSmall(3))
			{
				throw new ArgumentException("An array of vertices needs to have at least 3 vertices.", nameof(vertices));
			}
			if (indices == null)
			{
				throw new ArgumentNullException(nameof(indices), "An array of indexes cannot be null.");
			}
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Length of the array of indexes must be divisible by 3.", nameof(indices));
			}
			if (bvParams.MinTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bvParams),
													  "Minimal number of triangles per node must be greater then 0.");
			}
			if (bvParams.MaxTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bvParams),
													  "Maximal number of triangles per node must be greater then 0.");
			}
			if (vertices.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(vertices), "An array of vertices cannot be longer then 65535.");
			}

			fixed (Vector3* verticesPtr = vertices)
			fixed (ushort* indexesPtr = indices)
			{
				this.handle = CreateMeshBv(verticesPtr, indexesPtr, null, null, indices.Length / 3, flags,
										   ref bvParams, approximationTolerance);
			}
		}
		/// <summary>
		/// Creates a new geometry object.
		/// </summary>
		/// <param name="vertices">              An array of vertices the mesh consists of.</param>
		/// <param name="indices">               
		/// An array of indices of vertices that form the triangles that form the mesh.
		/// </param>
		/// <param name="flags">                 
		/// A set of flags that specify the geometry. <see cref="PhysicsMeshFlags.VoxelGrid"/> flag doesn't
		/// need to be set.
		/// </param>
		/// <param name="vgParams">              
		/// An object that provides the parameters that specify how to build the bounding volume tree.
		/// </param>
		/// <param name="approximationTolerance">
		/// A tolerance value to use when checking whether provided geometry resembles a primitive geometric
		/// object.
		/// </param>
		/// <exception cref="ArgumentException">
		/// An array of vertices needs to have at least 3 vertices.
		/// </exception>
		/// <exception cref="ArgumentNullException">An array of indexes cannot be null.</exception>
		/// <exception cref="ArgumentException">
		/// Length of the array of indexes must be divisible by 3.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Minimal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Maximal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An array of vertices cannot be longer then 65535.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Dimensions of the voxel grid cannot be less then 0.
		/// </exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public GeometryShape(Vector3[] vertices, ushort[] indices, PhysicsMeshFlags flags,
							 VoxelGridParameters vgParams, float approximationTolerance = 0.05f)
		{
			this.handle = IntPtr.Zero;

			if (vertices.IsNullOrTooSmall(3))
			{
				throw new ArgumentException("An array of vertices needs to have at least 3 vertices.", nameof(vertices));
			}
			if (indices == null)
			{
				throw new ArgumentNullException(nameof(indices), "An array of indexes cannot be null.");
			}
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Length of the array of indexes must be divisible by 3.", nameof(indices));
			}
			if (vgParams.MinTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(vgParams),
													  "Minimal number of triangles per node must be greater then 0.");
			}
			if (vgParams.MaxTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(vgParams),
													  "Maximal number of triangles per node must be greater then 0.");
			}
			if (vertices.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(vertices), "An array of vertices cannot be longer then 65535.");
			}
			if (vgParams.Size.X <= 0 || vgParams.Size.Y <= 0 || vgParams.Size.Z <= 0)
			{
				throw new ArgumentException("Dimensions of the voxel grid cannot be less then 0.", nameof(vgParams));
			}

			fixed (Vector3* verticesPtr = vertices)
			fixed (ushort* indexesPtr = indices)
			{
				this.handle = CreateMeshVg(verticesPtr, indexesPtr, null, null, indices.Length / 3,
										   flags | PhysicsMeshFlags.VoxelGrid, ref vgParams, approximationTolerance);
			}
		}
		/// <summary>
		/// Creates a new geometry object.
		/// </summary>
		/// <param name="vertices">              An array of vertices the mesh consists of.</param>
		/// <param name="indices">               
		/// An array of indices of vertices that form the triangles that form the mesh.
		/// </param>
		/// <param name="materialIds">           An array of indexes of materials per face.</param>
		/// <param name="flags">                 A set of flags that specify the geometry.</param>
		/// <param name="approximationTolerance">
		/// A tolerance value to use when checking whether provided geometry resembles a primitive geometric
		/// object.
		/// </param>
		/// <param name="minTrianglesPerNode">   
		/// Minimal number of triangle that can be present in a single bounding volume tree node. If one of
		/// the nodes has less then this number of triangles after split, a different one will be attempted.
		/// </param>
		/// <param name="maxTrianglesPerNode">   
		/// Maximal number of triangle that can be present in a single bounding volume tree node. If one of
		/// the nodes has more then this number of triangles, it gets split.
		/// </param>
		/// <param name="favorAabb">             
		/// When several BV trees are requested, it selects the one with the smallest volume; This field
		/// scales AABB's volume down.
		/// </param>
		/// <exception cref="ArgumentException">
		/// An array of vertices needs to have at least 3 vertices.
		/// </exception>
		/// <exception cref="ArgumentNullException">An array of indexes cannot be null.</exception>
		/// <exception cref="ArgumentException">
		/// Length of the array of indexes must be divisible by 3.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Minimal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Maximal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An array of vertices cannot be longer then 65535.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The number of ids for materials must be equal to number of triangles.
		/// </exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public GeometryShape(Vector3[] vertices, ushort[] indices, byte[] materialIds, PhysicsMeshFlags flags,
							 float approximationTolerance = 0.05f, int minTrianglesPerNode = 2, int maxTrianglesPerNode = 4,
							 float favorAabb = 1)
		{
			this.handle = IntPtr.Zero;

			if (vertices.IsNullOrTooSmall(3))
			{
				throw new ArgumentException("An array of vertices needs to have at least 3 vertices.", nameof(vertices));
			}
			if (indices == null)
			{
				throw new ArgumentNullException(nameof(indices), "An array of indexes cannot be null.");
			}
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Length of the array of indexes must be divisible by 3.", nameof(indices));
			}
			if (minTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minTrianglesPerNode),
													  "Minimal number of triangles per node must be greater then 0.");
			}
			if (maxTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(maxTrianglesPerNode),
													  "Maximal number of triangles per node must be greater then 0.");
			}
			if (vertices.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(vertices), "An array of vertices cannot be longer then 65535.");
			}
			int triangleCount = indices.Length / 3;
			if (triangleCount != materialIds.Length)
			{
				throw new ArgumentException("The number of ids for materials must be equal to number of triangles.");
			}

			fixed (Vector3* verticesPtr = vertices)
			fixed (ushort* indexesPtr = indices)
			fixed (byte* matPtr = materialIds)
			{
				this.handle = CreateMesh(verticesPtr, indexesPtr, matPtr, null, indices.Length / 3, flags,
										 approximationTolerance, minTrianglesPerNode, maxTrianglesPerNode, favorAabb);
			}
		}
		/// <summary>
		/// Creates a new geometry object.
		/// </summary>
		/// <param name="vertices">              An array of vertices the mesh consists of.</param>
		/// <param name="indices">               
		/// An array of indices of vertices that form the triangles that form the mesh.
		/// </param>
		/// <param name="materialIds">           An array of indexes of materials per face.</param>
		/// <param name="flags">                 A set of flags that specify the geometry.</param>
		/// <param name="bvParams">              
		/// An object that provides the parameters that specify how to build the bounding volume tree.
		/// </param>
		/// <param name="approximationTolerance">
		/// A tolerance value to use when checking whether provided geometry resembles a primitive geometric
		/// object.
		/// </param>
		/// <exception cref="ArgumentException">
		/// An array of vertices needs to have at least 3 vertices.
		/// </exception>
		/// <exception cref="ArgumentNullException">An array of indexes cannot be null.</exception>
		/// <exception cref="ArgumentException">
		/// Length of the array of indexes must be divisible by 3.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Minimal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Maximal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An array of vertices cannot be longer then 65535.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The number of ids for materials must be equal to number of triangles.
		/// </exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public GeometryShape(Vector3[] vertices, ushort[] indices, byte[] materialIds, PhysicsMeshFlags flags,
							 BoundingVolumeParameters bvParams, float approximationTolerance = 0.05f)
		{
			this.handle = IntPtr.Zero;

			if (vertices.IsNullOrTooSmall(3))
			{
				throw new ArgumentException("An array of vertices needs to have at least 3 vertices.", nameof(vertices));
			}
			if (indices == null)
			{
				throw new ArgumentNullException(nameof(indices), "An array of indexes cannot be null.");
			}
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Length of the array of indexes must be divisible by 3.", nameof(indices));
			}
			if (bvParams.MinTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bvParams),
													  "Minimal number of triangles per node must be greater then 0.");
			}
			if (bvParams.MaxTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(bvParams),
													  "Maximal number of triangles per node must be greater then 0.");
			}
			if (vertices.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(vertices), "An array of vertices cannot be longer then 65535.");
			}
			int triangleCount = indices.Length / 3;
			if (triangleCount != materialIds.Length)
			{
				throw new ArgumentException("The number of ids for materials must be equal to number of triangles.");
			}

			fixed (Vector3* verticesPtr = vertices)
			fixed (ushort* indexesPtr = indices)
			fixed (byte* matPtr = materialIds)
			{
				this.handle = CreateMeshBv(verticesPtr, indexesPtr, matPtr, null, indices.Length / 3, flags,
										   ref bvParams, approximationTolerance);
			}
		}
		/// <summary>
		/// Creates a new geometry object.
		/// </summary>
		/// <param name="vertices">              An array of vertices the mesh consists of.</param>
		/// <param name="indices">               
		/// An array of indices of vertices that form the triangles that form the mesh.
		/// </param>
		/// <param name="materialIds">           An array of indexes of materials per face.</param>
		/// <param name="flags">                 
		/// A set of flags that specify the geometry. <see cref="PhysicsMeshFlags.VoxelGrid"/> flag doesn't
		/// need to be set.
		/// </param>
		/// <param name="vgParams">              
		/// An object that provides the parameters that specify how to build the bounding volume tree.
		/// </param>
		/// <param name="approximationTolerance">
		/// A tolerance value to use when checking whether provided geometry resembles a primitive geometric
		/// object.
		/// </param>
		/// <exception cref="ArgumentException">
		/// An array of vertices needs to have at least 3 vertices.
		/// </exception>
		/// <exception cref="ArgumentNullException">An array of indexes cannot be null.</exception>
		/// <exception cref="ArgumentException">
		/// Length of the array of indexes must be divisible by 3.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Minimal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Maximal number of triangles per node must be greater then 0.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// An array of vertices cannot be longer then 65535.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// The number of ids for materials must be equal to number of triangles.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Dimensions of the voxel grid cannot be less then 0.
		/// </exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public GeometryShape(Vector3[] vertices, ushort[] indices, byte[] materialIds, PhysicsMeshFlags flags,
							 VoxelGridParameters vgParams, float approximationTolerance = 0.05f)
		{
			this.handle = IntPtr.Zero;

			if (vertices.IsNullOrTooSmall(3))
			{
				throw new ArgumentException("An array of vertices needs to have at least 3 vertices.", nameof(vertices));
			}
			if (indices == null)
			{
				throw new ArgumentNullException(nameof(indices), "An array of indexes cannot be null.");
			}
			if (indices.Length % 3 != 0)
			{
				throw new ArgumentException("Length of the array of indexes must be divisible by 3.", nameof(indices));
			}
			if (vgParams.MinTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(vgParams),
													  "Minimal number of triangles per node must be greater then 0.");
			}
			if (vgParams.MaxTrianglesPerNode <= 0)
			{
				throw new ArgumentOutOfRangeException(nameof(vgParams),
													  "Maximal number of triangles per node must be greater then 0.");
			}
			if (vertices.Length > ushort.MaxValue)
			{
				throw new ArgumentOutOfRangeException(nameof(vertices), "An array of vertices cannot be longer then 65535.");
			}
			int triangleCount = indices.Length / 3;
			if (triangleCount != materialIds.Length)
			{
				throw new ArgumentException("The number of ids for materials must be equal to number of triangles.");
			}
			if (vgParams.Size.X <= 0 || vgParams.Size.Y <= 0 || vgParams.Size.Z <= 0)
			{
				throw new ArgumentException("Dimensions of the voxel grid cannot be less then 0.", nameof(vgParams));
			}

			fixed (Vector3* verticesPtr = vertices)
			fixed (ushort* indexesPtr = indices)
			fixed (byte* matPtr = materialIds)
			{
				this.handle = CreateMeshVg(verticesPtr, indexesPtr, matPtr, null, indices.Length / 3,
										   flags | PhysicsMeshFlags.VoxelGrid, ref vgParams, approximationTolerance);
			}
		}
		/// <summary>
		/// Creates a geometry object that represents a primitive geometric object.
		/// </summary>
		/// <param name="type">     
		/// Type of geometric object to create. There is no way to detect a mismatch between a type what is
		/// actually passed with <paramref name="primitive"/>.
		/// </param>
		/// <param name="primitive">
		/// Reference to the base part of the object that provides information about a primitive.
		/// </param>
		/// <exception cref="NotSupportedException">Primitive type is not supported.</exception>
		public GeometryShape(int type, ref Primitive.BasePrimitive primitive)
		{
			this.handle = IntPtr.Zero;

			if (Array.BinarySearch(Primitive.RegisteredTypes, type) < 0)
			{
				throw new NotSupportedException($"Primitive type with identifier = {type} is not supported");
			}

			this.handle = CreatePrimitive(type, ref primitive);
		}
		#endregion
		#region Interface
		/// <summary>
		/// Increases the internal reference count for this geometry object.
		/// </summary>
		/// <remarks>
		/// Use this method when you use the same geometry object for multiple parts or entities.
		/// </remarks>
		/// <returns>Current number of references(?).</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public int IncrementReferenceCount()
		{
			this.AssertInstance();

			return AddRef(this.handle);
		}
		/// <summary>
		/// Decreases the internal reference count for this geometry object.
		/// </summary>
		/// <remarks>
		/// Use this method when you were using the same geometry object for multiple parts or entities when
		/// you don't need this geometry.
		/// </remarks>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void DecrementReferenceCount()
		{
			this.AssertInstance();

			Release(this.handle);
		}
		/// <summary>
		/// Locks this geometry object and prevents anyone from being able to write into its internal data
		/// buffers.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void LockWrite()
		{
			this.AssertInstance();

			Lock(this.handle, 1);
		}
		/// <summary>
		/// Locks this geometry object and prevents anyone from being able to read its internal data
		/// buffers.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void LockRead()
		{
			this.AssertInstance();

			Lock(this.handle, 0);
		}
		/// <summary>
		/// Unlocks this geometry object. The call to this method must come after <see cref="LockWrite"/>,
		/// otherwise the behavior is not defined.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void UnlockWrite()
		{
			this.AssertInstance();

			Unlock(this.handle, 1);
		}
		/// <summary>
		/// Unlocks this geometry object. The call to this method must come after <see cref="LockRead"/>,
		/// otherwise the behavior is not defined.
		/// </summary>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void UnlockRead()
		{
			this.AssertInstance();

			Unlock(this.handle, 0);
		}
		/// <summary>
		/// Determines whether this object contains specified point.
		/// </summary>
		/// <param name="point">Coordinates of the point to check against this geometry.</param>
		/// <returns>True, if this geometry contains the point.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public bool ContainsPoint(ref Vector3 point)
		{
			this.AssertInstance();

			return PointInsideStatus(this.handle, ref point) != 0;
		}
		/// <summary>
		/// Tests this geometry against another and returns a list of contacts.
		/// </summary>
		/// <param name="other">  Another geometry object to check this one against.</param>
		/// <param name="pdata1"> 
		/// Reference to the object that describes location and movement of this geometric object.
		/// </param>
		/// <param name="pdata2"> 
		/// Reference to the object that describes location and movement of <paramref name="other"/>
		/// geometric object.
		/// </param>
		/// <param name="pparams">
		/// Reference to the object that provides a set of parameters that specify the check.
		/// </param>
		/// <returns>The array of contacts.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		/// <exception cref="ArgumentNullException">
		/// An object that represents another geometry must not be valid.
		/// </exception>
		[CanBeNull]
		public GeometryContact[] Intersection(GeometryShape other, ref GeometryWorldData pdata1,
											  ref GeometryWorldData pdata2, ref IntersectionParameters pparams)
		{
			this.AssertInstance();
			if (!other.IsValid)
			{
				throw new ArgumentNullException(nameof(other), "An object that represents another geometry must not be valid.");
			}

			pparams.CompleteInitialization();

			return IntersectLocked(this.handle, other, ref pdata1, ref pdata2, ref pparams);
		}
		/// <summary>
		/// Tests this geometry against another and returns a list of contacts.
		/// </summary>
		/// <param name="other">Another geometry object to check this one against.</param>
		/// <returns>The array of contacts.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		/// <exception cref="ArgumentNullException">
		/// An object that represents another geometry must not be valid.
		/// </exception>
		[CanBeNull]
		public GeometryContact[] Intersection(GeometryShape other)
		{
			this.AssertInstance();
			if (!other.IsValid)
			{
				throw new ArgumentNullException(nameof(other), "An object that represents another geometry must not be valid.");
			}

			return IntersectLockedDefault(this.handle, other);
		}
		/// <summary>
		/// Finds the closest point on the surface of this geometry to specified point.
		/// </summary>
		/// <param name="start">   Coordinates of the point to go from.</param>
		/// <param name="maxIters">
		/// Optional value that specifies how many to tests to run. Higher - more expensive but higher
		/// probability of finding an actually closest point.
		/// </param>
		/// <returns>Point on the surface of the geometry.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public Vector3 FindClosestPoint(ref Vector3 start, int maxIters = 10)
		{
			GeometryWorldData wd = new GeometryWorldData();
			return this.FindClosestPoint(ref wd, ref start, maxIters);
		}
		/// <summary>
		/// Finds the closest point on the surface of this geometry to specified point.
		/// </summary>
		/// <param name="wd">      
		/// Reference to the object that specifies the location and movement of this geometry.
		/// </param>
		/// <param name="start">   Coordinates of the point to go from.</param>
		/// <param name="maxIters">
		/// Optional value that specifies how many to tests to run. Higher - more expensive but higher
		/// probability of finding an actually closest point.
		/// </param>
		/// <returns>Point on the surface of the geometry.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public Vector3 FindClosestPoint(ref GeometryWorldData wd, ref Vector3 start, int maxIters = 10)
		{
			Vector3 pointOnSurface, pointOnSegment;
			this.FindClosestPoint(ref wd, ref start, ref start, out pointOnSurface, out pointOnSegment, maxIters);
			return pointOnSurface;
		}
		/// <summary>
		/// Finds the closest point on the surface of this geometry to specified segment.
		/// </summary>
		/// <remarks>
		/// <paramref name="pointOnSurface"/> and <paramref name="pointOnSegment"/> form a segment that
		/// represents a shortest distance between provided segment and this geometry.
		/// </remarks>
		/// <param name="wd">            
		/// Reference to the object that specifies the location and movement of this geometry.
		/// </param>
		/// <param name="segment0">      Starting point of the segment.</param>
		/// <param name="segment1">      Ending point of the segment.</param>
		/// <param name="pointOnSurface">
		/// The coordinates of the closest point on the surface of geometry.
		/// </param>
		/// <param name="pointOnSegment">The coordinates of the closest point on the segment.</param>
		/// <param name="maxIters">      
		/// Optional value that specifies how many to tests to run. Higher - more expensive but higher
		/// probability of finding an actually closest point.
		/// </param>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void FindClosestPoint(ref GeometryWorldData wd, ref Vector3 segment0, ref Vector3 segment1,
									 out Vector3 pointOnSurface, out Vector3 pointOnSegment, int maxIters = 10)
		{
			this.AssertInstance();

			wd.CompleteInitialization();

			int prim, feature;
			Vector3* resultPoints = stackalloc Vector3[2];
			FindClosestPointInternal(this.handle, ref wd, out prim, out feature, ref segment0, ref segment1, resultPoints,
									 maxIters);
			pointOnSegment = resultPoints[1];
			pointOnSurface = resultPoints[0];
		}
		/// <summary>
		/// Finds the closest point on the surface of this geometry to specified segment.
		/// </summary>
		/// <remarks>
		/// <paramref name="pointOnSurface"/> and <paramref name="pointOnSegment"/> form a segment that
		/// represents a shortest distance between provided segment and this geometry.
		/// </remarks>
		/// <param name="segment0">      Starting point of the segment.</param>
		/// <param name="segment1">      Ending point of the segment.</param>
		/// <param name="pointOnSurface">
		/// The coordinates of the closest point on the surface of geometry.
		/// </param>
		/// <param name="pointOnSegment">The coordinates of the closest point on the segment.</param>
		/// <param name="maxIters">      
		/// Optional value that specifies how many to tests to run. Higher - more expensive but higher
		/// probability of finding an actually closest point.
		/// </param>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void FindClosestPoint(ref Vector3 segment0, ref Vector3 segment1,
									 out Vector3 pointOnSurface, out Vector3 pointOnSegment, int maxIters = 10)
		{
			GeometryWorldData wd = new GeometryWorldData();
			this.FindClosestPoint(ref wd, ref segment0, ref segment1, out pointOnSurface, out pointOnSegment, maxIters);
		}
		/// <summary>
		/// Calculates the effect the explosion would have on this geometric object.
		/// </summary>
		/// <remarks>
		/// Formula for pressure:
		/// <code>
		/// // k*dS*cos(surface_normal,direction to epicenter) / max(rmin, distance to epicenter)^2
		///
		/// float pressureModfier = k;
		/// float surfaceArea = dS;
		/// Vector3 surfaceNormal = surface_normal;
		/// Vector3 directionToEpicenter = direction to epicenter;
		/// float distance = Math.Max(rmin, distance to epicenter);
		///
		/// float pressure = pressureModfier * surfaceArea * (surfaceNormal * directionToEpicenter) / distance * distance;
		/// </code>
		/// </remarks>
		/// <param name="wd">            
		/// Reference to the object that specifies the location and movement of this geometry.
		/// </param>
		/// <param name="epicenter">     Coordinates of the epicenter of the explosion.</param>
		/// <param name="k">             Pressure modifier.</param>
		/// <param name="minRadius">     Minimal radius to use in calculation.</param>
		/// <param name="impulse">       Resultant linear impulse.</param>
		/// <param name="angularImpulse">Resultant angular impulse.</param>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void CalculateExplosionEffect(ref GeometryWorldData wd, ref Vector3 epicenter, float k, float minRadius,
											 out Vector3 impulse, out EulerAngles angularImpulse)
		{
			this.AssertInstance();

			wd.CompleteInitialization();

			CalcVolumetricPressure(this.handle, ref wd, ref epicenter, k, minRadius, out impulse, out angularImpulse);
		}
		/// <summary>
		/// Calculates buoyancy properties.
		/// </summary>
		/// <param name="surface">        
		/// Reference to the object that describe the surface of the liquid body.
		/// </param>
		/// <param name="wd">             
		/// Reference to the object that specifies the location and movement of this geometry.
		/// </param>
		/// <param name="submergedCenter">
		/// Resultant vector that provides coordinates of the center of submerged mass.
		/// </param>
		/// <returns>Volume of submerged part of this geometric object.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public float CalculateBuoyancy(ref Primitive.Plane surface, ref GeometryWorldData wd, out Vector3 submergedCenter)
		{
			this.AssertInstance();

			wd.CompleteInitialization();

			return CalculateBuoyancyInternal(this.handle, ref surface, ref wd, out submergedCenter);
		}
		/// <summary>
		/// Calculates buoyancy properties.
		/// </summary>
		/// <param name="surface">        
		/// Reference to the object that describes the surface of the liquid body.
		/// </param>
		/// <param name="submergedCenter">
		/// Resultant vector that provides coordinates of the center of submerged mass.
		/// </param>
		/// <returns>Volume of submerged part of this geometric object.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public float CalculateBuoyancy(ref Primitive.Plane surface, out Vector3 submergedCenter)
		{
			GeometryWorldData wd = new GeometryWorldData();
			return this.CalculateBuoyancy(ref surface, ref wd, out submergedCenter);
		}
		/// <summary>
		/// Calculates the medium resistance.
		/// </summary>
		/// <param name="surface">   
		/// Reference to the object that describes the plane that specifies where the medium affects the
		/// geometry.
		/// </param>
		/// <param name="wd">        
		/// Reference to the object that specifies the location and movement of this geometry.
		/// </param>
		/// <param name="pressure">  Resultant pressure from the medium(?).</param>
		/// <param name="resistance">Resultant resistant force from the medium(?).</param>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public void CalculateMediumResistance(ref Primitive.Plane surface, ref GeometryWorldData wd, out Vector3 pressure,
											  out Vector3 resistance)
		{
			this.AssertInstance();

			wd.CompleteInitialization();

			CalculateMediumResistanceInternal(this.handle, ref surface, ref wd, out pressure, out resistance);
		}
		/// <summary>
		/// Subtracts the shape out of this one.
		/// </summary>
		/// <param name="subtrahend">A geometric object to subtract from this one.</param>
		/// <param name="pdata1">    
		/// Reference to the object that describes location and movement of this geometric object.
		/// </param>
		/// <param name="pdata2">    
		/// Reference to the object that describes location and movement of <paramref name="subtrahend"/>.
		/// </param>
		/// <param name="logUpdates">
		/// Indicates whether a new <see cref="MeshUpdate"/> should be created in this object that details
		/// the changes that were to this object during subtraction.
		/// </param>
		/// <returns>
		/// True, if this geometric object "survived" the subtraction (if its volume is not equal to 0).
		/// </returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public bool Subtract(GeometryShape subtrahend, ref GeometryWorldData pdata1, ref GeometryWorldData pdata2,
							 bool logUpdates = true)
		{
			this.AssertInstance();

			if (!subtrahend.IsValid)
			{
				return true;
			}

			return Subtraction(this.handle, subtrahend, ref pdata1, ref pdata2, logUpdates) != 0;
		}
		/// <summary>
		/// Attempts to acquire the first object that represents the first set of changes that were made to
		/// this geometric object.
		/// </summary>
		/// <param name="firstUpdate">
		/// Resultant object that will only be assigned if this method returns <c>true</c>.
		/// </param>
		/// <returns>True, if <paramref name="firstUpdate"/> was assigned and is usable.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		public bool GetMeshUpdates(out MeshUpdate firstUpdate)
		{
			this.AssertInstance();

			MeshUpdate* first = GetMeshUpdatesInternal(this.handle);

			if (first == null)
			{
				firstUpdate = new MeshUpdate();

				return false;
			}

			firstUpdate = *first;
			return true;
		}
		/// <summary>
		/// Attempts to build a set of boxes that covers this geometry. Only works with triangular meshes.
		/// </summary>
		/// <param name="boxes">     An array of objects to put created boxes into.</param>
		/// <param name="parameters">Reference to the object that defines how to boxify this mesh.</param>
		/// <returns>Number of created boxes.</returns>
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		/// <exception cref="OverflowException">
		/// The array is multidimensional and contains more than <see cref="F:System.Int32.MaxValue"/>
		/// elements.
		/// </exception>
		public int Boxify(Primitive.Box[] boxes, ref BoxificationParameters parameters)
		{
			this.AssertInstance();

			if (boxes.IsNullOrEmpty())
			{
				return 0;
			}

			fixed (Primitive.Box* boxesPtr = boxes)
			{
				return BoxifyInternal(this.handle, boxesPtr, boxes.Length, ref parameters);
			}
		}
		#endregion
		#region Utilities
		/// <exception cref="NullReferenceException">This instance is not valid.</exception>
		private void AssertInstance()
		{
			if (!this.IsValid)
			{
				throw new NullReferenceException("This instance is not valid.");
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateMesh(Vector3* vertices, ushort* indices, byte* materialIds, int* foreignIds,
												int triangleCount, PhysicsMeshFlags flags,
												float approximationTolerance = 0.05f, int minTrianglesPerNode = 2,
												int maxTrianglesPerNode = 4, float favorAabb = 1);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateMeshBv(Vector3* vertices, ushort* indices, byte* materialIds, int* foreignIds,
												  int triangleCount, PhysicsMeshFlags flags,
												  ref BoundingVolumeParameters bvParams, float approximationTolerance = 0.05f);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateMeshVg(Vector3* vertices, ushort* indices, byte* materialIds, int* foreignIds,
												  int triangleCount, PhysicsMeshFlags flags, ref VoxelGridParameters vgParams,
												  float approximationTolerance = 0.05f);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreatePrimitive(int type, ref Primitive.BasePrimitive primitive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GeometryTypes GetGeometryType(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddRef(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Release(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Lock(IntPtr handle, int bWrite);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Unlock(IntPtr handle, int bWrite);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetBBox(IntPtr handle, out Primitive.Box pbox);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int PointInsideStatus(IntPtr handle, ref Vector3 pt);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GeometryContact[] IntersectLocked(IntPtr handle, GeometryShape pCollider,
																ref GeometryWorldData pdata1, ref GeometryWorldData pdata2,
																ref IntersectionParameters pparams);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GeometryContact[] IntersectLockedDefault(IntPtr handle, GeometryShape pCollider);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int FindClosestPointInternal(IntPtr handle, ref GeometryWorldData pgwd, out int iPrim,
														   out int iFeature, ref Vector3 ptdst0, ref Vector3 ptdst1,
														   Vector3* ptres, int nMaxIters);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalcVolumetricPressure(IntPtr handle, ref GeometryWorldData gwd, ref Vector3 epicenter,
														  float k, float rmin, out Vector3 P, out EulerAngles L);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float CalculateBuoyancyInternal(IntPtr handle, ref Primitive.Plane pplane,
															  ref GeometryWorldData pgwd, out Vector3 submergedMassCenter);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateMediumResistanceInternal(IntPtr handle, ref Primitive.Plane pplane,
																	 ref GeometryWorldData pgwd, out Vector3 dPres,
																	 out Vector3 dLres);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int IsConvexInternal(IntPtr handle, float tolerance);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetPrimitiveCount(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float GetVolume(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Vector3 GetCenter(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Subtraction(IntPtr handle, GeometryShape pGeom, ref GeometryWorldData pdata1,
											  ref GeometryWorldData pdata2, bool logUpdates = true);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetSubtractionsCount(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern MeshUpdate* GetMeshUpdatesInternal(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ForeignData GetForeignData(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetForeignData(IntPtr handle, ForeignData data);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetErrorCount(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int BoxifyInternal(IntPtr handle, Primitive.Box* pboxes, int nMaxBoxes,
												 ref BoxificationParameters parameters);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int SanityCheck(IntPtr handle);
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Primitive.BasePrimitive* GetData(IntPtr handle);
		#endregion
	}
}