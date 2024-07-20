﻿using Mapster;
using WePrepClass.Domain.Commons.Enums;
using WePrepClass.Domain.WePrepClassAggregates.Users;
using WePrepClass.Domain.WePrepClassAggregates.Users.ValueObjects;

namespace WePrepClass.Contracts.Users;

public class UserProfileDto : BasicAuditedEntityDto<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public Gender Gender { get; set; }
    public int BirthYear { get; set; } = 1960;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Role { get; set; } = "Learner"; // This currently is not mapped

    public string Avatar { get; set; } =
        "https://res.cloudinary.com/dhehywasc/image/upload/v1686121404/default_avatar2_ws3vc5.png";
}

public class LearnerForProfileDtoMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserProfileDto>()
            .Map(dest => dest.Id, src => src.Id.Value)
            .Map(dest => dest.City, src => src.Address.City)
            .Map(dest => dest.Avatar, src => src.Avatar)
            .Map(dest => dest.Country, src => src.Address.Country)
            .Map(dest => dest.Role, src => src.Role.ToString())
            .Map(dest => dest, src => src);

        config.NewConfig<UserProfileDto, User>()
            .Map(dest => dest.FirstName, src => src.FirstName)
            .Map(dest => dest.LastName, src => src.LastName)
            .Map(dest => dest.BirthYear, src => src.BirthYear)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.PhoneNumber, src => src.PhoneNumber)
            .Map(dest => dest.Address, src => Address.Create(src.City, src.Country))
            .Map(dest => dest.Gender, src => src.Gender)
            .IgnoreNonMapped(true);
    }
}