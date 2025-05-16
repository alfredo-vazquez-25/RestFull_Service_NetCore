using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Northwind.DataAccess
{
    public class SqlDependencyResolver : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            if (assemblyName.Name == "System.Data.SqlClient")
            {
                string path = Path.Combine(AppContext.BaseDirectory, "runtimes", "win", "lib", "net8.0", "System.Data.SqlClient.dll");
                if (File.Exists(path))
                {
                    return LoadFromAssemblyPath(path);
                }
            }
            return null;
        }
    }
}