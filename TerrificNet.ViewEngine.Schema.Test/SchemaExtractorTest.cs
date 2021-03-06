﻿using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Schema;
using Veil.Handlebars;
using Veil.Helper;

namespace TerrificNet.ViewEngine.Schema.Test
{
    [TestClass]
    public class SchemaExtractorTest
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Description("Test whether a single property is included in the schema")]
        public void TestSimpleSingleProperty()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "simpleSingleProperty.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Name", JSchemaType.String);
        }

        [TestMethod]
        [Description("Test whether a single property is included in the schema")]
        public void TestSimpleSinglePropertyPath()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "simpleSinglePropertyPath.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JSchemaType.String);
        }

        [TestMethod]
        [Description("Test whether a multiple properties are included in the schema")]
        public void TestMultipleProperties()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "multipleProperties.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Title", JSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Age", JSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Order", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"].Properties["Order"], "Count", JSchemaType.String);
        }

        [TestMethod]
        [Description("A property inside a if expression is not required")]
        public void TestNoRequiredPropertys()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "noRequiredProperty.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JSchemaType.String, false);
        }

        [TestMethod]
        [Description("A property only inside a if expression is a boolean")]
        public void TestBooleanProperty()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "booleanProperty.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "HasName", JSchemaType.Boolean, false);
        }

        [TestMethod]
        [Description("The helper shoudn't be part of the schema.")]
        public void TestIgnoreHelpers()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());

	        var helper = new Mock<IHelperHandler>();
	        helper.Setup(m => m.IsSupported(It.IsAny<string>())).Returns((string s) => s.StartsWith("helper"));

	        var helperHandlers = new [] { helper.Object };
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "ignoreHelpers.mustache")), null, helperHandlers);

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Name", JSchemaType.String);

            Assert.IsFalse(schema.Properties.ContainsKey("helper param=\"val1\""), "No property helper should be inside the schema.");
            //Assert.IsTrue(schema.Properties.ContainsKey("noregistredHelper param=\"val1\""), "The none registred helpers should be still included.");
            SchemaAssertions.AssertSingleProperty(schema, "variableExpressionWithWhitespace", JSchemaType.String);
        }

        [TestMethod]
        [Description("A property used inside a each expression is a array")]
        public void TestArrayProperty()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "arrayProperty.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"], "Addresses", JSchemaType.Array);
            Assert.IsNotNull(schema.Properties["Customer"].Properties["Addresses"].Items, "an items array should be given for an array type.");
            Assert.AreEqual(1, schema.Properties["Customer"].Properties["Addresses"].Items.Count, "expectects exactly on item inside items");

            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"].Properties["Addresses"].Items[0], "Street", JSchemaType.String);
            SchemaAssertions.AssertSingleProperty(schema.Properties["Customer"].Properties["Addresses"].Items[0], "ZipCode", JSchemaType.String);
        }

        [TestMethod]
        [Description("A property used inside a each expression is a array")]
        public void TestArrayPropertyParent()
        {
            var schemaExtractor = new SchemaExtractor(new HandlebarsParser());
			var schema = schemaExtractor.Run("test", new StreamReader(Path.Combine(TestContext.DeploymentDirectory, "arrayPropertyParent.mustache")), null, null);

            SchemaAssertions.AssertSingleProperty(schema, "Customer", JSchemaType.Object);
            SchemaAssertions.AssertSingleProperty(schema, "Name", JSchemaType.String);
        }
    }
}
