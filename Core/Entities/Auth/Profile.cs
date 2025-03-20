using Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.Auth;

public class Profile : User
{
    public string? Image { get; set; }

    public EUserSex? Sex { get; set; }

    public string? Referrer { get; set; }

    public string? NationalCode { get; set; }

    public DateTime? BirthDate { get; set; }

    public int? CityId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string FullName { get => $"{FirstName ?? string.Empty} {LastName ?? string.Empty}"; }

    [ForeignKey(nameof(CityId))]
    public virtual City? CityNavigation { get; set; }

}
