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
	
surface = CompactQuadraticForm.Sphere
mesh = MC.Instance.Create(HEMesh.Factory, surface, -1, 1, -1, 1, -1, 1, 8, 8, 8)

print "Vertex count: " + count(mesh.Vertices)
print "Vertex energy:"
print_statistics(mesh.Vertices, { v as Vertex | return Math.Pow(surface.Eval(v.Point), 2) })

print "Edge count: " + count(mesh.Edges)
print "Edge length:"
print_statistics(mesh.Edges, { e as Edge | return e.Begin.Point.DistanceTo(e.End.Point) })

print "Face count: " + count(mesh.Faces)
print "Face area:"
print_statistics(mesh.Faces, { f as Face | points = List[of Vertex](f.Vertices); return points[0].Point.AreaTo(points[1].Point, points[2].Point) })

face_energies = Dictionary[of string, face_evaluator]()
face_energies["precise"] = surface.FaceDistance
for name in TriangleImplicitApproximations.AvailableApproximations:
	face_energies[name] = TriangleImplicitApproximations.GetApproximation(surface.Eval, name)

performance = Dictionary[of string, TimeSpan]()
total_value = Dictionary[of string, double]()
rmsd = Dictionary[of string, double]()
for namedEnergy in face_energies:
	name = namedEnergy.Key
	energy = namedEnergy.Value
	performance[name] = measure_evaluator(energy, mesh)
	total_value[name] = 0
	rmsd[name] = 0
	fcount = 0
	for f in mesh.Faces:
		fcount++
		it = f.Vertices.GetEnumerator()
		it.MoveNext()
		p0 = it.Current.Point
		it.MoveNext()
		p1 = it.Current.Point
		it.MoveNext()
		p2 = it.Current.Point
		points = (p0, p1, p2)
		value = energy(points)
		precise_value = face_energies["precise"](points)
		error = value - precise_value
		total_value[name] += value
		rmsd[name] += error**2
	rmsd[name] = Math.Sqrt(rmsd[name] / fcount)	

print "Performance:"
print_info(performance)

print "Total value:"
print_info(total_value)

print "Root mean square deviation:"
print_info(rmsd)
