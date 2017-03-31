def torus(x as duck, y as duck, z as duck):
	r0 = 0.5
	r1 = 0.1
	b = 4 * r0 * r0
	c = r0 * r0 - r1 * r1
	x2 = x * x
	y2 = y * y
	z2 = z * z
	a = x2 + y2
	a = a + z2
	a = a + c
	return a * a - (x2 + y2) * b

class Expression:
	abstract def Add(right as Expression) as Expression:
		pass
	abstract def Subtract(right as Expression) as Expression:
		pass
	abstract def Multiply(right as Expression) as Expression:
		pass

	public static def op_Addition(left as Expression, right as Expression) as Expression:
		return AdditionExpression(left, right)
	public static def op_Addition(left as Expression, right as double) as Expression:
		return AdditionExpression(left, Constant(right))
	public static def op_Addition(left as double, right as Expression) as Expression:
		return AdditionExpression(Constant(left), right)
	public static def op_Subtraction(left as Expression, right as Expression) as Expression:
		return SubtractionExpression(left, right)
	public static def op_Multiply(left as Expression, right as Expression) as Expression:
		return MultiplicationExpression(left, right)
	public static def op_Multiply(left as Expression, right as double) as Expression:
		return MultiplicationExpression(left, Constant(right))

class Variable(Expression):
	private final _name as string
	def constructor(name as string):
		_name = name
	def ToString():
		return _name

class Constant(Expression):
	private final _value as double
	def constructor(value as double):
		_value = value
	def ToString():
		return _value.ToString()

class MultiplicationExpression(Expression):
	private final _operands = []
	def constructor(left as Expression, right as Expression):
		_operands = [left, right]
	def ToString():
		return "(${_operands[0]} * ${_operands[1]})"

class AdditionExpression(Expression):
	private final _operands = []
	def constructor(left as Expression, right as Expression):
		_operands = [left, right]
	def ToString():
		return "(${_operands[0]} + ${_operands[1]})"

class SubtractionExpression(Expression):
	private final _operands = []
	def constructor(left as Expression, right as Expression):
		_operands = [left, right]
	def ToString():
		return "(${_operands[0]} - ${_operands[1]})"

x = Variable('x')
y = Variable('y')
z = Variable('z')
expression as Expression = torus(x, y, z)

u = Variable('u')
v = Variable('v')
print expression