using System;
using System.Linq;
using Scripts_v2.Data.Access;
using UnityEngine;

namespace Scripts_v2.Scene.Controllers
{
    public class OperationListController : MonoBehaviour
    {
        private IDatabaseRepository databaseRepository;
        private void Awake() => databaseRepository = new DatabaseRepository();

        private void Start()
        {
            GetAllOperations();
        }

        private async void GetAllOperations()
        {
            var response = await databaseRepository.GetAllOperationsAsync();

            if (response.isSuccess)
            {
                foreach (var operation in response.data)
                {
                    Debug.Log(operation);
                }
            }
            else
            {
                var errorList = response.errors.Aggregate(string.Empty,
                    (current, error) => current + $"{error}\n");

                Debug.LogError(errorList + response.message);
            }
        }
    }
}