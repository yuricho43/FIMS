using Fims.Data.Entities;
using System;
using System.Linq.Expressions;


namespace Fims.Services.TSheets.Specifications
{
    internal class TSheetByModelSpecification : Specification<TSheet>
    {
        private readonly string Model;

        internal TSheetByModelSpecification(string model) => this.Model = model;

        protected override bool Include => this.Model != null;

        public override Expression<Func<TSheet, bool>> ToExpression()
            => tSheet => tSheet.ProductModel.ToLower().Contains(this.Model.ToLower());
    }
}
