import System
import System.Collections.Generic
import System.Diagnostics
import Matveev.Mtk.Core
import Matveev.Mtk.Library
import Matveev.Mtk.Library.Fields

def count(enumerable):
	result = 0
	for o in enumerable:
		result++
	return result

callable evaluator(param) as double
def print_statistics(enumerable, convert as evaluator):
	num = 0
	min = 10000.0
	max = 0.0
	sum = 0.0
	for o in enumerable:
		num++
		value = convert(o)
		if value < min: min = value
		if value > max: max = value
		sum += value
	avg = sum / num
	print "Min: ${min}"
	print "Max: ${max}"
	print "Sum: ${sum}"
	print "Avg: ${avg}"
	print ""

callable face_evaluator(points as (Point)) as double
def measure_evaluator(function as face_evaluator, mesh as Mesh):
	profiler = Stopwatch.StartNew()
	for i in range(10000):
		for f in mesh.Faces:
			function(array(Point, 3))
	profiler.Stop()
	return profiler.Elapsed	

def print_info(info_table as duck):
	for info as duck in info_table:
		print "${info.Key} => ${info.Value}"
	print ""
	
def gather_and_print_all(surface as IImplicitSurface, mesh as Mesh, precise as IFaceEnergyProvider):
	print "Vertex count: " + count(mesh.Vertices)
	print "Vertex energy:"
	print_statistics(mesh.Vertices, { v as Vertex | return Math.Pow(surface.Eval(v.Point), 2) })

	print "Edge count: " + count(mesh.Edges)
	print "Edge length:"
	print_statistics(mesh.Edges, { e as Edge | return e.Begin.Point.DistanceTo(e.End.Point) })

	print "Face count: " + count(mesh.Faces)
	print "Face area:"
	print_statistics(mesh.Faces, { f as Face | points = List[of Vertex](f.Vertices); return points[0].Point.AreaTo(points[1].Point, points[2].Point) })

	face_energies = Dictionary[of string, IFaceEnergyProvider]()
	face_energies["precise"] = precise
	for name in TriangleImplicitApproximations.AvailableApproximations:
		face_energies[name] = TriangleImplicitApproximations.GetApproximation(surface, name)

	# Gathering statistics
	performance = Dictionary[of string, TimeSpan]()
	relative_error = Dictionary[of string, double]()
	absolute_error = Dictionary[of string, double]()
	for name in face_energies.Keys:
		performance[name] = measure_evaluator(face_energies[name].FaceEnergy, mesh)
		absolute_error[name] = 0.0
	total_value = 0.0
	for f in mesh.Faces:
		it = f.Vertices.GetEnumerator()
		it.MoveNext()
		p0 = it.Current.Point
		it.MoveNext()
		p1 = it.Current.Point
		it.MoveNext()
		p2 = it.Current.Point
		points = (p0, p1, p2)
		area = points[0].AreaTo(points[1], points[2])
		precise_value = area * face_energies["precise"].FaceEnergy(points)
		total_value += precise_value**2
		for namedEnergy in face_energies:
			name = namedEnergy.Key
			energy = namedEnergy.Value.FaceEnergy
			value = area * energy(points)
			error = value - precise_value
			absolute_error[name] += error**2
	for name in face_energies.Keys:
		relative_error[name] = Math.Sqrt(absolute_error[name] / total_value)
	#end of gathering
		
	print "Performance:"
	print_info(performance)

	print "Absolute error:"
	print_info(absolute_error)

	print "Relative error:"
	print_info(relative_error)

print "Torus [-1, 1]^3 8x8x8"
surface as IImplicitSurface = Torus.Sample
mesh = MC.Instance.Create(HEMesh.Factory, surface, -1, 1, -1, 1, -1, 1, 8, 8, 8)
gather_and_print_all(surface, mesh, Torus.Sample)

print "-----------------------------------"
print "Sphere [-1, 1]^3 2x2x2"
surface = CompactQuadraticForm.Sphere
mesh = MC.Instance.Create(HEMesh.Factory, surface, -1, 1, -1, 1, -1, 1, 2, 2, 2)
gather_and_print_all(surface, mesh, CompactQuadraticForm.Sphere)

print "-----------------------------------"
print "Sphere [-1, 1]^3 4x4x4"
surface = CompactQuadraticForm.Sphere
mesh = MC.Instance.Create(HEMesh.Factory, surface, -1, 1, -1, 1, -1, 1, 4, 4, 4)
gather_and_print_all(surface, mesh, CompactQuadraticForm.Sphere)