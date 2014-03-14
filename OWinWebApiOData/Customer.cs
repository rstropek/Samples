using System.Runtime.Serialization;

namespace WebApi
{
	// JSON example: {"City":"Vienna","FirstName":"Max","HouseNumber":"1","ID":1,"LastName":"Muster","State":"Vienna","Street":"Schwedenplatz","ZipCode":"0000"}

    [DataContract]
    public class Customer
    {
        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string HouseNumber { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }
    }
}