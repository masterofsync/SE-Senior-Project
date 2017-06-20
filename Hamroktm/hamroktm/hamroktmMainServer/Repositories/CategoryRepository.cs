using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using contracts.Models;
using hamroktmMainServer.Database.Public;

namespace hamroktmMainServer.Repositories
{
    public interface ICategoryRepository
    {
        HttpStatusCodeResult PostCategoryAndSub(CategoryContract model);
        HttpStatusCodeResult EditCategoryAndSub(CategoryContract model);
        CategoryContract GetCategoriesAndSubById(int id);
        List<CategoryContract> GetCategoriesAndSub();
        HttpStatusCodeResult DeleteCategory(int id);
        bool checkCategoryifValid(CategoryContract model);
    }

    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private DbSet<Database.Public.Category> CategoriesDbSet { get; set; }
        private DbSet<Database.Public.SubCategory> SubCategoryDbSet { get; set; }
        private DbSet<Database.Public.Category_SubCategory> Category_SubCategoryDbSet { get; set; }

        public CategoryRepository()
        {
            CategoriesDbSet = hamroktmDb.Categories;
            SubCategoryDbSet = hamroktmDb.SubCategories;
            Category_SubCategoryDbSet = hamroktmDb.Category_SubCategory;
        }

        //Category
        public HttpStatusCodeResult PostCategoryAndSub(CategoryContract model)
        {
            if (checkCategoryifValid(model))
            {
                using (var db = hamroktmDb)
                {
                    var newCategory = new Category()
                    {
                        Name = model.Category.Name
                    };

                    CategoriesDbSet.Add(newCategory);
                    SaveChanges();

                    foreach (var sub in model.SubCategories)
                    {
                        //checking if the provided subcategory already exists
                        var subExists = db.SubCategories.FirstOrDefault(x => x.Name == sub.Name);

                        //if not exists, create new sub category
                        if (subExists == null)
                        {
                            var newSubCat = new SubCategory()
                            {
                                Name = sub.Name
                            };

                            SubCategoryDbSet.Add(newSubCat);

                            var entity = new Category_SubCategory()
                            {
                                CategoryId = newCategory.CategoryId,
                                SubCategoryId = newSubCat.SubCategoryId
                            };
                            Category_SubCategoryDbSet.Add(entity);
                        }
                        //if exists, dont create new sub category just take the id from sub category
                        else
                        {
                            var entity = new Category_SubCategory()
                            {
                                CategoryId = newCategory.CategoryId,
                                SubCategoryId = subExists.SubCategoryId
                            };
                            Category_SubCategoryDbSet.Add(entity);

                        }
                        SaveChanges();
                    }

                    return new HttpStatusCodeResult(HttpStatusCode.OK);

                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //Category Edit 
        public HttpStatusCodeResult EditCategoryAndSub(CategoryContract model)
        {
            if (checkCategoryifValid(model))
            {
                using (var db = hamroktmDb)
                {
                    var record = CategoriesDbSet.FirstOrDefault(x => x.CategoryId == model.Category.CategoryId);

                    //check if categoryname is different.. if yes update it
                    if (record.Name != model.Category.Name)
                    {
                        record.Name = model.Category.Name;
                    }
                    SaveChanges();

                    bool anyChange = false;
                    //remove all subCategory if there is any change
                    var removesubCategoryId =
                        Category_SubCategoryDbSet.Where(x => x.CategoryId == model.Category.CategoryId);


                    if (removesubCategoryId.Count() != model.SubCategories.Count)
                    {
                        anyChange = true;
                    }

                    var ModelIndex = 0;

                    //check if subcategory names are same or if it is changed, set the anychange value to true
                    if (!anyChange)
                    {
                        foreach (var item in removesubCategoryId)
                        {
                            if (item.SubCategory.Name != model.SubCategories[ModelIndex].Name)
                            {
                                anyChange = true;
                                break;
                            }
                            ModelIndex++;
                        }
                    }

                    if (anyChange)
                    {
                        if (removesubCategoryId.Any())
                        {
                            foreach (var sub in removesubCategoryId)
                            {
                                //Checking if the subcategory is used with other categories.
                                var totalSubId =
                                    Category_SubCategoryDbSet.Where(x => x.SubCategoryId == sub.SubCategoryId);

                                //removing all the connection one by one
                                Category_SubCategoryDbSet.Remove(sub);

                                //if subcategory to be deleted is not used with other sub categories
                                if (totalSubId.Count() == 1)
                                {
                                    var removeSub =
                                        SubCategoryDbSet.FirstOrDefault(x => x.SubCategoryId == sub.SubCategoryId);
                                    SubCategoryDbSet.Remove(removeSub);
                                }
                            }
                            SaveChanges();
                        }

                        foreach (var sub in model.SubCategories)
                        {
                            //checking if the provided subcategory already exists
                            var subExists = SubCategoryDbSet.FirstOrDefault(x => x.Name == sub.Name);

                            //if not exists, create new sub category
                            if (subExists == null)
                            {
                                var newSubCat = new SubCategory
                                {
                                    Name = sub.Name
                                };

                                SubCategoryDbSet.Add(newSubCat);
                                Category_SubCategoryDbSet.Add(new Category_SubCategory()
                                {
                                    CategoryId = model.Category.CategoryId,
                                    SubCategoryId = newSubCat.SubCategoryId
                                });
                            }
                            //if exists, dont create new sub category just take the id from sub category
                            else
                            {
                                Category_SubCategoryDbSet.Add(new Category_SubCategory()
                                {
                                    CategoryId = model.Category.CategoryId,
                                    SubCategoryId = subExists.SubCategoryId
                                });

                            }
                            SaveChanges();
                        }
                    }
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }

            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        //CategoryById
        public CategoryContract GetCategoriesAndSubById(int id)
        {
            var singleCategory = CategoriesDbSet.FirstOrDefault(x => x.CategoryId == id);

            if (singleCategory != null)
            {
                var finalSingleCategoryAndSub = new CategoryContract();

                finalSingleCategoryAndSub.Category.CategoryId = singleCategory.CategoryId;
                finalSingleCategoryAndSub.Category.Name = singleCategory.Name;

                var subList = Category_SubCategoryDbSet.Where(x => x.CategoryId == singleCategory.CategoryId);

                foreach (var subCategory in subList)
                {
                    var subcat = new SubCategoryContract()
                    {
                        Name = subCategory.SubCategory.Name,
                        SubCategoryId = subCategory.SubCategory.SubCategoryId
                    };
                    finalSingleCategoryAndSub.SubCategories.Add(subcat);
                }
                return finalSingleCategoryAndSub;
            }
            return null;
        }


        public List<CategoryContract> GetCategoriesAndSub()
        {
            var allCategoriesSub = CategoriesDbSet;

            if (allCategoriesSub != null)
            {
                var finalCategoryAndSub = new List<CategoryContract>();
                foreach (var cat in allCategoriesSub)
                {
                    var totalCategoryAndSub = new CategoryContract();

                    totalCategoryAndSub.Category.CategoryId = cat.CategoryId;
                    totalCategoryAndSub.Category.Name = cat.Name;

                    var subIdList = Category_SubCategoryDbSet.Where(x => x.CategoryId == cat.CategoryId);

                    foreach (var subcategory in subIdList)
                    {
                        var subcat = new SubCategoryContract
                        {
                            Name = subcategory.SubCategory.Name,
                            SubCategoryId = subcategory.SubCategory.SubCategoryId
                        };

                        totalCategoryAndSub.SubCategories.Add(subcat);
                    }
                    finalCategoryAndSub.Add(totalCategoryAndSub);
                }
                return finalCategoryAndSub;
            }
            return null;
        }

        //Deleting CAtegory by Category Id
        public HttpStatusCodeResult DeleteCategory(int id)
        {
            var deleteCategory = CategoriesDbSet.FirstOrDefault(x => x.CategoryId == id);
            if (deleteCategory != null)
            {

                CategoriesDbSet.Remove(deleteCategory);

                //getting all category_subcategory columns where category id matches
                var removesubCategoryId = Category_SubCategoryDbSet.Where(x => x.CategoryId == id);

                if (removesubCategoryId.Any())
                {
                    foreach (var sub in removesubCategoryId)
                    {
                        //Checking if the subcategory is used with other categories.
                        var totalSubId = Category_SubCategoryDbSet.Where(x => x.SubCategoryId == sub.SubCategoryId);
                        var tobedeleted = Category_SubCategoryDbSet.FirstOrDefault(x => x.CategoryId == id && x.SubCategoryId == sub.SubCategoryId);
                        if (tobedeleted != null)
                        {
                            Category_SubCategoryDbSet.Remove(tobedeleted);
                        }
                        //if subcategory to be deleted is not used with other sub categories
                        if (totalSubId.Count() == 1)
                        {
                            var removeSub = SubCategoryDbSet.FirstOrDefault(x => x.SubCategoryId == sub.SubCategoryId);
                            SubCategoryDbSet.Remove(removeSub);
                        }
                    }
                }
                return SaveChanges();
            }
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public bool checkCategoryifValid(CategoryContract model)
        {
            if (string.IsNullOrEmpty(model.Category.Name))
                return false;

            foreach (var item in model.SubCategories)
            {
                if (string.IsNullOrEmpty(item.Name))
                {
                    return false;
                }
            }
            return true;
        }
    }
}