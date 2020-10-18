using Dawn;
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
        private readonly string _idPrefix;

        public EntityReferenceRepository(string prefix, string idPrefix)
        {
            _prefix = prefix;
            _idPrefix = idPrefix;
        }

        public virtual string GenerateId()
        {
            return $"{_idPrefix}{System.Guid.NewGuid()}";
        }

        public TEntity Create(string parentId, TEntity model)
        {
            if (string.IsNullOrEmpty(parentId) || string.IsNullOrWhiteSpace(parentId))
                throw new ArgumentException("Parent id cannot be null or empty or whitespace");
            if (string.IsNullOrEmpty(model.Id) || string.IsNullOrWhiteSpace(model.Id))
                throw new EntityNotFoundException<TEntity>();
            if (!model.Id.StartsWith(_idPrefix, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException($"Invalid id: {model.Id}");

            var json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(model.Id, json);

            AddToList(parentId, model.Id);

            return model;
        }

        public TEntity Update(TEntity model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
                return null;

            var json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(model.Id, json);

            return model;
        }

        public TEntity Get(string id)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new EntityNotFoundException<TEntity>();
            if (!id.StartsWith(_idPrefix, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException($"Invalid id: {id}");

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

        public void Delete(string parentId, string id)
        {
            if (string.IsNullOrEmpty(parentId) || string.IsNullOrWhiteSpace(parentId))
                throw new ArgumentException("Parent id cannot be null or empty or whitespace");
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id cannot be null or empty or whitespace");
            if (!id.StartsWith(_idPrefix, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException($"Invalid id: {id}");

            var data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new EntityNotFoundException<TEntity>(id);

            RemoveFromList(parentId, id);

            Debug.Log($"{GetListKey(id)} Deleted (" + id + ")");
            PlayerPrefs.DeleteKey(id);
        }

        public void Delete(string parentId, IEnumerable<string> ids)
        {
            if (string.IsNullOrEmpty(parentId) || string.IsNullOrWhiteSpace(parentId))
                throw new ArgumentException("Parent id cannot be null or empty or whitespace");

            foreach (var id in ids)
            {
                if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
                    throw new ArgumentException("Id cannot be null or empty or whitespace");
                if (!id.StartsWith(_idPrefix, StringComparison.InvariantCultureIgnoreCase))
                    throw new ArgumentException($"Invalid id: {id}");

                var data = PlayerPrefs.GetString(id, null);
                if (string.IsNullOrEmpty(data))
                    throw new EntityNotFoundException<TEntity>(id);
            }

            RemoveFromList(parentId, ids);

            foreach (var id in ids)
            {
                Debug.Log($"{GetListKey(id)} Deleted (" + id + ")");
                PlayerPrefs.DeleteKey(id);
            }
        }

        private void RemoveFromList(string parentId, string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            var key = GetListKey(parentId);
            string listIds = PlayerPrefs.GetString(key, null);
            if (listIds == null)
                return;

            var list = JsonUtility.FromJson<TEntityList>(listIds);
            Debug.Log($"{key} Before Remove: " + listIds);

            if (!list.Ids.Contains(id))
                return;

            list.Ids.Remove(id);

            var json = JsonUtility.ToJson(list);
            Debug.Log($"{key} After Remove: " + list);

            PlayerPrefs.SetString(key, json);
        }

        private void RemoveFromList(string parentId, IEnumerable<string> ids)
        {
            if (ids == null)
                return;

            var key = GetListKey(parentId);
            string listIds = PlayerPrefs.GetString(key, null);
            if (listIds == null)
                return;

            var list = JsonUtility.FromJson<TEntityList>(listIds);
            Debug.Log($"{key} Before Remove: " + listIds);

            var idsToRemove = ids.Intersect(list.Ids);
            if (idsToRemove.Count() != ids.Count())
                Debug.Log($"Warning: Not all ids can be removed. IntersectCount: {idsToRemove.Count()} == IdsCount: {ids.Count()}");
            
            foreach (var id in idsToRemove)
            {
                list.Ids.Remove(id);
            }
            
            var json = JsonUtility.ToJson(list);
            Debug.Log($"{key} After Remove: " + list);

            PlayerPrefs.SetString(key, json);
        }
    }
}
