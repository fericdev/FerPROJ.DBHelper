using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.DBCrud {
    public static class BaseRepositoryHelper {
        public static async Task<TResult> ExecuteMethodAsync<TResult>(Type repositoryType, string methodName, params object[] parameters) where TResult : class {

            var method = ResolveMethod(repositoryType, methodName, parameters);

            if (method == null) {
                throw new InvalidOperationException("Method not found.");
            }

            using (var freshDbContext = (DbContext)Activator.CreateInstance(CAppConstants.DbContextType)) {
                
                var instance = Activator.CreateInstance(repositoryType, freshDbContext);

                // Invoke method
                var taskObject = method.Invoke(instance, parameters);

                if (taskObject is Task task)
                {
                    // Await the task to ensure it completes
                    await task;

                    // If it's Task<TResult>, extract Result
                    if (task.GetType().IsGenericType) {
                        var resultProperty = task.GetType().GetProperty("Result");
                        return (TResult)resultProperty.GetValue(task);
                    }
                }
            }

            // If it's just Task (no result)
            return default;
        }

        private static MethodInfo ResolveMethod(
            Type repositoryType,
            string methodName,
            object[] parameters) {
            var methods = repositoryType
                .GetMethods()
                .Where(m => m.Name == methodName);

            foreach (var method in methods) {
                var methodParams = method.GetParameters();

                if (methodParams.Length != parameters.Length)
                    continue;

                bool match = true;

                for (int i = 0; i < methodParams.Length; i++) {
                    var paramType = methodParams[i].ParameterType;
                    var arg = parameters[i];

                    if (arg == null) {
                        // null is valid only for reference types or nullable types
                        if (paramType.IsValueType &&
                            Nullable.GetUnderlyingType(paramType) == null) {
                            match = false;
                            break;
                        }
                    }
                    else if (!paramType.IsAssignableFrom(arg.GetType())) {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return method;
            }

            return null;
        }
    }
}
