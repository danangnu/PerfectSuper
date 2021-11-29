namespace API.Entities
{
    public class Employees
    {
        public int Id { get; set; }
        public int PayrollID { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string OtherGivenName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Locality { get; set; }
        public string State { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string CustomList2 { get; set; }
    }
}