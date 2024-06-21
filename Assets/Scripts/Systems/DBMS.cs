// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using System.Configuration;
// using System.Net;
// using UnityEngine;
// using Microsoft.Azure.Cosmos;
// using Damath.Systems.DB;

// namespace Damath.Systems
// {
//     public class DBMS : MonoBehaviour
//     {
//         static readonly string EndpointUrl = "https://cosmosrgeastusac643035-1509-4649-9d83db.documents.azure.com:443/";
//         static readonly string PrimaryKey = "Acvn0ZNhqd0knUn2mVMRkbY4uhEt8Ek9kDIuy99sDk92ACg5eACkNw2KvL49x5sioVpO31oMvyyAACDbNTgw5Q==";
        
//         CosmosClient cosmosClient;
//         string databaseId = "damath";

//         internal void Initialize()
//         {
//             cosmosClient = new CosmosClient(
//                 EndpointUrl,
//                 PrimaryKey,
//                 new CosmosClientOptions() {
//                     ApplicationName = "damath_game"
//                 });
//         }

//         void CreateAccount()
//         {
//             try
//             {
//                 ItemResponse<UserAccount> andersenFamilyResponse = await this.container.ReadItemAsync<UserAccount>(
//                     andersenFamily.Id,
//                     new PartitionKey(andersenFamily.PartitionKey)
//                 );
//                 Console.WriteLine("Item in database with id: {0} already exists\n", andersenFamilyResponse.Resource.Id);
//             } 
//             catch(CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
//             {
//                 ItemResponse<UserAccount> andersenFamilyResponse = await this.container.CreateItemAsync<UserAccount>(
//                     andersenFamily,
//                     new PartitionKey(andersenFamily.PartitionKey)
//                 );
//                 Console.WriteLine("Created item in database with id: {0} Operation consumed {1} RUs.\n", andersenFamilyResponse.Resource.Id, andersenFamilyResponse.RequestCharge);
//             }
//         }
//     }
// }