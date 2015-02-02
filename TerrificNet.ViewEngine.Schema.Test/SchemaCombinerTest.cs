﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Schema;

namespace TerrificNet.ViewEngine.Schema.Test
{
    [TestClass]
    public class SchemaCombinerTest
    {
        [TestMethod]
        public void TestTitleUsedFromAvailableSchema()
        {
            const string expectedResult = "title";

            var schema1 = new JsonSchema();
            var schema2 = new JsonSchema
            {
                Title = expectedResult
            };

            var underTest = new SchemaCombiner();
            var result = underTest.Apply(schema1, schema2, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, result.Title);
        }

        [TestMethod]
        public void TestReportTitleConflictWhenBothHaveTitle()
        {
            var schema1 = new JsonSchema
            {
                Title = "s1"
            };
            var schema2 = new JsonSchema
            {
                Title = "s2"
            };

            var underTest = new SchemaCombiner();
            var report = new SchemaComparisionReport();
            var result = underTest.Apply(schema1, schema2, report);

            var failures = report.GetFailures().ToList();

            Assert.AreEqual(1, failures.Count, "One failure expected");
            Assert.IsInstanceOfType(failures[0], typeof (ValueConflict));

            var valueConflict = (ValueConflict)failures[0];
            Assert.AreEqual("title", valueConflict.PropertyName);
            Assert.AreEqual(schema1, valueConflict.SchemaPart);
            Assert.AreEqual(schema2, valueConflict.SchemaBasePart);
            Assert.AreEqual("s1", valueConflict.Value1);
            Assert.AreEqual("s2", valueConflict.Value2);            
        }

        [TestMethod]
        public void TestCombineSchemaProperties()
        {
            var schema1 = new JsonSchema
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    {"prop1", new JsonSchema()}
                }
            };
            var schema2 = new JsonSchema
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    {"prop2", new JsonSchema()}
                }
            };

            var underTest = new SchemaCombiner();
            var result = underTest.Apply(schema1, schema2, null);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(2, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("prop1"), "Expect the property from the first schema.");
            Assert.IsTrue(result.Properties.ContainsKey("prop2"), "Expect the property from the second schema.");
        }

        [TestMethod]
        public void TestCombineSubSchemaProperties()
        {
            var schema1 = new JsonSchema
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    {"prop1", new JsonSchema
                    {
                        Properties = new Dictionary<string, JsonSchema>
                        {
                            {"sub_prop1", new JsonSchema()}
                        }
                    }}
                }
            };
            var schema2 = new JsonSchema
            {
                Properties = new Dictionary<string, JsonSchema>
                {
                    {"prop1", new JsonSchema
                    {
                        Properties = new Dictionary<string, JsonSchema>
                        {
                            {"sub_prop2", new JsonSchema()}
                        }
                    }}
                }
            };

            var underTest = new SchemaCombiner();
            var result = underTest.Apply(schema1, schema2, null);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Properties);
            Assert.AreEqual(1, result.Properties.Count);
            Assert.IsTrue(result.Properties.ContainsKey("prop1"), "Expect the property from both schema.");

            var innerProperties = result.Properties["prop1"].Properties;
            Assert.AreEqual(2, innerProperties.Count);
            Assert.IsTrue(innerProperties.ContainsKey("sub_prop1"), "Expect the property from the first schema.");
            Assert.IsTrue(innerProperties.ContainsKey("sub_prop2"), "Expect the property from the second schema.");
        }
    }
}
