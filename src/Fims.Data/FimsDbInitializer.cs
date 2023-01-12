using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Fims.Data.Contracts;
using Fims.Data.Entities;

using static Fims.Common.Constants;


namespace Fims.Data
{
    public class FimsDbInitializer : IInitializer
    {
        private readonly FimsDbContext dbContext;
        private readonly UserManager<FimsUser> userManager;
        private readonly RoleManager<FimsRole> roleManager;
        private readonly IEnumerable<IInitialData> initialDataProviders;

        public FimsDbInitializer(
            FimsDbContext dbContext,
            UserManager<FimsUser> userManager,
            RoleManager<FimsRole> roleManager,
            IEnumerable<IInitialData> initialDataProviders)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.initialDataProviders = initialDataProviders;
        }

        public void Initialize()
        {
            //JBH: This will "Migrate the db, so we don't need to execute [PM> Update-Database] manually.
            //     This will create/register an user "admin" with the "Admin" role
            //     This will also seed the initial data (Products & Categories data) to Db upon the first run.
        
            this.dbContext.Database.Migrate();

            this.AddAdministrator();

            foreach (var initialDataProvider in this.initialDataProviders)
            {
                if (this.DataSetIsEmpty(initialDataProvider.EntityType))
                {
                    var data = initialDataProvider.GetData();

                    foreach (var entity in data)
                    {
                        this.dbContext.Add(entity);
                    }
                }
            }

            this.dbContext.SaveChanges();
        }

        private void AddAdministrator()
            => Task
                .Run(async () =>
                {
                    var existingRole = await this.roleManager.FindByNameAsync(AdministratorRole);
                    if (existingRole != null)
                    {
                        return;
                    }


                    // create the AdministratorRole, and create a Administrator
                    var adminRole = new FimsRole(AdministratorRole);
                    await this.roleManager.CreateAsync(adminRole);
                    var adminUser = new FimsUser
                    {
                        FirstName = "장성욱",
                        LastName = "JSW",
                        Email = "admin@fstc.co.kr",
                        UserName = "admin@fstc.co.kr",
                        SecurityStamp = "RandomSecurityStamp"
                    };
                    await this.userManager.CreateAsync(adminUser, "admin123456");
                    await this.userManager.AddToRoleAsync(adminUser, AdministratorRole);


                    // create the ManagerRole, and create a Manager
                    var managerRole = new FimsRole(ManagerRole);
                    await this.roleManager.CreateAsync(managerRole);
                    var managerUser = new FimsUser
                    {
                        FirstName = "홍과장",
                        LastName = "HGJ",
                        Email = "manager@fstc.co.kr",
                        UserName = "manager@fstc.co.kr",
                        SecurityStamp = "RandomSecurityStamp"
                    };
                    await this.userManager.CreateAsync(managerUser, "manager123456");
                    await this.userManager.AddToRoleAsync(managerUser, ManagerRole);


                    // create the InspectorRole, and create an Inspector
                    var inspectorRole = new FimsRole(InspectorRole);
                    await this.roleManager.CreateAsync(inspectorRole);
                    var inspectorUser = new FimsUser
                    {
                        FirstName = "김철수",
                        LastName = "KCS",
                        Email = "inspector@fstc.co.kr",
                        UserName = "inspector@fstc.co.kr",
                        SecurityStamp = "RandomSecurityStamp"
                    };
                    await this.userManager.CreateAsync(inspectorUser, "inspector123456");
                    await this.userManager.AddToRoleAsync(inspectorUser, InspectorRole);


                    // create the ReporterRole, and create an Reporter
                    var reporterRole = new FimsRole(ReporterRole);
                    await this.roleManager.CreateAsync(reporterRole);
                    var reporterUser = new FimsUser
                    {
                        FirstName = "장보고",
                        LastName = "JBG",
                        Email = "reporter@fstc.co.kr",
                        UserName = "reporter@fstc.co.kr",
                        SecurityStamp = "RandomSecurityStamp"
                    };
                    await this.userManager.CreateAsync(reporterUser, "reporter123456");
                    await this.userManager.AddToRoleAsync(reporterUser, ReporterRole);
                })
                .GetAwaiter()
                .GetResult();


        private bool DataSetIsEmpty(Type type)
        {
            var setMethod = this.GetType()
                .GetMethod(nameof(this.GetSet), BindingFlags.Instance | BindingFlags.NonPublic)
                ?.MakeGenericMethod(type);

            var set = setMethod?.Invoke(this, Array.Empty<object>());

            var countMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == nameof(Queryable.Count) && m.GetParameters().Length == 1)
                .MakeGenericMethod(type);

            var result = (int)countMethod.Invoke(null, new[] { set })!;

            return result == 0;
        }

        private DbSet<TEntity> GetSet<TEntity>()
            where TEntity : class
            => this.dbContext.Set<TEntity>();
    }
}
