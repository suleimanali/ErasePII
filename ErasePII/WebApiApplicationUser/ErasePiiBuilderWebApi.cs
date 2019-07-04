using System;
using System.Collections.Generic;
using System.Dynamic;
using Xrm.Tools.WebAPI;
using Xrm.Tools.WebAPI.Requests;

namespace ErasePII.WebApiApplicationUser
{
    public class ErasePiiBuilderWebApi
    {
        private Guid _entityId;
        private string _entityName;
        private CRMWebAPI _api;
        private string _fieldName;
        private FieldType _fieldType;
        private Dictionary<string, FieldType> _fieldCollectionDictionary;
        private List<ExpandoObject> _entityRecords;
        private string _resource;
        private string _accessToken;
        private string _entityIdLogicalName;
        private string _fieldValue;

        public ErasePiiBuilderWebApi(CRMWebAPI api)
        {
            _api = api;
        }

        public ErasePiiBuilderWebApi GetEntityId()
        {
            return this;
        }

        public ErasePiiBuilderWebApi EntityName(string entityName)
        {
            _entityName = entityName;
            return this;
        }
        public ErasePiiBuilderWebApi FieldName(string fieldName)
        {
            _fieldName = fieldName;
            return this;
        }
        public ErasePiiBuilderWebApi FieldType(FieldType fieldType)
        {
            _fieldType = fieldType;
            return this;
        }

        public ErasePiiBuilderWebApi GetEntityRecords(string entityName)
        {
            var fetchXml = "<fetch mapping='logical'><entity name='" + entityName + "'> <all-attributes/></entity></fetch>";
            var fetchResults = _api.GetList(entityName, new CRMGetListOptions
            {
                FetchXml = fetchXml
            });
            _entityRecords = fetchResults.Result.List;
            return this;
        }

        public ErasePiiBuilderWebApi FieldAndTypeCollection(Dictionary<string, FieldType> fieldCollectionDictionary)
        {
            _fieldCollectionDictionary = fieldCollectionDictionary;
            return this;
        }


        public ErasePiiBuilderWebApi Resource(string resource)
        {
            _resource = resource;
            return this;
        }

        public ErasePiiBuilderWebApi AccessToken(string accessToken)
        {
            _accessToken = accessToken;
            return this;
        }
        public ErasePiiBuilderWebApi EntityIdLogicalName(string entityIdLogicalName)
        {
            _entityIdLogicalName = entityIdLogicalName;
            return this;
        }
        public ErasePiiBuilderWebApi FieldValue(string fieldValue)
        {
            _fieldValue = fieldValue;
            return this;
        }
        public ErasePiiObjectWebApi Build()
        {
            var erasePiiWebApi = new ErasePiiObjectWebApi
            {
                Resource =  _resource,
                AccessToken = _accessToken,
                EntityId = _entityId,
                EntityName = _entityName,
                FieldCollectionDictionary = _fieldCollectionDictionary,
                EntityRecords = _entityRecords,
                API = _api,
                FieldType = _fieldType,
                FieldName = _fieldName,
                FieldValue =  _fieldValue,
                EntityIdLogicalName = _entityIdLogicalName
            };
            return erasePiiWebApi;
        }
    }
}
