﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDDTests
{
    // Sitewide definitions and useful methods
    public class Common
    {
        public const string BaseUrl = "https://localhost:7297";
        // Page names that everyone should use
        public const string HomePageName = "Home";
        public const string LoginPageName = "Login";
        public const string AccessPageName = "Access";
        public const string ResultsPageName = "Results";
        public const string CreatePageName = "Create";

        // A handy way to look these up
        public static readonly Dictionary<string, string> Paths = new()
        {
            { HomePageName, "/" },
            { LoginPageName, "/Identity/Account/Login" },
            { AccessPageName, "/Access" },
            { ResultsPageName, "/Access/Results" },
            { CreatePageName, "/Create" }
        };

        public static string PathFor(string pathName) => Paths[pathName];
        public static string UrlFor(string pathName) => BaseUrl + Paths[pathName];
    }
}
