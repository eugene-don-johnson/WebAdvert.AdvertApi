﻿using AdvertApi.Models;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace AdvertApi.Services
{
    public class DynamoDBAdvertStorage : IAdvertStorageService
    {
        private readonly IMapper _mapper;

        public DynamoDBAdvertStorage(IMapper mapper) 
        {
            _mapper = mapper;
        }

        public async Task<string> Add(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDto>(model);
            
            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;

            using (var client = new AmazonDynamoDBClient()) 
            {
                using (var context = new DynamoDBContext(client)) 
                {
                   await context.SaveAsync(dbModel);    
                }
            }

            return dbModel.Id;  
        }

        public async Task Confirm(ConfirmAdvertModel model)
        {
            using (var client = new AmazonDynamoDBClient())
            using (var context = new DynamoDBContext(client))
            {
                var record = await context.LoadAsync<AdvertDto>(model.Id);

                if (record == null)
                    throw new Exception($"A record with ID: {model.Id} not found");

                if (model.Status == AdvertStatus.Active)
                {
                    record.Status = model.Status;
                    await context.SaveAsync(record);
                }
                else 
                {
                    await context.DeleteAsync(record);
                }
            }
        }
    }
}
