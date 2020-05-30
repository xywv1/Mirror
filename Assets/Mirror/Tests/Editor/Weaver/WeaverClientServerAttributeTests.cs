using System.IO;
using System.Linq;
using Mono.CecilX;
using Mono.CecilX.Cil;
using NUnit.Framework;

namespace Mirror.Weaver.Tests
{
    public class WeaverClientServerAttributeTests : WeaverTestsBuildFromTestName
    {
        [Test]
        public void NetworkBehaviourServer()
        {
            Assert.That(weaverErrors, Is.Empty);

            string networkServerGetActive = Weaver.NetworkServerGetActive.ToString();
            CheckAddedCode(networkServerGetActive, "WeaverClientServerAttributeTests.NetworkBehaviourServer.NetworkBehaviourServer", "ServerOnlyMethod");
        }

        [Test]
        public void ServerAttributeOnVirutalMethod()
        {
            Assert.That(weaverErrors, Is.Empty);

            string networkServerGetActive = Weaver.NetworkServerGetActive.ToString();
            CheckAddedCode(networkServerGetActive, "WeaverClientServerAttributeTests.ServerAttributeOnVirutalMethod.ServerAttributeOnVirutalMethod", "ServerOnlyMethod");
        }

        [Test]
        public void ServerAttributeOnAbstractMethod()
        {
            Assert.That(weaverErrors, Contains.Item("Server Attribute can't be added to abstract method (at System.Void WeaverClientServerAttributeTests.ServerAttributeOnAbstractMethod.ServerAttributeOnAbstractMethod::ServerOnlyMethod())"));
        }

        [Test]
        public void ServerAttributeOnOverrideMethod()
        {
            Assert.That(weaverErrors, Is.Empty);

            string networkServerGetActive = Weaver.NetworkServerGetActive.ToString();
            CheckAddedCode(networkServerGetActive, "WeaverClientServerAttributeTests.ServerAttributeOnOverrideMethod.ServerAttributeOnOverrideMethod", "ServerOnlyMethod");
        }

        [Test]
        public void NetworkBehaviourClient()
        {
            Assert.That(weaverErrors, Is.Empty);

            string networkClientGetActive = Weaver.NetworkClientGetActive.ToString();
            CheckAddedCode(networkClientGetActive, "WeaverClientServerAttributeTests.NetworkBehaviourClient.NetworkBehaviourClient", "ClientOnlyMethod");
        }

        /// <summary>
        /// Checks that first Instructions in MethodBody is addedString
        /// </summary>
        /// <param name="addedString"></param>
        /// <param name="methodName"></param>
        static void CheckAddedCode(string addedString, string className, string methodName)
        {
            string assemblyName = Path.Combine(WeaverAssembler.OutputDirectory, WeaverAssembler.OutputFile);
            using (AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(assemblyName))
            {
                TypeDefinition type = assembly.MainModule.GetType(className);
                MethodDefinition method = type.Methods.First(m => m.Name == methodName);
                MethodBody body = method.Body;

                Instruction top = body.Instructions[0];

                Assert.AreEqual(top.OpCode, OpCodes.Call);
                Assert.AreEqual(top.Operand.ToString(), addedString);
            }
        }
    }
}
