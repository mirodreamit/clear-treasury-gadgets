using CT.Application.Abstractions.Enums;
using CT.Application.Abstractions.QueryParameters;
using CT.Application.Features.Categories.Commands;
using CT.Application.Features.Categories.Queries;
using CT.Application.Features.GadgetCategories.Commands;
using CT.Application.Features.Gadgets.Commands;
using CT.Application.Features.Gadgets.Queries;
using CT.Tests.Fixtures;
using static CT.Application.Features.Gadgets.Commands.CreateGadgetFullCommand;

namespace CT.Tests.Features.UseCases
{
    public class CreateGadgetWithCategoriesTest : BaseFixture
    {
        [Fact]
        public async void ShouldCreateNewCategories()
        {
            var qIds = await CreateTwoCategoriesAsync();

            // cleanup

            await DeleteCategoriesAsync(qIds);

            Assert.True(qIds.Count > 0);
        }

        [Fact]
        public async void ShouldCreateGadgetWithCategories()
        { 
            var gadget = await CreateGadgetWithCategories();

            // cleanup
            await DeleteGadgetFull(gadget.Id);
            await DeleteCategoriesAsync(gadget.CategoriesCreatedIds);

            Assert.True(gadget != null);
        }


        [Fact]
        public async void ShouldChangeGadgetCategories()
        {
            var newGadget = await CreateGadgetWithCategories();

            var gadgetQuery = new GetGadgetFullByIdQuery(newGadget.Id);
            var gadgetResponse = await _mediator.Send(gadgetQuery);

            var gadget = gadgetResponse.Model!;

            GadgetCategoryResponseModel qq = gadget.GadgetCategories.Items.First();

            var newOrdinal = 10000;
            var updateGCCmd = new UpsertGadgetCategoryCommand(qq.Id, new UpsertGadgetCategoryCommand.CreateGadgetCategoryRequestModel
            {
                Ordinal = newOrdinal,
                CategoryId = qq.CategoryId,
                GadgetId = qq.GadgetId
            });

            await _mediator .Send(updateGCCmd); 

            var gadgetResponse2 = await _mediator.Send(gadgetQuery);
            var gadget2 = gadgetResponse2.Model!;

            // cleanup
            await DeleteGadgetFull(newGadget.Id);
            await DeleteCategoriesAsync(newGadget.CategoriesCreatedIds);

            Assert.True(gadget2.GadgetCategories.Items.FirstOrDefault(x=>x.Ordinal == newOrdinal) != null);
        }

        [Fact]
        public async void ShouldSearchCategories()
        {
            var qIds = await CreateTwoCategoriesAsync();

            var foundCategoryIds = await SearchCategoriesAsync(); 

            // cleanup
            await DeleteCategoriesAsync(qIds);

            Assert.True(foundCategoryIds.Count > 0);
        }

        #region private methods
        private async Task<List<Guid>> SearchCategoriesAsync()
        {
            var query = new GetCategoriesQuery()
            {
                FilterParameters =
                [
                    new FilterQueryParameter("name",
                                [
                                    new(FilterQueryOperation.Gte, "mo"),
                                    new(FilterQueryOperation.Contains, "obil")
                                ])
                ]
            };

            var response = await _mediator.Send(query);
            
            var qIds = response.Model!.Select(x=>x.Id).ToList();

            return qIds;
        }

        private async Task<CreateGadgetFullResponseModel> CreateGadgetWithCategories()
        {
            var newCategoryIds = await CreateTwoCategoriesAsync();

            var gadget = new CreateGadgetFullCommand(Guid.NewGuid(), new CreateGadgetFullRequestModel
            {
                Name = "This is a test Gadget 11",
                Categories =
                [
                    new CreateAssignCategoryRequestModel
                    {
                        CategoryId = newCategoryIds.First()
                    },
                    new CreateAssignCategoryRequestModel
                    {
                        CategoryName = "Smartphones"
                    },
                    new CreateAssignCategoryRequestModel
                    {
                        CategoryId = newCategoryIds.Skip(1).First()
                    },
                ]
            });

            var gadgetResponse = await _mediator.Send(gadget);

            newCategoryIds.AddRange(gadgetResponse.Model!.CategoriesCreatedIds);

            gadgetResponse.Model.CategoriesCreatedIds = newCategoryIds;

            return gadgetResponse.Model!;
        }

        private async Task<Guid> CreateGadgetCategoryAsync(Guid GadgetId, Guid CategoryId, int ordinal)
        {
            var q1 = new UpsertGadgetCategoryCommand(Guid.NewGuid(), new UpsertGadgetCategoryCommand.CreateGadgetCategoryRequestModel { GadgetId = GadgetId, CategoryId = CategoryId, Ordinal = ordinal } );

            var q1Response = await _mediator.Send(q1);

            return q1Response.Model!.Id;
        }

        private async Task<List<Guid>> CreateTwoCategoriesAsync()
        {
            var q1 = new UpsertCategoryCommand(Guid.NewGuid(), new UpsertCategoryCommand.CreateCategoryRequestModel
            {
                Name = "This is a test category 1"
            });

            var q1Response = await _mediator.Send(q1);

            var q2 = new UpsertCategoryCommand(Guid.NewGuid(), new UpsertCategoryCommand.CreateCategoryRequestModel
            {
                Name = "This is a test category 2"
            });

            var q2Response = await _mediator.Send(q2);

            return [q1Response.Model!.CategoryId, q2Response.Model!.CategoryId];
        }
        
        private async Task DeleteGadgetCategoriesAsync(List<Guid> ids)
        {
            foreach (var qId in ids)
            {
                var delQ = new DeleteGadgetCategoryCommand(qId);
                await _mediator.Send(delQ);
            }
        }

        private async Task DeleteCategoriesAsync(List<Guid> ids)
        {
            foreach (var qId in ids)
            {
                var delQ = new DeleteCategoryCommand(qId);
                await _mediator.Send(delQ);
            }
        }

        private async Task DeleteGadgetFull(Guid id)
        {
            var delGadgetCmd = new DeleteGadgetFullCommand(id);

            await _mediator.Send(delGadgetCmd);
        }

        #endregion
    }
}