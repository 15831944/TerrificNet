﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TerrificNet.ViewEngine.Config;

namespace TerrificNet.ViewEngine
{
    public class DefaultModuleRepository : IModuleRepository
    {
        private readonly ITerrificNetConfig _configuration;
        private readonly ITemplateRepository _templateRepository;
	    private readonly Lazy<List<ModuleDefinition>> _getAllCache; 

        public DefaultModuleRepository(ITerrificNetConfig configuration, ITemplateRepository templateRepository)
        {
            _configuration = configuration;
            _templateRepository = templateRepository;

			_getAllCache = new Lazy<List<ModuleDefinition>>(() => _templateRepository.GetAll()
				.Where(t => t.Id.StartsWith(_configuration.ModulePath))
				.GroupBy(t => Path.GetDirectoryName(t.Id))
				.Select(CreateModuleDefinition)
				.ToList());
        }

        public IEnumerable<ModuleDefinition> GetAll()
        {
            return _getAllCache.Value;
        }

        private static ModuleDefinition CreateModuleDefinition(IGrouping<string, TemplateInfo> t)
        {
            var moduleId = GetModuleId(t.Key);
            var defaultTemplateCandidates = GetDefaultTemplateCandidates(moduleId);
            var defaultTemplate = t.FirstOrDefault(a => defaultTemplateCandidates.Contains(a.Id));
            var templates = t.ToList();

            if (defaultTemplate == null && templates.Count == 1)
                defaultTemplate = templates[0];

            var skins = templates.Where(t1 => t1 != defaultTemplate).ToDictionary(GetSkinName);
            if (defaultTemplate == null && skins.TryGetValue(string.Empty, out defaultTemplate))
                skins.Remove(string.Empty);

            return new ModuleDefinition(moduleId, defaultTemplate, skins);
        }

        private static IEnumerable<string> GetDefaultTemplateCandidates(string moduleId)
        {
            yield return string.Concat(moduleId, '/', "default");
            yield return string.Concat(moduleId, '/', Path.GetFileName(moduleId));
        }

        private static string GetModuleId(string moduleId)
        {
            return moduleId.Replace('\\', '/');
        }

        private static string GetSkinName(TemplateInfo templateInfo)
        {
            var parts = templateInfo.Id.Split('-');
            if (parts.Length > 1)
                return parts[parts.Length - 1];

            return string.Empty;
        }

        public bool TryGetModuleDefinitionById(string id, out ModuleDefinition moduleDefinition)
        {
	        moduleDefinition = this.GetAll().FirstOrDefault(k => id.Equals(k.Id, StringComparison.Ordinal));
	        return moduleDefinition != null;
        }
    }
}
