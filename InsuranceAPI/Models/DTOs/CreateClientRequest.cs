﻿namespace InsuranceAPI.Models.DTOs
{
    public class CreateClientRequest
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string AadhaarNumber { get; set; } = string.Empty;
        public string PANNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
