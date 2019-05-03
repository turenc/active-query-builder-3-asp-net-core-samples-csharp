﻿using System.IO;
using ActiveQueryBuilder.Core;
using ActiveQueryBuilder.Web.Core;
using ActiveQueryBuilder.Web.Server.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ASP.NET_Core.Controllers
{
    public class JavaScriptRenderingDemoController : Controller
    {
        private readonly IQueryBuilderService _aqbs;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _config;

        // Use IQueryBuilderService to get access to the server-side instances of Active Query Builder objects. 
        // See the registration of this service in the Startup.cs.
        public JavaScriptRenderingDemoController(IQueryBuilderService aqbs, IHostingEnvironment env, IConfiguration config)
        {
            _aqbs = aqbs;
            _env = env;
            _config = config;
        }

        const string InstanceID = "JavaScript";

        // GET
        public IActionResult Index()
        {
            if (_aqbs.Get(InstanceID) == null)
                CreateQueryBuilder(InstanceID);

            return View();
        }

        private void CreateQueryBuilder(string AInstanceID)
        {
            // Create an instance of the QueryBuilder object.
            var queryBuilder = _aqbs.Create(AInstanceID);

            // Create an instance of the proper syntax provider for your database server.
            queryBuilder.SyntaxProvider = new MSSQLSyntaxProvider();

            // Denies metadata loading requests from live database connection.
            queryBuilder.MetadataLoadingOptions.OfflineMode = true;

            // Load MetaData from the pre-generated XML document.
            var path = _config["NorthwindXmlMetaData"];
            var xml = Path.Combine(_env.WebRootPath, path);

            queryBuilder.MetadataContainer.ImportFromXML(xml);

            //Set default query.
            queryBuilder.SQL = GetDefaultSql();
        }

        private string GetDefaultSql()
        {
            return @"Select o.OrderID,
                        c.CustomerID,
                        s.ShipperID,
                        o.ShipCity
                    From Orders o
                        Inner Join Customers c On o.CustomerID = c.CustomerID
                        Inner Join Shippers s On s.ShipperID = o.OrderID
                    Where o.ShipCity = 'A'";
        }
    }
}