using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

//using Serilog;
using AutoMapper;

using Fims.Data;
using Fims.Data.Models;
using Fims.Data.Models.TSheets;
using Fims.Data.Entities;
using Fims.Services.TSheets.Specifications;

namespace Fims.Services.TSheets
{

    public class TSheetsService : BaseService<TSheet>, ITSheetsService
    {
        private const int TSheetsPerPage = 6;
        private ILogger<TSheetsService> logger;

        public TSheetsService(FimsDbContext dbContext, IMapper mapper, ILogger<TSheetsService> logger)
            : base(dbContext, mapper)
        {
            this.logger = logger;
        }

        public async Task<int> CreateAsync(TSheet tSheet, string userId, int itype)
        {
            //var tSheet = new TSheet
            //{
            //    Customer = tSheet.Customer,
            //    Description = tSheet.Description,
            //    ImageSource = tSheet.ImageSource,
            //    Quantity = tSheet.Quantity,
            //    Date = tSheet.Date,
            //};
            if (itype == 1)         // create
            {
                await this.TheDbContext.AddAsync(tSheet);
            }
            else if (itype == 2)    // update
            {
                var allTSheetsList = await this.All().AsNoTracking().ToListAsync();
                if (allTSheetsList.Any()) {
                    TSheet ts = allTSheetsList.Where(x => x.ProductSerial == tSheet.ProductSerial).LastOrDefault();
                    //await DeleteAsync(ts.Id); 
                    await this.TheDbContext.AddAsync(tSheet);
                    logger.LogInformation($"serial({tSheet.ProductSerial}) is updated (deleted and addedd)");
                }
                else
                {
                    await this.TheDbContext.AddAsync(tSheet);
                }
            }
            int writtenEntriesCount = await this.TheDbContext.SaveChangesAsync(); //JBH FIXME: use the return value
            logger.LogInformation($"TSheet created and saved to DB for: itype:1(add), 2(update)({itype}) Serial({tSheet.ProductSerial}) Inspector({tSheet.InspectorName}) Closer({tSheet.CloserName}) by: UserId({userId})");

            return tSheet.Id;
        }

        public async Task<Result> UpdateAsync(int id, TSheet newTSheet, string userId)
        {
            var tSheet = await this.FindTSheetWithTItemsByIdAsync(id);

            if (tSheet == null)
            {
                return false;
            }

            //tSheet.Customer = newTSheet.Customer;
            //tSheet.Description = newTSheet.Description;
            //tSheet.ImageSource = newTSheet.ImageSource;
            //tSheet.Quantity = newTSheet.Quantity;
            //tSheet.Date = newTSheet.Date;

            tSheet = newTSheet;

            int writtenEntriesCount = await this.TheDbContext.SaveChangesAsync(); //JBH FIXME: use the return value
            logger.LogInformation($"TSheet updated to DB for: Serial({tSheet.ProductSerial}) Inspector({tSheet.InspectorName}) Closer({tSheet.CloserName}) by: UserId({userId})");

            return true;
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var tSheet = await this.FindTSheetWithTItemsByIdAsync(id);

            if (tSheet == null)
            {
                return false;
            }

            var tItems = this.TheDbContext.TItems.Where(t => t.TSheetId == id);
            this.TheDbContext.TItems.RemoveRange(tItems);
 
            this.TheDbContext.Remove(tSheet);

            int writtenEntriesCount = await this.TheDbContext.SaveChangesAsync(); //JBH FIXME: use the return value
            logger.LogInformation($"TSheet deleteed from DB for: Serial({tSheet.ProductSerial}) Inspector({tSheet.InspectorName}) Closer({tSheet.CloserName})");

            return true;
        }

        //public async Task<TSheet> DetailsAsync(int id)
        //    => await this.Mapper
        //        .ProjectTo<TSheet>(this
        //            .AllAsNoTracking()
        //            .Where(p => p.Id == id))
        //        .FirstOrDefaultAsync();

        public async Task<TSheetsComplexSearchResponseModel> ComplexSearchAsync(TSheetsComplexSearchRequestModel searchRequest)
        {
            searchRequest.MaxDateTime = DateTime.Now; //JBH to fix the "Overflow Exception of the conversion from double to decimal.

            var specification = this.GetTSheetSpecification(searchRequest);

            //var tSheets = await this.Mapper
            //    .ProjectTo<TSheetsListingResponseModel>(this
            //        .AllAsNoTracking()
            //        .Where(specification)
            //        .OrderBy(x => x.Id) //JBH add to supress an warning
            //        .Skip((searchRequest.Page - 1) * TSheetsPerPage)
            //        .Take(TSheetsPerPage))
            //    .ToListAsync();
            var tSheets = new List<TSheet>();

            var totalPages = await this.GetNumberOfTotalPages(searchRequest);

            return new TSheetsComplexSearchResponseModel
            {
                TSheets = tSheets,
                Page = searchRequest.Page,
                TotalPages = totalPages
            };
        }

        public async Task<IEnumerable<TSheet>> AllTSheetsAsync()
        {
            // return await this.Mapper
            //     .ProjectTo<TSheet>(this
            //         .AllAsNoTracking())
            //     .ToListAsync();

            //JBH: defined @ BaseService
            // protected IQueryable<TEntity> All() => this.TheDbContext.Set<TEntity>();
            // protected IQueryable<TEntity> AllAsNoTracking() => this.All().AsNoTracking();

            //var allTSheets = this.All().AsNoTracking();
            //var allTSheetsResponseList = await this.Mapper.ProjectTo<TSheetsListingResponseModel>(allTSheets).ToListAsync();
            //return allTSheetsResponseList;
            var allTSheetsList = await this.All().AsNoTracking().ToListAsync();
            return allTSheetsList;
        }

        public async Task<TSheet> FindTSheetWithTItemsByIdAsync(int id)
            => await this
                .All()
                .Include(c => c.TItems)
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

        private async Task<int> GetNumberOfTotalPages(TSheetsComplexSearchRequestModel model) //JBH: changed GetTotalPages to GetNumberOfTotalPages
        {
            var specification = this.GetTSheetSpecification(model);

            var total = await this
                .AllAsNoTracking()
                .Where(specification)
                .CountAsync();

            return (int)Math.Ceiling((double)total / TSheetsPerPage);
        }

        private Specification<TSheet> GetTSheetSpecification(TSheetsComplexSearchRequestModel searchRequest)
            => new TSheetByCustomerSpecification(searchRequest.Customer)
                .And(new TSheetByDateSpecification(searchRequest.MinDateTime, searchRequest.MaxDateTime))
                .And(new TSheetBySheetIdSpecification(searchRequest.TSheetId));
    }
}
