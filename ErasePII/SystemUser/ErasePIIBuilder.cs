using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;

namespace ErasePII.SystemUser
{
    public class ErasePiiBuilder
    {
        private string _entityName;
        private FieldType _fieldType;
        private string _fieldName;
        private CrmServiceClient _clientContext;    
        private Entity _entity;
        private EntityCollection _entityCollection;
        private Dictionary<string, FieldType> _fieldCollectionDictionary;
        //private ErasePIIObject erasePiiObject;

        public ErasePiiBuilder EntityName(string entityName)
        {
            _entityName = entityName;
            return this;
        }

        public ErasePiiBuilder ClientContext(CrmServiceClient crmClient)
        {
            _clientContext = crmClient;
            return this;
        }



        public ErasePiiBuilder GetEntity(CrmServiceClient crmClient, string entityName, Guid entityId,
            string[] columnSetFields, bool allColumnSet)
        {
            _entity = crmClient.Retrieve(entityName, entityId,
                allColumnSet ? new ColumnSet(true) : new ColumnSet(columnSetFields));

            return this;
        }

        public ErasePiiBuilder FieldName(string fieldName)
        {
            _fieldName = fieldName;
            return this;
        }

        public ErasePiiBuilder FieldType(FieldType fieldType)
        {
            _fieldType = fieldType;
            return this;
        }

        public ErasePiiBuilder GetEntityRecords(CrmServiceClient crmClient, string entityLogical)
        {
            _entityCollection = QueryRecords.GetEntityRecords(crmClient, entityLogical);
            return this;
        }

        public ErasePiiBuilder FieldAndTypeCollection(Dictionary<string, FieldType> fieldCollectionDictionary)
        {
            _fieldCollectionDictionary = fieldCollectionDictionary;
            return this;
        }
        public ErasePiiObject Build()
        {
            var erasePiiObject = new ErasePiiObject
            {
                EntityCollection = _entityCollection,
                EntityName = _entityName,
                Entity = _entity,
                FieldType = _fieldType,
                FieldName = _fieldName,
                ClientContext = _clientContext,
                FieldCollection = _fieldCollectionDictionary
            };
               
            return erasePiiObject;
        }
    }
}