.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}

.assembly extern CILLib
{
  .ver 1:0:0:0
}

.assembly CILEntryPoint
{
	.hash algorithm 0x00008004
	.ver 1:0:0:0
}
.module CILEntryPoint.exe

.namespace CILEntryPoint
{

	.class private auto ansi beforefieldinit Program
		extends [mscorlib]System.Object
	{
	.method private hidebysig static void
    Main(string[] args) cil managed
    {
      // Marks the entry point of the *.exe.
      .entrypoint
      .maxstack 8

      // Declare a local CILCar variable and push
      // values onto the stack for ctor call.
      .locals init ([0] class
      [CILLib]CILLib.CILCar myCilCar)
      ldc.i4 55
      ldstr "Junior"

      // Make new CilCar; store and load reference.
      newobj instance void
        [CILLib]CILLib.CILCar::.ctor(int32, string)
      stloc.0
      ldloc.0

      // Call Display() and pass in topmost value on stack.
      call void [CILLib]CILLib.CILCarInfo::Display(
           class [CILLib]CILLib.CILCar)
     ret
  }
	}
}