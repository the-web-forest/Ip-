﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ipe.Domain.Models
{
	[BsonIgnoreExtraElements]
	public class User: Model
	{
		[BsonElement("name")]
		public string Name { get; set; }

		[BsonElement("email")]
		public string Email { get; set; }

        [BsonElement("photo")]
        public string Photo { get; set; }

        [BsonElement("password")]
		public string Password { get; set; }

		[BsonElement("city")]
		public string City { get; set; }

		[BsonElement("state")]
		public string State { get; set; }

		[BsonElement("emailVerified")]
		public bool EmailVerified { get; set; }

        [BsonElement("origin")]
        public string Origin { get; set; }

		[BsonElement("allowNewsletter")]
		public bool AllowNewsletter { get; set; } = true;
    }
}

