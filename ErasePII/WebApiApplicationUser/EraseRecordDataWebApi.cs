using System;
using System.Collections.Generic;
using System.Dynamic;
using Xrm.Tools.WebAPI;

namespace ErasePII.WebApiApplicationUser
{
    class EraseRecordDataWebApi
    {
        private ErasePiiObjectWebApi erasePiiObject;
        private readonly string _entityName;
        private readonly List<ExpandoObject> _entityRecords;
        private readonly Guid _entityId;
        private readonly CRMWebAPI _api;

        public EraseRecordDataWebApi(ErasePiiObjectWebApi erasePiiObject)
        {
            _entityName = erasePiiObject.EntityName;
            _entityRecords = erasePiiObject.EntityRecords;
            _entityId = erasePiiObject.EntityId;
            _api = erasePiiObject.API;

            if (_entityRecords.Count > 0)
                BulkDeleteRecordsWebApi();

        }

        private async void BulkDeleteRecordsWebApi()
        {
            await _api.Delete(_entityName, _entityId);
        }
    }
}
