using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CommonLibrary.Entities.Profile
{
    [Serializable()]
    public class ProfilePropertyDefinitionCollection : CollectionBase
    {
        public ProfilePropertyDefinitionCollection()
            : base()
        {
        }
        public ProfilePropertyDefinitionCollection(ArrayList definitionsList)
        {
            AddRange(definitionsList);
        }
        public ProfilePropertyDefinitionCollection(ProfilePropertyDefinitionCollection collection)
        {
            AddRange(collection);
        }
        public ProfilePropertyDefinition this[int index]
        {
            get { return (ProfilePropertyDefinition)List[index]; }
            set { List[index] = value; }
        }
        public ProfilePropertyDefinition this[string name]
        {
            get { return GetByName(name); }
        }
        public int Add(ProfilePropertyDefinition value)
        {
            return List.Add(value);
        }
        public void AddRange(ArrayList definitionsList)
        {
            foreach (ProfilePropertyDefinition objProfilePropertyDefinition in definitionsList)
            {
                Add(objProfilePropertyDefinition);
            }
        }
        public void AddRange(ProfilePropertyDefinitionCollection collection)
        {
            foreach (ProfilePropertyDefinition objProfilePropertyDefinition in collection)
            {
                Add(objProfilePropertyDefinition);
            }
        }
        public bool Contains(ProfilePropertyDefinition value)
        {
            return List.Contains(value);
        }
        public ProfilePropertyDefinitionCollection GetByCategory(string category)
        {
            ProfilePropertyDefinitionCollection collection = new ProfilePropertyDefinitionCollection();
            foreach (ProfilePropertyDefinition profileProperty in InnerList)
            {
                if (profileProperty.PropertyCategory == category)
                {
                    collection.Add(profileProperty);
                }
            }
            return collection;
        }
        public ProfilePropertyDefinition GetById(int id)
        {
            ProfilePropertyDefinition profileItem = null;
            foreach (ProfilePropertyDefinition profileProperty in InnerList)
            {
                if (profileProperty.PropertyDefinitionId == id)
                {
                    profileItem = profileProperty;
                }
            }
            return profileItem;
        }
        public ProfilePropertyDefinition GetByName(string name)
        {
            ProfilePropertyDefinition profileItem = null;
            foreach (ProfilePropertyDefinition profileProperty in InnerList)
            {
                if (profileProperty.PropertyName == name)
                {
                    profileItem = profileProperty;
                }
            }
            return profileItem;
        }
        public int IndexOf(ProfilePropertyDefinition value)
        {
            return List.IndexOf(value);
        }
        public void Insert(int index, ProfilePropertyDefinition value)
        {
            List.Insert(index, value);
        }
        public void Remove(ProfilePropertyDefinition value)
        {
            List.Remove(value);
        }
        public void Sort()
        {
            InnerList.Sort(new ProfilePropertyDefinitionComparer());
        }
    }
}
