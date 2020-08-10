using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tcs.Core.Entity
{
    public class EntityReferenceRepository<TEntity, TEntityList> 
        where TEntity : Entity 
        where TEntityList : EntityList, new()
    {
        private readonly string _prefix;

        public EntityReferenceRepository(string prefix)
        {
            _prefix = prefix;
        }

        public TEntity Create(string parentId, TEntity model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
                return null;

            var json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(model.Id, json);

            AddToList(parentId, model.Id);

            return model;
        }

        public TEntity Update(string parentId, TEntity model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
                return null;

            var json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(model.Id, json);

            return model;
        }

        public TEntity Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new EntityNotFoundException<TEntity>();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new EntityNotFoundException<TEntity>();

            var model = JsonUtility.FromJson<TEntity>(data);

            return model;
        }

        public IEnumerable<TEntity> GetAll(string parentId)
        {
            var key = GetListKey(parentId);
            if (PlayerPrefs.HasKey(key))
            {
                var listIds = PlayerPrefs.GetString(key);
                if (string.IsNullOrEmpty(listIds))
                    return new List<TEntity>();

                var list = JsonUtility.FromJson<TEntityList>(listIds);
                return list.Ids.Select(Get);
            }

            return new List<TEntity>();
        }

        private string GetListKey(string parentId)
        {
            return $"{_prefix}-{parentId}";
        }

        private void AddToList(string parentId, string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            TEntityList list;
            var key = GetListKey(parentId);

            string listIds = PlayerPrefs.GetString(key, null);
            if (!string.IsNullOrEmpty(listIds))
                list = JsonUtility.FromJson<TEntityList>(listIds);
            else
                list = new TEntityList();

            var exists = list.Ids.Contains(id);
            
            if (exists)
                return;

            list.Ids.Add(id);
            var json = JsonUtility.ToJson(list);
            
            PlayerPrefs.SetString(key, json);
        }

        public void Delete(string id)
        {
            var data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new EntityNotFoundException<TEntity>();

            RemoveFromList(id);

            Debug.Log($"{GetListKey(id)} Deleted (" + id + ")");
            PlayerPrefs.DeleteKey(id);
        }

        private void RemoveFromList(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var key = GetListKey(id);
            string listIds = PlayerPrefs.GetString(key, null);
            if (listIds == null)
                return;

            var list = JsonUtility.FromJson<TEntityList>(listIds);
            Debug.Log($"{GetListKey(id)} Before Remove: " + listIds);

            if (!list.Ids.Contains(id))
                return;

            list.Ids.Remove(id);

            var json = JsonUtility.ToJson(list);
            Debug.Log($"{GetListKey(id)} After Remove: " + list);

            PlayerPrefs.SetString(id, json);
        }
    }
}
