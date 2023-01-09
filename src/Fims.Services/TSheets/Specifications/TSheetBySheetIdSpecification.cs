using System;
using System.Linq.Expressions;

using Fims.Data.Entities;


namespace Fims.Services.TSheets.Specifications
{
    internal class TSheetBySheetIdSpecification : Specification<TSheet>
    {
        private readonly int? TSheetId;

        internal TSheetBySheetIdSpecification(int? tSheetId)
            => this.TSheetId = tSheetId;

        protected override bool Include => this.TSheetId != null;

        public override Expression<Func<TSheet, bool>> ToExpression()
            //=> tSheet => tSheet.Id.Id == this.Id;
            => tSheet => tSheet.Id == this.TSheetId;
    }
}