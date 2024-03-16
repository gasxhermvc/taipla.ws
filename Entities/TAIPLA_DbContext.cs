using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taipla.Webservice.Cores;
using Taipla.Webservice.Helpers.DbUtilities;

namespace Taipla.Webservice.Entities
{
    public class TAIPLA_DbContext : xxxContext
    {
        public readonly IDbUtilityService Utility;
        public readonly EFUploadFileUtilities UploadFileUtilities;
        public readonly EFFindUsers UserUtilities;
        public readonly EFFindCountry CountryUtilities;
        public readonly EFFindCulture CultureUtilities;
        private readonly IConfiguration _configuration;

        public TAIPLA_DbContext(IDbUtilityService utility, IConfiguration configuration, IWebHostEnvironment env)
        {
            Utility = utility;
            _configuration = configuration;
            UploadFileUtilities = new EFUploadFileUtilities(this, env, configuration);
            UserUtilities = new EFFindUsers(this);
            CountryUtilities = new EFFindCountry(this);
            CultureUtilities = new EFFindCulture(this);
            Utility.SetDbContext(this);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql(_configuration.GetConnectionString("DefaultConnection"));
            }
        }
    }
}
