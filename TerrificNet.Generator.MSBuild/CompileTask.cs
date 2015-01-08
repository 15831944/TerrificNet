﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TerrificNet.Configuration;
using TerrificNet.ViewEngine;
using TerrificNet.ViewEngine.SchemaProviders;
using Newtonsoft.Json.Schema;

namespace TerrificNet.Generator.MSBuild
{
    public class CompileTask : Task
    {
        public override bool Execute()
        {
            Execute(SourcePath, OutputAssembly);

            return true;
        }

        public static void Execute(string sourcePath, string outputAssembly)
        {
            ExecuteInternal(sourcePath, (c, s) => CompileToAssembly(c, s, outputAssembly));
        }

        public static void ExecuteFile(string sourcePath, string fileName)
        {
            ExecuteInternal(sourcePath, (c, s) => WriteToFile(c, s, fileName));
        }

        public static string ExecuteString(string sourcePath)
        {
            using (var stream = new MemoryStream())
            {
                ExecuteInternal(sourcePath, (c, s) => c.WriteTo(s, stream));

                stream.Seek(0, SeekOrigin.Begin);

                return new StreamReader(stream).ReadToEnd();
            }
        }

        private static void ExecuteInternal(string sourcePath, Action<JsonSchemaCodeGenerator, IEnumerable<JsonSchema>> executeAction)
        {
            var config = new TerrificNetConfig
            {
                BasePath = sourcePath,
                ViewPath = Path.Combine(sourcePath, "views"),
                ModulePath = Path.Combine(sourcePath, "components/modules")
            };

            var schemaProvider = new HandlebarsViewSchemaProvider();
            var repo = new TerrificTemplateRepository(config);
            var codeGenerator = new JsonSchemaCodeGenerator();

            var schemas = repo.GetAll().Select(t =>
            {
                var schema = schemaProvider.GetSchemaFromTemplate(t);
                schema.Title = t.Id + "Model";
                return schema;
            }).ToList();

            executeAction(codeGenerator, schemas);
        }

        private static void WriteToFile(JsonSchemaCodeGenerator codeGenerator, IEnumerable<JsonSchema> schemas,
            string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.Create))
            {
                codeGenerator.WriteTo(schemas, stream);
            }
        }

        private static void CompileToAssembly(JsonSchemaCodeGenerator codeGenerator, IEnumerable<JsonSchema> schemas, string outputAssembly)
        {
            using (var stream = new FileStream(outputAssembly, FileMode.Create))
            {
                codeGenerator.CompileTo(schemas, stream);
            }
        }

        [Required]
        public string SourcePath { get; set; }

        [Required]
        public string OutputAssembly { get; set; }
    }
}
