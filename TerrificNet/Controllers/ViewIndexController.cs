﻿using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Practices.Unity;
using TerrificNet.Models;
using TerrificNet.UnityModule;
using TerrificNet.ViewEngine;

namespace TerrificNet.Controllers
{
    public class ViewIndexController : TemplateControllerBase
    {
        private readonly TerrificNetApplication[] _applications;

        public ViewIndexController(TerrificNetApplication[] applications)
        {
            _applications = applications;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
			var model = new ApplicationOverviewModel
            {
                Applications = _applications.Select(a => new ViewOverviewModel
                {
                    Name = a.Name,
                    Views = GetViews(a.Section, a.Container.Resolve<ITemplateRepository>()).ToList()
                }).ToList()
            };

            return View("index", model);
        }

        private IEnumerable<ViewItemModel> GetViews(string section, ITemplateRepository templateRepository)
        {
            foreach (var file in templateRepository.GetAll())
            {
                yield return new ViewItemModel
                {
                    Text = file.Id,
                    Url = string.Format("/{0}", file.Id),
                    EditUrl = string.Format("/web/edit.html?template={0}", file.Id),
                    SchemaUrl = string.Format("{0}schema/{1}", section, file.Id)
                };
            }
        }
    }
}