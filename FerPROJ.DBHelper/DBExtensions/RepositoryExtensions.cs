using FerPROJ.DBHelper.DBCrud;
using FerPROJ.DBHelper.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBExtensions {
    public static class RepositoryExtensions {
        public static Type GetRepositoryType<TEntity>() where TEntity : class {
            var repoType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => {

                    if (t.IsAbstract) 
                        return false;

                    var baseType = t.BaseType;

                    while (baseType != null && baseType.IsGenericType) {

                        var genericDef = baseType.GetGenericTypeDefinition();

                        if (genericDef == typeof(BaseRepository<,,,>)) {

                            var genericArgs = baseType.GetGenericArguments();

                            if (genericArgs[2] == typeof(TEntity)) // third type parameter is TEntity
                                return true;

                        }

                        baseType = baseType.BaseType;
                    }

                    return false;
                });

            if (repoType == null)
                throw new InvalidOperationException($"Repository for {typeof(TEntity).Name} not found.");

            return repoType;
        }
    }
}
