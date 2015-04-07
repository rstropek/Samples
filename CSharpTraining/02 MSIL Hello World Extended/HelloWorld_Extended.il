.assembly extern mscorlib {}
.assembly hello {}

.class C 
{
	.method public rtspecialname specialname instance void .ctor() cil managed
	{
		.maxstack 1
		// call super constructor
		ldarg.0 // load this pointer
		call instance void [mscorlib]System.Object::.ctor()
		ret
	}

	.method public virtual void MyMethod()
	{
		ldstr "I am inside the method!"
		call void [mscorlib]System.Console::WriteLine(string)
		ret
	}
}

.method static public void main() cil managed
{
	.entrypoint
	
	.maxstack 1
	.locals init ( class C myInstance )
	
	newobj instance void C::.ctor()
	callvirt instance void C::MyMethod()
	
	ldstr "Hello world!"
	call void [mscorlib]System.Console::WriteLine(class System.String)
	
	ret
}
