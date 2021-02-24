using System.Threading.Tasks;
using OrchardCore.Data.Migration;
using OrchardCore.Recipes.Services;

namespace OrchardCore.Menu
{
    public class Migrations : DataMigration
    {
        private readonly IRecipeMigrator _recipeMigrator;

        public Migrations(IRecipeMigrator recipeMigrator)
        {
            _recipeMigrator = recipeMigrator;
        }

        public async Task<int> CreateAsync()
        {
            await _recipeMigrator.ExecuteAsync("menu.recipe.json", this);

            // Shortcut other migration steps on new content definition schemas.
            return 3;
        }

        // Add content menu. This only needs to run on old content definition schemas.
        // This code can be removed in a later version.
        public async Task<int> UpdateFrom1Async()
        {
            await _recipeMigrator.ExecuteAsync("content-menu-updatefrom1.recipe.json", this);

            return 2;
        }

        // Add html menu. This only needs to run on old content definition schemas.
        // This code can be removed in a later version.
        public async Task<int> UpdateFrom2Async()
        {
            await _recipeMigrator.ExecuteAsync("html-menu-updatefrom2.recipe.json", this);

            return 3;
        }
    }
}
