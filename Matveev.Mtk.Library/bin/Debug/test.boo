namespace Matveev.Mtk.Library
import Matveev.Mtk.Core

class EdgeSplit (EdgeTransform):
	private final _t as double
	private final _r as double

	def constructor(t as double):
		_t = t
		_r = 1 - t

	def constructor():
		self(0.5)

	def IsPossible(edge as Edge, _ as IVertexConstraintsProvider):
		return true

	def Execute(edge as Edge):
		mesh = edge.Mesh
		v0 = edge.Begin
		v1 = edge.End
		v2 = edge.Next.End
		v = mesh.AddVertex(v0.Point + _t * (v1.Point - v0.Point), Vector.Normalize(_r * v0.Normal + _t * v1.Normal))
		if edge.Pair != null:
			v3 = edge.Pair.Next.End
			mesh.DeleteFace(edge.Pair.ParentFace)
			mesh.CreateFace(v3, v, v0)
			mesh.CreateFace(v1, v, v3)
		mesh.DeleteFace(edge.ParentFace)
		mesh.CreateFace(v0, v, v2)
		mesh.CreateFace(v2, v, v1)
		return v