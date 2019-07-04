using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json.Linq;
using Xrm.Tools.WebAPI;
using Xrm.Tools.WebAPI.Requests;

namespace ErasePII.WebApiApplicationUser
{
    public class EraseFieldDataWebApi
    {
        private static string _fieldName;
        private static string _entityName;
        private readonly CRMWebAPI _api;
        private static Guid _entityId;
        private readonly Entity _entity;
        private readonly Dictionary<string, FieldType> _fieldCollectionDictionary;
        private int _optionSetValue;
        private FieldType _fieldType;
        private static string _resource;
        private static string _accessToken;
        private static string _entityIdLogicalName;
        private static string _fieldValue;
        private static string nextReqLink;
        private int myFetchResults;
        private int myApiCount;

        public EraseFieldDataWebApi(ErasePiiObjectWebApi erasePiiObject)
        {
            _fieldCollectionDictionary = erasePiiObject.FieldCollectionDictionary;
            _entity = erasePiiObject.Entity;
            _fieldName = erasePiiObject.FieldName;
            _entityName = erasePiiObject.EntityName;
            _entityId = erasePiiObject.EntityId;
            _entityIdLogicalName = erasePiiObject.EntityIdLogicalName;
            _resource = erasePiiObject.Resource;
            _accessToken = erasePiiObject.AccessToken;
            _api = erasePiiObject.API;
            _fieldValue = erasePiiObject.FieldValue;
            _fieldType = erasePiiObject.FieldType;
            CheckFieldType(_fieldType);

            //Task.WaitAll(Task.Run(async () => await EraseTextFieldTypeAsync()));
        }

