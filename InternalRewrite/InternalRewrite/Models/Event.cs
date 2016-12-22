namespace InternalRewrite.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Event")]
    public partial class Event
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Event()
        {
            VolunteerPosition = new HashSet<VolunteerPosition>();
        }

        public string Id { get; set; }

        [Required]
        [StringLength(30)]
        public string EventName { get; set; }

        [Required]
        [StringLength(128)]
        public string UserID { get; set; }

        //[DataType(DataType.Date)]
        [Required]
        [CheckDateRange]
        public DateTime StartDateTime { get; set; }

        [Required]
        [DateGreaterThan("StartDateTime")]
        public DateTime EndDateTime { get; set; }

        [Required]
        [StringLength(30)]
        public string Address { get; set; }

        [Required]
        [StringLength(30)]
        public string City { get; set; }

        [Required]
        [StringLength(30)]
        public string State { get; set; }

        //[Required]
        //public List<VolunteerPosition> EventVolunteers { get; set; }
        
        public int NumVolunteers { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VolunteerPosition> VolunteerPosition { get; set; }
    }


    [AttributeUsage(AttributeTargets.Property)]
    public class DateGreaterThanAttribute : ValidationAttribute
    {
        public DateGreaterThanAttribute(string dateToCompareToFieldName)
        {
            DateToCompareToFieldName = dateToCompareToFieldName;
        }

        private string DateToCompareToFieldName { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime earlierDate = (DateTime)value;

            DateTime laterDate = (DateTime)validationContext.ObjectType.GetProperty(DateToCompareToFieldName).GetValue(validationContext.ObjectInstance, null);

            if (laterDate > earlierDate)
            {
                //return ValidationResult.Success;
                return new ValidationResult("Start Date/Time is later than End Date/Time");
            }
            else
            {
                //return new ValidationResult("Start Date/Time is later than End Date/Time");
                return ValidationResult.Success;
            }
        }
    }

    public class CheckDateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            DateTime dt = (DateTime)value;
            DateTime timeUtc = DateTime.UtcNow;
            timeUtc = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
            //if (dt >= DateTime.UtcNow) 
            if(dt >= timeUtc)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Start Date/Time must be later than the current date and time");
        }

    }
}
