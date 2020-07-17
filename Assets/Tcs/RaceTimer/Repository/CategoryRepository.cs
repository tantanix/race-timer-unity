using System;
using System.Collections.Generic;
using System.Linq;
using Tcs.RaceTimer.Exceptions;
using Tcs.RaceTimer.Models;
using UnityEngine;

namespace Tcs.RaceTimer.Repository
{
    public class CategoryRepository
    {
        public const string CategoryListIds = "categoryListIds";

        public Category Create(string id, string name)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty or whitespace.");

            if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty or whitespace.");

            var category = new Category
            {
                Id = id,
                Name = name
            };

            var json = JsonUtility.ToJson(category, true);
            Debug.Log("Create Category(" + category.Id + "): " + json);
            PlayerPrefs.SetString(category.Id, json);

            AddToList(category.Id);

            return category;
        }

        private void AddToList(string categoryId)
        {
            if (string.IsNullOrEmpty(categoryId))
                return;

            CategoryList categoryList;

            string playerListIds = PlayerPrefs.GetString(CategoryListIds, null);
            if (!string.IsNullOrEmpty(playerListIds))
                categoryList = JsonUtility.FromJson<CategoryList>(playerListIds);
            else
                categoryList = new CategoryList();

            var exists = categoryList.Ids.Contains(categoryId);
            Debug.Log("Adding category data to the list: " + categoryId + " - Exists: " + exists);

            if (exists)
                return;

            categoryList.Ids.Add(categoryId);
            var json = JsonUtility.ToJson(categoryList);
            Debug.Log("Category list new: " + json);

            PlayerPrefs.SetString(CategoryListIds, json);
        }

        public Category Get(string id)
        {
            if (id == null)
                throw new CategoryNotFoundException();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new CategoryNotFoundException();

            Debug.Log("Get Category(" + id + "): " + data);
            var category = JsonUtility.FromJson<Category>(data);

            return category;
        }

        public IEnumerable<Category> GetAll()
        {
            if (PlayerPrefs.HasKey(CategoryListIds))
            {
                var categoryListIds = PlayerPrefs.GetString(CategoryListIds);
                if (string.IsNullOrEmpty(categoryListIds))
                    return new List<Category>();

                var categoryList = JsonUtility.FromJson<CategoryList>(categoryListIds);
                return categoryList.Ids.Select(Get);
            }

            return new List<Category>();
        }

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
