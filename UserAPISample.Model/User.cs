using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace UserAPISample.Model
{
    public class User
    {
        [BsonId]
        [BsonElement("_id")]
        public int Id { get; set; }

        [BsonElement("name")]
        [BsonRequired]
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; }

        [BsonElement("username")]
        [BsonRequired]
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }

        [BsonElement("email")]
        [BsonRequired]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [BsonElement("address")]
        public Address Address { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("website")]
        [Url]
        public string Website { get; set; }

        [BsonElement("company")]
        public Company Company { get; set; }
    }

    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; }

        [BsonElement("suite")]
        public string Suite { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("zipcode")]
        public string Zipcode { get; set; }

        [BsonElement("geo")]
        public Geo Geo { get; set; }
    }

    public class Geo
    {
        [BsonElement("lat")]
        [Required]
        [RegularExpression(@"^(-?\d+(\.\d+)?)$")]
        public string Lat { get; set; }

        [BsonElement("lng")]
        [Required]
        [RegularExpression(@"^(-?\d+(\.\d+)?)$")]
        public string Lng { get; set; }
    }

    public class Company
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("catchPhrase")]
        public string CatchPhrase { get; set; }

        [BsonElement("bs")]
        public string Bs { get; set; }
    }
}
