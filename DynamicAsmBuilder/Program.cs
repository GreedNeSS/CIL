using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using System.Reflection.Emit;

namespace DynamicAsmBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("***** Dynamic assembly builder app *****");

            AppDomain appDomain = Thread.GetDomain();

            CreateMyAsm(appDomain);
            Console.WriteLine("=> Finished creating NewAssembly.dll");

            Console.WriteLine("=> Loading NewAssembly.dll from file.");
            Assembly a = Assembly.Load("NewAssembly");

            Type hello = a.GetType("NewAssembly.HelloWorld");

            Console.WriteLine("=> Enter message to pass HelloWorld class: ");
            string msg = Console.ReadLine();
            object[] ctorArgs = { msg };
            object obj = Activator.CreateInstance(hello, ctorArgs);

            Console.WriteLine("=> Calling SayHello() via late binding.");
            MethodInfo mi = hello.GetMethod("SayHello");
            mi.Invoke(obj, null);

            mi = hello.GetMethod("GetMsg");
            Console.WriteLine($"GetMsg(): {mi.Invoke(obj, null)}");
        }

        public static void CreateMyAsm(AppDomain appDomain)
        {
            AssemblyName assemblyName = new AssemblyName()
            {
                Name = "NewAssembly",
                Version = new Version("1.0.0.0")
            };

            AssemblyBuilder newAssembly = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Save);

            ModuleBuilder module = newAssembly.DefineDynamicModule("NewAssembly", "NewAssembly.dll");

            TypeBuilder helloWorldClass = module.DefineType("NewAssembly.HelloWorld", TypeAttributes.Public);

            FieldBuilder msgField = helloWorldClass.
                DefineField("theMessage", typeof(string), attributes: FieldAttributes.Private);

            Type[] constructorArgs = new Type[1];
            constructorArgs[0] = Type.GetType("System.String");
            ConstructorBuilder constructor = helloWorldClass.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard, constructorArgs);
            ILGenerator constructorIL = constructor.GetILGenerator();
            constructorIL.Emit(OpCodes.Ldarg_0);
            Type objectClass = typeof(object);
            ConstructorInfo superConstructor = objectClass.GetConstructor(new Type[0]);
            constructorIL.Emit(OpCodes.Call, superConstructor);
            constructorIL.Emit(OpCodes.Ldarg_0);
            constructorIL.Emit(OpCodes.Ldarg_1);
            constructorIL.Emit(OpCodes.Stfld, msgField);
            constructorIL.Emit(OpCodes.Ret);

            helloWorldClass.DefineDefaultConstructor(MethodAttributes.Public);

            MethodBuilder getMsgMethod = helloWorldClass.DefineMethod("GetMsg", MethodAttributes.Public, typeof(string), null);
            ILGenerator methodIL = getMsgMethod.GetILGenerator();
            methodIL.Emit(OpCodes.Ldarg_0);
            methodIL.Emit(OpCodes.Ldfld, msgField);
            methodIL.Emit(OpCodes.Ret);

            MethodBuilder sayHiMethod = helloWorldClass.DefineMethod("SayHello", MethodAttributes.Public, null, null);
            methodIL = sayHiMethod.GetILGenerator();
            methodIL.EmitWriteLine("Hello from the HelloWorld class!");
            methodIL.Emit(OpCodes.Ret);

            helloWorldClass.CreateType();

            newAssembly.Save("NewAssembly.dll");
        }
    }
}
