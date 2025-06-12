public class UpdateClientRequest
{
    public int ClientId { get; set; }

    public NameUpdate? NameUpdate { get; set; }
    public DateOfBirthUpdate? DateOfBirthUpdate { get; set; }
    public GenderUpdate? GenderUpdate { get; set; }
    public PhoneUpdate? PhoneUpdate { get; set; }
    public AadhaarUpdate? AadhaarUpdate { get; set; }
    public PANUpdate? PANUpdate { get; set; }
    public AddressUpdate? AddressUpdate { get; set; }
}

public class NameUpdate
{
    public string NewName { get; set; } = string.Empty;
}

public class DateOfBirthUpdate
{
    public DateTime? NewDateOfBirth { get; set; }
}

public class GenderUpdate
{
    public string NewGender { get; set; } = string.Empty;
}

public class PhoneUpdate
{
    public string NewPhoneNumber { get; set; } = string.Empty;
}

public class AadhaarUpdate
{
    public string NewAadhaarNumber { get; set; } = string.Empty;
}

public class PANUpdate
{
    public string NewPANNumber { get; set; } = string.Empty;
}

public class AddressUpdate
{
    public string NewAddress { get; set; } = string.Empty;
}
