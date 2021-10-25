using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class DataRecords
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public DateTime Updated { get; set; }
        public int MYOB { get; set; }
        public string Desc { get; set; }
    }
}