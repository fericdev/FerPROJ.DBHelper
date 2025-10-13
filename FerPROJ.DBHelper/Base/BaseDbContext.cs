using FerPROJ.DBHelper.Entity.Users;
using FerPROJ.Design.Class;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FerPROJ.DBHelper.Base {
    public partial class BaseDbContext : DbContext {
        public BaseDbContext() : base("name=BaseDbConnection") {
            Database.Connection.ConnectionString = CAppConstants.ENTITY_CONNECTION_STRING;
            Database.SetInitializer<BaseDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
    }
}
