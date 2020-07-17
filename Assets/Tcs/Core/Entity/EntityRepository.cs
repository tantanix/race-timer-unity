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

        public EntityRepository(string listKey)
        {
            _listKey = listKey;
        }

        public TEntity Create(TEntity model)
        {
            if (model == null)
                return null;

            var json = JsonUtility.ToJson(model);
            PlayerPrefs.SetString(model.Id, json);

            AddToList(model.Id);

            return model;
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

        public TEntity Get(string id)
        {
            if (id == null)
                throw new EntityNotFoundException<TEntity>();

            string data = PlayerPrefs.GetString(id, null);
            if (string.IsNullOrEmpty(data))
                throw new EntityNotFoundException<TEntity>();

            var model = JsonUtility.FromJson<TEntity>(data);

            return model;
        }

        public IEnumerable<TEntity> GetAll()
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
    }
}
