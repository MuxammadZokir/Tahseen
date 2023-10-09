﻿using System.ComponentModel.DataAnnotations;
using Tahseen.Domain.Commons;

namespace Tahseen.Domain.Entities.Rewards;

public class UserBadges : Auditable
{
    public long UserId { get; set; }
    public long BadgeId { get; set; }

    [Required]
    [MinLength(1), MaxLength(100)]
    public string Description { get; set; }
}