using Tcs.Core.Entity;
using Tcs.RaceTimer.Models;

namespace Tcs.RaceTimer.Repository
{
    public class CategoryRepository : EntityRepository<Category, CategoryList>
    {
        public CategoryRepository() : base("CategoryListIds") { }

        public Category FindByName(string name)
        {
            var categories = GetAll();
            foreach (var category in categories)
            {
                if (category.Name == name)
                    return category;
            }

            return null;
        }
    }
}