        private static async Task EraseTextFieldTypeAsync(string reqLink = null)
        {
            try
            {
                var queryString = string.Empty;
                if (reqLink == null)
                    queryString = "/api/data/v9.1/" + _entityName + "s?$select=" + _fieldName + "";
                else
                    queryString = reqLink;
             
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(_resource);
                    httpClient.Timeout = new TimeSpan(0, 0, 4, 0);
                    httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                    httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _accessToken);
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    var retrieveRequest = await httpClient.GetAsync(queryString);
                    Console.WriteLine(retrieveRequest.IsSuccessStatusCode);
                    if (retrieveRequest.IsSuccessStatusCode)
                    {
                        ///Log the guid into a text file....
                        var reqResults = JObject.Parse(retrieveRequest.Content.ReadAsStringAsync().Result);
                        Console.WriteLine(reqResults.Count);
                        if (reqResults.Count > 1)
                        {
                            var reqResultList =
                                reqResults["value"].Select(e => (Guid)e.SelectToken(_entityIdLogicalName)).ToList();
                            var newEntityJObject = new JObject { { _fieldName, _fieldValue } };
                            Console.WriteLine(reqResultList.Count);
                      
                            //foreach (var reqGuid in reqResultList)
                            //{
                            //    var updateResourceUri =
                            //        new Uri(_resource + "/api/data/v9.1/" + _entityName + "s(" + reqGuid + ")");
                            //    var updaterequest = new HttpRequestMessage(new HttpMethod("PATCH"), updateResourceUri);
                            //    updaterequest.Content = new StringContent(newEntityJObject.ToString(), Encoding.UTF8, "application/json");

                            //    var updateResponse = httpClient
                            //        .SendAsync(updaterequest, HttpCompletionOption.ResponseContentRead).Result;
                            //    Console.WriteLine(updateResponse.IsSuccessStatusCode);
                            //}
                        }
                        if (reqResults.Count == 3)
                        {
                            var pattern = "^(.*?)\\.com";
                            var newRequestQueryString = Regex.Replace(reqResults["@odata.nextLink"].ToString(), pattern, "");
                            nextReqLink = newRequestQueryString;
                            Console.WriteLine("Next link request JObject: " + nextReqLink);
                        }

                        if (!string.IsNullOrEmpty(nextReqLink))
                            await EraseTextFieldTypeAsync(nextReqLink);
                        //await NextPageReqEraseTextFieldTypeAsync(nextReqLink);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #region SampleHttpRequest...
        //private static async Task NextPageReqEraseTextFieldTypeAsync(string nextReqLink)
        //{

        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.BaseAddress = new Uri(_resource);
        //        httpClient.Timeout = new TimeSpan(0, 0, 4, 0);
        //        httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
        //        httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
        //        httpClient.DefaultRequestHeaders.Accept.Add(
        //            new MediaTypeWithQualityHeaderValue("application/json"));
        //        httpClient.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Bearer", _accessToken);
        //        ServicePointManager.SecurityProtocol =
        //            SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
        //        var retrieveRequest = await httpClient.GetAsync(nextReqLink);
        //        Console.WriteLine(retrieveRequest.IsSuccessStatusCode);
        //        if (retrieveRequest.IsSuccessStatusCode)
        //        {
        //            Console.WriteLine("Inside the next page request, success code: " + retrieveRequest.IsSuccessStatusCode);
        //        }
        //    }
        //}
        #endregion
        private void CheckFieldType(FieldType fieldType)
        {

            switch (fieldType)
            {
                case FieldType.Int:
                    EraseIntFieldData(_fieldName);
                    break;
                case FieldType.Money:
                    EraseMoneyFieldData(_fieldName);
                    break;
                case FieldType.OptionSet:
                    EraseOptionSetFieldData(_fieldName, _optionSetValue);
                    break;
                case FieldType.Text:
                    //Task.WaitAll(Task.Run(async () => await EraseTextFieldTypeAsync()));
                    EraseTextFieldData(_fieldName);
                    break;
                case FieldType.MultipleFieldsWithTypes:
                    Task.WaitAll(Task.Run(async () => await EraseMultipleFieldsWithTypesAsync()));
                    EraseFieldCollectionData(_entity, _fieldCollectionDictionary);
                    break;
                case FieldType.Default:
                    break;
                default:
                    throw new Exception("The filed type was not created...");
            }
        }

        private async Task EraseMultipleFieldsWithTypesAsync()
        {
            throw new NotImplementedException();
        }

        private void EraseFieldCollectionData(Entity entity, Dictionary<string, FieldType> fieldCollectionDictionary)
        {
            if (fieldCollectionDictionary == null) return;
            foreach (var fieldType in fieldCollectionDictionary)
            {
                _fieldName = fieldType.Key;
                CheckFieldType(fieldType.Value);
            }
        }

        private void EraseTextFieldData(string fieldName)
        {
            ///Get all entities and entityId...
            ///add them to an expando object....
            ///Loop the entities to update the records...
            Task.Run(async () =>
            {
                const string fetchXml
                    = "<fetch><entity name='contact'><all-attributes/></entity></fetch>";
                var fetchResults = await _api.GetList("contacts", 
                    new CRMGetListOptions
                    {
                        FetchXml = fetchXml
                    });
              
                var myListTrackChanged = fetchResults.TrackChangesLink;
                Console.WriteLine("the tracked changes are: " + myListTrackChanged);
               myFetchResults = fetchResults.List.Count;
         
                myApiCount = await _api.GetCount("contacts");
               

                //var getEntityFetchRecords = await _api.GetList(_entityName + "s", new CRMGetListOptions
                //{

                //    FormattedValues = true
                //});

                //if (getEntityFetchRecords.List.Count > 0)
                //{
                //    foreach (var entities in getEntityFetchRecords.List)
                //    {
                //        Console.WriteLine("The number of entities is: " + entities.ToList().Count);
                //        foreach (var entity in entities)
                //        {
                //            if (entity.Key == "value") continue;
                //            if (entity.Key == fieldName)
                //            {
                //                var updateObj = new ExpandoObject() as IDictionary<string, object>;
                //                updateObj.Add(fieldName, string.Empty);
                //                await _api.Update(_entityName, _entityId, updateObj);
                //            }
                //        }
                //    }
                //}
            }).Wait();
            Console.WriteLine("The fetch results is: " + myFetchResults);
        }

        private void EraseOptionSetFieldData(string fieldName, int optionSetValue)
        {
            Task.Run(async () =>
            {
                var updateObj = new ExpandoObject() as IDictionary<string, object>;
                updateObj.Add(fieldName, new OptionSetValue(optionSetValue));
                await _api.Update(_entityName, _entityId, updateObj);
            });

        }

        private void EraseMoneyFieldData(string fieldName)
        {
            Task.Run(async () =>
            {
                var updateObj = new ExpandoObject() as IDictionary<string, object>;
                updateObj.Add(fieldName, new Money(0));
                await _api.Update(_entityName, _entityId, updateObj);
            });

        }

        private void EraseIntFieldData(string fieldName)
        {
            Task.Run(async () =>
            {
                var updateObj = new ExpandoObject() as IDictionary<string, object>;
                updateObj.Add(fieldName, 0);
                await _api.Update(_entityName, _entityId, updateObj);
            });
        }
    }
}
