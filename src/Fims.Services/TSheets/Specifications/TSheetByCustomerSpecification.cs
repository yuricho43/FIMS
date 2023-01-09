using System;
using System.Linq.Expressions;

using Fims.Data.Entities;


namespace Fims.Services.TSheets.Specifications
{
    internal class TSheetByCustomerSpecification : Specification<TSheet>
    {
        private readonly string Customer;

        internal TSheetByCustomerSpecification(string customer) => this.Customer = customer;

        protected override bool Include => this.Customer != null;

        public override Expression<Func<TSheet, bool>> ToExpression()
            => tSheet => tSheet.Customer.ToLower().Contains(this.Customer.ToLower());
    }
}
