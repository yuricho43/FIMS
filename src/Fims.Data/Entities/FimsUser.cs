using System;
using System.Collections.Generic;

using Microsoft.AspNetCore.Identity;

using Fims.Data.Contracts;


namespace Fims.Data.Entities
{
    // public class FimsUser : IdentityUser, IAuditInfo, IDeletableEntity
    public class FimsUser : IdentityUser, IAuditInfo
    {
        public FimsUser() => this.Id = Guid.NewGuid().ToString();

        public string HangulName { get; set; }

        public string EnglishName { get; set; }

        //for IAuditInfo
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }

        // //For IDeletableEntity
        // public bool IsDeleted { get; set; }
        // public DateTime? DeletedOn { get; set; }
    }
}