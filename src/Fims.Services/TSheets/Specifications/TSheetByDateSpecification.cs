using System;
using System.Linq.Expressions;

using Fims.Data.Entities;


namespace Fims.Services.TSheets.Specifications
{
    internal class TSheetByDateSpecification : Specification<TSheet>
    {
        private readonly DateTime MinDateTime;
        private readonly DateTime MaxDateTime;

        internal TSheetByDateSpecification(
            DateTime? minDateTime = default,
            DateTime? maxDateTime = default)
        {
            this.MinDateTime = minDateTime ?? default;
            this.MaxDateTime = maxDateTime ?? DateTime.MaxValue;
        }

        public override Expression<Func<TSheet, bool>> ToExpression()
            => tSheet => this.MinDateTime < tSheet.InspectionEndDateTime && this.MaxDateTime > tSheet.InspectionEndDateTime;
    }
}
