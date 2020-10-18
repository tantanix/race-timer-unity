﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tcs.Core.Entity
{
    public class EntityRepository<TEntity, TEntityList>
        where TEntity : Entity
        where TEntityList : EntityList, new()
    {
        private readonly string _listKey;
        private readonly string _idPrefix;

        public EntityRepository(string listKey, string idPrefix)
        {
            _listKey = listKey;
            _idPrefix = idPrefix;
        }

        public virtual string GenerateId()
        {
            return $"{_idPrefix}{System.Guid.NewGuid()}";
        }

        public virtual TEntity Create(TEntity model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
                return null;

            var json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(model.Id, json);

            AddToList(model.Id);

            return model;
        }

        public virtual TEntity Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new EntityNotFoundException<TEntity>();

            if (!id.StartsWith(_idPrefix, StringComparison.InvariantCultureIgnoreCase))
                throw new ArgumentException($"Invalid id: {id}");

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new EntityNotFoundException<TEntity>();

            var model = JsonUtility.FromJson<TEntity>(data);

            return model;
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            if (PlayerPrefs.HasKey(_listKey))
            {
                var listIds = PlayerPrefs.GetString(_listKey);
                if (string.IsNullOrEmpty(listIds))
                    return new List<TEntity>();

                var list = JsonUtility.FromJson<TEntityList>(listIds);
                return list.Ids.Select(Get);
            }

            return new List<TEntity>();
        }

        public virtual void Delete(string id)
        {
            var data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new EntityNotFoundException<TEntity>();

            RemoveFromList(id);

            Debug.Log($"{_listKey} Deleted (" + id + ")");
            PlayerPrefs.DeleteKey(id);
        }

        private void AddToList(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            TEntityList list;

            string listIds = PlayerPrefs.GetString(_listKey, null);
            if (!string.IsNullOrEmpty(listIds))
                list = JsonUtility.FromJson<TEntityList>(listIds);
            else
                list = new TEntityList();

            var exists = list.Ids.Contains(id);

            if (exists)
                return;

            list.Ids.Add(id);
            var json = JsonUtility.ToJson(list);

            PlayerPrefs.SetString(_listKey, json);
        }

        private void RemoveFromList(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            string listIds = PlayerPrefs.GetString(_listKey, null);
            if (listIds == null)
                return;

            var list = JsonUtility.FromJson<TEntityList>(listIds);
            Debug.Log($"{_listKey} Before Remove: " + listIds);

            if (!list.Ids.Contains(id))
                return;

            list.Ids.Remove(id);

            var json = JsonUtility.ToJson(list);
            Debug.Log($"{_listKey} After Remove: " + list);

            PlayerPrefs.SetString(_listKey, json);
        }
    }
}
