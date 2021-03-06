﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoRepository;

namespace ZorkSms.Data.Models
{
    public class SmsMessage : Entity
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Content { get; set; }
        public string Msg_ID { get; set; }
        public string Keyword { get; set; }
    }
}