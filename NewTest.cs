using Microsoft.Playwright;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Allure.Core;
using NUnit.Framework;
using NUnit.Framework.Internal;
//using NUnit.Framework.Internal.Commands;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;




namespace newplaywright

{
    [AllureNUnit]

    internal class NewTest
    {

        [Test]
        [Category("A")]




        public void TestA()
        {
            Console.WriteLine("Console");
        }

        [Test]
        [Category("B")]
        public void TestB()
        {
            Console.WriteLine("B");
        }

        [Test]
        [Category("C")]
        public void TestC()
        {
            Console.WriteLine("C");
        }

        [Test]
        [Category("A")]
        [Category("B")]
        public void TestAB()
        {
            Console.WriteLine("A-B");
        }

        [Test]
        [Category("A")]
        [Category("C")]
        public void TestAC()
        {
            Console.WriteLine("A-C");
        }

        [Test]
        [Category("B")]
        [Category("C")]
        public void TestBC()
        {
            Console.WriteLine("B-C");
        }

        [Test]
        [Category("A")]
        [Category("B")]
        [Category("C")]
        public void TestABC()
        {
            Console.WriteLine("A-B-C");
        }
    }
}