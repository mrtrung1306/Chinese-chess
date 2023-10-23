﻿using System.ComponentModel.DataAnnotations;

namespace DemoQLDA.Models
{
    public class TokenRequest
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefeshToken { get; set; }
    }
}
