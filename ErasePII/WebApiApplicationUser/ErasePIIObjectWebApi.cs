using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Xrm.Sdk;
using Xrm.Tools.WebAPI;

namespace ErasePII.WebApiApplicationUser
{
    public class ErasePiiObjectWebApi
    {
        public CRMWebAPI API;
        public Dictionary<string, FieldType> FieldCollectionDictionary;
        public Entity Entity;
        public string FieldName { get; set; }
        public string EntityName { get; set; }
        public string EntityIdLogicalName { get; set; }
        public Guid EntityId { get; set; }
        public List<ExpandoObject> EntityRecords { get; set; }
        public FieldType FieldType { get; set; }
        public string Resource { get; set; }
        public string AccessToken { get; set; }
        public string FieldValue { get; set; }
    }
}