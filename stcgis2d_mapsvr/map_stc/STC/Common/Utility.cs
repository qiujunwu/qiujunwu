using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace STC.Common
{
    public class Utility
    {
        public static void appendSQL(ref string where, string append)
        {
            where += (where == "") ? " where " + append : " and " + append;
        }
    }
}